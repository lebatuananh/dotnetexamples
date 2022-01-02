namespace User.Api;

public class UserEndpointConfig
{
    public string BaseUrl { get; set; }
    public string ClientName { get; set; }
    public string ExternalUser { get; set; }
    public string ChangePassword { get; set; }
    public string ChangePasswordUser { get; set; }
    public string GetRoles { get; set; }
    public string AssignRole { get; set; }
}