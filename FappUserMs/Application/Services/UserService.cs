using Application.Common.Dtos.UserDtos;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Application.Services;

public class UserService
{
    private readonly IApplicationDbContext _context;

    public UserService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LiteUserDto>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _context.Users.AsQueryable()
            .Select(u => new LiteUserDto(u.Id, u.UserName))
            .ToListAsync(cancellationToken: cancellationToken);
    }
}