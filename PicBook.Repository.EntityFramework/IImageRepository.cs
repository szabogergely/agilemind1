using System.Threading.Tasks;
using PicBook.Domain;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace PicBook.Repository.EntityFramework
{
    public interface IImageRepository
    {
        Task Create(Image entity);
        Task<Image> FindByIdentifier(string imageIdentifier);

        Task<List<Image>> FindByUserIdentifier(string userIdentifier);
    }
}
