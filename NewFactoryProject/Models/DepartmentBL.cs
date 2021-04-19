using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFactoryProject.Models
{
    public class DepartmentBL
    {
        private NewFactoryDBEntities db = new NewFactoryDBEntities();
        public List<DepartmentManager> GetAllDepartmentsManagerData()
        {
            var result = from department in db.departments
                         join employee in db.employees on department.manager equals employee.ID
                         orderby department.ID
                         select new DepartmentManager
                         {
                             ID = department.ID,
                             Department = department.name,
                             ManagerID = department.manager,
                             FullName = employee.first_name + " " + employee.last_name
                         };

            return result.ToList();
        }


        //Add Department :
        public bool Add(DepartmentManager departmentManager)
        {
            // Check if department name is in use
            int count = db.departments.Where(x => x.name == departmentManager.Department).Count();

            if (count != 0)
            {
                return false;
            }

            // Check if Employee exists
            count = db.employees.Where(x => x.ID == departmentManager.ManagerID).Count();

            if (count == 0)
            {
                return false;
            }

            // Check if Employee already is managing a department
            count = db.departments.Where(x => x.manager == departmentManager.ManagerID).Count();

            if (count != 0)
            {
                return false;
            }

            department dep = new department();
            dep.name = departmentManager.Department;
            dep.manager = departmentManager.ManagerID;

            db.departments.Add(dep);
            db.SaveChanges();

            employee emp = db.employees.Where(x => x.ID == dep.manager).First();

            emp.departmentID = dep.ID;
            db.SaveChanges();

            return true;
        }


        public bool DepartmentHasEmployees(int departmentID, int managerID)
        {
            int result = db.employees.Where(x => x.departmentID == departmentID && x.ID != managerID).Count();

            if (result == 0)
                return false;
            return true;
        }

        //Get Department In Combobox :
        public List<SelectListItem> GetDepartmentsInComboBox()
        {
            DepartmentManager departmentManager = new DepartmentManager();

            foreach (var department in db.departments)
            {
                var selectListItem = new SelectListItem();
                selectListItem.Value = department.ID.ToString();
                selectListItem.Text = department.name;

                departmentManager.Departments.Add(selectListItem);
            }

            return departmentManager.Departments;
        }

        //Edit Department :
        public bool Edit(DepartmentManager departmentManager)
        {
            // Check if employee exists
            int count = db.employees.Where(x => x.ID == departmentManager.ManagerID).Count();

            if (count == 0)
            {
                return false;
            }

    
            // Check if Employee already is managing a department
            count = db.departments.Where(x => x.manager == departmentManager.ManagerID).Count();

            department currentDep = new department();

            if (count != 0)
            {
                // Get other manager's details in order to perform swap between them
                currentDep = db.departments.Where(x => x.manager == departmentManager.ManagerID).First();
            }

            department dep = db.departments.Where(x => x.ID == departmentManager.ID).First();
            Nullable<int> currentManagerID = dep.manager;


            dep.name = departmentManager.Department;

            if (count != 0)
            {
                currentDep.manager = dep.manager;
                db.SaveChanges();
            }

            dep.manager = departmentManager.ManagerID;
            db.SaveChanges();

            employee emp = db.employees.Where(x => x.ID == departmentManager.ManagerID).First();
            emp.departmentID = departmentManager.ID;
            db.SaveChanges();

            if (count != 0)
            {
                employee currentEmp = db.employees.Where(x => x.ID == currentDep.manager).First();
                currentEmp.departmentID = currentDep.ID;
                db.SaveChanges();
            }

            return true;
        }

        //Get Department Manager :
        public DepartmentManager GetDepartmentManager(int id)
        {
            List<DepartmentManager> list = this.GetAllDepartmentsManagerData();

            foreach (DepartmentManager element in list)
            {
                if (element.ID == id)
                {
                    return element;
                }
            }

            return null;
        }

        //Delete Department :
        public void Delete(int id)
        {
            department department = db.departments.Where(x => x.ID == id).First();
           
            db.departments.Remove(department);
            db.SaveChanges();
        }
    }
}