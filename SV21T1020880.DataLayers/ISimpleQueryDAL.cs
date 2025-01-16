namespace SV21T1020880.DataLayers
{
    /// <summary>
    /// Dinh nghia chuc nang truy van don gian
    /// </summary>
    public interface ISimpleQueryDAL<T> where T : class
    {
        /// <summary>
        /// Truy van toan bo du lieu Provinces
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
