using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FamilyFinances.Models
{
    public class PaySource
    {
        public int ID { get; set; }
        [Display(Name = "Код")]
        [StringLength(50)]
        public string Name { get; set; }
        [Display(Name = "Наименование")]
        [StringLength(50)]
        public string FullName { get; set; }
        // As on bank Card 05/22
        [Display(Name = "Годен до")]
        [StringLength(5)]
        public string ValidThru { get; set; }

        [Display(Name = "Остаток")]
        [Column(TypeName = "money")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Balance { get; set; }

        [Display(Name = "Группа")]
        [Column(TypeName = "int")]
        public int? Group { get; set; }
    }
}
