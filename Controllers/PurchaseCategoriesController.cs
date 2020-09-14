using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using FamilyFinances.Data;
using FamilyFinances.Models;

namespace FamilyFinances.Controllers
{
    public class PurchaseCategoriesController : Controller
    {
        private readonly FamilyFinancesContext _context;
        private readonly IStringLocalizer<PurchaseCategoriesController> _localizer;

        public PurchaseCategoriesController(FamilyFinancesContext context, IStringLocalizer<PurchaseCategoriesController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        // GET: PurchaseCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.PurchaseCategories.ToListAsync());
        }

        // GET: PurchaseCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseCategory = await _context.PurchaseCategories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (purchaseCategory == null)
            {
                return NotFound();
            }

            return View(purchaseCategory);
        }

        // GET: PurchaseCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PurchaseCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] PurchaseCategory purchaseCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchaseCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(purchaseCategory);
        }

        // GET: PurchaseCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseCategory = await _context.PurchaseCategories.FindAsync(id);
            if (purchaseCategory == null)
            {
                return NotFound();
            }
            return View(purchaseCategory);
        }

        // POST: PurchaseCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] PurchaseCategory purchaseCategory)
        {
            if (id != purchaseCategory.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseCategoryExists(purchaseCategory.ID))
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
            return View(purchaseCategory);
        }

        // GET: PurchaseCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseCategory = await _context.PurchaseCategories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (purchaseCategory == null)
            {
                return NotFound();
            }

            return View(purchaseCategory);
        }

        // POST: PurchaseCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseCategory = await _context.PurchaseCategories.FindAsync(id);
            _context.PurchaseCategories.Remove(purchaseCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseCategoryExists(int id)
        {
            return _context.PurchaseCategories.Any(e => e.ID == id);
        }
    }
}
