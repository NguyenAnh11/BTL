using System;
using System.Collections.Generic;
using BTL.Models.Interfaces;

namespace BTL.Models
{
    public class Order : IDateCreatedEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime CreateAt { get; set; }
        public string ShippingAddress { get; set; }
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus
        {
            get { return (OrderStatus)OrderStatusId; }
            set { OrderStatusId = (int)value; }
        }
        public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}