using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFactoryProject.Models
{
    public class EmployeeBL
    {
        private NewFactoryDBEntities db = new NewFactoryDBEntities();

        public List<EmployeesData> GetAllEmployeesData()
        {
            var result = from employee in db.employees
                         join department in db.departments on employee.departmentID equals department.ID
                         join employeeShift in db.employeeshifts on employee.ID equals employeeShift.employeeID
                         join shift in db.shifts on employeeShift.shiftID equals shift.ID

                         orderby employee.ID

                         select new EmployeesData
                         {
                             ID = employee.ID,
                             FirstName = employee.first_name,
                             LastName = employee.last_name,
                             StartWorkYear = employee.start_work_year,
                             DepartmentID = department.ID,
                             DepartmentName = department.name,
                             DepartmentManager = department.manager,
                             ShiftID = shift.ID,
                             Date = shift.date,
                             StartTime = shift.start_time,
                             EndTime = shift.end_time
                         };

            return result.ToList();
        }

        //Get Employees Data :
        public EmployeesData GetEmployeeData(int id)
        {
            List<EmployeesData> list = this.GetAllEmployeesData();
            return list.Where(x => x.ID == id).First();
        }

        public List<employee> GetEmployeesThatAreNotManagers()
        {
            List<employee> listOfEmployees = new List<employee>();

            foreach (var employee in db.employees)
            {
                if (db.departments.Where(x => x.manager == employee.ID).Count() == 0)
                {
                    listOfEmployees.Add(employee);
                }
            }

            return listOfEmployees;
        }

        //Get Employee :
        public List<employee> GetEmployees()
        {
            List<employee> listOfEmployees = new List<employee>();

            foreach (var employee in db.employees)
            {
                listOfEmployees.Add(employee);
            }

            return listOfEmployees;
        }

        public employee GetEmployee(int id)
        {
            return db.employees.Where(x => x.ID == id).First();
        }


       
        public Dictionary<employee, List<shift>> GetAllEmployeesShift()
        {
            List<EmployeesData> employeeData = this.GetAllEmployeesData();
            int id = -1;
            Dictionary<employee, List<shift>> employeeShiftData = new Dictionary<employee, List<shift>>();
            List<shift> employeeShift = null;

            foreach (EmployeesData element in employeeData)
            {
                if (id == element.ID)
                {

                    employeeShift.Add(new shift { ID = element.ShiftID, date = element.Date, start_time = element.StartTime, end_time = element.EndTime });
                }
                else
                {
                    if (id != -1)
                    {
                        employeeShiftData.Add(this.GetEmployee(id), employeeShift);
                    }

                    employeeShift = new List<shift>();
                    id = element.ID;

                    employeeShift.Add(new shift { ID = element.ShiftID, date = element.Date, start_time = element.StartTime, end_time = element.EndTime });
                }
            }

            return employeeShiftData;
        }


        //Search Employee (By First Name):
        public List<EmployeesData> SearchByFirstName(string name)
        {
            if (String.IsNullOrEmpty(name) == true)
            {
                return null;
            }

            List<EmployeesData> employees = GetAllEmployeesData();

            var result = employees.Where(x => x.FirstName.Contains(name));

            if (result.Count() == 0)
            {
                return null;
            }

            return result.ToList();
        }

        //Search Employee (By Last Name):
        public List<EmployeesData> SearchByLastName(string name)
        {
            if (String.IsNullOrEmpty(name) == true)
            {
                return null;
            }

            List<EmployeesData> employees = GetAllEmployeesData();

            var result = employees.Where(x => x.LastName.Contains(name));

            if (result.Count() == 0)
            {
                return null;
            }

            return result.ToList();
        }

        //Search Employee (Department Name):
        public List<EmployeesData> SearchByDepartmentName(string name)
        {
            if (String.IsNullOrEmpty(name) == true)
            {
                return null;
            }

            List<EmployeesData> employees = GetAllEmployeesData();

            var result = employees.Where(x => x.DepartmentName.Contains(name));

            if (result.Count() == 0)
            {
                return null;
            }

            return result.ToList();
        }

        //Edit Employee :
        public void Edit(EmployeesData employeesData)
        {
            employee emp = db.employees.Where(x => x.ID == employeesData.ID).First();
            emp.first_name = employeesData.FirstName;
            emp.last_name = employeesData.LastName;
            emp.start_work_year = employeesData.StartWorkYear;
            emp.departmentID = employeesData.DepartmentID;

            db.SaveChanges();
        }

        public bool Add(EmployeesData employeeData)
        {
            // Check if employee exists
            int count = db.employees.Where(x => x.ID == employeeData.ID).Count();

            if (count == 0)
            {
                return false;
            }

            // Check if Shift exists
            List<EmployeesData> data = this.GetAllEmployeesData();

            bool isShiftAlreadyExists = false;
            int shiftID = 0;

            foreach (shift element in db.shifts)
            {
                if (element.date.Year == employeeData.Date.Year && element.date.Month == employeeData.Date.Month && element.date.Day == employeeData.Date.Day
                    && element.start_time == employeeData.StartTime && element.end_time == employeeData.EndTime)
                {
                    isShiftAlreadyExists = true;
                    shiftID = element.ID;
                    break;
                }
            }

            if (isShiftAlreadyExists == false)
            {
                return false;
            }

            // Check if Employee's shift is duplicated
            List<EmployeesData> specificEmployeeData = data.Where(x => x.ID == employeeData.ID).ToList();

            foreach (EmployeesData element in specificEmployeeData)
            {
                if (element.Date.Year == employeeData.Date.Year && element.Date.Month == employeeData.Date.Month && element.Date.Day == employeeData.Date.Day
                    && element.StartTime == employeeData.StartTime && element.EndTime == employeeData.EndTime)
                {
                    return false;
                }
            }

            // Add Shift to Employee
            employeeshift empShift = db.employeeshifts.Where(x => x.employeeID == employeeData.ID).First();
            empShift.shiftID = shiftID;

            db.employeeshifts.Add(empShift);
            db.SaveChanges();

            return true;
        }

        public bool Delete(EmployeesData employeeData)
        {
            // Check if employee exists
            int count = db.employees.Where(x => x.ID == employeeData.ID).Count();

            if (count == 0)
            {
                return false;
            }

            // Check if employee is a manager, if yes, don't delete him
            count = db.departments.Where(x => x.manager == employeeData.ID).Count();

            if (count != 0)
            {
                return false;
            }

            // Remove Employee's Shifts
            var employeeShift = db.employeeshifts.Where(x => x.employeeID == employeeData.ID);

            foreach (var element in employeeShift)
            {
                db.employeeshifts.Remove(element);
            }

            db.SaveChanges();


            // Remove Employee
            var employee = db.employees.Where(x => x.ID == employeeData.ID).First();
            db.employees.Remove(employee);
            db.SaveChanges();

            return true;
        }

        public List<shift> GetAllShifts()
        {
            List<shift> shifts = new List<shift>();

            foreach (var element in db.shifts)
            {
                shifts.Add(element);
            }

            return shifts;
        }

        public List<EmployeesData> GetAllSpecificEmployeeShifts(int employeeID)
        {
            List<EmployeesData> allEmployeesData = GetAllEmployeesData();

            return allEmployeesData.Where(x => x.ID == employeeID).ToList();
        }

        
    }
}