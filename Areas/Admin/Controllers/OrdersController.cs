using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BookStoreApp.Infrastructure;
using BookStoreApp.Models;

namespace BookStoreApp.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class OrdersController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.User).OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }

        public ActionResult Details(int id)
        {
            var order = db.Orders.Include(o => o.OrderDetails.Select(d => d.Book)).FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();
            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int orderId, string status)
        {
            var order = db.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = orderId });
        }
    }
}