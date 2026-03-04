using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; // Slide 8 - Trang 12
using FastFoodOnline.Web.Models;
using System.Text;

namespace FastFoodOnline.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FastFoodManagerController : Controller
    {
        private readonly string _baseUrl = "http://localhost:5006/api/FastFoodApi";

        // 1. GET: Lấy danh sách (Slide 8 - Trang 13)
        public async Task<IActionResult> Index()
        {
            List<Food> foodList = new List<Food>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_baseUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        foodList = JsonConvert.DeserializeObject<List<Food>>(apiResponse);
                    }
                }
            }
            return View(foodList);
        }

        // 2. CREATE (GET): Hiển thị form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREATE (POST): Gửi dữ liệu lên API (Slide 8 - Trang 21)
        [HttpPost]
        public async Task<IActionResult> Create(Food food)
        {
            // Gán giá trị mặc định nếu null
            if (string.IsNullOrEmpty(food.ImageUrl)) food.ImageUrl = "/images/placeholder.jpg";
            
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(food), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(_baseUrl, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            ModelState.AddModelError("", "Lỗi khi gọi API thêm mới.");
            return View(food);
        }

        // 4. EDIT (GET): Lấy chi tiết món (Slide 8 - Trang 26)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Food food = new Food();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{_baseUrl}/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        food = JsonConvert.DeserializeObject<Food>(apiResponse);
                    }
                }
            }
            return View(food);
        }

        // 5. EDIT (POST): Cập nhật món (Slide 8 - Trang 27)
        [HttpPost]
        public async Task<IActionResult> Edit(Food food)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(food), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync($"{_baseUrl}/{food.Id}", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(food);
        }

        // 6. DELETE: Xóa món
        public async Task<IActionResult> Delete(int id)
        {
            using (var httpClient = new HttpClient())
            {
                await httpClient.DeleteAsync($"{_baseUrl}/{id}");
            }
            return RedirectToAction("Index");
        }
    }
}