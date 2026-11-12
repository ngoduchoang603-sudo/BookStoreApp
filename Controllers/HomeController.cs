using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using BookStoreApp.Models;

namespace BookStoreApp.Controllers
{
    public class HomeController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        public ActionResult Index(string searchString, int? categoryId)
        {
            var books = db.Books.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                books = books.Where(b => b.CategoryID == categoryId);
            }

            ViewBag.Categories = db.Categories.ToList();
            return View(books.ToList());
        }

        public ActionResult Details(int id)
        {
            var book = db.Books.Include(b => b.Category).Include(b => b.Reviews.Select(r => r.User)).FirstOrDefault(b => b.BookID == id);
            if (book == null) return HttpNotFound();
            return View(book);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddReview(int bookId, int rating, string comment)
        {
            int userId = (int)Session["UserID"];
            Review review = new Review
            {
                BookID = bookId,
                UserID = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = System.DateTime.Now
            };
            db.Reviews.Add(review);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = bookId });
        }
    }
}