using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Parking_Reservation_V0._1.Models
{
    public class RegisterModel 
    {
        //[Key]
        //public int Id { get; set; }

        [Required(ErrorMessage ="아이디는 필수입니다.")]
        [DisplayName("UserId")]
        public string UserId { get; set; }

        [Required(ErrorMessage ="패스워드는 필수입니다.")]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "패스워드 확인은 필수입니다.")]
        [DataType(DataType.Password)]
        [DisplayName("Password Confirm")]
        [Compare("Password", ErrorMessage = "패스워드가 일치하지 않습니다. 다시 입력하세요.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage ="차 번호를 입력하세요.")]
        public string CarId { get; set; }

        
    }
}
