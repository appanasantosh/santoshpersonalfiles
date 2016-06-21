
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMImportImplementation.Entities
{
	
	public class CertifiedPayRoll
	{
		public Guid ID { get; set; }
        public Guid? CompanyID { get; set; }
        public Guid ProjectID { get; set; }
        public int Onsite { get; set; }
        public int SF1413 { get; set; }
        public int SWFP1185 { get; set; }
        public int Training { get; set; }
        public string WorkDescription { get; set; }
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
        public Guid? ParentID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyAddressLine2 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyZip { get; set; }
        public string AccountNo { get; set; }       
        public int Level1 { get; set; }        
        public int Level2 { get; set; }        
        public int Level3 { get; set; }        
        public int Level4 { get; set; }        
        public int Level5 { get; set; }        
        public Guid OldProjectID { get; set; }       
        public Guid OldCompanyID { get; set; }        
        public int OldID { get; set; }
        
        
	}
}
