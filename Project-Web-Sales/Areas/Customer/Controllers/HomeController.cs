using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Web_Sales.Data;
using Project_Web_Sales.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Project_Web_Sales.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<SanPham> sanpham = _db.SanPham.Include(sp => sp.TheLoai).ToList();
            return View(sanpham);
        }
        public IActionResult Shop()
        {
            return View();
        }
        public IActionResult Detail(int sanphamId)
        {
            // get info give giohang
            GioHang giohang = new GioHang()
            {
                SanPhamId = sanphamId,
                Quantity = 1,
                SanPham = _db.SanPham.Include("TheLoai").FirstOrDefault(item => item.Id == sanphamId),


            };
            return View(giohang);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Detail(GioHang giohang)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            // dua thong tin vao gio hang
            giohang.ApplicationUserId = claim.Value;

            //database
            
            //kiem tra trung sp
            var giohangdb = _db.GioHang.FirstOrDefault(sp => sp.SanPhamId == giohang.SanPhamId && sp.ApplicationUserId == giohang.ApplicationUserId);
            if(giohangdb == null) // neu khong co sp trong gio hang
            {//them vao db
                _db.GioHang.Add(giohang);
            }
            else
            {//da co sp va trung ng mua thi cong so luong sp
                giohangdb.Quantity += giohang.Quantity;
            }
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Checkout()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}