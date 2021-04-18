using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFactoryProject.Models
{
    public class ShiftBL
    {
        private NewFactoryDBEntities db = new NewFactoryDBEntities();

        public List<EmployeesData> GetAllEmployeeShifts()
        {
            EmployeeBL employeeBL = new EmployeeBL();
            List<EmployeesData> list = employeeBL.GetAllEmployeesData();
            List<EmployeesData> orderedList = list.OrderBy(x => x.ShiftID).ToList();

            return orderedList;
        }


        //Add Shift :
        public void Add(EmployeesData data)
        {
            shift shift = new shift();
            shift.date = data.Date;
            shift.start_time = data.StartTime;
            shift.end_time = data.EndTime;

            db.shifts.Add(shift);
            db.SaveChanges();
        }
    }
}