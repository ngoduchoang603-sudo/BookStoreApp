using System.Linq;
using System.Web.Mvc;
using BookStoreApp.Infrastructure;
using BookStoreApp.Models;

namespace BookStoreApp.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class DashboardController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        public ActionResult Index()
        {
            ViewBag.TotalRevenue = db.Orders.Where(o => o.Status == "Hoàn thành").Sum(o => (decimal?)o.TotalAmount) ?? 0;
            ViewBag.TotalOrders = db.Orders.Count();
            ViewBag.TotalBooks = db.Books.Count();
            ViewBag.TotalUsers = db.Users.Count(u => u.Role == "Customer");

            var recentOrders = db.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToList();
            return View(recentOrders);
        }
    }
}