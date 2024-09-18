using Microsoft.AspNetCore.Mvc;

namespace EcommerceClient.Controllers
{
    public class ProductController : Controller
    {
        private string BaseUrl = "https://localhost:7213/";
        public IActionResult Index()
        {
            return View();
        }
    }
}
