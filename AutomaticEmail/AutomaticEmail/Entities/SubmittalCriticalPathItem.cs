using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticEmail.Entities
{
    public class SubmittalCriticalPathItem
    {
        public string ProjectNumber { get; set; }
        public string SpecNumber { get; set; }
        public string SpecPara { get; set; }
        public string SpecTitle { get; set; }

        public DateTime? ActualBuyOutDate { get; set; }
        public string SubmittalsDescription { get; set; }

        public int? AEReviewAndProcessingDays { get; set; }
        public DateTime? ToAEDate { get; set; }
        public DateTime? DateReturnedFromAE { get; set; }

        public string CompanyName { get; set; }

        public DateTime? AEDue { get; set; }

        public string SubmittalNumber { get; set; }
        public string SpecNumberPara { get; set; }

        public DateTime? ActualToAEDate { get; set; }

        public string SendStatus { get; set; }
        public string SubVendorName { get; set; }
        public string AccountingNumber { get; set; }

        public DateTime? SubmittalDate { get; set; }
        public string AC4 { get; set; }

        public string ProjectName { get; set; }
        public string AEComments { get; set; }
    }
}
