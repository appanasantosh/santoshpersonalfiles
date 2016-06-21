using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class Transmittal
    {
         
        public Guid ID { get; set; }
        public Guid ProjectID { get; set; }         
        public Guid? ToContactID { get; set; }         
        public Guid? FromContactID { get; set; }        
        public int? SerialNumber { get; set; }         
        public string FromAddress { get; set; }        
        public string ToAddress { get; set; }        
        public string ToContactName { get; set; }         
        public string Specification { get; set; }        
        public int IsResubmittedTransmittal { get; set; }        
        public string Remarks { get; set; }         
        public string ProjectMode { get; set; }         
        public int IsNewTransmittal { get; set; }        
        public int IsGovtApproval { get; set; }         
        public string FromContactName { get; set; }         
        public int IsFIOTransmittal { get; set; }         
        public string ContractNumber { get; set; }        
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

        public int OldId { get; set; }
        public Guid OldProjectID { get; set; }
        public string OldToContactID { get; set; }
        public string OldFromContactID { get; set; }

    }
}
