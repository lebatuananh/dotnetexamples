using Microsoft.AspNetCore.Http;
using Shared.HttpClient;
using User.Api.Models;

namespace User.Api;

public class UserApi : BaseApiClient, IUserApi
{
    private readonly UserEndpointConfig _userEndpointConfig;

    public UserApi(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor,
        UserEndpointConfig userEndpointConfig) : base(
        httpClientFactory, httpContextAccessor)
    {
        _userEndpointConfig = userEndpointConfig;
    }


    public async Task<UserResponse> RegisterUser(ExternalUserRequest externalUserRequest)
    {
        var user = await PostAsync<ExternalUserRequest, UserResponse>(_userEndpointConfig.ExternalUser,
            _userEndpointConfig.ClientName,
            _userEndpointConfig.BaseUrl, externalUserRequest);
        return user;
    }

    public async Task ChangePassword(ChangePasswordRequest changePasswordRequest)
    {
        await PostAsync<ChangePasswordRequest, object>(_userEndpointConfig.ChangePassword,
            _userEndpointConfig.ClientName, _userEndpointConfig.BaseUrl, changePasswordRequest);
    }

    public async Task ChangePasswordUser(ChangePasswordUserRequest changePasswordUserRequest)
    {
        await PostAsync<ChangePasswordUserRequest, object>(_userEndpointConfig.ChangePasswordUser,
            _userEndpointConfig.ClientName, _userEndpointConfig.BaseUrl, changePasswordUserRequest);
    }

    public async Task AssignRole(AssignRoleRequest assignRoleRequest)
    {
        await PostAsync<AssignRoleRequest, object>(_userEndpointConfig.AssignRole,
            _userEndpointConfig.ClientName, _userEndpointConfig.BaseUrl, assignRoleRequest);
    }
}