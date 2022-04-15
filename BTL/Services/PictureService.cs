using BTL.Data;
using BTL.Models;
using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTL.Services
{
    public class PictureService : AbstractEntityService<Picture>, IPictureService
    {
        public PictureService(ShopDbContext db) : base(db)
        {

        }
        public string GetPictureLocalPath(string fileName)
        {
            return $"/Images/{fileName}";
        }

        public void SavePictureInLocal(byte[] pictureBinary, string pictureName)
        {
            var absolutePath = GetPictureLocalPath(pictureName);

            var picturePath = AppContext.BaseDirectory + absolutePath;

            File.WriteAllBytes(picturePath, pictureBinary);
        }

        public async Task<string> GetPictureUrlAsync(int pictureId, PictureType pictureType = PictureType.Entity)
        {
            var picture = await GetByIdAsync(pictureId);

            return GetPictureUrl(picture);
        }

        public string GetPictureUrl(Picture picture, PictureType pictureType = PictureType.Entity)
        {
            if(picture == null)
            {
                return GetDefaultPictureUrl(pictureType);
            }

            var filePath = GetPictureLocalPath(picture.FileName);

            return filePath;
        }

        public string GetDefaultPictureUrl(PictureType pictureType = PictureType.Entity)
        {
            string fileName = string.Empty;

            switch(pictureType)
            {
                case PictureType.Avatar:
                    fileName = "default-avatar-image.png";
                    break;
                case PictureType.Entity:
                    fileName = "default-entity-image.png";
                    break;
            }

            var filePath = GetPictureLocalPath(fileName);

            return filePath;
        }

        public async Task<Picture> SavePictureAsync(byte[] pictureBinary, string pictureName, string altAttribute = null, string titleAttribute = null)
        {
            var picture = new Picture
            {
                FileName = pictureName,
                Title = titleAttribute,
                Alt = altAttribute,
            };

            _db.Set<Picture>().Add(picture);

            await _db.SaveChangesAsync();

            SavePictureInLocal(pictureBinary, pictureName);

            return picture;
        }

        public async Task<(string, Picture)> SavePictureAsync(HttpPostedFileBase file)
        {
            var pictureExtension = new List<string>
            {
                ".jpeg",
                ".jpg",
                ".png",
                ".gif",
                ".bmp"
            } as IReadOnlyCollection<string>;

            var maximumFileSizeUpload = 30 * 1024 * 1024;

            var extension = Path.GetExtension(file.FileName);
            if(pictureExtension.All(p => !p.Equals(extension, StringComparison.CurrentCultureIgnoreCase)))
            {
                return ("Định dạng file không phù hợp", null);
            }

            var size = file.ContentLength;
            if(size > maximumFileSizeUpload)
            {
                return ($"Kích thước file quá lớn. Tối thiểu {maximumFileSizeUpload} mb", null);
            }

            var pictureName = Guid.NewGuid().ToString() + extension;

            byte[] pictureBinary = new byte[file.ContentLength];
            await file.InputStream.ReadAsync(pictureBinary, 0, file.ContentLength);

            var picture = await SavePictureAsync(pictureBinary, pictureName);

            return (string.Empty, picture);
        }

        public async Task DeletePictureAsync(Picture picture)
        {
            if(picture == null)
                throw new ArgumentNullException(nameof(picture));

            var absolutePath = GetPictureLocalPath(picture.FileName);

            var filePath = AppContext.BaseDirectory + absolutePath;

            File.Delete(filePath);

            _db.Set<Picture>().Remove(picture);

            await _db.SaveChangesAsync();
        }
    }
}