using DataAnnotationsExtensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RecrutingApi.Model
{
    public class Authentciate
    {
        [Email(ErrorMessage = "The email address is not valid")]
        [Required]
        public string emailAddress { get; set; }

        [Required]
        [PasswordPropertyText]
        public string password { get; set; }
    }
}