namespace Application.Repositories;

public class UserRepository
{
    private readonly IApplicationDbContext _context;

    public UserRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> Exist(string userId, CancellationToken cancellationToken = default)
    {
        return _context.Users
            .Find(u => u.Id == userId)
            .AnyAsync(cancellationToken: cancellationToken);
    }
}