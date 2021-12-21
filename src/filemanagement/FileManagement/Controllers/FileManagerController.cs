using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;

namespace FileManagement.Controllers;

[Route("file-manager")]
[Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
public class FileManagerController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Route("browse")]
    public IActionResult Browse()
    {
        return View();
    }
}