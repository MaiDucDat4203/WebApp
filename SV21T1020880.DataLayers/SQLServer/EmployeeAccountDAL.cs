using Dapper;
using Microsoft.Data.SqlClient;
using SV21T1020880.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020880.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt cho tài khoản của nhân viên
    /// </summary>
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public UserAccount Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID as UserID,
                                    Email as UserName,
                                    FullName as DisplayName,
                                    Photo,
                                    RoleNames
                            from    Employees where Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassWord"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            bool result = false;
            // so sánh mật khẩu cũ trong database
            UserAccount userAccount = Authorize(userName, oldPassword);
            if (userAccount == null) return result;
            using (var connection = OpenConnection())
            {
                // cập nhật mật khẩu mới
                var sql = @"UPDATE  Employees
                            SET     Password = @Password
                            WHERE   Email = @Email";

                var parameters = new
                {
                    Email = userName,
                    Password = newPassword
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? GetCustomerById(int customerId)
        {
            throw new NotImplementedException();
        }
    }
}
