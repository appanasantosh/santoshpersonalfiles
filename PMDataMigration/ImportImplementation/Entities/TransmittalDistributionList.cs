using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class TransmittalDistributionList
    {
        public Guid ID { get; set; }
        public Guid ProjectContactID { get; set; }
        public Guid TransmittalID { get; set; }
        public int Cc { get; set; }
        public int Bcc { get; set; }
        public int To { get; set; }
        public string ContactEmail { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
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

        public string OldID { get; set; }
        public int OldTransmittalID { get; set; }
        public string OldProjectContactID { get; set; }
        public string OldContactName { get; set; }
        public string OldContactEmail { get; set; }
        public string OldContactCompany { get; set; }
    }
}
