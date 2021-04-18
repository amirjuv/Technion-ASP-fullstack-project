using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFactoryProject.Models
{
    public class DepartmentManager
    {
        
            public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
            public int ID { get; set; }
            public string Department { get; set; }
            public Nullable<int> ManagerID { get; set; }
            public string FullName { get; set; }

        
    }
}