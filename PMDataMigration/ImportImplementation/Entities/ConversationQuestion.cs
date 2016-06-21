using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class ConversationQuestion
    {
        
        public Guid ID { get; set; }
        public Guid CategoryID { get; set; }
        public string Question { get; set; }
        public string Response { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int IsActive { get; set; }
    }
}
