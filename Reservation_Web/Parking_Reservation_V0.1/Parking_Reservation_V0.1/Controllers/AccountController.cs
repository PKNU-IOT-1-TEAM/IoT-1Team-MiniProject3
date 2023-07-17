using Parking_Reservation_V0._1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Composition.Convention;

namespace Parking_Reservation_V0._1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            // 생성자 마법사로 만드세요.
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 비동기가 아니면 return값은 IActionResult
        // 비동기가 되면 Task<IActionResult>
        public async Task<IActionResult> Register(RegisterModel model)
        {
            ModelState.Remove("ConfirmPassword");       // 핸드폰 번호는 입력값 검증에서 제거
            if (ModelState.IsValid)      // 데이터를 제대로 입력해서 검증 성공한다면
            {
                // ASP.NET user - aspnetusers 테이블에 데이터 넣기 위해서 매핑되는 인스턴스를 생성
                var user = new IdentityUser()
                {
                    UserName = model.UserId,
                    PasswordHash = model.Password,
                    PhoneNumber = model.CarId
                };
                // aspnetusers 테이블에 사용자 데이터를 대입
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 회원가입을 성공했다면 로그인한 뒤 localhost:7181/Home/Index로 가라
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    // TODO : 회원가입 후 토스트 메시지 띄우기
                    TempData["succeed"] = "회원가입 성공";
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model); // 회원가입을 실패하면 그화면 그대로 유지
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // 파라미터 순서 : 아이디, 패스워드. isPersistent = RememberMe, 실패할 때 계정 잠그기
                var result = await _signInManager.PasswordSignInAsync(model.UserId, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // TODO : 로그인 후 토스트 메시지 띄우기
                    TempData["succeed"] = "로그인 성공";

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "로그인 실패");
                TempData["error"] = "로그인 실패";

            }
            return View(model);     // 입력검증이 실패하면 화면에 그대로 대기
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            // TODO : 로그아웃 후 토스트 메시지 띄우기
            TempData["succeed"] = "로그아웃 성공";
            return RedirectToAction("Index", "Home");
        }
    }
}
