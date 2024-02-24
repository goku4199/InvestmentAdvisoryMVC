using System.ComponentModel.DataAnnotations;

namespace InvestmentAdvisory.Models
{
    public class Advisor
    {
        [Key]
        public int AdvisorId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Specialty { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
