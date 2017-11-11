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
            var images = await FindAll(u => u.UserIdentifier == userIdentifier);
            return images.ToList();
        }
    }
}