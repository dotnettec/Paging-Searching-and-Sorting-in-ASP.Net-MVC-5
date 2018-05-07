using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeManagement.Model
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CityID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
    public class City
    {
        [Key]        
        public int CityID { get; set; }       
        public string CityName { get; set; }

    }
}