using System.Linq;
using System.Threading.Tasks;
using PicBook.Domain;
using System.Collections.Generic;

namespace PicBook.Repository.EntityFramework
{
    public class ImageRepository : GenericCrudRepository<Image>, IImageRepository
    {
        public ImageRepository(ApplicationDbContext dbContext)
        {
            Context = dbContext;
        }

        public async Task<Image> FindByIdentifier(string imageIdentifier)
        {
            var images = await FindAll(u => u.ImageIdentifier == imageIdentifier);


            return images.FirstOrDefault();
        }

        public async Task<List<Image>> FindByUserIdentifier(string userIdentifier)
        {
            var images = await FindAll(u => u.UserIdentifier == userIdentifier && u.IsArchived != true);
            return images.ToList();
        }
        public  async Task DeletePic(Image entity)
        {
            var image = await GetById(entity.Id);
            image.IsArchived = true;
            Context.SaveChanges();
            
        }
        public async Task PublicPic(Image entity)
        {
            var image = await GetById(entity.Id);
            image.PublicToAll = !image.PublicToAll;
            Context.SaveChanges();

        }
        public async Task<List<Image>> GetAllPublicPic()
        {
            var images = await FindAll(u => u.PublicToAll && !u.IsArchived);
            return images.ToList();
        }
    }
}