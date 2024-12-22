using DataAnnotationsExtensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static RecrutingApi.Model.Role;

namespace RecrutingApi.Model
{
    public class Users
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int uId { get; set; }

        [Email(ErrorMessage = "The email address is not valid")]
        [Required]
        public string emailAddress { get; set; }

        [Required]
        [PasswordPropertyText]
        public string password { get; set; }

        [Required]
        public role roles { get; set; }

        public string? usrAuthKey { get; set; }

        public DateTime? keyExpireTime { get; set; }
    }
}