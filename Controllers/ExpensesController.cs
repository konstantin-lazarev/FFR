using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using FamilyFinances.Data;
using FamilyFinances.Models;
using FamilyFinances.ViewModels;

namespace FamilyFinances.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly FamilyFinancesContext _context;
        private readonly IStringLocalizer<ExpensesController> _localizer;

        public ExpensesController(FamilyFinancesContext context, IStringLocalizer<ExpensesController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        // GET: Expenses
        [Route("api/expenses")]
        [HttpGet("{searchPurchaseString}/{searchPaySourceID}/{searchCategoryID}/{searchStartDateString}/{searchEndDateString}")]
        //        public async Task<IActionResult> Index(string searchPurchaseString, string searchCategoryString, string searchPaySourceID)
        public IActionResult Index(string searchPurchaseString, string searchPaySourceID, string searchCategoryID
            , string searchStartDateString, string searchEndDateString, string searchCurrentMonthString)
        {
            // Retrieves the requested culture
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            // Culture contains the information of the requested culture
            // .Culture is used for formatting
            // .UICulture is used for text input
            var hhtpRequestUICulture = rqf.RequestCulture.UICulture;

            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;
            var currentMonth = !string.IsNullOrEmpty(searchCurrentMonthString);

            if (!currentMonth)
            {
                if (!String.IsNullOrEmpty(searchStartDateString)
                    && DateTime.TryParse(searchStartDateString, hhtpRequestUICulture, DateTimeStyles.None, out start))
                {
                    var s = start;
                }
                if (!String.IsNullOrEmpty(searchEndDateString)
                    && DateTime.TryParse(searchEndDateString, hhtpRequestUICulture, DateTimeStyles.None, out end))
                {
                    var e = end;
                }
            }
            else
            {
                start = _context.Inpayment.Where(x => x.MonthlyIncome).Max(m => m.TimeStamp);
                searchStartDateString = start.ToString("dd.MM.yyyy");
                searchEndDateString = null;
                searchCurrentMonthString = "on";
            }

            var cc = CultureInfo.CurrentCulture;

            ViewData["PurchaseFilter"] = searchPurchaseString;
            ViewData["PaySourceID"] = searchPaySourceID;
            ViewData["CategoryID"] = searchCategoryID;
            ViewData["StartDateFilter"] = searchStartDateString;
            ViewData["EndDateFilter"] = searchEndDateString;
            ViewData["CurrentMonthFilter"] = currentMonth ? "checked" : null;//.ToString();// searchCurrentMonthString;

            var expenses = _context.Expenses
                .Include(x => x.Category)
                .Include(ps => ps.PaySource)
                .OrderBy(x => x.TimeStamp)
                .ThenBy(x => x.Category)
                .Where(s =>     
                    (String.IsNullOrEmpty(searchPurchaseString) || s.Purchase.Contains(searchPurchaseString)) 
                    && (String.IsNullOrEmpty(searchPaySourceID) || searchPaySourceID == "0" || s.PaySource.ID.ToString() == searchPaySourceID)
                    && (String.IsNullOrEmpty(searchCategoryID) || searchCategoryID == "0" || s.Category.ID.ToString() == searchCategoryID)
                    && s.TimeStamp.Date >= start.Date && s.TimeStamp.Date <= end.Date
                    )
                .AsNoTracking();

            var total = expenses.Sum(x => x.Sum);
            ViewData["Total"] = total.ToString("N2", CultureInfo.InvariantCulture);

            string selectedValue = searchPaySourceID ?? null;
            List<PaySourceModel> paySourceModels = _context.PaySources
               .Select(ps => new PaySourceModel
               { 
                   ID = ps.ID, 
                   Name = ps.Name,
                   Selected = string.Equals(ps.ID.ToString(), selectedValue)
               })
               .ToList();
            paySourceModels.Insert(0, new PaySourceModel { ID = 0, Name = _localizer["--- Select ---"] });

            string selectedValue1 = searchCategoryID ?? null;
            List<CategoryModel> categoryModels = _context.PurchaseCategories
               .Select(cat => new CategoryModel 
               { 
                   ID = cat.ID, 
                   Name = cat.Name,
                   Selected = string.Equals(cat.ID.ToString(), selectedValue1)
               })
               .ToList();
            categoryModels.Insert(0, new CategoryModel { ID = 0, Name = _localizer["--- Select ---"] });

            IndexViewModel ivm = new IndexViewModel { PaySources = paySourceModels, Categories = categoryModels, Expenses = expenses.ToList() };

            return View(ivm);
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(m => m.ID == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create()
        {
            var expense = new Expense
            {
                TimeStamp = DateTime.Now
            };

            PopulateCategoryDropDownList();
            PopulatePaySourceDropDownList();
            return View(expense);
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TimeStamp,Sum,Purchase,PaySourceID,CategoryID")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expense);
                Helpers.Money.Spend(_context, expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategoryDropDownList();
            PopulatePaySourceDropDownList();
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var expense = await _context.Expenses.FindAsync(id);

            var expense = await _context.Expenses
                .Include(x => x.PaySource)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (expense == null)
            {
                return NotFound();
            }
            PopulateCategoryDropDownList();
            PopulatePaySourceDropDownList();
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TimeStamp,Sum,Purchase,PaySourceID,CategoryID")] Expense expense)
        {
            if (id != expense.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Helpers.Money.CheckExpenseSumAndAjustPaySource(_context, expense);
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.ID))
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
            PopulateCategoryDropDownList();
            PopulatePaySourceDropDownList();
            return View(expense);
        }

        /*
        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseToUpdate = await _context.Expenses
                .Include(x => x.Category)
                .Include(x => x.PaySource)
                .FirstOrDefaultAsync(e => e.ID == id);

            if (await TryUpdateModelAsync<Expense>(expenseToUpdate,
                "",
                e => e.TimeStamp, e => e.Sum, e => e.Purchase, e => e.CategoryID, e => e.PaySourceID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateCategoryDropDownList();
            PopulatePaySourceDropDownList();
            return View(expenseToUpdate);
        }
        */


        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(m => m.ID == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            _context.Expenses.Remove(expense);
            Helpers.Money.RemoveSpend(_context, expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //public static void DetachLocal<T>(this DbContext context, T t, string entryId) where T : class, IIdentifier
        //{
        //    var local = context.Set<T>()
        //        .Local
        //        .FirstOrDefault(entry => entry.Id.Equals(entryId));
        //    if (!local.IsNull())
        //    {
        //        context.Entry(local).State = EntityState.Detached;
        //    }
        //    context.Entry(t).State = EntityState.Modified;
        //}

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.ID == id);
        }

        private void PopulatePaySourceDropDownList(object selectedPaySource = null) 
        { 
            var paySourcesQuery = 
                from ps in _context.PaySources 
                orderby ps.Name 
                //select ps;
                select new { ps.ID, ps.Name, selected = false };
            ViewBag.PaySourceID = new SelectList(paySourcesQuery.AsNoTracking(), "ID", "Name", selectedPaySource); 
        }

        private void PopulateCategoryDropDownList(object selectedCategory = null)
        {
            var categoryQuery =
                from pc in _context.PurchaseCategories
                orderby pc.Name
                //select pc;
                select new { pc.ID, pc.Name, selected = false };
            ViewBag.CategoryID = new SelectList(categoryQuery.AsNoTracking(), "ID", "Name", selectedCategory);
        }
    }
}
