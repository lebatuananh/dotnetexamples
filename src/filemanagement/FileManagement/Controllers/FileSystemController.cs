using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FileManagement.Controllers;

[Route("el-finder/file-system")]
public class FileSystemController : Controller
{
    [Route("connector")]
    public async Task<IActionResult> Connector()
    {
        var connector = GetConnector();
        return await connector.ProcessAsync(Request);
    }

    [Route("thumb/{hash}")]
    public async Task<IActionResult> Thumbs(string hash)
    {
        var connector = GetConnector();
        return await connector.GetThumbnailAsync(HttpContext.Request, HttpContext.Response, hash);
    }

    private Connector GetConnector()
    {
        var driver = new FileSystemDriver();

        var absoluteUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host);
        var uri = new Uri(absoluteUrl);

        var root = new RootVolume(
            PathHelper.MapPath("~/files"),
            $"{uri.Scheme}://{uri.Authority}/files/",
            $"{uri.Scheme}://{uri.Authority}/el-finder/file-system/thumb/")
        {
            IsReadOnly = !User.Identity.IsAuthenticated,
            IsLocked = false,
            Alias = "files",
            AccessControlAttributes = new HashSet<NamedAccessControlAttributeSet>
            {
                new(PathHelper.MapPath("~/files/public"))
                {
                    Write = true,
                    Locked = false,
                    Read = true
                },
                new(PathHelper.MapPath("~/files/private"))
                {
                    Read = User.IsInRole("Administrator"),
                    Write = User.IsInRole("Administrator"),
                    Locked = !User.IsInRole("Administrator")
                }
            }
        };

        driver.AddRoot(root);

        return new Connector(driver)
        {
            // This allows support for the "onlyMimes" option on the client.
            MimeDetect = MimeDetectOption.Internal
        };
    }
}