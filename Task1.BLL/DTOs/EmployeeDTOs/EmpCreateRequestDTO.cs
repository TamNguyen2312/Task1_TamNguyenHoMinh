using System.ComponentModel.DataAnnotations;
using Task1.BLL.Helper.Extension.Employees;
using Task1.BLL.Helper.Validation.EmpValid;

namespace Task1.BLL.DTOs.EmployeeDTOs
{
    public class EmpCreateRequestDTO
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(20, ErrorMessage = "First name cannot be longer than 20 characters")]
        public string Fname { get; set; } = null!;

        [StringLength(1, ErrorMessage = "Middle initial cannot be longer than 1 character")]
        public string? Minit { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(30, ErrorMessage = "Last name cannot be longer than 30 characters")]
        public string Lname { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required")]
        [EmpGenderValid(typeof(EmpGender))]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Job ID is required")]
        public short JobId { get; set; } 

        [Range(1, 255, ErrorMessage = "Job level must be between 1 and 255")]
        public byte? JobLvl { get; set; } 

        [Required(ErrorMessage = "Publisher ID is required")]
        [StringLength(4, ErrorMessage = "Publisher ID cannot be longer than 4 characters")]
        public string PubId { get; set; } = null!;

        [Required(ErrorMessage = "Hire date is required")]
        public DateTime HireDate { get; set; } 
    }
}
