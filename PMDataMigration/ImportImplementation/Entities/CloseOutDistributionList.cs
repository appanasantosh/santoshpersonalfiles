using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class CloseOutDistributionList
    {
          
        public Guid ID { get; set; }
        public Guid CloseOutID { get; set; }
        public Guid ProjectContactID { get; set; }
        public int Type { get; set; }
        public int BCC { get; set; }
        public int CC { get; set; }
        public string ProjectContactEMail { get; set; }
        public string ProjectContactName { get; set; }
        public string ProjectContactCompany { get; set; }
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

        public int OldID { get; set; }
        public int OldCloseOutID { get; set; }
        public string OldProjectContactID { get; set; }
        public string OldProjectContactName { get; set; }
        public string OldProjectContactEMail { get; set; }
        public string OldProjectContactContactCompany { get; set; }

    }
}
