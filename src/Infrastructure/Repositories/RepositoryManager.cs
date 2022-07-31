using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly AppDbContext _appDbContext;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IUserRoleRepository> _userRoleRepository;
        private readonly Lazy<ITokenRepository> _tokenRepository;
        private readonly Lazy<IReadListRepository> _readListRepository;
        private readonly Lazy<ReviewRepository> _reviewRepository;
        private readonly Lazy<IBookRepository> _bookRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<IRoleRepository> _roleRepository;

        public RepositoryManager(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(appDbContext));
            _userRoleRepository = new Lazy<IUserRoleRepository>(() => new UserRoleRepository(appDbContext));
            _tokenRepository = new Lazy<ITokenRepository>(() => new TokenRepository(appDbContext));
            _readListRepository = new Lazy<IReadListRepository>(() => new ReadListRepository(appDbContext));
            _reviewRepository = new Lazy<ReviewRepository>(() => new ReviewRepository(appDbContext));
            _bookRepository = new Lazy<IBookRepository>(() => new BookRepository(appDbContext));
            _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(appDbContext));
            _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(appDbContext));
        }

        public IUserRepository User => _userRepository.Value;
        public ITokenRepository Token => _tokenRepository.Value;
        public IUserRoleRepository UserRole => _userRoleRepository.Value;
        public IReadListRepository ReadList => _readListRepository.Value;
        public IReviewRepository Review => _reviewRepository.Value;
        public IBookRepository Book => _bookRepository.Value;
        public ICategoryRepository Category => _categoryRepository.Value;
        public IRoleRepository Role => _roleRepository.Value;

        public async Task SaveChangesAsync() => await _appDbContext.SaveChangesAsync();
        public async Task BeginTransaction(Func<Task> action)
        {
            await using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                await action();

                await SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
