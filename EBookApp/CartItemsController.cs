using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EBookApp.Models;
using EBookApp.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EBookApp.Controllers
{
    [Authorize]
    public class CartItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CartItems
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.CartItem
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ToListAsync();

            return View(cartItems);
        }

        // POST: CartItems/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity)
        {
            var userId = _userManager.GetUserId(User);

            // Check if userId is null
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if userId is null
            }

            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    BookId = bookId,
                    Quantity = quantity,
                    UserId = userId // This assignment is now guaranteed to be non-null
                };
                _context.CartItem.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Books");
        }

        // DELETE: CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .Include(c => c.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,UserId,Quantity")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", cartItem.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cartItem.UserId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", cartItem.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cartItem.UserId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,UserId,Quantity")] CartItem cartItem)
        {
            if (id != cartItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", cartItem.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cartItem.UserId);
            return View(cartItem);
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItem.Any(e => e.Id == id);
        }

        // GET: CartItems/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var savedDetails = await _context.CheckoutDetails
                .Where(cd => cd.UserId == userId)
                .ToListAsync();

            var viewModel = new CheckoutViewModel
            {
                SavedDetails = savedDetails
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel viewModel)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Handle unauthenticated user
            }

            CheckoutDetails? billingDetails = null; // Allow nullable assignment

            if (viewModel.SelectedCheckoutId.HasValue)
            {
                billingDetails = await _context.CheckoutDetails
                    .FirstOrDefaultAsync(cd => cd.Id == viewModel.SelectedCheckoutId.Value && cd.UserId == userId);

                if (billingDetails == null)
                {
                    ModelState.AddModelError("", "Invalid checkout details selected.");
                    return View(viewModel); // Return with validation errors
                }
            }
            else if (ModelState.IsValid)
            {
                // Assign UserId from the authenticated user context
                viewModel.NewDetails.UserId = userId;

                billingDetails = viewModel.NewDetails;

                _context.CheckoutDetails.Add(billingDetails);
                await _context.SaveChangesAsync();
            }
            else
            {
                return View(viewModel); // Return with validation errors
            }

            // Ensure billingDetails is not null before processing the order
            if (billingDetails == null)
            {
                throw new InvalidOperationException("Failed to retrieve or create billing details.");
            }

            // Process the order
            var orderId = await ProcessOrder(billingDetails);
            return RedirectToAction("OrderConfirmation", new { orderId });
        }


        // Process order and save it
        private async Task<int> ProcessOrder(CheckoutDetails billingDetails)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var order = new Order
            {
                UserId = userId,
                BillingDetailsId = billingDetails.Id,
                BillingDetails = billingDetails,
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            var cartItems = await _context.CartItem
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ToListAsync();

            if (!cartItems.Any())
            {
                throw new InvalidOperationException("Cannot process order: Cart is empty.");
            }

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Book == null)
                {
                    throw new InvalidOperationException($"CartItem with ID {cartItem.Id} has no associated Book.");
                }

                var orderItem = new OrderItem
                {
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Book.Price,
                    Order = order
                };

                order.OrderItems.Add(orderItem);
                order.TotalAmount += cartItem.Quantity * cartItem.Book.Price;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _context.CartItem.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return order.Id;
        }

        // GET: CartItems/OrderConfirmation
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems!)
                .ThenInclude(oi => oi.Book!)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            ViewData["OrderId"] = order.Id;
            ViewData["OrderDate"] = order.OrderDate.ToString("dd/MM/yyyy");
            ViewData["TotalAmount"] = order.TotalAmount;
            ViewData["OrderItems"] = order.OrderItems;

            return View();
        }

        // GET: ViewOrders
        public async Task<IActionResult> ViewOrders()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();

            var orderViewModels = orders.Select(o => new UserOrderViewModel
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems
                    .Where(oi => oi != null) // Ensure non-null OrderItems
                    .Select(oi => new UserOrderItemViewModel
                    {
                        BookTitle = oi.Book?.Title ?? "Unknown", // Handle null Book
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
            }).ToList();

            return View(orderViewModels);
        }
    }
}
