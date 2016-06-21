using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class Meetings
    {
 
        public Guid ID { get; set; }
        public Guid ProjectID { get; set; }
        public Guid RecorderID { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public DateTime? NextMeeting { get; set; }
        public string SpecialNotes { get; set; }
        public string SpecialNotes2 { get; set; }
        public string Status { get; set; }
        public int Locked { get; set; }
        public int ProjectMode { get; set; }
        public string TimeZone { get; set; }
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
        public string RecorderName { get; set; }
       

        public int OldID { get; set; }
        public Guid OldProjectID { get; set; }
        public string OldProjectContactID { get; set; }
    }
}
