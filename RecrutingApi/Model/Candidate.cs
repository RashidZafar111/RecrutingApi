using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace RecrutingApi.Model
{
    public class Candidate
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "the length should not be more then 255")]
        public string firstName { get; set; }

        [StringLength(255, ErrorMessage = "the length should not be more then 255")]
        public string middleName { get; set; }

        [StringLength(255, ErrorMessage = "the length should not be more then 255")]
        public string lastName { get; set; }

        [StringLength(255, ErrorMessage = "the length should not be more then 255")]
        [Email(ErrorMessage = "The email address is not valid")]
        [Required]
        public string emailAddress { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "the length should not be more then 15")]
        public string mobileNumber { get; set; }

        [Required]
        [StringLength(56, ErrorMessage = "the length should not be more then 56")]
        public string country { get; set; }

        [Required]
        [StringLength(70, ErrorMessage = "the length should not be more then 70")]
        public string city { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "the length should not be more then 10")]
        public string postcode { get; set; }

        [Required]
        public int minSalary { get; set; }

        [Required]
        public int maxSalary { get; set; }

        [Required]
        [StringLength(70, ErrorMessage = "the length should not be more then 70")]
        public string title { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "the length should not be more then 255")]
        public string description { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "the length should not be more then 255")]
        public string hightestEducation { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "the length should not be more then 255")]
        public string professionalCertificate { get; set; }

        public DateTime? recordCreateDateTime { get; set; }

        public DateTime? recordUpdateDateTime { get; set; }

        public string? resumefName { get; set; }
    }
}