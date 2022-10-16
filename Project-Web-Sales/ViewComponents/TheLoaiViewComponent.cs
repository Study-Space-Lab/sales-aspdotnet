using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Web_Sales.Data;
using Project_Web_Sales.Models;

namespace Project_Web_Sales.ViewComponents
{
    public class TheLoaiViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public TheLoaiViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var theloai = await _db.TheLoai.ToListAsync();
            return View(theloai);
        }
    }
}
