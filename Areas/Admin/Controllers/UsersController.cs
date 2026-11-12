using BookStoreApp.Infrastructure;
using BookStoreApp.Models;
using System.Web.Mvc;
using System.Linq;
[AdminAuthorize]
public class UsersController : Controller
{
    private BookStoreDBEntities db = new BookStoreDBEntities();

    public ActionResult Index()
    {
        var users = db.Users.OrderByDescending(u => u.UserID).ToList();
        return View(users);
    }
}