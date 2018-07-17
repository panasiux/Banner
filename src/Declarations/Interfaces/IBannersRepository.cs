using System.Collections.Generic;
using System.Threading.Tasks;

namespace Declarations.Interfaces
{
    public interface IBannersRepository
    {
        Task<IBanner> Add(IBanner banner);
        Task<IBanner> FindById(int id);
        Task<IBanner> Update(int id, IBanner banner);
        Task Delete(int id);
        Task<IList<IBanner>> List(int take, int skip);
    }
}