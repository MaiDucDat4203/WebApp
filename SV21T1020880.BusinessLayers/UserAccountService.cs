using SV21T1020880.DataLayers;
using SV21T1020880.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020880.BusinessLayers
{
    public class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;

        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;

            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);

        }

        public static UserAccount? Authorize(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
                return employeeAccountDB.Authorize(username, password);
            else
                return customerAccountDB.Authorize(username, password);
        }

        public static bool ChangePassword(UserTypes userTypes, string username, string oldpassword ,string newpassword)
        {
            if (userTypes == UserTypes.Employee)
                return employeeAccountDB.ChangePassword(username, oldpassword, newpassword);
            else 
                return customerAccountDB.ChangePassword(username, oldpassword, newpassword);
        }

        public static Customer? GetCustomerDetails(int customerId)
        {
            return customerAccountDB.GetCustomerById(customerId);
        }

    }
    public enum UserTypes
    {
        Employee,
        Customer
    }
}
