using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class TransmittalItem
    {
         
        public Guid ID { get; set; }      
        public Guid TransmittalID { get; set; }         
        public int SerialNumber { get; set; }         
        public int? Copies { get; set; }         
        public string ItemVersion { get; set; }         
        public string UseCode { get; set; }         
        public string ShtNumber { get; set; }         
        public string SpecParaNumber { get; set; }         
        public string ItemDescription { get; set; }        
        public string BrochNumber { get; set; }        
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

        public int oldID { get; set; }
        public int oldTransmittalID { get; set; }
    }
}
