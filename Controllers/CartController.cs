using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BookStoreApp.Models;

namespace BookStoreApp.Controllers
{
    public class CartController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        public List<CartItem> GetCart()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        public ActionResult AddToCart(int id)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.FirstOrDefault(p => p.BookID == id);

            if (item == null)
            {
                Book book = db.Books.Find(id);
                if (book != null)
                {
                    cart.Add(new CartItem
                    {
                        BookID = book.BookID,
                        Title = book.Title,
                        ImageUrl = book.ImageUrl,
                        Price = book.Price,
                        Quantity = 1
                    });
                }
            }
            else
            {
                item.Quantity++;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            var cart = GetCart();
            ViewBag.TotalAmount = cart.Sum(i => i.Total);
            return View(cart);
        }

        [HttpPost]
        public ActionResult UpdateCart(int id, int quantity)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.FirstOrDefault(p => p.BookID == id);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
            }
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(int id)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.FirstOrDefault(p => p.BookID == id);
            if (item != null) cart.Remove(item);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Count == 0) return RedirectToAction("Index");
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string fullName, string phone, string address, string paymentMethod)
        {
            var cart = GetCart();
            if (cart == null || cart.Count == 0) return RedirectToAction("Index");

            int userId = (int)Session["UserID"];
            if (string.IsNullOrEmpty(paymentMethod)) paymentMethod = "COD";

            decimal totalAmount = cart.Sum(i => i.Total);

            // 1. Tạo đơn hàng trong DB
            Order order = new Order
            {
                UserID = userId,
                OrderDate = System.DateTime.Now,
                TotalAmount = totalAmount,
                Status = (paymentMethod == "COD") ? "Chờ xác nhận" : "Chờ thanh toán",
                ShippingAddress = $"{fullName} - {phone} - {address}",
                Phone = phone,
                PaymentMethod = paymentMethod
            };

            db.Orders.Add(order);
            db.SaveChanges(); // Lưu để lấy OrderID

            // 2. Tạo chi tiết đơn hàng & Trừ tồn kho
            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    BookID = item.BookID,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };
                db.OrderDetails.Add(detail);

                var book = db.Books.Find(item.BookID);
                if (book != null) book.StockQuantity -= item.Quantity;
            }
            db.SaveChanges();

            // 3. Xóa giỏ hàng
            Session["Cart"] = null;

            // 4. Phân luồng theo Phương thức thanh toán
            if (paymentMethod == "VNPAY")
            {
                // Chuyển hướng sang Cổng VNPay Sandbox
                string vnpayUrl = CreateVNPayUrl(order.OrderID, totalAmount);
                return Redirect(vnpayUrl);
            }

            // Với COD, BANK (VietQR), MOMO -> Chuyển sang trang đặt hàng thành công
            return View("OrderSuccess", order);
        }

        // Hàm hỗ trợ tạo URL Thanh toán VNPay
        private string CreateVNPayUrl(int orderId, decimal amount)
        {
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_TmnCode = "DEMO_TMN_CODE"; // Thay bằng TMNCode của bạn khi đăng ký Sandbox
            string vnp_HashSecret = "DEMO_HASH_SECRET"; // Thay bằng HashSecret của bạn

            // Định dạng dữ liệu gửi sang VNPay
            var vnpay = new Dictionary<string, string>
    {
        { "vnp_Version", "2.1.0" },
        { "vnp_Command", "pay" },
        { "vnp_TmnCode", vnp_TmnCode },
        { "vnp_Amount", ((long)(amount * 100)).ToString() }, // Đơn vị: VNĐ x 100
        { "vnp_CreateDate", System.DateTime.Now.ToString("yyyyMMddHHmmss") },
        { "vnp_CurrCode", "VND" },
        { "vnp_IpAddr", Request.UserHostAddress },
        { "vnp_Locale", "vn" },
        { "vnp_OrderInfo", "Thanh toan don hang #" + orderId },
        { "vnp_OrderType", "other" },
        { "vnp_ReturnUrl", Url.Action("PaymentCallback", "Cart", null, Request.Url.Scheme) },
        { "vnp_TxnRef", orderId.ToString() }
    };

            // Tạo chuỗi mã hóa
            var query = string.Join("&", vnpay.Select(p => $"{p.Key}={System.Uri.EscapeDataString(p.Value)}"));
            return vnp_Url + "?" + query;
        }

        // Callback khi thanh toán VNPay hoàn tất
        public ActionResult PaymentCallback()
        {
            string orderId = Request.QueryString["vnp_TxnRef"];
            string responseCode = Request.QueryString["vnp_ResponseCode"];

            if (!string.IsNullOrEmpty(orderId))
            {
                int id = int.Parse(orderId);
                var order = db.Orders.Find(id);
                if (order != null && responseCode == "00") // 00 là thanh toán thành công
                {
                    order.Status = "Đã thanh toán (VNPay)";
                    db.SaveChanges();
                    return View("OrderSuccess", order);
                }
            }
            ViewBag.Error = "Thanh toán không thành công hoặc đã bị hủy.";
            return RedirectToAction("Index", "Home");
        }
        // Mua ngay: Thêm vào giỏ hàng và chuyển hướng thẳng sang trang Checkout
        public ActionResult BuyNow(int id)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.FirstOrDefault(p => p.BookID == id);

            if (item == null)
            {
                Book book = db.Books.Find(id);
                if (book != null)
                {
                    cart.Add(new CartItem
                    {
                        BookID = book.BookID,
                        Title = book.Title,
                        ImageUrl = book.ImageUrl,
                        Price = book.Price,
                        Quantity = 1
                    });
                }
            }
            else
            {
                item.Quantity++;
            }

            // Chuyển hướng thẳng sang trang Checkout (Thanh toán)
            return RedirectToAction("Checkout");
        }
    }
}