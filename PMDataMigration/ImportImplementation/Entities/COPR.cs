using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class COPR
    {
        public Guid ID { get; set; }
        public Guid? EstimateID { get; set; }
        public Guid ProjectID { get; set; }
        public Guid ProjectAreaID { get; set; }
        public Guid? ProjectControlID { get; set; }
        public Guid? OwnerChangeOrderID { get; set; }
        public int AcctMark { get; set; }
        public decimal Approved { get; set; }
        public int BaseMark { get; set; }
        public string COPRLetter { get; set; }
        public DateTime? Date { get; set; }
        public int? DaysAdded { get; set; }
        public int? DaysValid { get; set; }
        public string COPRDescription { get; set; }
        public int DupType { get; set; }
        public string FolderName { get; set; }
        public int IsControlFromBudget { get; set; }
        public int IsLocked { get; set; }
        public int SerialNumber { get; set; }
        public decimal Pending { get; set; }
        public decimal Rejected { get; set; }
        public string Status { get; set; }
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
        public int? Alias { get; set; }

        public int OldID { get; set; }
        public string OldEstimateID { get; set; }
        public string OldProjectID { get; set; }
        public int OldProjectAreaID { get; set; }
        public int OldProjectControlID { get; set; }
        public int OldOwnerChangeOrderID { get; set; }
    }
}
