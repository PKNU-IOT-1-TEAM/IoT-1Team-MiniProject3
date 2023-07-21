using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSH_Parking_Assist_APP.Data;
using PSH_Parking_Assist_APP.Models;

namespace PSH_Parking_Assistant_APP.Controllers
{
    public class User_InformationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public User_InformationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User_Information
        public async Task<IActionResult> Index()
        {
              return _context.User_Informations != null ? 
                          View(await _context.User_Informations.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.User_Informations'  is null.");
        }

        // GET: User_Information/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.User_Informations == null)
            {
                return NotFound();
            }

            var user_Information = await _context.User_Informations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_Information == null)
            {
                return NotFound();
            }

            return View(user_Information);
        }

        // GET: User_Information/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User_Information/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Login_ID,Login_PW,NFC,Authority")] User_Information user_Information)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user_Information);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user_Information);
        }

        // GET: User_Information/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.User_Informations == null)
            {
                return NotFound();
            }

            var user_Information = await _context.User_Informations.FindAsync(id);
            if (user_Information == null)
            {
                return NotFound();
            }
            return View(user_Information);
        }

        // POST: User_Information/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Login_ID,Login_PW,NFC,Authority")] User_Information user_Information)
        {
            if (id != user_Information.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user_Information);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!User_InformationExists(user_Information.Id))
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
            return View(user_Information);
        }

        // GET: User_Information/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User_Informations == null)
            {
                return NotFound();
            }

            var user_Information = await _context.User_Informations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_Information == null)
            {
                return NotFound();
            }

            return View(user_Information);
        }

        // POST: User_Information/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User_Informations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.User_Informations'  is null.");
            }
            var user_Information = await _context.User_Informations.FindAsync(id);
            if (user_Information != null)
            {
                _context.User_Informations.Remove(user_Information);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool User_InformationExists(int id)
        {
          return (_context.User_Informations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
