namespace User.Api.Models;

public class ChangePasswordRequest
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class ChangePasswordUserRequest : ChangePasswordRequest
{
    public Guid UserId { get; set; }
}