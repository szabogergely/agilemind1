using PicBook.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PicBook.ApplicationService
{
    public class TagService : ITagService
    {
        private readonly PicBook.Repository.EntityFramework.ITagRepository dbtagRepo;

        public TagService(PicBook.Repository.EntityFramework.ITagRepository dbtagRepo)
        {
            this.dbtagRepo = dbtagRepo;
        }
        public async Task<bool> SaveTags(List<String> tags, String imageIdentifier)
        {
            foreach(String tag in tags) {
                var u = new Tag()
                {
                    TagName = tag,
                    ImageIdentifier = imageIdentifier
                };
                await dbtagRepo.Create(u);
            }
            return true;
        }

    }
}
