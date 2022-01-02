namespace User.Api.Models;

public class UserResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
}