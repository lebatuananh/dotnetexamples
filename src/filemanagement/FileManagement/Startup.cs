using System;
using System.Net.Http;
using System.Threading.Tasks;
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
                options.Authority = adminApiConfiguration.Authority;
                options.ClientId = adminApiConfiguration.ClientId;
                options.ClientSecret = adminApiConfiguration.ClientSecret;
                options.ResponseType = "code";
                options.RequireHttpsMetadata = false;
                options.SaveTokens = true;

                //Fix bypass SSL connection validate
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                options.BackchannelHttpHandler = handler;
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