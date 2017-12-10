using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PicBook.ApplicationService
{
    public interface ITagService
    {
        Task<bool> SaveTags(List<String> tags, String imageIdentifier);
    }
}
