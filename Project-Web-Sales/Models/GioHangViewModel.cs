namespace Project_Web_Sales.Models
{
    public class GioHangViewModel
    {
        public IEnumerable<GioHang> DsGioHang { get; set; }
        public double Total { get; set; }
        public HoaDon HoaDon { get; set; }
    }
}
