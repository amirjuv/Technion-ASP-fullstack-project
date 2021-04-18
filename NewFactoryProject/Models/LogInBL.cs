using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFactoryProject.Models
{
    public class LogInBL
    {
        private NewFactoryDBEntities db = new NewFactoryDBEntities();
        private int maxNumberOfActions = 15;


        public enum Status
        {
            Authorized,
            AuthorizedButReachMaximumActionsPerDay,
            Unauthorized
        }
         
        //Get User ID:
        private int GetID(string username, string password)
        {
            var result = db.users.Where(x => x.user_name == username && x.password == password).ToList();

            if (result.Count != 0)
            {
                return result[0].ID;
            }

            return -1;
        }

        //Get User Full Name:
        private string GetFullName(int id)
        {
            var result = db.users.Where(x => x.ID == id).ToList();

            if (result.Count != 0)
            {
                return result[0].full_name;
            }

            return null;
        }

        //Get User Number OF Actions:
        private int GetNumOfActions(int id)
        {
            var result = db.users.Where(x => x.ID == id).ToList();

            if (result.Count != 0)
            {
                return result[0].num_of_action;
            }

            return -1;
        }

        //Get Last LogIn (date):
        private Nullable<DateTime> GetLastLogIn(int id)
        {
            var result = db.users.Where(x => x.ID == id).ToList();

            if (result.Count != 0)
            {
                return (Nullable<DateTime>)result[0].last_login;
            }

            return null;
        }

        //Update Last LogIn (date):
        private void UpdateLastLogIn(int id)
        {
            user user = db.users.Where(x => x.ID == id).First();
            user.last_login = System.DateTime.Now;
            db.SaveChanges();
        }

        //Update Number Of Actions:
        public void UpdateNumOfActions(int id, int num)
        {
            user user = db.users.Where(x => x.ID == id).First();
            user.num_of_action = num;
            db.SaveChanges();
        }

        public Status LogIn(string username, string password, out int id, out string fullName, out int numOfActions)
        {
            id = this.GetID(username, password);
            numOfActions = this.GetNumOfActions(id);
            fullName = this.GetFullName(id);
            Nullable<DateTime> lastLogin = this.GetLastLogIn(id);

            Status status = Status.Unauthorized; // unauthorized

            if (id > 0)
            {
                // Authorized
                status = Status.Authorized;

                // Authorized user never was logged in before to the system
                if (lastLogin == null)
                {
                    this.UpdateLastLogIn(id);
                    this.UpdateNumOfActions(id, this.maxNumberOfActions);
                    numOfActions = this.maxNumberOfActions;
                }

                if (numOfActions == 0)
                {
                    // Num of Maximum actions per day for Authorized user has been reached 
                    status = Status.AuthorizedButReachMaximumActionsPerDay;

                    // Check if a new day has begun, if yes reset num of operations
                    DateTime currentDayTime = System.DateTime.Now;
                    int dayDifference = (currentDayTime - (DateTime)lastLogin).Days;

                    if (dayDifference >= 1)
                    {
                        numOfActions = this.maxNumberOfActions;
                        status = Status.Authorized;
                        this.UpdateLastLogIn(id);
                        this.UpdateNumOfActions(id, this.maxNumberOfActions);
                    }
                }
                else
                {
                    this.UpdateLastLogIn(id);
                }
            }

            return status;
        }

        public void LogOut(int id, int num)
        {
            this.UpdateNumOfActions(id, num);
        }
    }
}