using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyFinances.Models
{
    public class Inpayment
    {
        public int ID { get; set; }
        [Display(Name = "Дата")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "Сумма")]
        [Column(TypeName = "money")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Sum { get; set; }
        [Display(Name = "Назначение")]
        public string Income { get; set; }

        [Display(Name = "Мес поступление")]
        public bool MonthlyIncome { get; set; }

        //public int CategoryID { get; set; }
        [Display(Name = "Code")]
        public int PaySourceID { get; set; } 

        //[Display(Name = "Категория")]
        //public PurchaseCategory Category { get; set; }
        [Display(Name = "Источник поступления")]
        public PaySource PaySource { get; set; }

    }
}
