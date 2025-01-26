using System.ComponentModel.DataAnnotations;

namespace Employee_Chat.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal Salary { get; set; }
    }
}
