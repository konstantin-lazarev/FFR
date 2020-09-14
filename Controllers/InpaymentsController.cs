using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using FamilyFinances.Data;
using FamilyFinances.Models;

namespace FamilyFinances.Controllers
{
    public class InpaymentsController : Controller
    {
        private readonly FamilyFinancesContext _context;
        private readonly IStringLocalizer<InpaymentsController> _localizer;

        public InpaymentsController(FamilyFinancesContext context, IStringLocalizer<InpaymentsController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        // GET: Inpayments
        public async Task<IActionResult> Index()
        {
            var familyFinancesContext = _context.Inpayment.Include(i => i.PaySource)
                .Include(ps => ps.PaySource)
            ;
            return View(await familyFinancesContext.ToListAsync());
        }

        // GET: Inpayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inpayment = await _context.Inpayment
                .Include(i => i.PaySource)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inpayment == null)
            {
                return NotFound();
            }

            return View(inpayment);
        }

        // GET: Inpayments/Create
        public IActionResult Create()
        {
            ViewData["PaySourceID"] = new SelectList(_context.PaySources, "ID", "ID");

            var inpayment = new Inpayment
            {
                TimeStamp = DateTime.Now
            };

            PopulateCategoryDropDownList();
            PopulatePaySourceDropDownList();
            return View(inpayment);
        }

        // POST: Inpayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TimeStamp,Sum,Income,MonthlyIncome,PaySourceID")] Inpayment inpayment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inpayment);
                Helpers.Money.Earn(_context, inpayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaySourceID"] = new SelectList(_context.PaySources, "ID", "ID", inpayment.PaySourceID);
            PopulateCategoryDropDownList();
            PopulatePaySourceDropDownList();
            return View(inpayment);
        }

        // GET: Inpayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inpayment = await _context.Inpayment.FindAsync(id);
            if (inpayment == null)
            {
                return NotFound();
            }
            ViewData["PaySourceID"] = new SelectList(_context.PaySources, "ID", "ID", inpayment.PaySourceID);
            PopulateCategoryDropDownList();
            PopulatePaySourceDropDownList();
            return View(inpayment);
        }

        // POST: Inpayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TimeStamp,Sum,Income,MonthlyIncome,PaySourceID")] Inpayment inpayment)
        {
            if (id != inpayment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Helpers.Money.CheckInpaymentSumAndAjustPaySource(_context, inpayment);
                    _context.Update(inpayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InpaymentExists(inpayment.ID))
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
            ViewData["PaySourceID"] = new SelectList(_context.PaySources, "ID", "ID", inpayment.PaySourceID);
            PopulateCategoryDropDownList();
            PopulatePaySourceDropDownList();
            return View(inpayment);
        }

        // GET: Inpayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inpayment = await _context.Inpayment
                .Include(i => i.PaySource)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inpayment == null)
            {
                return NotFound();
            }

            return View(inpayment);
        }

        // POST: Inpayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inpayment = await _context.Inpayment.FindAsync(id);
            _context.Inpayment.Remove(inpayment);
            Helpers.Money.RemoveEarn(_context, inpayment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InpaymentExists(int id)
        {
            return _context.Inpayment.Any(e => e.ID == id);
        }

        private void PopulatePaySourceDropDownList(object selectedPaySource = null)
        {
            var paySourcesQuery =
                from ps in _context.PaySources
                orderby ps.Name
                select ps;
            ViewBag.PaySourceID = new SelectList(paySourcesQuery.AsNoTracking(), "ID", "Name", selectedPaySource);
        }

        private void PopulateCategoryDropDownList(object selectedCategory = null)
        {
            var categoryQuery =
                from pc in _context.PurchaseCategories
                orderby pc.Name
                select pc;
            ViewBag.CategoryID = new SelectList(categoryQuery.AsNoTracking(), "ID", "Name", selectedCategory);
        }

    }
}
