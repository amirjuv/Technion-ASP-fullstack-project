using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFactoryProject.Models
{
    public class EmployeesData
    {
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();

        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int StartWorkYear { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> DepartmentManager { get; set; }
        public int ShiftID { get; set; }

        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }
}