using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV21T1020880.DomainModels;

namespace SV21T1020880.DataLayers
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu liên quan đến tài khoản
    /// </summary>
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Kiểm tra xem tên đăng nhập và mật khẩu của người dùng có hợp lệ ~
        /// Nếu hợp lệ thì trả về thông tin của người dùng và ngược lại thì trả về null
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string username, string password);

        /// <summary>
        /// Đổi mật khẩu của người dùng
        /// </summary>
        /// <param name="username"></param>Tên đăng nhập
        /// <param name="oldpassword"></param>Mật khẩu cũ
        /// <param name="newpassword"></param>Mật khẩu mới
        /// <returns></returns>
        bool ChangePassword(string username, string oldpassword, string newpassword);
        Customer? GetCustomerById(int customerId);
    }
}
