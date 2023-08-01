
using Application.Common.Dtos;
using Domain.Entities;

namespace Application.Services;

public class UserService
{
    private readonly IApplicationDbContext _context;

    public UserService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateAsync(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        User user = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        
        await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
        return user.Id;
    }
}