using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class Instruction
    {
        public DateTime? ToContractorDate { get; set; }
        public string SendStatus { get; set; }
        public TimeSpan? ResponseTime { get; set; }
        public int ResponseDays { get; set; }
        public Guid? ProjectID { get; set; }
        public string PIDescription { get; set; }
        public string Action{get;set;}
        public string Originator { get; set; }
        public DateTime? OriginationDate { get; set; }
        public int SerialNumber { get; set; }
        public string Suffix { get; set; }
        public int IsSent { get; set; }
        public int IsActive { get; set; }
        public Guid ID { get; set; }
        public int DuplicateType { get; set; }
        public string InstructionDescription { get; set; }
        public Guid ProjectControlID { get; set; }
        public Guid? ProjectAreaID { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int DataRowVersion { get; set; }
        public int? UserNumber { get; set; }
        public string AC1 { get; set; }
        public string AC2 { get; set; }
        public string AC3 { get; set; }
        public string AC4 { get; set; }
        public string AC5 { get; set; }
        public string AC6 { get; set; }
        public int Alias { get; set; }

        public int OldID { get; set; }
        public string OldProjectID { get; set; }
        public int OldProjectControlID { get; set; }
    }
}
