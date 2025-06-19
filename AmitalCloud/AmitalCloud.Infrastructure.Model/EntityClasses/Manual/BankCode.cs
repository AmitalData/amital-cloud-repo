using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
   
    public class BankCode
    {
	 string dbms;

           [Column("Code")]
	    public string Code { get; set; }
        [Column("EnglishName")]
	    public string EnglishName { get; set; }
        [Column("SearchFields")]
	    public string SearchFields { get; set; }
        [Column("Tenant")]
	    public int Tenant { get; set; }
        [Column("LocalName")]
	    public string LocalName { get; set; }
     [Key]
        [Column("Id")]
	    public string Id { get; set; }
        [Column("Inactive")]
	    public bool? Inactive { get; set; }
        [Column("LogoId")]
	    public string LogoId { get; set; }
        [Column("DateFormat")]
	    public string DateFormat { get; set; }
    }
}
	 