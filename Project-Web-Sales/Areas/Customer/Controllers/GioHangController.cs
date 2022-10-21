using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Web_Sales.Data;
using Project_Web_Sales.Models;
using System.Security.Claims;

namespace Project_Web_Sales.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GioHangController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GioHangController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang.Include("SanPham")
                .Where(gh => gh.ApplicationUserId == claim.Value)
                .ToList(),
                HoaDon = new HoaDon()
            };
            foreach (var item in giohang.DsGioHang)
            {
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                //cong don so tien trong gio hang
                giohang.Total += item.ProductPrice;
                giohang.HoaDon.Total += item.ProductPrice;
            }
            return View(giohang);
        }
        public IActionResult Giam(int giohangId)
        {
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            giohang.Quantity -= 1;
            if (giohang.Quantity == 0)
            {
                _db.GioHang.Remove(giohang);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Tang(int giohangId)
        {
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            giohang.Quantity += 1;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Xoa(int giohangId)
        {
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            _db.GioHang.Remove(giohang);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult ThanhToan()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang
                .Include("SanPham")
                .Where(gh => gh.ApplicationUserId == claim.Value)
                .ToList(),
                HoaDon = new HoaDon()
            };
            //search InFor
            giohang.HoaDon.ApplicationUser = _db.ApplicationUser.FirstOrDefault(user => user.Id == claim.Value);
            // gan thon tin tk vao hoa don
            giohang.HoaDon.Name = giohang.HoaDon.ApplicationUser.Name;
            giohang.HoaDon.Address = giohang.HoaDon.ApplicationUser.Address;
            giohang.HoaDon.PhoneNumber = giohang.HoaDon.ApplicationUser.PhoneNumber;
            foreach (var item in giohang.DsGioHang)
            {
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                giohang.HoaDon.Total += item.ProductPrice;
            }
            return View(giohang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThanhToan(GioHangViewModel giohang)
        {

            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            giohang.DsGioHang = _db.GioHang
                .Include("SanPham")
                .Where(gh => gh.ApplicationUserId == claim.Value)
                .ToList();
            giohang.HoaDon.ApplicationUserId = claim.Value;
            giohang.HoaDon.OrderDate = DateTime.Now;
            giohang.HoaDon.OrderStatus = "Đang xác nhận";
            foreach(var item in giohang.DsGioHang)
            {
                item.ProductPrice = item.Quantity * item.SanPham.Price;
                giohang.HoaDon.Total += item.ProductPrice;      
            }
            _db.HoaDon.Add(giohang.HoaDon);
            _db.SaveChanges();
            foreach(var item in giohang.DsGioHang)
            {
                ChiTietHoaDon chitiethoadon = new ChiTietHoaDon()
                {
                    SanPhamId = item.SanPhamId,
                    HoaDonId = giohang.HoaDon.Id,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity
                };
                _db.ChiTietHoaDon.Add(chitiethoadon);
                _db.SaveChanges();
            }
            //Xoa thong tin sp
            _db.GioHang.RemoveRange(giohang.DsGioHang);
            _db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
