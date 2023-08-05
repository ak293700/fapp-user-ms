namespace Domain.Entities.UserEntities;

public class Friend
{
    public string UserId { get; set; } = null!;
    public JoiningState JoiningState { get; set; }
}