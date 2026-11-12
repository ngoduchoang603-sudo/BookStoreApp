using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using BookStoreApp.Helpers;
using BookStoreApp.Models;

namespace BookStoreApp.Controllers
{
    public class AccountController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: Register
        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var check = db.Users.FirstOrDefault(s => s.Email == user.Email);
                if (check == null)
                {
                    user.Password = SecurityHelper.HashPassword(user.Password);
                    user.Role = "Customer";
                    user.CreatedAt = System.DateTime.Now;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                ViewBag.Error = "Email này đã được sử dụng!";
            }
            return View(user);
        }

        // GET: Login
        public ActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            string hashedPassword = SecurityHelper.HashPassword(password);
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Email, false);
                Session["User"] = user;
                Session["UserID"] = user.UserID;
                Session["Role"] = user.Role;
                Session["UserName"] = user.FullName;

                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Email hoặc mật khẩu không đúng!";
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Xem Lịch sử Đơn hàng
        [Authorize]
        public ActionResult OrderHistory()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login");
            int userId = (int)Session["UserID"];
            var orders = db.Orders.Where(o => o.UserID == userId).OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }

        // Chi tiết Đơn hàng Khách xem
        [Authorize]
        public ActionResult OrderDetail(int id)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login");
            int userId = (int)Session["UserID"];
            var order = db.Orders.FirstOrDefault(o => o.OrderID == id && o.UserID == userId);
            if (order == null) return HttpNotFound();
            return View(order);
        }
    }
}