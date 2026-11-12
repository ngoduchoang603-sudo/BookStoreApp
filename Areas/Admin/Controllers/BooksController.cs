using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStoreApp.Infrastructure;
using BookStoreApp.Models;

namespace BookStoreApp.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class BooksController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.Category).ToList();
            return View(books);
        }

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + "_" + System.DateTime.Now.ToString("yyMMddHHmmss") + extension;
                    book.ImageUrl = "/Images/" + fileName;
                    ImageFile.SaveAs(Path.Combine(Server.MapPath("~/Images/"), fileName));
                }

                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", book.CategoryID);
            return View(book);
        }

        public ActionResult Edit(int id)
        {
            var book = db.Books.Find(id);
            if (book == null) return HttpNotFound();
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", book.CategoryID);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + "_" + System.DateTime.Now.ToString("yyMMddHHmmss") + extension;
                    book.ImageUrl = "/Images/" + fileName;
                    ImageFile.SaveAs(Path.Combine(Server.MapPath("~/Images/"), fileName));
                }

                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", book.CategoryID);
            return View(book);
        }

        public ActionResult Delete(int id)
        {
            var book = db.Books.Find(id);
            if (book != null)
            {
                db.Books.Remove(book);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}