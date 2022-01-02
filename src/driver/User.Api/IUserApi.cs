using User.Api.Models;

namespace User.Api;

public interface IUserApi
{
    Task<UserResponse> RegisterUser(ExternalUserRequest externalUserRequest);
    Task ChangePassword(ChangePasswordRequest changePasswordRequest);
    Task ChangePasswordUser(ChangePasswordUserRequest changePasswordUserRequest);
    Task AssignRole(AssignRoleRequest assignRoleRequest);
}