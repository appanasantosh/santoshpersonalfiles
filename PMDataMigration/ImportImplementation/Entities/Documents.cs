using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class Documents
    {
        public string PC_DOC_ID { get; set; }
        public string PC_DOC_PRJ_FK { get; set; }
        public string PC_DOC_ASG_TYPE { get; set; }
        public  DateTime? PC_DOC_REC_DATE { get; set; }
        public string PC_DOC_DESC { get; set; }
        public string PC_DOC_TYPE { get; set; }
        public string PC_DOC_TYPE_ID { get; set; }
        public string PC_DOC_TYPE_DESC { get; set; }
        public string PC_DOC_PATH { get; set; }
        public string PC_DOC_IS_ACTIVE { get; set; }
        public string PC_DOC_CATEGORY { get; set; }
        public DateTime? PC_DOC_DOC_DATE { get; set; }
        public string PC_DOC_CNTR_DOC { get; set; }
        public string PC_DOC_REFERENCE { get; set; }
        public DateTime? PC_DOC_REV_DOC_DATE { get; set; }
        public string PC_DOC_SHEETS_PAGES { get; set; }
        public string PC_DOC_FOLDER_PATH { get; set; }
        public string PC_DOC_CONTACT_FK { get; set; }
        public int PC_DOC_CONTROL_FK { get; set; }
        public int PC_DOC_PRJ_MODE { get; set; }
        public string PC_DOC_IS_LOCKED { get; set; }
        public int PC_DOC_FLDR_NODE_ID { get; set; }
        public int PC_DOC_FLDR_PARENT_ID { get; set; }
        public int PC_DOC_FLDR_ROOT_ID { get; set; }
        public string PC_DOC_FLDR_WEB_MARK { get; set; }
    }
}
