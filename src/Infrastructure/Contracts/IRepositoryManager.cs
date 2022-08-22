namespace Infrastructure.Contracts
{
    public interface IRepositoryManager
    {
        IUserRepository User { get; }
        ITokenRepository Token { get; }
        IReviewRepository Review { get; }
        IReadListRepository ReadList { get; }
        IBookRepository Book { get; }
        ICategoryRepository Category { get; }
        IRoleRepository Role { get; }
        IUserRoleRepository UserRole { get; }
        IPhotoRepository Photo { get; }
        Task BeginTransaction(Func<Task> action);
        Task SaveChangesAsync();
    }
}
