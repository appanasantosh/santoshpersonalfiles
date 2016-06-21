using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class MeetingItems
    {
  
        public Guid ID { get; set; }

       
        public Guid MeetingTopicID { get; set; }

        //[DataMember]
        //public string Topic { get; set; }

        //        [DataMember]
        //        public double SubNo { get; set; }

      
        public string Status { get; set; }

      
        public decimal? Number { get; set; }

 
        public DateTime? Due { get; set; }

     
        public string MeetingItemsDescription { get; set; }

      
        public DateTime? Date { get; set; }


        public string BIC { get; set; }


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

        public int OldID { get; set; }

        public int OldMeetingID { get; set; }
    }
}
