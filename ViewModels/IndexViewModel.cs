using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyFinances.Models;


namespace FamilyFinances.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Expense> Expenses { get; set; }
        public IEnumerable<PaySourceModel> PaySources { get; set; }
        public IEnumerable<PaySourceSummaryModel> PaySourceSummaries { get; set; }
        public IEnumerable<CategoryModel> Categories { get; set; }
        public IEnumerable<CategorySummaryModel> CategorySummaries { get; set; }
        public IEnumerable<InpaymentModel> Inpayments { get; set; }
    }
}
