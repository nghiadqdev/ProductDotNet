using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;

namespace Eshop.Controllers
{
    public class CartsController : Controller
    {
        private readonly EshopContext _context;

        public CartsController(EshopContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var eshopContext = _context.Carts.Include(c => c.Account).Include(c => c.Product)
                                             .Where(c => c.Account.Username == "john");
            return View(await eshopContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountId,ProductId,Quantity")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,ProductId,Quantity")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public IActionResult Delete(int id)
        {
            string username = "john";
            int accountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
            Cart cart = _context.Carts.Where(c => c.AccountId == accountId && c.ProductId == id).FirstOrDefault();
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Add(int id, int quantity = 1)
        {
            string username = "john";
            int accountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
            Cart cart = _context.Carts.Where(c => c.AccountId == accountId && c.ProductId == id).FirstOrDefault();
            if (cart == null)
            {
                _context.Carts.Add(new Cart
                {
                    AccountId = accountId,
                    ProductId = id,
                    Quantity = quantity
                });                
            }
            else
            {
                cart.Quantity += quantity;
                _context.Carts.Update(cart);
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Pay()
        {
            string username = "john";
            int accountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
            if (_context.Carts.Any(c => c.AccountId == accountId))
            {
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Pay(string ShippingAddress, string ShippingPhone)
        {
            string username = "john";
            int accountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
            List<Cart> carts = _context.Carts.Include(c => c.Product)
                                             .Where(c => c.AccountId == accountId).ToList();
            int total = carts.Sum(c => c.Product.Price * c.Quantity);

            // Bước 1: Tạo Invoice (hóa đơn)
            Invoice invoice = new Invoice
            {
                Code = DateTime.Now.ToString("yyMMddhhmmss"),
                AccountId = accountId,
                IssuedDate = DateTime.Now,
                ShippingAddress = ShippingAddress,
                ShippingPhone = ShippingPhone,
                Total = total,
                Status = true
            };
            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            // Bước 2: Tạo InvoiceDetails (chi tiết hóa đơn) và xóa sản phẩm trong giỏ hàng, cập nhật SLTK
            foreach (var cart in carts)
            {
                InvoiceDetail detail = new InvoiceDetail
                {
                    InvoiceId = invoice.Id,
                    ProductId = cart.ProductId,
                    Quantity = cart.Quantity,
                    UnitPrice = cart.Product.Price
                };
                _context.InvoiceDetails.Add(detail);
                _context.Carts.Remove(cart);
                cart.Product.Stock -= cart.Quantity;
                _context.Products.Update(cart.Product);
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}
