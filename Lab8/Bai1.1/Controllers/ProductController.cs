using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Bai1_._1.Models;
using System.Text;

namespace Bai1_._1.Controllers
{
    public class ProductController : Controller
    {
        private readonly string _baseUrl = "http://localhost:5005/api/Products";
        private readonly HttpClient _client = new HttpClient();

        public async Task<IActionResult> Index()
        {
            List<Product> listProducts = new List<Product>();

            HttpResponseMessage response = await _client.GetAsync(_baseUrl);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Product>>>(jsonString);

                if (apiResponse != null && apiResponse.Success)
                {
                    listProducts = apiResponse.Data;
                    ViewBag.Message = apiResponse.Message; 
                }
            }

            return View(listProducts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(_baseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            string errorResponse = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Lỗi API ({response.StatusCode}): {errorResponse}";
            ViewBag.Error = "Thêm thất bại!";
            return View(product);
        }


        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage response = await _client.GetAsync(_baseUrl + "/" + id);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<Product>>(jsonString);

                if (apiResponse != null && apiResponse.Success)
                {
                    return View(apiResponse.Data);
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync(_baseUrl + "/" + id, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Cập nhật thất bại!";
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync(_baseUrl + "/" + id);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Xóa thất bại!";
            return RedirectToAction("Index");
        }

    }
}