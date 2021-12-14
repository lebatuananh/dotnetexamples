namespace Shared.Identity;

public class AuthenticationSettings
{
    public string Authority { get; set; }
    public string ClientId { get; set; }
    public string ApiName { get; set; }
    public string ApiSecret { get; set; }
    public bool RequireHttpsMetadata { get; set; }
    public string AdministrationRole { get; set; }
}