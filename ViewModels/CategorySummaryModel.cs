using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FamilyFinances.ViewModels
{
    public class CategorySummaryModel
    {
        public int ID { get; set; }
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Sum { get; set; }
    }
}
