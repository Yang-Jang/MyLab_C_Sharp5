using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReservationClient.Models;
using System.Text;
using System.Diagnostics; // Dùng cho ErrorViewModel nếu có

namespace ReservationClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _apiUrl = "http://localhost:5193/api/Reservation";

        public HomeController()
        {
        }

        public async Task<IActionResult> Index()
        {
            List<Reservation> listReservation = new List<Reservation>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_apiUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        listReservation = JsonConvert.DeserializeObject<List<Reservation>>(apiResponse);
                    }
                }
            }
            return View(listReservation);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(reservation), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(_apiUrl, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(reservation);
        }
    }
}