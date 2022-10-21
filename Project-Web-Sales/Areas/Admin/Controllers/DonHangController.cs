using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Web_Sales.Data;
using Project_Web_Sales.Models;

namespace Project_Web_Sales.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DonHangController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<HoaDon> hoadon = _db.HoaDon.Include("ApplicationUser").ToList();
            return View(hoadon);
        }
    }
}
