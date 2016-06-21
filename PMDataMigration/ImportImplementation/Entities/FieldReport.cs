using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class FieldReport
    {
        
        public Guid ID { get; set; }
        public Guid ProjectID { get; set; }
        //public Guid? CompletedByID { get; set; }
        public decimal? TempHigh { get; set; }
        public decimal? TempLow { get; set; }
        public string Comments { get; set; }
        public string DeliveryLog { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public string SiteConditions { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public int Value4 { get; set; }
        public int Value5 { get; set; }
        public string Weather { get; set; }
        public string WorkConditions { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int IsActive { get; set; }
        public int DataRowVersion { get; set; }
        public int? UserNumber { get; set; }
        public string AC1 { get; set; }
        public string AC2 { get; set; }
        public string AC3 { get; set; }
        public string AC4 { get; set; }
        public string AC5 { get; set; }
        public string AC6 { get; set; }
        public string CompletedBy { get; set; }
        public string ProjectName { get; set; }

        public int OldID { get; set; }
        //public string OldCompletedByID { get; set; }
        public string OldProjectID { get; set; }
    }
}
