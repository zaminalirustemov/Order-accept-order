using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok_book_sales_app.Models;

namespace Pustok_book_sales_app.Areas.Manage.Controllers
{

    [Area("Manage")]
    public class OrderController : Controller
    {
        private readonly PustokDbContext _pustokDbContext;

        public OrderController(PustokDbContext pustokDbContext)
        {
            _pustokDbContext = pustokDbContext;
        }
        public IActionResult Index()
        {
            List<Order> orders=_pustokDbContext.Orders.ToList();
            return View(orders);
        }
        public IActionResult Detail(int id)
        {
            Order order = _pustokDbContext.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);
            if (order==null) return     View("Error");
            


            return View(order);
        }
        public IActionResult Accept(int id)
        {
            Order order = _pustokDbContext.Orders.FirstOrDefault(x => x.Id == id);
            if (order == null) return View("Error");

            order.OrderStatus=Enums.OrderStatus.Accepted;
            _pustokDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Reject(int id)
        {
            Order order = _pustokDbContext.Orders.FirstOrDefault(x => x.Id == id);
            if (order == null) return View("Error");


            order.OrderStatus = Enums.OrderStatus.Rejected;
            _pustokDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}
