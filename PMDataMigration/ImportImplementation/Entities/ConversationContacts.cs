using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class ConversationContacts
    {
        public Guid ID { get; set; }
        public Guid ConversationID { get; set; }
        public Guid ProjectContactsID { get; set; }
        public Guid LoginID { get; set; }
        public Guid ContactsID { get; set; }
        public string IsActive { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
