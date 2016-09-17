using CVF.App.Manager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CVF.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly PluginManager manager;

        public HomeController(PluginManager manager, IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.manager = manager;
        }

        public IActionResult Index()
        {
            return View(this.manager.Plugins);
        }

        public IActionResult Plugin(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var plugin = this.manager.GetPlugin(name);
                if (plugin != null)
                {
                    return View(plugin);
                }
            }

            return NotFound();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
