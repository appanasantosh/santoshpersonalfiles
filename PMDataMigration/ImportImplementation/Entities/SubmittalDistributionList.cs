using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class SubmittalDistributionList
    {
        public Guid ID { get; set; }
        public Guid SubmittalID { get; set; }
        public Guid ProjectContactsID { get; set; }
        public int DistType { get; set; }
        public int BCC { get; set; }
        public int CC { get; set; }
        public string ProjectCompanyName { get; set; }
        public string ProjectContactsEMail { get; set; }
        public string ProjectContactsName { get; set; }
        public int DistTo { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
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
        public int OldSubmittalID { get; set; }
    }
}
