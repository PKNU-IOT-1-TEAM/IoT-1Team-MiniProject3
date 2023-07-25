using Microsoft.AspNetCore.Mvc;

namespace Parking_Reservation_V0._1.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
