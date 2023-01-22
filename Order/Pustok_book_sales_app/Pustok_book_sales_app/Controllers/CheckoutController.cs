using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok_book_sales_app.Models;
using Pustok_book_sales_app.ViewModel;
using static NuGet.Packaging.PackagingConstants;

namespace Pustok_book_sales_app.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly PustokDbContext _pustokDbContext;
        private readonly UserManager<AppUser> _userManager;

        public CheckoutController(PustokDbContext pustokDbContext, UserManager<AppUser> userManager)
        {
            _pustokDbContext = pustokDbContext;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<CheckoutItemViewModel> checkoutItems = new List<CheckoutItemViewModel>();
            CheckoutItemViewModel checkoutItem = null;
            List<BasketItem> memberBasketItems = null;
            OrderViewModel orderViewModel = null;
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];


            if (member == null)
            {

                if (basketItemsStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                    foreach (var item in basketItems)
                    {
                        checkoutItem = new CheckoutItemViewModel
                        {
                            Book = _pustokDbContext.Books.FirstOrDefault(x => x.Id == item.BookId),
                            Count = item.Count,
                        };
                        checkoutItems.Add(checkoutItem);
                        
                    }


                }
            }
            else
            {
                memberBasketItems = _pustokDbContext.BasketItems.Include(x=>x.Book).Where(x => x.AppUserId == member.Id).ToList();

                foreach (var item in memberBasketItems)
                {
                    if (!item.IsDeleted)
                    {
                        checkoutItem = new CheckoutItemViewModel
                        {
                            Book = item.Book,
                            Count = item.Count
                        };
                        checkoutItems.Add(checkoutItem);
                    }
                    item.IsDeleted=true;
                }
            }

            orderViewModel = new OrderViewModel
            {
                CheckoutItemViewModels = checkoutItems,
                FullName=member?.FullName,
                Email=member?.Email,
                Phone=member?.PhoneNumber,
                
            };
            return View(orderViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(OrderViewModel orderVM)
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<CheckoutItemViewModel> checkoutItems = new List<CheckoutItemViewModel>();
            CheckoutItemViewModel checkoutItem = null;
            List<BasketItem> memberBasketItems = null;
            OrderViewModel orderViewModel = null;
            OrderItem orderItem = null;
            double totalPrice = 0;
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }

            Order order = null;

            order = new Order
            {
                OrderStatus= Enums.OrderStatus.Pending,
                FullName = orderVM.FullName,
                ZipCode = orderVM.ZipCode,
                Country =orderVM.Country,
                Adress = orderVM.Adress,
                AppUserId = member?.Id,
                Phone = orderVM.Phone,
                Email =orderVM.Email,
                City =orderVM.City,
                Note=orderVM.Note,
            };



            if (member == null)
            {

                if (basketItemsStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                    foreach (var item in basketItems)
                    {
                        Book book = _pustokDbContext.Books.FirstOrDefault(x => x.Id == item.BookId);
                        orderItem = new OrderItem
                        {
                            Book = book,
                            BookName =book.Name,
                            CostPrice=book.CostPrice,
                            DiscountPrice=book.DiscountPrice,
                            SalePrice=(book.SalePrice*(1-(book.DiscountPrice/100))),
                            Count=item.Count,
                            Order=order
                        };
                        totalPrice += orderItem.SalePrice * orderItem.Count;
                        order.OrderItems.Add(orderItem);

                    }


                }
            }
            else
            {
                memberBasketItems = _pustokDbContext.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == member.Id).ToList();

                foreach (var item in memberBasketItems)
                {
                    Book book = _pustokDbContext.Books.FirstOrDefault(x => x.Id == item.BookId);
                    orderItem = new OrderItem
                    {
                        Book = book,
                        BookName = book.Name,
                        CostPrice = book.CostPrice,
                        DiscountPrice = book.DiscountPrice,
                        SalePrice = (book.SalePrice * (1 - (book.DiscountPrice / 100))),
                        Count = item.Count,
                        Order = order
                    };
                    totalPrice += orderItem.SalePrice * orderItem.Count;
                    order.OrderItems.Add(orderItem);
                }
            }
            order.TotalPrice = totalPrice;
            _pustokDbContext.Orders.Add(order);
            _pustokDbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }



        public async Task<IActionResult> AddToBasket(int bookId)
        {
            if (!_pustokDbContext.Books.Any(x => x.Id == bookId)) return NotFound();

            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            BasketItemViewModel basketItem = null;
            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            }
            string basketItemStr = HttpContext.Request.Cookies["BasketItems"];
            if (member == null)
            {

                if (basketItemStr is not null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                    basketItem = basketItems.FirstOrDefault(x => x.BookId == bookId);
                    if (basketItem is not null) basketItem.Count++;
                    else
                    {
                        basketItem = new BasketItemViewModel
                        {
                            BookId = bookId,
                            Count = 1
                        };

                        basketItems.Add(basketItem);
                    }


                }
                else
                {
                    basketItem = new BasketItemViewModel
                    {
                        BookId = bookId,
                        Count = 1
                    };

                    basketItems.Add(basketItem);
                }



                basketItemStr = JsonConvert.SerializeObject(basketItems);

                HttpContext.Response.Cookies.Append("BasketItems", basketItemStr);
            }
            else
            {
                BasketItem memberBasketItem = _pustokDbContext.BasketItems.FirstOrDefault(x => x.AppUserId == member.Id && x.BookId == bookId);

                if (memberBasketItem != null)
                {
                    memberBasketItem.Count++;
                }
                else
                {
                    memberBasketItem = new BasketItem
                    {
                        AppUserId = member.Id,
                        BookId = bookId,
                        Count = 1
                    };

                    _pustokDbContext.BasketItems.Add(memberBasketItem);

                }
                await _pustokDbContext.SaveChangesAsync();

            }

            return Ok();
        }

        public IActionResult GetBasketItems()
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            string basketItemStr = HttpContext.Request.Cookies["BasketItems"];

            if (basketItemStr is not null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
            }

            return Json(basketItems);
        }


    }
}
