using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Demo.ViewModels;

namespace Saritasa.NetForge.Demo.Controllers;

/// <summary>
/// Home controller.
/// </summary>
[Route("")]
public class HomeController : Controller
{
    private readonly AdminOptions adminOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    public HomeController(AdminOptions adminOptions)
    {
        this.adminOptions = adminOptions;
    }

    /// <summary>
    /// Display the home page.
    /// </summary>
    [HttpGet("")]
    public ActionResult Index()
    {
        var viewModel = new HomeViewModel
        {
            AdminPanelUrl = adminOptions.AdminPanelEndpoint
        };

        return View(viewModel);
    }
}