using SV21T1020880.DomainModels;

namespace SV21T1020880.DataLayers
{

    /// <summary>
    /// DInh nghia cac phep xu lys du lieu thuong dung tren bang (Customer, Employee,...)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tim kiem va lay danh sach du lieu (kieu la T) duoi dang co phan trang
        /// </summary>
        /// <param name="page"> Trang can hien thi </param>
        /// <param name="pageSize"> So dong duoc hien thi tren moi trang (bang 0 neu ko phan trang) </param>
        /// <param name="searchValue"> Gia tri can tim kiem (neu rong thi hien thi het toan bo du lieu) </param>
        /// <returns></returns>
        List<T> List(int page = 0, int pageSize = 0, string searchValue = "");
        /// <summary>
        /// Dem so luong dong du lieu tim kiem duoc
        /// </summary>
        /// <param name="searchValue"> Gia tri can tim (chuoi rong neu dem tren toan bo du lieu) </param>
        /// <returns></returns>
        int Count(string searchValue);
        /// <summary>
        /// Lấy một bảng ghi dữ liệu dựa vào khóa chính/id (Trả về null nếu dữ liệu không tồn tại) 
        /// </summary>
        /// <param name="id">  </param>
        /// <returns></returns>
        T? Get(int id);
        /// <summary>
        /// Bổ sung một bảng ghi vào CSDL. Hàm trả về Id của dữ liệu vừa bổ sung.(nếu có)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
        /// <summary>
        /// Cập nhật một bảng ghi dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);
        /// <summary>
        /// Xóa một bảng ghi dữ liệu dựa vào khóa chính/Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiểm tra xem một bảng ghi dữ liệu có khóa là id hiện đang có dữ liệu tham chiếu ở bảng khác hay không.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}
//Ctrl + M + O
//Ctrl + M + L