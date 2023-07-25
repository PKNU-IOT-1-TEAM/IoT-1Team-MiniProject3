using NuGet.Protocol;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parking_Reservation_V0._1.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="아이디는 필수입니다.")]
        [DisplayName("User Id")]
        public string UserId { get; set; }

        [Required(ErrorMessage ="패스워드는 필수입니다.")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Remember me")]
        public bool RememberMe { get; set; }
    }
}
