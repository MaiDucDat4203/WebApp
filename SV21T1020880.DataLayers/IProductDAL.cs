﻿using SV21T1020880.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020880.DataLayers
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu liên quan đến mặt hàng
    /// </summary>
    public interface IProductDAL
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Sô dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Tên mặt hàng cần tìm (chuỗi rổng nếu không tìm kiếm</param>
        /// <param name="categoryID">Mã loại háng</param>
        /// <param name="supplierID"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        IList<Product> List(int page = 1, int pageSize = 0,
                            string searchValue = "", int categoryID = 0, int supplierID = 0,
                            decimal minPrice = 0, decimal maxPrice = 0);
        /// <summary>
        /// Đếm số lượng mặt hàng tìm kiếm được
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
        /// <summary>
        /// Lấy thông tin mặt hàng theo mã hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        Product? Get(int productID);
        /// <summary>
        /// Bổ sung mặt hàng mới (hàm trả về mã của mặt hàng được bổ sung)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(Product data);
        /// <summary>
        /// Cập nhật thông tin mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Product data);
        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        bool Delete(int productID);
        /// <summary>
        /// Kiểm tra xem mặt hàng hiện có đơn hàng liên quan hay không?
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        bool IsUsed(int productID);
        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng (sắp xếp theo thứ tự của DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductPhoto> ListPhotos(int productID);
        /// <summary>
        /// Lấy thông tin 1 ảnh dựa vào ID
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        IList<ProductPhoto> ListPhotos();
        IList<ProductAttribute> ListAttributes();
        ProductPhoto? GetPhoto(long photoID);
        /// <summary>
        /// Bổ sung 1 ảnh cho mặt hàng (hàm trả về mã của ảnh được bổ sung)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        long AddPhoto(ProductPhoto data);
        /// <summary>
        /// Cập nhật ảnh của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool UpdatePhoto(ProductPhoto data);
        /// <summary>
        /// Xóa ảnh của mặt hàng
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        bool DeletePhoto(long photoID);
        /// <summary>
        /// Lấy danh sách các thuộc tính của mặt hàng, sắp xếp theo thứ tự của DisplayOrder
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductAttribute> ListAttributes(int productID);
        /// <summary>
        /// Lấy thông tin của thuộc tính theo mã thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        ProductAttribute? GetAttribute(long attributeID);
        /// <summary>
        /// Bổ sung thuộc tính cho mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        long AddAttribute(ProductAttribute data);
        /// <summary>
        /// Cập nhật thuộc tính của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool UpdateAttribute(ProductAttribute data);
        /// <summary>
        /// Xóa thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        bool DeleteAttribute(long attributeID);
    }
}

