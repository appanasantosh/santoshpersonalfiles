using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Entities
{
    public class Submittals
    {

        public Guid ID { get; set; }
        public Guid? AreaID { get; set; }
        public Guid? SubVendorID { get; set; }
        public Guid ProjectID { get; set; }
        public DateTime? ActualToOwnerDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public DateTime? ActualToSubVendorDate { get; set; }
        public DateTime? ActualToAEDate { get; set; }
        public DateTime? BuyOutDate { get; set; }
        public DateTime? DateReturnedFromAE { get; set; }
        public DateTime? DateReturnedFromOwner { get; set; }
        public DateTime? ActualBuyOutDate { get; set; }
        public DateTime? MatNeededBy { get; set; }
        public DateTime? RequiredDateFromAE { get; set; }
        public DateTime? ToSubVendorDate { get; set; }
        public DateTime? SubmittalDate { get; set; }
        public DateTime? ToAEDate { get; set; }
        public DateTime? RequiredDeliveryDate { get; set; }
        public int? AEReviewAndProcessingDays { get; set; }
        public int? FabricationAndDeliveryDays { get; set; }
        public int? OwnerReviewAndProcessingDays { get; set; }
        public int? DaysLateFromOwner { get; set; }
        public int? DaysLateFromUEC { get; set; }
        public int? DupType { get; set; }
        public int? Number { get; set; }
        public int? SubmittalPreparationDays { get; set; }
        public int Status { get; set; }
        public int IsActive { get; set; }
        public string AENotes { get; set; }
        public string ContractNumber { get; set; }
        public string SubmittalsDescription { get; set; }
        public int? Mark1 { get; set; }
        public int? Mark2 { get; set; }
        public int? Mark3 { get; set; }
        public int? Mark4 { get; set; }
        public int? Mark5 { get; set; }
        public int? Mark6 { get; set; }
        public string DocumentDescription { get; set; }
        public string ExternalStatus { get; set; }
        public string OwnerComments { get; set; }
        public int? FIO { get; set; }
        public int? GovtAppr { get; set; }
        public int? NewTrans { get; set; }
        public string Notes { get; set; }
        public Guid? Orginator { get; set; }
        public string OwnerNotes { get; set; }
        public string Recipient { get; set; }
        public string RecipientRep { get; set; }
        public string Remarks { get; set; }
        public string SubmittalSuffix { get; set; }
        public int? ReSubTrans { get; set; }
        public string ReSubTransNumber { get; set; }
        public string RetNotes { get; set; }
        public string SDNumber { get; set; }
        public string SendStatus { get; set; }
        public Guid? Sender { get; set; }
        public string SpecNumber { get; set; }
        public string SpecPara { get; set; }
        public string SpecTitle { get; set; }
        public string Specifications { get; set; }
        public string AEComments { get; set; }
        public string ToNotes { get; set; }
        public string TransNumber { get; set; }

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
        public int? Alias { get; set; }
        public DateTime? ToOwnerDate { get; set; }
        public int? AreaNumber { get; set; }
        public string DescriptionArea { get; set; }
        public string SubvendorName { get; set; }
        public string SubVendorAccNo { get; set; }
        public string BuyOutStatus { get; set; }
        public string AEStatus { get; set; }
        public string GovernmentStatus { get; set; }
        public string SubVendorStatus { get; set; }
        public string DeliveryStatus { get; set; }



        #region OldProcon ID's
        public int OldID { get; set; }
        public int? OldAreaID { get; set; }
        public string OldOrginator { get; set; }
        public string OldProjectID { get; set; }
        public string OldSender { get; set; }
        public string OldSubVendorID { get; set; }
        #endregion

    }
}
