namespace Application.Services;

public class AuthService
{
    private readonly IApplicationDbContext _context;

    public AuthService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Login(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Register(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}