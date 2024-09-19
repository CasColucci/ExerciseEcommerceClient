using EcommerceClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace EcommerceClient.Controllers
{
    public class ProductController : Controller
    {
        private string baseUrl = "https://localhost:7213/";
        private readonly HttpClient _client;

        public ProductController()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<IActionResult> Products()
        {
            List<Product> products = new List<Product>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress).Result;
            if(response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(data);
            }
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var enumData = from Category c in Enum.GetValues(typeof(Category))
                           select new
                           {
                               ID = (int)c,
                               Name = c.ToString()
                           };
            ViewBag.EnumList = new SelectList(enumData, "ID", "Name");
            return View();
        }

        public async Task<IActionResult> CreateProduct(Product product)
        {
            using (var _httpClient = new HttpClient())
            {
                _httpClient.BaseAddress = new Uri(baseUrl + "api/");
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                product.CreatedDate = DateTime.Now;

                HttpResponseMessage getData = await _httpClient.PutAsJsonAsync("", product);

                if (getData.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("ErrorPage");
                }
            }
        }

        public IActionResult ErrorPage()
        {
            return View();
        }
    }
}
