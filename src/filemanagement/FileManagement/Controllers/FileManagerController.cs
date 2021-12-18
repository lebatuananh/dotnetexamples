using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManagement.Controllers;

[Route("file-manager")]
[Authorize]
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