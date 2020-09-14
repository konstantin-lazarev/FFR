using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyFinances.ViewModels
{
    public class InpaymentModel
    {
        public int ID { get; set; }
        public string MonthStart { get; set; }
        public string MonthEnd { get; set; }
        public bool Selected{ get; set; }
    }
}
