using BTL.Models;
using System.Threading.Tasks;
using System.Web;

namespace BTL.Services
{
    public interface IPictureService : IAbstractEntityService<Picture>
    {
        Task<string> GetPictureUrlAsync(int pictureId, PictureType pictureType = PictureType.Entity); 
        string GetPictureUrl(Picture picture, PictureType pictureType = PictureType.Entity);
        string GetDefaultPictureUrl(PictureType pictureType = PictureType.Entity);
        Task<(string, Picture)> SavePictureAsync(HttpPostedFileBase file);
        Task<Picture> SavePictureAsync(byte[] pictureBinary, string pictureName, string altAttribute = null, string titleAttribute = null);
        Task DeletePictureAsync(Picture picture);
    }
}
