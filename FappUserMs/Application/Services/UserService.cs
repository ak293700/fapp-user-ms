
using Application.Common.Dtos.UserDtos;
using MongoDB.Driver;

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
        return _context.Users
            .AsQueryable()
            .Select(u => new LiteUserDto(u.Id, u.UserName))
            .ToList();
    }
}