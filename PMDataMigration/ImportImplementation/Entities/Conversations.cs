using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class Conversations
    {
        
        public Guid ID { get; set; }
        public Guid PersonID { get; set; }   
        public Guid ProjectID { get; set; } 
        public Guid ProjectContactID { get; set; } 
        public int IsActive { get; set; }
        public int DataRowVersion { get; set; }
        public int ProjectMode { get; set; }
        public string Comments { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string ContactName { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public int? UserNumber { get; set; }
        public string AC1 { get; set; }
        public string AC2 { get; set; }
        public string AC3 { get; set; }
        public string AC4 { get; set; }
        public string AC5 { get; set; }
        public string AC6 { get; set; }


        public int OldID { get; set; }
        public string OldPersonID { get; set; }
        public string OldProjectContactID { get; set; } 
    }
}
