using EcommerceClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EcommerceClient.Controllers
{
    public class ProductController : Controller
    {
        private string baseUrl = "https://localhost:7213/api/Product";
        private readonly HttpClient _client;

        public ProductController()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            List<Product> products = new List<Product>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/GetAllProducts").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(data);
            }
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductById(int id)
        {
            Product product;
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/GetProductById/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                product = JsonConvert.DeserializeObject<Product>(response.Content.ReadAsStringAsync().Result);
                return View(product);
            }
            return View("ErrorPage");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            try
            {
                string data = JsonConvert.SerializeObject(product);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/UpdateProduct", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Product updated successfully!";
                    return RedirectToAction("Products");
                }
                return View("ErrorPage");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View("ErrorPage");
            }
        }

        [HttpGet]
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

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            try
            {
                string data = JsonConvert.SerializeObject(product);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/CreateProduct", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Product created successfully!";
                    return RedirectToAction("Products");
                }
                return View("ErrorPage");
            }
            catch (Exception ex) 
            { 
                TempData["errorMessage"] = ex.Message;
                return View("ErrorPage");
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)

        {
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/DeleteProduct" + id).Result;
            if(response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Product deleted successfully!";
                return RedirectToAction("Products");
            }
            using (var _httpClient = new HttpClient())
            {
                try
                {
                    _httpClient.BaseAddress = new Uri(baseUrl + "/DeleteProduct");
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage getData = await _httpClient.DeleteAsync("?id=" + id);

                    if (getData.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View("ErrorPage");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
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
