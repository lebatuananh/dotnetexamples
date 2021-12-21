using System;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Constants;
using Shared.Identity;

namespace FileManagement;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public static string WebRootPath { get; private set; }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var adminApiConfiguration = Configuration.GetSection("Authentication").Get<AuthenticationSettings>();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = AuthenticationConsts.SignInScheme;
                options.DefaultChallengeScheme = AuthenticationConsts.OidcAuthenticationScheme;
            })
            .AddCookie(AuthenticationConsts.SignInScheme, options =>
            {
                options.Cookie.Name = adminApiConfiguration.CookieName;
                options.LoginPath = "/login.html";
                options.Events = new CookieAuthenticationEvents()
                {
                    OnValidatePrincipal = context =>
                    {
                        if (context.Properties.Items.ContainsKey(".Token.expires_at"))
                        {
                            var expire = DateTime.Parse(context.Properties.Items[".Token.expires_at"]);
                            if (expire < DateTime.Now)
                            {
                                context.ShouldRenew = true;
                                context.RejectPrincipal();
                            }
                        }

                        return Task.FromResult(0);
                    }
                };
            })
            .AddOpenIdConnect(AuthenticationConsts.OidcAuthenticationScheme, options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = adminApiConfiguration.Authority;
                options.ClientId = adminApiConfiguration.ClientId;
                options.ClientSecret = adminApiConfiguration.ClientSecret;
                options.ResponseType = "code id_token";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.SaveTokens = true;
                options.Scope.Add("roles");
                options.Scope.Add("profile");
                options.Scope.Add("file-management");
                options.Scope.Add("offline_access");
                options.Scope.Add("openid");
                options.ClaimActions.MapJsonKey("role", "role", "role");
                options.ClaimActions.MapJsonKey("scope", "scope", "scope");
                options.TokenValidationParameters.RoleClaimType = "role";

            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                policy =>
                    policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == $"http://schemas.microsoft.com/ws/2008/06/identity/claims/{JwtClaimTypes.Role}" &&
                            c.Value == adminApiConfiguration.AdministrationRole ||
                            c.Type == JwtClaimTypes.Role && c.Value == adminApiConfiguration.AdministrationRole ||
                            c.Type == $"client_{JwtClaimTypes.Role}" &&
                            c.Value == adminApiConfiguration.AdministrationRole
                        ) && context.User.HasClaim(c =>
                            c.Type == JwtClaimTypes.Scope && c.Value == adminApiConfiguration.ApiName)
                    ));
        });
        services.AddControllersWithViews();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCors(opts =>
        {
            opts.AllowAnyHeader();
            opts.AllowAnyMethod();
            opts.AllowCredentials();
            opts.SetIsOriginAllowed(origin => true);
        });
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");
        });

        WebRootPath = env.WebRootPath;
    }
}