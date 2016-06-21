
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMImportImplementation.Entities
{
	public class CertifiedPayReports
	{
        public Guid ID { get; set; }
        public Guid CertifiedPayRollID { get; set; }
        public int IsActive { get; set; }
        public int Number { get; set; }
        public int SendStatus { get; set; }
        public int EmpDed { get; set; }
        public string Notes { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DateSend { get; set; }
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
        public int OldID { get; set; }
        public int OldCertifiedPayRollID { get; set; }
	}
}
