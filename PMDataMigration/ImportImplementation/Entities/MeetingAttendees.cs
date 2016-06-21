using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class MeetingAttendees
    {


        
        public Guid ID { get; set; }

        public Guid MeetingID { get; set; }

       
        public Guid ProjectContactID { get; set; }

        public int Attendee { get; set; }

   
        public string Contact { get; set; }

       
        public string Company { get; set; }

     
        public string Initials { get; set; }

   
        public string Phone1 { get; set; }

       
        public string Phone2 { get; set; }

     
        public string Email { get; set; }

      
        public int IsActive { get; set; }

        public int Dist { get; set; }

     
        public int ATT { get; set; }

        
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

        public int OldMeetingID { get; set; }

        public int OldID { get; set; }

    }
}
