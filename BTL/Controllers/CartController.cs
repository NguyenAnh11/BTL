using BTL.Data;
using BTL.Models;
using BTL.Services;
using BTL.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BTL.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopDbContext _db;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        public CartController(
            ShopDbContext db,
            IProductService productService, 
            IPictureService pictureService)
        {
            _db = db;
            _productService = productService;
            _pictureService = pictureService;
        }
        public ActionResult Index()
        {
            var carts = (List<CartModel>)Session["Cart"];
            return View(carts);
        }
        [HttpPost]
        public async Task<ActionResult> AddToCart(int productId, int quantity)
        {
            if (Session["Id"] == null)
                return Json(new { success = false, msg = "Bạn phải đăng nhập để mua sắm " }, JsonRequestBehavior.AllowGet);

            var cart = (List<CartModel>)Session["Cart"];

            var product = await _productService.GetByIdAsync(productId);

            if (product == null)
                return Json(new { success = false, msg = "Sản phẩm không tồn tại" });

            var pictures = await _productService.GetPicturesByProductIdAsync(product);

            int pictureId = 0;
            if (pictures != null && pictures.Count == 1)
                pictureId = pictures[0].Id;

            var pictureUrl = await _pictureService.GetPictureUrlAsync(pictureId);

            if(cart == null)
                cart = new List<CartModel>();

            if (cart.FindIndex(p => p.ProductId == product.Id) != -1)
            {
                return Json(new { success = false, msg = "Sản phẩm đã nằm trong giỏ hàng" });
            }

            cart.Add(new CartModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                PictureUrl = pictureUrl,
                Quantity = quantity,
                Price = product.Price,
            });

            Session["Cart"] = cart;
            Session["Number"] = cart.Count;

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpDelete]
        public async Task<ActionResult> RemoveFromCart(int productId)
        {
            if(Session["Id"] == null)
                return Json(new { success = false, msg = "Bạn phải đăng nhập để mua sắm " }, JsonRequestBehavior.AllowGet);

            var cart = (List<CartModel>)Session["Cart"];

            var product = await _productService.GetByIdAsync(productId);

            if(product == null)
                return Json(new { success = false, msg = "Sản phẩm không tồn tại" });

            if (cart == null || !cart.Any(p => p.ProductId == productId))
                return Json(new { success = false, msg = "Sản phẩm không trong giỏ hàng" });

            cart.RemoveAll(p => p.ProductId == productId);

            Session["Cart"] = cart;
            Session["Number"] = cart.Count;

            return Json(new { success = true, number = Session["Number"] }, JsonRequestBehavior.AllowGet);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateCart(int productId, int quantity)
        {
            if (Session["Id"] == null)
                return Json(new { success = false, msg = "Bạn phải đăng nhập để mua sắm " }, JsonRequestBehavior.AllowGet);

            var cart = (List<CartModel>)Session["Cart"];

            var product = await _productService.GetByIdAsync(productId);

            if (product == null)
                return Json(new { success = false, msg = "Sản phẩm không tồn tại" });

            if (cart == null || !cart.Any(p => p.ProductId == productId))
                return Json(new { success = false, msg = "Sản phẩm không trong giỏ hàng" });

            int index = cart.FindIndex(p => p.ProductId == product.Id);

            cart[index].Quantity = quantity;

            Session["Cart"] = cart;
            Session["Number"] = cart.Count;

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult CartIcon()
        {
            var cart = Session["Cart"] as List<CartModel>;

            int number = 0;
            if (cart != null)
                number = cart.Count;

            ViewBag.Number = number;

            return PartialView();
        }
        public PartialViewResult CartMoney()
        {
            var cart = Session["Cart"] as List <CartModel>;

            ViewBag.Money = cart.Sum(p => p.Quantity * p.Price);

            return PartialView();
        }
        [HttpPost]
        public async Task<ActionResult> Order(string address)
        {
            var cart = Session["Cart"] as List<CartModel>;

            if (cart == null || cart.Count == 0)
                return Json(new { success = false, msg = "Bạn phải tiến hành chọn hàng cần mua" }, JsonRequestBehavior.AllowGet);

            int.TryParse(Session["Id"].ToString(), out int userId);

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order
                    {
                        UserId = userId,
                        ShippingAddress = address,
                        OrderStatus = OrderStatus.Success
                    };

                    _db.Set<Order>().Add(order);

                    await _db.SaveChangesAsync();

                    foreach(var item in cart)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            Price = item.Price,
                            Quantity = item.Quantity,
                        };

                        var product = await _productService.GetByIdAsync(item.ProductId);

                        product.StockQuantity -= orderItem.Quantity;

                        _db.Set<OrderItem>().Add(orderItem);
                    }    

                    await _db.SaveChangesAsync();

                    Session["Cart"] = null;

                    transaction.Commit();

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw ex;
                }
            }    
        }
    }
}