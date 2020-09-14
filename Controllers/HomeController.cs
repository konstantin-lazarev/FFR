using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using FamilyFinances.Data;
using FamilyFinances.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OurFinances.Models;
using FamilyFinances.ViewModels;
using System.Dynamic;
using Microsoft.AspNetCore.Routing;
//using Expando;

namespace FamilyFinances.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly FamilyFinancesContext _context;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(FamilyFinancesContext context, IStringLocalizer<HomeController> localizer) 
        { 
            _context = context;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public async Task<ActionResult> Main()
        public ActionResult Main(string searchStartDateString)
        {
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var hhtpRequestUICulture = rqf.RequestCulture.UICulture;

            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;

            var startInpayment = _context.Inpayment.FirstOrDefault(s => s.ID.ToString() == searchStartDateString);
            if (startInpayment != null)
            {
                start = startInpayment.TimeStamp;
            }
            else
            {
                startInpayment = _context.Inpayment.Where(x => x.MonthlyIncome).OrderBy(o => o.TimeStamp).LastOrDefault();  //.Max(m => m.TimeStamp);
                start = startInpayment.TimeStamp;
            }

            var endInpayment = _context.Inpayment.OrderBy(o => o.TimeStamp).FirstOrDefault(x => (x.TimeStamp > start) && x.MonthlyIncome);
            if (endInpayment != null)
            {
                //The next perod found. Take previuos date which belongs to current month period
                end = endInpayment.TimeStamp;   //.AddDays(-1);
            }
            else
            {
                // If current period is the last then last expenses date is taken
                end = _context.Expenses.Max(x => x.TimeStamp);
            }

            string selectedValue = startInpayment?.ID.ToString();
            PopulateMonthsDropDownList(selectedValue);
            var inpaymentsModel = GetImpaymentsModel(end, selectedValue);

            var paysourceSummary = GetExpensesByPaySource(start.Date, end.Date);
            var pssTotal = paysourceSummary.Sum(x => x.Sum);
            ViewData["PaySourceTotal"] = pssTotal.ToString("N2", CultureInfo.InvariantCulture);

            var categorySummary = GetExpensesByCategory(start.Date, end.Date);
            var categoryTotal = categorySummary.Sum(x => x.Sum);
            ViewData["CategoryTotal"] = categoryTotal.ToString("N2", CultureInfo.InvariantCulture);
            
            /* Excluding Expenses printout here
            var expenses GetExpenses(stat, end);
            var total = expenses.Sum(x => x.Sum);
            ViewData["Total"] = total.ToString("N2", CultureInfo.InvariantCulture);
            */

            IndexViewModel ivm = new IndexViewModel 
            { 
                Inpayments = inpaymentsModel, 
                PaySourceSummaries = paysourceSummary.AsEnumerable(),
                CategorySummaries = categorySummary.AsEnumerable()//,
                //Expenses = expenses.ToList() 
            };

            return View(ivm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        { 
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<InpaymentModel> GetImpaymentsModel(DateTime end, string selectedValue)
        {
            return _context.Inpayment
                .Where(p => p.MonthlyIncome)
                .OrderBy(o => o.TimeStamp)
                .Select(ps => new InpaymentModel
                {
                    ID = ps.ID,
                    MonthStart = ps.TimeStamp.ToString("dd.MM.yyyy"),
                    MonthEnd = end.ToString("dd.MM.yyyy"),
                    Selected = string.Equals(ps.ID.ToString(), selectedValue)
                })
                .ToList();
        }

        private List<Expense> GetExpenses(DateTime start, DateTime end)
        {
            return _context.Expenses
                .Include(x => x.Category)
                .Include(ps => ps.PaySource)
                .Where(s => s.TimeStamp.Date >= start && s.TimeStamp.Date <= end)
                .OrderBy(x => x.TimeStamp)
                .AsNoTracking()
                .ToList();
        }

        private List<PaySourceSummaryModel> GetExpensesByPaySource(DateTime start, DateTime end)
        {
            var paysourceSummary = from ps in _context.Set<PaySource>()
                                   join e in _context.Set<Expense>()
                                       on ps.ID equals e.PaySourceID //into joined 
                                   where e.TimeStamp.Date >= start && e.TimeStamp.Date <= end
                                   select new { ps.ID, ps.Name, e.Sum } into qr
                                   group qr by new { qr.ID, qr.Name } into gg
                                   select new { gg.Key, Sum = gg.Sum(x => x.Sum) };

            // I need to come back to this implementation. It is shorter and shoould be quicker
            //var query = from ps in _context.Set<PaySource>()
            //            join e in _context.Set<Expense>()
            //                on ps.ID equals e.PaySourceID into grouping
            //            select new { Key = ps.ID, Name = ps.Name, Sum = grouping.Sum(x => x.Sum) };

            List<PaySourceSummaryModel> paysources = new List<PaySourceSummaryModel>();
            foreach (var item in paysourceSummary)
            {
                PaySourceSummaryModel psm = new PaySourceSummaryModel
                {
                    ID = item.Key.ID,
                    Name = item.Key.Name,
                    Sum = item.Sum
                };
                paysources.Add(psm);
            }

            return paysources;
        }
        private List<CategorySummaryModel> GetExpensesByCategory(DateTime start, DateTime end)
        {
            var categorySummary = from pc in _context.Set<PurchaseCategory>()
                                   join e in _context.Set<Expense>()
                                       on pc.ID equals e.CategoryID //into joined 
                                   where e.TimeStamp.Date >= start && e.TimeStamp.Date <= end
                                   select new { pc.ID, pc.Name, e.Sum } into qr
                                   group qr by new { qr.ID, qr.Name } into gg
                                   select new { gg.Key, Sum = gg.Sum(x => x.Sum) };

            List<CategorySummaryModel> categories = new List<CategorySummaryModel>();
            foreach (var item in categorySummary)
            {
                CategorySummaryModel psm = new CategorySummaryModel
                {
                    ID = item.Key.ID,
                    Name = item.Key.Name,
                    Sum = item.Sum
                };
                categories.Add(psm);
            }

            return categories;
        }
        private void PopulateMonthsDropDownList(object selectedMonth = null)
        {
            var monthsQuery =
                from m in _context.Inpayment
                where m.MonthlyIncome
                orderby m.TimeStamp
                select new { m.ID, Name = m.TimeStamp.ToString("dd.MM.yyyy"), selected = false };
            ViewBag.MonthID = new SelectList(monthsQuery.AsNoTracking(), "ID", "Name", selectedMonth);
        }

    }
}
