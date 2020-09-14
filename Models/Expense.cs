using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyFinances.Models
{
    public class Expense
    {
        //private DateTime? dateCreated = null;

        public int ID { get; set; }
        [Display(Name = "Дата")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "Сумма")]
        [Column(TypeName = "money")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Sum { get; set; }
        [Display(Name = "Покупка")]
        public string Purchase { get; set; }

        public int CategoryID { get; set; }
        [Display(Name = "Code")]
        public int PaySourceID { get; set; }

        [Display(Name = "Категория")]
        public PurchaseCategory Category { get; set; }
        [Display(Name = "Источник оплаты")]
        public PaySource PaySource { get; set; }
    }
}
