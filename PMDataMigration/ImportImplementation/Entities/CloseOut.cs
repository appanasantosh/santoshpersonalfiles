using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class CloseOut
    {
          
        public Guid ID { get; set; }
        public Guid ProjectID { get; set; }
        public Guid ProjectContactID { get; set; }
        public Guid? AreaID { get; set; }
        public int? Number { get; set; }
        public int? SpecNo { get; set; }
        public string Description { get; set; }
        public string DocumentDescription { get; set; }
        public string SpecPara { get; set; }
        public string SpecTitle { get; set; }

                
        public string Status { get; set; }

          
        public DateTime? DateToAEO { get; set; }
        public DateTime? DueDate { get; set; }
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
        public string Representative { get; set; }
        public int? AreaPackages { get; set; }

        public int OldID { get; set; }
        public Guid OldProjectID { get; set; }
        public string OldProjectContactID { get; set; }
        public int OldProjectAreaID { get; set; }


    }
}
