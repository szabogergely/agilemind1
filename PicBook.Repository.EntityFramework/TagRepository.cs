using PicBook.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicBook.Repository.EntityFramework
{
    public class TagRepository : GenericCrudRepository<Tag>, ITagRepository
    {
        public TagRepository(ApplicationDbContext dbContext)
        {
            Context = dbContext;
        }

        public async Task<List<Tag>> FindByImageIdentifier(string imageIdentifier)
        {
            var tags = await FindAll(u => u.ImageIdentifier == imageIdentifier);
            return tags.ToList();
        }

        public async Task<List<Tag>> FindByTag(string tag)
        {
            var tags = await FindAll(u => u.TagName == tag);
            return tags.ToList();
        }
    }
}
