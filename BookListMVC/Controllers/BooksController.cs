using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]           //once you bind that property, on Post u wont have to retrieve that u automatically be bind it.
        public Book Book { get; set; }   //Create Book Object of name Book (type)
        public BooksController(ApplicationDbContext db) //using Dependency Injection
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)  //nullable int for create
        {
            Book = new Book();
            if (id == null)
            {
                return View(Book);
            }
            Book = _db.Books.FirstOrDefault(u=> u.Id==id);
            if (Book == null)
            {
                return NotFound();
            }
            return View(Book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]  //to prevent some attacks and using for security purpose.
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                {
                    //create
                    _db.Books.Add(Book);
                }
                else
                {
                    _db.Books.Update(Book);
                }
                _db.SaveChanges();
                //in Razor Page, we used RedirectToPage but in MVC, have to use RedirectToAction.
                return RedirectToAction("Index"); 
            }
            return View(Book);
        }
        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _db.Books.FirstOrDefaultAsync(u => u.Id == id);
            if (bookFromDb == null)
            {
                return Json(new { success = "false", message = "Error while deleting" });
            }
            _db.Books.Remove(bookFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = "true", message = "Delete Successfully" });
        }
        #endregion
    }
}
