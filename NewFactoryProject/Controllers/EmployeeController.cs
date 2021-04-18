using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewFactoryProject.Models;

namespace NewFactoryProject.Controllers
{
    public class EmployeeController : Controller
    {
        static EmployeeBL employeeBL = new EmployeeBL();
        private static string errorMessage = null;

        // GET: Employee
        public ActionResult Index()
        {
            if (Session["fullName"] != null)
            {
                return Redirect("Show");
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

        public ActionResult Show()
        {
            if (Session["fullName"] != null)
            {
                ViewBag.employeesData = employeeBL.GetAllEmployeesData();
                ViewBag.ErrorMessage = errorMessage;
                errorMessage = null;

                return View("Show");
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

        //Edit Employee :
        public ActionResult Edit(EmployeesData employeesData)
        {
            if (Session["fullName"] != null)
            {
                DepartmentBL departmentBL = new DepartmentBL();
                EmployeesData departments = new EmployeesData();
                departments.Departments = departmentBL.GetDepartmentsInComboBox();

                employeesData = employeeBL.GetEmployeeData(employeesData.ID);
                employeesData.Departments = departments.Departments;

                return View("Edit", employeesData);
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

        [HttpPost]
        public ActionResult EditEmployee(EmployeesData employeesData)
        {
            employeeBL.Edit(employeesData);

            errorMessage = null;

            int numOfActions = this.UpdateNumberOfActions();

            if (numOfActions == 0)
            {
                return Redirect("/LogIn/LogOut");
            }

            return Redirect("Show");
        }

        //Delete Employee With All Of His Data :
        public ActionResult Delete(EmployeesData employeesData)
        {
            if (Session["fullName"] != null)
            {
                bool isSuccessful = employeeBL.Delete(employeesData);

                if (isSuccessful == false)
                {
                    errorMessage = "Error: Selected Employee is a Manager, therefore cannot be deleted!";
                }
                else
                {
                    errorMessage = null;

                    int numOfActions = this.UpdateNumberOfActions();

                    if (numOfActions == 0)
                    {
                        return Redirect("/LogIn/LogOut");
                    }
                }

                return Redirect("../Show");
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }


        //Add Employee To Employee`s List :
        public ActionResult Add(EmployeesData employeesData)
        {
            if (Session["fullName"] != null)
            {
                ViewBag.AllEmloyeeShifts = employeeBL.GetAllSpecificEmployeeShifts(employeesData.ID);
                ViewBag.AllShifts = employeeBL.GetAllShifts();

                return View("Add", employeesData);
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }


        //Add Shift :
        [HttpPost]
        public ActionResult AddShift(EmployeesData employeesData)
        {
            bool isSuccessful = employeeBL.Add(employeesData);

            if (isSuccessful == false)
            {
                errorMessage = "Error: Choosen Shift does not exists\nOr Employee already assigned to it.";
            }
            else
            {
                errorMessage = null;

                int numOfActions = this.UpdateNumberOfActions();

                if (numOfActions == 0)
                {
                    return Redirect("/LogIn/LogOut");
                }
            }

            return Redirect("Show");
        }


        //Search by First Name/Last Name/Department Name :
        public ActionResult Search(string name)
        {
            if (Session["fullName"] != null)
            {
                List<EmployeesData> byFirstName = employeeBL.SearchByFirstName(name);
                ViewBag.FirstName = byFirstName;

                List<EmployeesData> byLastName = employeeBL.SearchByLastName(name);
                ViewBag.LastName = byLastName;

                List<EmployeesData> byDepartmentName = employeeBL.SearchByDepartmentName(name);
                ViewBag.DepartmentName = byDepartmentName;

                this.UpdateNumberOfActions();

                return View("Search");
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

        //< UpdateNumberOfActions > :
        private int UpdateNumberOfActions()
        {
            int numberOfActions = (int)Session["numOfActions"];
            numberOfActions = numberOfActions - 1;
            Session["numOfActions"] = numberOfActions;

            return numberOfActions;
        }
    }
}