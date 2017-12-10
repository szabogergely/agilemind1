using PicBook.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PicBook.Repository.EntityFramework
{
    public interface ITagRepository
    {
        Task Create(Tag entity);
        Task<List<Tag>> FindByImageIdentifier(string imageIdentifier);
    }
}
