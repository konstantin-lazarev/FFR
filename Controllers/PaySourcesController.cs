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
    //[Route("api/[controller]")]
    public class PaySourcesController : Controller
    {
        private readonly FamilyFinancesContext _context;
        private readonly IStringLocalizer<PaySourcesController> _localizer;

        public PaySourcesController(FamilyFinancesContext context, IStringLocalizer<PaySourcesController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        // GET: PaySources
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaySources.ToListAsync());
        }

        // GET: PaySources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paySource = await _context.PaySources
                .FirstOrDefaultAsync(m => m.ID == id);
            if (paySource == null)
            {
                return NotFound();
            }

            return View(paySource);
        }

        // GET: PaySources/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaySources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,FullName,ValidThru,Balance,Group")] PaySource paySource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paySource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paySource);
        }

        // GET: PaySources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paySource = await _context.PaySources.FindAsync(id);
            if (paySource == null)
            {
                return NotFound();
            }
            return View(paySource);
        }

        // POST: PaySources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,FullName,ValidThru,Balance,Group")] PaySource paySource)
        {
            if (id != paySource.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paySource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaySourceExists(paySource.ID))
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
            return View(paySource);
        }

        // GET: PaySources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paySource = await _context.PaySources
                .FirstOrDefaultAsync(m => m.ID == id);
            if (paySource == null)
            {
                return NotFound();
            }

            return View(paySource);
        }

        // POST: PaySources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paySource = await _context.PaySources.FindAsync(id);
            _context.PaySources.Remove(paySource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaySourceExists(int id)
        {
            return _context.PaySources.Any(e => e.ID == id);
        }
    }
}
