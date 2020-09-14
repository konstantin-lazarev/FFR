using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyFinances.Models
{
    public class PurchaseCategory
    {
        public int ID { get; set; }
        [Display(Name = "Категория")]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
