using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public bool IsSupervisor { get; set; }
        public int DepartmentId { get; set; }
        public Department department { get; set; }
        public Computer computer { get; set; }
    }
}
