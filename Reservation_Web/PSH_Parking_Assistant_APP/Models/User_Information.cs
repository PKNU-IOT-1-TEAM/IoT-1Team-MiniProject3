using System.ComponentModel.DataAnnotations;

namespace PSH_Parking_Assist_APP.Models
{
    public class User_Information
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Login_ID { get; set; }
        public string Login_PW { get; set;}
        public string NFC { get; set; }
        public int Authority { get; set; }
    }
}
