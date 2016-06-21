using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class Folders
    {
        public int PC_DOC_FLDR_ID { get; set; }
        public string PC_DOC_FLDR_NAME { get; set; }
        public string PC_DOC_FLDR_USER_ID { get; set; }
        public int PC_DOC_FLDR_NODE_ID { get; set; }
        public int PC_DOC_FLDR_PARENT_ID { get; set; }
        public string PC_DOC_FLDR_PRJ_ID { get; set; }
        public string PC_DOC_FLDR_TYPE_ID { get; set; }
        public int PC_DOC_FLDR_PRJ_MODE { get; set; }
        public string PC_DOC_FLDR_IS_LOCKED { get; set; }
        public string PC_DOC_FLDR_WEB_MARK { get; set; }
    }
}
