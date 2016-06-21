using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class SubmittalItems
    {
     
        public Guid ID { get; set; }
        public Guid SubmittalID { get; set; }
        public string ActionCode { get; set; }
        public string BrochNumber { get; set; }
        public int? Copies { get; set; }
        public string SubmittalItemDescription { get; set; }
        public int? Number { get; set; }
        public string ParaNumber { get; set; }
        public string DrawingSheetNumber { get; set; }
        public string ContrCode { get; set; }
        public string Var { get; set; }
        public string ForCEUseCode { get; set; }
        public string GovernmentActionCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public int? UserNumber { get; set; }
        public string AC1 { get; set; }
        public string AC2 { get; set; }
        public string AC3 { get; set; }
        public string AC4 { get; set; }
        public string AC5 { get; set; }
        public string AC6 { get; set; }
        public int DataRowVersion { get; set; }

        public int OldID { get; set; }
        public int OldSubmittalID { get; set; }
    }
}
