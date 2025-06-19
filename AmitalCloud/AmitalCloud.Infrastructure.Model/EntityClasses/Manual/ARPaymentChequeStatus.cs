using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{
   
    public class ARPaymentChequeStatus
    {
	 string dbms;

        [Key]
        [Column("Code")]
	    public string Code { get; set; }
        [Column("SearchFields")]
	    public string SearchFields { get; set; }
        [Column("LocalName")]
	    public string LocalName { get; set; }
        [Column("EnglishName")]
	    public string EnglishName { get; set; }
        [Column("Inactive")]
	    public bool Inactive { get; set; }
    }
}
	 