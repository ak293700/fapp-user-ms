using MongoDB.Bson;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;

    public IList<ObjectId> FriendsIds { get; set; } = null!; 
}