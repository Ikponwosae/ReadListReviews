using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class ReadListRepository : RepositoryBase<ReadList>, IReadListRepository
    {
        public ReadListRepository(AppDbContext context) : base(context)
        {

        }
    }
}
