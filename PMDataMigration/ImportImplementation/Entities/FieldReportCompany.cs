using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class FieldReportCompany
    {
        
        public Guid ID { get; set; }
        public Guid FieldReportID { get; set; }
        public int TimeCard { get; set; }
        public string Notes { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int IsActive { get; set; }
    }
}
