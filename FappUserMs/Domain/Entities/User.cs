using MongoDB.Bson;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    
    public string Email { get; set; } = null!;

    public IList<ObjectId> FriendsIds { get; set; } = null!; 
}