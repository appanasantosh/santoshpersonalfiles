using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class COPRDistributionList
    {
        public Guid ID { get; set; }
        public Guid CoprID { get; set; }
        public Guid ProjectContactID { get; set; }
        public int BCC { get; set; }
        public int CC { get; set; }
        public string ProjectContactEmail { get; set; }
        public string ProjectContactName { get; set; }
        public int To { get; set; }
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
        public int OldCoprID { get; set; }
        public string OldprojectContactID { get; set; }
        public string OldProjectId { get; set; }
        public string OldContactName { get; set; }
        public string OldContactEmail { get; set; }
        public string OldContatCompany { get; set; }
    }
}
