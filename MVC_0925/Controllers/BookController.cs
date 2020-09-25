using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_0925.Data;
using MVC_0925.Models;

namespace MVC_0925.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Book book { get; set; }

        public BookController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Books.ToListAsync());
        }

        public IActionResult Upsert(int? id)
        {
            book = new Book();
            if(id == null)
            {
                //Crear
                return View(book);
            }
            //Editar
            book = _db.Books.FirstOrDefault(u => u.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if(book.Id == 0)
                {
                    //Crear
                    _db.Books.Add(book);
                }
                else
                {
                    _db.Books.Update(book);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
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
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _db.Books.Remove(bookFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
