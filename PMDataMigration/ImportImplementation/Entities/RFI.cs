using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class RFI
    {
        public Guid ID { get; set; }
        public Guid ProjectID { get; set; }
        public Guid OriginatorID { get; set; }
        public Guid? ReceipentID { get; set; }
        public Guid? SenderID { get; set; }
        public Guid? ProjectControlID { get; set; }
        public Guid ProjectAreaID { get; set; }
        public Guid DepartmentID { get; set; }
        public int SerialNumber { get; set; }
        public DateTime? OriginationDate { get; set; }
        public int ResponseDays { get; set; }
        public DateTime? AERespondedDate { get; set; }
        public string RequestNotes { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public double? Spec { get; set; }
        public string Section { get; set; }
        public string RFIDescription { get; set; }
        public int ProjectMode { get; set; }
        public string Type { get; set; }
        public string Suffix { get; set; }
        public int DuplicateType { get; set; }
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
        public int IsLocked { get; set; }

        #region Non-DBTable Related Columns
        public int Area { get; set; }
        public int Control { get; set; }
        public string OriginatorCompanyName { get; set; }
        public string OriginatorContactName { get; set; }
        public string SenderCompanyName { get; set; }
        public string SenderContactName { get; set; }
        public string ReceipentCompanyName { get; set; }
        public string ReceipentContactName { get; set; }
        #endregion

        public int OldID { get; set; }
        public string OldProjectID { get; set; }
        public string OldOriginatorID { get; set; }
        public string OldRecipientID { get; set; }
        public string OldSenderID { get; set; }
        public int OldProjectAreaID { get; set; }
        public int OldProjectControlID { get; set; }
        public int? OldDepartmentID { get; set; }
    }
}
