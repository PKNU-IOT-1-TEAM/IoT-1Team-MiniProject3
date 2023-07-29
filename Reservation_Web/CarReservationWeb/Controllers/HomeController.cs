using CarReservationWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarReservationWeb.Controllers
{
	public class HomeController : Controller
	{
		private Team1IotContext db;

		public HomeController(Team1IotContext db)
		{
			this.db = db;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(AccountParking account)
		{
			if (ModelState.IsValid)
			{
				db.AccountParkings.Add(account);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View();
		}

		public ActionResult Login()
		{
			return RedirectToAction("Reservation", "Home");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(AccountParking account)
		{
			if (ModelState.IsValid)
			{
				using (db)
				{
					var user = db.AccountParkings.FirstOrDefault(u => u.Id.Equals(account.Id) && u.Password.Equals(account.Password));
					if (user != null)
					{
						HttpContext.Session.SetInt32("USER_LOGIN_KEY", user.IdX);
						return RedirectToAction("Reservation", "Home");
					}
				}
			}
			else
			{
				ModelState.AddModelError("", "Invalid Credentials");
			}
			return RedirectToAction("Index");
		}

        [HttpPost]
        public ActionResult Submit(AccountParking account, ParkingStatus status)
        {	
			// nfc 값, parking_status 모두 바꾸고 저장
            if (ModelState.IsValid)
            {
                db.ParkingStatuses.Add(status);
                db.SaveChanges();
				return View();
            }
            return View();
        }

        public ActionResult Reservation()
		{
			return View();
		}

		public ActionResult Personal()
		{
			return View();
		}


	}
}