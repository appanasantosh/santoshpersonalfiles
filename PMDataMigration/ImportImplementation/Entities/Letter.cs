using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class Letter
    {
        public Guid ID { get; set; } 
        public Guid ProjectID { get; set; }         
        public Guid ProjectAreaID { get; set; }         
        public Guid ProjectControlID { get; set; }         
        public Guid? RecipientID { get; set; }         
        public Guid? SenderID { get; set; }         
        public string Description { get; set; }         
        public string Regarding { get; set; }         
        public string Greeting { get; set; }         
        public string Enclosure { get; set; }         
        public string Closing { get; set; }         
        public string Body { get; set; }         
        public int SerialNumber { get; set; }         
        public int ProjectMode { get; set; }         
        public DateTime? Date { get; set; }         
        public int IsActive { get; set; }         
        public DateTime? Created { get; set; }         
        public DateTime? LastUpdated { get; set; }         
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
        public string OldProjectID { get; set; }
        public int OldProjectAreaID { get; set; }
        public string OldRecipientID { get; set; }
        public string OldSenderID { get; set; }
        public int OldProjectControlID { get; set; }
    }
}
