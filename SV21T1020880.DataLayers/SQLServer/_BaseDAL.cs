using Microsoft.Data.SqlClient;

namespace SV21T1020880.DataLayers.SQLServer
{
    /// <summary>
    /// Lớp cơ sở (cha) của các lớp cài đặt các phép xử lý dữ liệu trên CSDL trên SQL Sever
    /// </summary>
    public abstract class BaseDAL
    {
        /// <summary>
        /// Chuỗi tham số kết nối đến CSDL SQL Sever
        /// </summary>
        protected string connectionString = "";
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Tạo và mở một kết nối đến cơ sở dữ liệu (SQL Sever)
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
