using Dapper;
using SV21T1020880.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020880.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT  CustomerID AS UserID,
                                    Email AS UserName,
                                    CustomerName AS DisplayName,
                                    NULL AS Photo,
                                    NULL AS RoleNames
                            FROM    Customers 
                            WHERE   Email = @Email AND Password = @Password";

                var parameters = new
                {
                    Email = username,
                    Password = password
                };

                // Thực hiện truy vấn và lấy tài khoản đầu tiên
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        public bool ChangePassword(string username, string oldpassword, string newpassword)
        {
            bool result = false;

            // Xác thực thông tin cũ
            UserAccount userAccount = Authorize(username, oldpassword);
            if (userAccount == null) return result;

            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE  Customers
                            SET     Password = @Password
                            WHERE   Email = @Email";

                var parameters = new
                {
                    Email = username,
                    Password = newpassword
                };

                // Thực thi câu lệnh cập nhật
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            }

            return result;
        }

        public Customer? GetCustomerById(int customerId)
        {
            using (var connection = OpenConnection())
            {
                string query = @"SELECT * FROM Customers WHERE CustomerID = @CustomerId";
                return connection.QueryFirstOrDefault<Customer>(query, new { CustomerId = customerId });
            }
        }

    }
}
