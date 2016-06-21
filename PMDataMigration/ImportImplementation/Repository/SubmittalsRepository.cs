using MySql.Data.MySqlClient;
using PMImportImplementation.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMImportImplementation.Repository
{
    public class SubmittalsRepository
    {
        RestoreStatus restoreStatus = RestoreStatus.None;
        public int RestoreSubmittalsData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreSubmittalsData");
            int result = 0;
            try
            {

                PMMigrationLogger.Log("DATA RETRIEVAL STARTED ---------------------", Color.Black, FontStyle.Bold);
                List<Submittals> submittals = new List<Submittals>();
                List<SubmittalItems> submittalItems = new List<SubmittalItems>();
                List<SubmittalDistributionList> submittalDistributionLists = new List<SubmittalDistributionList>();

                submittals = GetSubmittalDataOldProcon(projectID, mysqlCon);
                PMMigrationLogger.Log("Number of submittals retrived are : " + submittals.Count);

                if (restoreStatus == RestoreStatus.Success)
                {
                    submittalItems = GetSubmittalItemsDataOldProcon(submittals, mysqlCon);
                    PMMigrationLogger.Log("Number of submittalItems retrived are : " + submittalItems.Count);
                }

                if (restoreStatus == RestoreStatus.Success)
                {
                    submittalDistributionLists = GetSubmittalDistributionListDataFromOldProcon(submittals, mysqlCon);
                    PMMigrationLogger.Log("Number of submittalDistributionLists retrived are : " + submittalDistributionLists.Count);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Creating Old ProjectArea---- New Project Area Relation ....It might tale several minutes , please be patient", Color.Black, FontStyle.Bold);
                    submittals = SubmittalProjectAreaMapping(submittals, sqlCon);
                    PMMigrationLogger.Log("Creating Old ProjectArea---- New Project Area Relation Completed", Color.Black, FontStyle.Bold);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Creating Old SubVendor---- New SubVendor Relation ....It might tale several minutes , please be patient", Color.Black, FontStyle.Bold);
                    submittals = SubmittalSubVendorMapping(submittals, sqlCon);
                    PMMigrationLogger.Log("Creating Old SubVendor---- New SubVendor Relation Completed", Color.Black, FontStyle.Bold);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("DATA INSERTION INTO IDBO STARTED  ---------------------", Color.Black, FontStyle.Bold);
                    InsertSubmittalsDataToIDBO(submittals, submittalItems, submittalDistributionLists, sqlCon);
                    PMMigrationLogger.Log("DATA INSERTION INTO IDBO COMPLETED  ---------------------", Color.Black, FontStyle.Bold);
                }
                PMMigrationLogger.Log("Out RestoreSubmittalsData");
            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                PMMigrationLogger.Log(ex.ToString(), Color.Red, FontStyle.Regular);
                throw ex;
            }
            return result;
        }

        private void InsertSubmittalsDataToIDBO(List<Submittals> submittals, List<SubmittalItems> submittalItems, List<SubmittalDistributionList> submittalDistributionLists, SqlConnection sqlCon)
        {
            InsertSubmittalsToIDBO(submittals, sqlCon);
            InsertSubmittalItemsToIDBO(submittalItems, sqlCon);
            InsertSubmittalDistributionListToIDBO(submittalDistributionLists, sqlCon);
        }

        private void InsertSubmittalDistributionListToIDBO(List<SubmittalDistributionList> submittalDistributionLists, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertSubmittalDistributionListToIDBO : Restoring SubmittalDistributionLists started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var submittalDistributionList in submittalDistributionLists)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;

                        string qrySubmittalDistributionList = string.Format("INSERT INTO [PMSubmittalDistributionList]" +
                                                                            "([ID],[ProjectContactsID],[BCC],[CC],[SubmittalID],[DistTo],[DistType],[AC1],[OldID],[OldSubmittalID])" +
                                                                            "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                        submittalDistributionList.ID, submittalDistributionList.ProjectContactsID, submittalDistributionList.BCC, submittalDistributionList.CC, submittalDistributionList.SubmittalID,
                        submittalDistributionList.DistTo, submittalDistributionList.DistType, submittalDistributionList.AC1.Replace("'", "''"), submittalDistributionList.OldID, submittalDistributionList.OldSubmittalID);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qrySubmittalDistributionList;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN SubmittalDistributionLists INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
        }

        private void InsertSubmittalItemsToIDBO(List<SubmittalItems> submittalItems, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertSubmittalItemsDataToIDBO : Restoring SubmittalItems started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;

                foreach (var submittalItem in submittalItems)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qrySubmittalItems = string.Format("INSERT INTO [PMSubmittalItems]" +
                                                           "([ID],[SubmittalID],[ActionCode],[BrochNumber],[Copies],[SubmittalItemDescription]," +
                                                           "[Number],[ParaNumber],[DrawingSheetNumber],[ContrCode],[Var],[ForCEUseCode]," +
                                                           "[GovernmentActionCode],[AC1],[OldID],[OldSubmittalID])" +
                                                        "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}'," +
                                                        "'{6}','{7}','{8}','{9}','{10}','{11}'," +
                                                         "'{12}','{13}','{14}','{15}')",
                                                         submittalItem.ID, submittalItem.SubmittalID, submittalItem.ActionCode.Replace("'", "''"), submittalItem.BrochNumber.Replace("'", "''"), submittalItem.Copies, submittalItem.SubmittalItemDescription.Replace("'", "''"),
                                                         submittalItem.Number, submittalItem.ParaNumber.Replace("'", "''"), submittalItem.DrawingSheetNumber.Replace("'", "''"), submittalItem.ContrCode.Replace("'", "''"), submittalItem.Var.Replace("'", "''"), submittalItem.ForCEUseCode.Replace("'", "''"),
                                                         submittalItem.GovernmentActionCode.Replace("'", "''"), submittalItem.AC1.Replace("'", "''"), submittalItem.OldID, submittalItem.OldSubmittalID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qrySubmittalItems;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN SubmittalItems INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertSubmittalItemsDataToIDBO : Restoring SubmittalItems started  --------------------", Color.Black, FontStyle.Bold);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }

        }

        private List<Submittals> SubmittalProjectAreaMapping(List<Submittals> umpappedSubmittals, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In ProjectAreaMapping");
            List<Submittals> mappedSubmittals = new List<Submittals>();
            try
            {
                sqlCon.Open();

                foreach (var submittal in umpappedSubmittals)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        SqlDataReader reader = null;
                        cmd.Connection = sqlCon;
                        string areaQry = String.Format("select ID,OldID from FinProjectArea where OldID='{0}';", submittal.OldAreaID);

                        cmd.CommandText = areaQry;
                        reader = cmd.ExecuteReader();
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Guid ID;
                                Guid.TryParse(reader["ID"].ToString(), out ID);
                                submittal.AreaID = ID;
                            }
                        }
                        mappedSubmittals.Add(submittal);
                        PMMigrationLogger.Log("Old Area -> New Area : " + submittal.OldAreaID + "->" + submittal.AreaID);
                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        PMMigrationLogger.Log("MAPPING ERROR : Some error occured ", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR  : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                    sqlCon.Close();
            }
            PMMigrationLogger.Log("Out ProjectAreaMapping");
            return mappedSubmittals;
        }


        private List<Submittals> SubmittalSubVendorMapping(List<Submittals> umpappedSubmittals, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In SubmittalSubVendorMapping");
            List<Submittals> mappedSubmittals = new List<Submittals>();
            try
            {
                sqlCon.Open();

                foreach (var submittal in umpappedSubmittals)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        SqlDataReader reader = null;
                        cmd.Connection = sqlCon;
                        string areaQry = String.Format("select ID from GenCompany where ID='{0}';", submittal.OldSubVendorID);

                        cmd.CommandText = areaQry;
                        reader = cmd.ExecuteReader();
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Guid ID;
                                Guid.TryParse(reader["ID"].ToString(), out ID);
                                submittal.SubVendorID = ID;
                            }
                        }
                        mappedSubmittals.Add(submittal);
                        PMMigrationLogger.Log("Old SubVendor -> New SubVendor : " + submittal.OldSubVendorID + "->" + submittal.SubVendorID);
                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        PMMigrationLogger.Log("MAPPING ERROR : Some error occured ", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR  : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                    sqlCon.Close();
            }
            PMMigrationLogger.Log("Out SubmittalSubVendorMapping");
            return mappedSubmittals;
        }

        private void InsertSubmittalsToIDBO(List<Submittals> submittals, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertSubmittalsDataToIDBO : Restoring Submittals started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var submittal in submittals)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qrySubmittal = string.Format("INSERT INTO [PMSubmittals]([ID],[ProjectID],[SubVendorID],[ActualToOwnerDate],[ActualDeliveryDate]," +
                                                       "[ActualToSubVendorDate],[AENotes],[AreaID],[ActualToAEDate],[BuyOutDate]," +
                                                       "[ContractNumber],[DateReturnedFromAE],[DateReturnedFromOwner],[ActualBuyOutDate],[AEReviewAndProcessingDays]," +
                                                       "[FabricationAndDeliveryDays],[OwnerReviewAndProcessingDays],[DaysLateFromOwner],[DaysLateFromUEC]," +
                                                       "[SubmittalsDescription],[Mark1],[Mark2],[Mark3],[Mark4],[Mark5],[Mark6]," +
                                                       "[DocumentDescription],[DupType],[ExternalStatus],[OwnerComments],[FIO],[GovtAppr],[MatNeededBy]," +
                                                       "[NewTrans],[Number],[Notes],[Orginator],[OwnerNotes],[SubmittalPreparationDays],[Recipient]," +
                                                       "[RecipientRep],[Remarks],[RequiredDateFromAE],[ToSubVendorDate],[ToAEDate],[SubmittalDate]," +
                                                       "[RequiredDeliveryDate],[SubmittalSuffix],[ReSubTrans],[ReSubTransNumber],[RetNotes]," +
                                                       "[SDNumber],[SendStatus],[Sender],[SpecNumber],[SpecPara],[SpecTitle]," +
                                                       "[Specifications],[Status],[AEComments],[ToNotes],[TransNumber]," +
                                                       "[AC1],[ToOwnerDate],[OldID],[OldAreaID],[OldOrginator],[OldProjectID]," +
                                                       "[OldSender],[OldSubVendorID])" +
                                                       "VALUES ('{0}','{1}','{2}',{3},{4}," +
                                                        "{5},'{6}','{7}',{8},{9}," +
                                                        "'{10}',{11},{12},{13},'{14}'," +
                                                        "'{15}','{16}','{17}','{18}'," +
                                                        "'{19}','{20}','{21}','{22}','{23}','{24}','{25}'," +
                                                        "'{26}','{27}','{28}','{29}','{30}','{31}','{32}'," +
                                                        "'{33}','{34}','{35}','{36}','{37}','{38}','{39}'," +
                                                        "'{40}','{41}',{42},{43},{44},{45}," +
                                                        "{46},'{47}','{48}','{49}','{50}'," +
                                                        "'{51}','{52}','{53}','{54}','{55}','{56}'," +
                                                        "'{57}','{58}','{59}','{60}','{61}'," +
                                                        "'{62}',{63},'{64}','{65}','{66}','{67}'," +
                                                        "'{68}','{69}')",
                                                        submittal.ID, submittal.ProjectID, submittal.SubVendorID, (submittal.ActualToOwnerDate == null) ? (object)"null" : "'" + submittal.ActualToOwnerDate + "'", (submittal.ActualDeliveryDate == null) ? (object)"null" : "'" + submittal.ActualDeliveryDate + "'",
                                                        (submittal.ActualToSubVendorDate == null) ? (object)"null" : "'" + submittal.ActualToSubVendorDate + "'", submittal.AENotes.Replace("'", "''"), submittal.AreaID, (submittal.ActualToAEDate == null) ? (object)"null" : "'" + submittal.ActualToAEDate + "'", (submittal.BuyOutDate == null) ? (object)"null" : "'" + submittal.BuyOutDate + "'",
                                                        submittal.ContractNumber, (submittal.DateReturnedFromAE == null) ? (object)"null" : "'" + submittal.DateReturnedFromAE + "'", (submittal.DateReturnedFromOwner == null) ? (object)"null" : "'" + submittal.DateReturnedFromOwner + "'", (submittal.ActualBuyOutDate == null) ? (object)"null" : "'" + submittal.ActualBuyOutDate + "'", submittal.AEReviewAndProcessingDays,
                                                        submittal.FabricationAndDeliveryDays, submittal.OwnerReviewAndProcessingDays, submittal.DaysLateFromOwner, submittal.DaysLateFromUEC,
                                                        submittal.SubmittalsDescription.Replace("'", "''"), submittal.Mark1, submittal.Mark2, submittal.Mark3, submittal.Mark4, submittal.Mark5, submittal.Mark6,
                                                        submittal.DocumentDescription.Replace("'", "''"), submittal.DupType, submittal.ExternalStatus.Replace("'", "''"), submittal.OwnerComments.Replace("'", "''"), submittal.FIO, submittal.GovtAppr, submittal.MatNeededBy,
                                                        submittal.NewTrans, submittal.Number, submittal.Notes.Replace("'", "''"), submittal.Orginator, submittal.OwnerNotes.Replace("'", "''"), submittal.SubmittalPreparationDays, submittal.Recipient,
                                                        submittal.RecipientRep, submittal.Remarks.Replace("'", "''"), (submittal.RequiredDateFromAE == null) ? (object)"null" : "'" + submittal.RequiredDateFromAE + "'", (submittal.ToSubVendorDate == null) ? (object)"null" : "'" + submittal.ToSubVendorDate + "'", (submittal.ToAEDate == null) ? (object)"null" : "'" + submittal.ToAEDate + "'", (submittal.SubmittalDate == null) ? (object)"null" : "'" + submittal.SubmittalDate + "'",
                                                        (submittal.RequiredDeliveryDate == null) ? (object)"null" : "'" + submittal.RequiredDeliveryDate + "'", submittal.SubmittalSuffix.Replace("'", "''"), submittal.ReSubTrans, submittal.ReSubTransNumber.Replace("'", "''"), submittal.RetNotes.Replace("'", "''"),
                                                        submittal.SDNumber.Replace("'", "''"), submittal.SendStatus.Replace("'", "''"), submittal.Sender, submittal.SpecNumber.Replace("'", "''"), submittal.SpecPara.Replace("'", "''"), submittal.SpecTitle.Replace("'", "''"),
                                                        submittal.Specifications.Replace("'", "''"), submittal.Status, submittal.AEComments.Replace("'", "''"), submittal.ToNotes, submittal.TransNumber,
                                                        submittal.AC1.Replace("'", "''"), (submittal.ToOwnerDate == null) ? (object)"null" : "'" + submittal.ToOwnerDate + "'", submittal.OldID, submittal.OldAreaID, submittal.OldOrginator, submittal.OldProjectID,
                                                        submittal.OldSender, submittal.OldSubVendorID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qrySubmittal;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN Submittals INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertSubmittalsDataToIDBO : Restoring Submittals completed  --------------------", Color.Black, FontStyle.Bold);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
        }

        private List<Submittals> GetSubmittalDataOldProcon(Guid projectID, MySqlConnection mysqlCon)
        {
            PMMigrationLogger.Log("In GetSubmittalDataOldProcon : GETTING SUBMITTALS  STARTED ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader reader = null;
            List<Submittals> submittals = new List<Submittals>();

            try
            {
                //string.Format("Select * from pc_submitals where PC_SUB_PRJ_FK = '{0}'", projectID);

                string myQuery = string.Format("select * from pc_submitals where PC_SUB_PRJ_FK IN (SELECT  PC_PRJ_ID FROM pc_project_list where PC_PRJ_PM_STATUS='Active' and PC_PRJ_IS_DELETED='N'  and " +
                                            "pc_project_list.PC_PRJ_CORP_FK not in(Select pc_corp_ID from procon_migration_db.pc_Corp where pc_corp_id not in(" +
                                            "Select  corp.pc_corp_ID FROM procon_migration_db.temp_Company join procon_migration_db.pc_company on temp_Company.ID = pc_company.PC_COMP_ID " +
                                            "join procon_migration_db.pc_corp corp on  (corp.PC_CORP_ID = pc_company.PC_COMP_CORP_FK )  where pc_company.PC_COMP_IS_HIDDEN = 'N' and pc_company.PC_COMP_IS_DEFAULT = 'Y'and corp.PC_CORP_NAME not like 'Synergy')))");

                mysqlCon.Open();
                mySqlCmd.Connection = mysqlCon;
                mySqlCmd.CommandText = myQuery;
                reader = mySqlCmd.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Submittals submittal = new Submittals();

                        submittal.ID = Guid.NewGuid();

                        int oldID;
                        int.TryParse(reader["PC_SUB_ID"].ToString(), out oldID);
                        submittal.OldID = oldID;


                        submittal.OldProjectID = reader["PC_SUB_PRJ_FK"].ToString();

                        Guid m_ProjectID;
                        Guid.TryParse(reader["PC_SUB_PRJ_FK"].ToString(), out m_ProjectID);
                        submittal.ProjectID = m_ProjectID;

                        submittal.AreaID = Guid.Empty;
                        submittal.Orginator = Guid.Empty;
                        submittal.Sender = Guid.Empty;
                        submittal.SubVendorID = Guid.Empty;

                        /////////////////////////////

                        submittal.SubmittalsDescription = reader["PC_SUB_DESC"].ToString();

                        if (reader["PC_SUB_ACTUAL_DELIVERY_DATE"] == DBNull.Value)
                            submittal.ActualDeliveryDate = null;
                        else
                        {
                            DateTime m_ActualDeliveryDate;
                            DateTime.TryParse(reader["PC_SUB_ACTUAL_DELIVERY_DATE"].ToString(), out m_ActualDeliveryDate);
                            submittal.ActualDeliveryDate = m_ActualDeliveryDate;
                        }

                        if (reader["PC_SUB_ACTUAL_OWNER_RESPONDED"] == DBNull.Value)
                            submittal.ActualToSubVendorDate = null;
                        else
                        {
                            DateTime m_ActualToSubVendorDate;
                            DateTime.TryParse(reader["PC_SUB_ACTUAL_OWNER_RESPONDED"].ToString(), out m_ActualToSubVendorDate);
                            submittal.ActualToSubVendorDate = m_ActualToSubVendorDate;
                        }

                        submittal.AENotes = reader["PC_SUB_AE_NOTES"].ToString();

                        if (reader["PC_SUB_AE_RESPONDED"] == DBNull.Value)
                            submittal.ActualToAEDate = null;
                        else
                        {
                            DateTime m_ActualToAEDate;
                            DateTime.TryParse(reader["PC_SUB_AE_RESPONDED"].ToString(), out m_ActualToAEDate);
                            submittal.ActualToAEDate = m_ActualToAEDate;
                        }

                        if (reader["PC_SUB_AREA_PKGS"] == DBNull.Value)
                            submittal.OldAreaID = null;
                        else
                        {
                            int m_OldAreaID;
                            int.TryParse(reader["PC_SUB_AREA_PKGS"].ToString(), out m_OldAreaID);
                            submittal.OldAreaID = m_OldAreaID;
                        }

                        if (reader["PC_SUB_BUYOUT_DATE"] == DBNull.Value)
                            submittal.BuyOutDate = null;
                        else
                        {
                            DateTime m_BuyOutDate;
                            DateTime.TryParse(reader["PC_SUB_BUYOUT_DATE"].ToString(), out m_BuyOutDate);
                            submittal.BuyOutDate = m_BuyOutDate;
                        }

                        submittal.ContractNumber = reader["PC_SUB_CONTRACT_NO"].ToString();


                        if (reader["PC_SUB_DATE_RETURNED_FROM_AE"] == DBNull.Value)
                            submittal.DateReturnedFromAE = null;
                        else
                        {
                            DateTime m_DateReturnedFromAE;
                            DateTime.TryParse(reader["PC_SUB_DATE_RETURNED_FROM_AE"].ToString(), out m_DateReturnedFromAE);
                            submittal.DateReturnedFromAE = m_DateReturnedFromAE;
                        }

                        if (reader["PC_SUB_DATE_RETURNED_FROM_OWNER"] == DBNull.Value)
                            submittal.DateReturnedFromOwner = null;
                        else
                        {
                            DateTime m_DateReturnedFromOwner;
                            DateTime.TryParse(reader["PC_SUB_DATE_RETURNED_FROM_OWNER"].ToString(), out m_DateReturnedFromOwner);
                            submittal.DateReturnedFromOwner = m_DateReturnedFromOwner;
                        }

                        if (reader["PC_SUB_DATE_TO_AE"] == DBNull.Value)
                            submittal.ActualBuyOutDate = null;
                        else
                        {
                            DateTime m_ActualBuyOutDate;
                            DateTime.TryParse(reader["PC_SUB_DATE_TO_AE"].ToString(), out m_ActualBuyOutDate);
                            submittal.ActualBuyOutDate = m_ActualBuyOutDate;
                        }

                        if (reader["PC_SUB_ACTUAL_DATE_TO_OWNER"] == DBNull.Value)
                            submittal.ActualToOwnerDate = null;
                        else
                        {
                            DateTime m_ActualToOwnerDate;
                            DateTime.TryParse(reader["PC_SUB_ACTUAL_DATE_TO_OWNER"].ToString(), out m_ActualToOwnerDate);
                            submittal.ActualToOwnerDate = m_ActualToOwnerDate;
                        }

                        if (reader["PC_SUB_DAYS_ALLOWED"] == DBNull.Value)
                            submittal.AEReviewAndProcessingDays = null;
                        else
                        {
                            int m_AEReviewAndProcessingDays;
                            int.TryParse(reader["PC_SUB_DAYS_ALLOWED"].ToString(), out m_AEReviewAndProcessingDays);
                            submittal.AEReviewAndProcessingDays = m_AEReviewAndProcessingDays;
                        }

                        if (reader["PC_SUB_DAYS_ALLOWED_FOR_FABRICATION"] == DBNull.Value)
                            submittal.FabricationAndDeliveryDays = null;
                        else
                        {
                            int m_FabricationAndDeliveryDays;
                            int.TryParse(reader["PC_SUB_DAYS_ALLOWED_FOR_FABRICATION"].ToString(), out m_FabricationAndDeliveryDays);
                            submittal.FabricationAndDeliveryDays = m_FabricationAndDeliveryDays;
                        }

                        if (reader["PC_SUB_DAYS_ALLOWED_OWNER"] == DBNull.Value)
                            submittal.OwnerReviewAndProcessingDays = null;
                        else
                        {
                            int m_OwnerReviewAndProcessingDays;
                            int.TryParse(reader["PC_SUB_DAYS_ALLOWED_OWNER"].ToString(), out m_OwnerReviewAndProcessingDays);
                            submittal.OwnerReviewAndProcessingDays = m_OwnerReviewAndProcessingDays;
                        }

                        if (reader["PC_SUB_DAYS_LATE_FROM_OWNER"] == DBNull.Value)
                            submittal.DaysLateFromOwner = null;
                        else
                        {
                            int m_DaysLateFromOwner;
                            int.TryParse(reader["PC_SUB_DAYS_LATE_FROM_OWNER"].ToString(), out m_DaysLateFromOwner);
                            submittal.DaysLateFromOwner = m_DaysLateFromOwner;
                        }

                        if (reader["PC_SUB_DAYS_LATE_FROM_UEC"] == DBNull.Value)
                            submittal.DaysLateFromUEC = null;
                        else
                        {
                            int m_DaysLateFromUEC;
                            int.TryParse(reader["PC_SUB_DAYS_LATE_FROM_UEC"].ToString(), out m_DaysLateFromUEC);
                            submittal.DaysLateFromUEC = m_DaysLateFromUEC;
                        }

                        submittal.DocumentDescription = reader["PC_SUB_DOC_DESC"].ToString();

                        if (reader["PC_SUB_DUP_TYPE"] == DBNull.Value)
                            submittal.DupType = null;
                        else
                        {
                            int m_DupType;
                            int.TryParse(reader["PC_SUB_DUP_TYPE"].ToString(), out m_DupType);
                            submittal.DupType = m_DupType;
                        }

                        submittal.ExternalStatus = reader["PC_SUB_EXTERNAL_STATUS"].ToString();
                        submittal.OwnerComments = reader["PC_SUB_EXTERNAL_SV_NOTES"].ToString();

                        if (reader["PC_SUB_FIO"] == DBNull.Value)
                            submittal.FIO = null;
                        else
                        {
                            submittal.FIO = (reader["PC_SUB_FIO"].ToString() == "N") ? 0 : 1;
                        }

                        if (reader["PC_SUB_GOVT_APPR"] == DBNull.Value)
                            submittal.GovtAppr = null;
                        else
                        {
                            submittal.GovtAppr = (reader["PC_SUB_GOVT_APPR"].ToString() == "N") ? 0 : 1;
                        }

                        submittal.Mark1 = (reader["PC_SUB_DISTRIBUTION_MK1"].ToString() == "N") ? 0 : 1;
                        submittal.Mark2 = (reader["PC_SUB_DISTRIBUTION_MK2"].ToString() == "N") ? 0 : 1;
                        submittal.Mark3 = (reader["PC_SUB_DISTRIBUTION_MK3"].ToString() == "N") ? 0 : 1;
                        submittal.Mark4 = (reader["PC_SUB_DISTRIBUTION_MK4"].ToString() == "N") ? 0 : 1;
                        submittal.Mark5 = (reader["PC_SUB_DISTRIBUTION_MK5"].ToString() == "N") ? 0 : 1;
                        submittal.Mark6 = (reader["PC_SUB_DISTRIBUTION_MK6"].ToString() == "N") ? 0 : 1;

                        if (reader["PC_SUB_MAT_NEEDED_BY"] == DBNull.Value)
                            submittal.MatNeededBy = null;
                        else
                        {
                            DateTime m_MatNeededBy;
                            DateTime.TryParse(reader["PC_SUB_MAT_NEEDED_BY"].ToString(), out m_MatNeededBy);
                            submittal.MatNeededBy = m_MatNeededBy;
                        }

                        if (reader["PC_SUB_NEW_TRANS"] == DBNull.Value)
                            submittal.NewTrans = null;
                        else
                        {
                            submittal.NewTrans = (reader["PC_SUB_NEW_TRANS"].ToString() == "N") ? 0 : 1;
                        }

                        submittal.Notes = reader["PC_SUB_NOTES"].ToString();

                        if (reader["PC_SUB_NO"] == DBNull.Value)
                            submittal.Number = null;
                        else
                        {
                            int m_Number;
                            int.TryParse(reader["PC_SUB_NO"].ToString(), out m_Number);
                            submittal.Number = m_Number;
                        }

                        submittal.OldOrginator = reader["PC_SUB_ORIGINATOR_FK"].ToString();

                        submittal.OwnerNotes = reader["PC_SUB_OWNER_NOTES"].ToString();

                        if (reader["PC_SUB_PREP_DAYS"] == DBNull.Value)
                            submittal.SubmittalPreparationDays = null;
                        else
                        {
                            int m_SubmittalPreparationDays;
                            int.TryParse(reader["PC_SUB_PREP_DAYS"].ToString(), out m_SubmittalPreparationDays);
                            submittal.SubmittalPreparationDays = m_SubmittalPreparationDays;
                        }

                        submittal.Recipient = reader["PC_SUB_RECIPIENT"].ToString();
                        submittal.RecipientRep = reader["PC_SUB_RECIPIENT_REP"].ToString();
                        submittal.Remarks = reader["PC_SUB_REMARKS"].ToString();

                        if (reader["PC_SUB_REQ_DATE_FROM_AE"] == DBNull.Value)
                            submittal.RequiredDateFromAE = null;
                        else
                        {
                            DateTime m_RequiredDateFromAE;
                            DateTime.TryParse(reader["PC_SUB_REQ_DATE_FROM_AE"].ToString(), out m_RequiredDateFromAE);
                            submittal.RequiredDateFromAE = m_RequiredDateFromAE;
                        }

                        if (reader["PC_SUB_REQ_DATE_OWNER"] == DBNull.Value)
                            submittal.ToSubVendorDate = null;
                        else
                        {
                            DateTime m_ToSubVendorDate;
                            DateTime.TryParse(reader["PC_SUB_REQ_DATE_OWNER"].ToString(), out m_ToSubVendorDate);
                            submittal.ToSubVendorDate = m_ToSubVendorDate;
                        }

                        if (reader["PC_SUB_REQUIRED_DELIVERY_DATE"] == DBNull.Value)
                            submittal.RequiredDeliveryDate = null;
                        else
                        {
                            DateTime m_RequiredDeliveryDate;
                            DateTime.TryParse(reader["PC_SUB_REQUIRED_DELIVERY_DATE"].ToString(), out m_RequiredDeliveryDate);
                            submittal.RequiredDeliveryDate = m_RequiredDeliveryDate;
                        }

                        if (reader["PC_SUB_REQ_OWNER_SUB_DATE"] == DBNull.Value)
                            submittal.SubmittalDate = null;
                        else
                        {
                            DateTime m_SubmittalDate;
                            DateTime.TryParse(reader["PC_SUB_REQ_OWNER_SUB_DATE"].ToString(), out m_SubmittalDate);
                            submittal.SubmittalDate = m_SubmittalDate;
                        }

                        submittal.SubmittalSuffix = reader["PC_SUB_RESUB_LTR"].ToString();

                        if (reader["PC_SUB_RESUB_TRANS"] == DBNull.Value)
                            submittal.ReSubTrans = null;
                        else
                        {
                            submittal.ReSubTrans = (reader["PC_SUB_RESUB_TRANS"].ToString() == "N") ? 0 : 1;
                        }

                        submittal.ReSubTransNumber = reader["PC_SUB_RESUB_TRANS_NO"].ToString();
                        submittal.RetNotes = reader["PC_SUB_RET_NOTES"].ToString();
                        submittal.SDNumber = reader["PC_SUB_SD_NO"].ToString();


                        submittal.OldSender = reader["PC_SUB_SENDER_FK"].ToString();


                        submittal.SendStatus = reader["PC_SUB_SEND_STATUS"].ToString();
                        submittal.Specifications = reader["PC_SUB_SPECIFICATIONS"].ToString();
                        submittal.SpecNumber = reader["PC_SUB_SPEC_NO"].ToString();
                        submittal.SpecPara = reader["PC_SUB_SPEC_PARA"].ToString();
                        submittal.SpecTitle = reader["PC_SUB_SPEC_TITLE"].ToString();


                        int m_Status;
                        int.TryParse(reader["PC_SUB_STATUS"].ToString(), out m_Status);
                        submittal.Status = m_Status;

                        if (reader["PC_SUB_REQ_SUB_DATE"] == DBNull.Value)
                            submittal.ToAEDate = null;
                        else
                        {
                            DateTime m_ToAEDate;
                            DateTime.TryParse(reader["PC_SUB_REQ_SUB_DATE"].ToString(), out m_ToAEDate);
                            submittal.ToAEDate = m_ToAEDate;
                        }

                        //TODO: Read the remaining fields from the database.

                        submittal.OldSubVendorID = reader["PC_SUB_SUBVENDOR_ID"].ToString();

                        submittal.AEComments = reader["PC_SUB_SUBVENDOR_NOTES"].ToString();
                        submittal.ToNotes = reader["PC_SUB_TO_NOTES"].ToString();
                        submittal.TransNumber = reader["PC_SUB_TRANS_NO"].ToString();


                        submittal.AC1 = "Migrated Data";

                        //Getting data based on JOINS.
                        //if (reader["AreaNumber"] == DBNull.Value)
                        //{
                        //    submittal.AreaNumber = null;
                        //}
                        //else
                        //{
                        //    int m_AreaNumber;
                        //    int.TryParse(reader["AreaNumber"].ToString(), out m_AreaNumber);
                        //    submittal.AreaNumber = m_AreaNumber;
                        //}

                        //submittal.DescriptionArea = reader["DescriptionArea"].ToString();

                        //submittal.SubvendorName = reader["SubvendorName"].ToString();
                        //submittal.SubVendorAccNo = reader["SubVendorAccNo"].ToString();

                        //submittal.BuyOutStatus = reader["BuyOutStatus"].ToString();
                        //submittal.AEStatus = reader["AEStatus"].ToString();
                        //submittal.GovernmentStatus = reader["GovernmentStatus"].ToString();
                        //submittal.SubVendorStatus = reader["SubVendorStatus"].ToString();
                        //submittal.DeliveryStatus = reader["DeliveryStatus"].ToString();

                        submittals.Add(submittal);
                    }
                }

                restoreStatus = RestoreStatus.Success;
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                PMMigrationLogger.Log("Out GetSubmittalDataOldProcon : GETTING SUBMITTAL DATA  ENDED ...", Color.Black, FontStyle.Bold);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (mySqlCmd.Connection != null || mySqlCmd.Connection.State == ConnectionState.Open)
                {
                    mySqlCmd.Connection.Close();
                    //reader.Close();
                }
            }
            return submittals;
        }

        private List<SubmittalItems> GetSubmittalItemsDataOldProcon(List<Submittals> submittals, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetSubmittalDataOldProcon : GETTING SUBMITTAL ITEMS   STARTED ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader reader = null;
            List<SubmittalItems> submittalItems = new List<SubmittalItems>();
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;

                foreach (var submittal in submittals)
                {
                    string myQuery = string.Format("Select * from pc_transmittal_details where PC_TRANS_DTL_SUB_FK = '{0}'", submittal.OldID);
                    mySqlCmd.CommandText = myQuery;
                    reader = mySqlCmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SubmittalItems submittalItem = new SubmittalItems();

                            submittalItem.ID = Guid.NewGuid();

                            int m_OldID;
                            int.TryParse(reader["PC_TRANS_DTL_ID"].ToString(), out m_OldID);
                            submittalItem.OldID = m_OldID;

                            submittalItem.SubmittalID = submittal.ID;

                            int m_OldSubmittalID;
                            int.TryParse(reader["PC_TRANS_DTL_SUB_FK"].ToString(), out m_OldSubmittalID);
                            submittalItem.OldSubmittalID = m_OldSubmittalID;

                            submittalItem.ActionCode = reader["PC_TRANS_DESIGN_ACTION_CODE"].ToString();
                            submittalItem.BrochNumber = reader["PC_TRANS_DTL_BROCH_NO"].ToString();

                            int m_Copies;
                            int.TryParse(reader["PC_TRANS_DTL_COPIES"].ToString(), out m_Copies);
                            submittalItem.Copies = m_Copies;

                            submittalItem.SubmittalItemDescription = reader["PC_TRANS_DTL_DESC"].ToString();

                            int m_Number;
                            int.TryParse(reader["PC_TRANS_DTL_NO"].ToString(), out m_Number);
                            submittalItem.Number = m_Number;

                            submittalItem.AC1 = "Migrated Data";

                            submittalItem.ParaNumber = reader["PC_TRANS_DTL_PARA_NO"].ToString();
                            submittalItem.DrawingSheetNumber = reader["PC_TRANS_DTL_SHT_NO"].ToString();
                            submittalItem.ContrCode = reader["PC_TRANS_DTL_USE_CODE"].ToString();
                            submittalItem.Var = reader["PC_TRANS_DTL_VAR"].ToString();
                            submittalItem.ForCEUseCode = reader["PC_TRANS_FORCE_USE_CODE"].ToString();
                            submittalItem.GovernmentActionCode = reader["PC_TRANS_GOVT_ACTION_CODE"].ToString();

                            submittalItems.Add(submittalItem);
                        }
                    }
                    restoreStatus = RestoreStatus.Success;
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
                PMMigrationLogger.Log("Out GetSubmittalItemsDataOldProcon : GETTING SUBMITTAL ITEMS   ENDED ...", Color.Black, FontStyle.Bold);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (mySqlCmd.Connection != null || mySqlCmd.Connection.State == ConnectionState.Open)
                {
                    mySqlCmd.Connection.Close();
                    //reader.Close();
                }
            }
            return submittalItems;
        }

        private List<SubmittalDistributionList> GetSubmittalDistributionListDataFromOldProcon(List<Submittals> submittals, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetSubmittalDistributionListDataFromOldProcon : GETTING SUBMITTAL DISTRIBUTION LIST   STARTED ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader reader = null;
            List<SubmittalDistributionList> submittalDistributionLists = new List<SubmittalDistributionList>();

            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;

                foreach (var submittal in submittals)
                {
                    string myQuery = string.Format("Select subdist.PC_SUB_DIST_ID," +
                                                "subdist.PC_SUB_DIST_SUB_FK," +
                                                "subdist.PC_SUB_DIST_PRJ_CONT_NAME," +
                                                "subdist.PC_SUB_DIST_PRJ_COMP_NAME," +
                                                "subdist.PC_SUB_DIST_PRJ_CONT_EMAIL," +
                                                "subdist.PC_SUB_DIST_TO," +
                                                "subdist.PC_SUB_DIST_BCC," +
                                                "subdist.PC_SUB_DIST_CC," +
                                                "subdist.PC_SUB_DIST_TYPE," +
                                                "pc.PC_PRJ_CONT_ID" +
                                                " FROM pc_sub_distribution_list subdist JOIN pc_submitals sub ON(subdist.PC_SUB_DIST_SUB_FK = sub.PC_SUB_ID)" +
                                                " JOIN pc_project_contacts pc ON (sub.PC_SUB_PRJ_FK = pc.PC_PRJ_CONT_PRJ_FK AND pc.PC_PRJ_CONT_PROJECT_MODE = 2)" +
                                                " WHERE PC_SUB_DIST_SUB_FK = '{0}'" +
                                                " AND subdist.PC_SUB_DIST_PRJ_COMP_NAME = pc.PC_PRJ_CONT_COMP_NAME" +
                                                " AND subdist.PC_SUB_DIST_PRJ_CONT_NAME = pc.PC_PRJ_CONT_NAME" +
                                                " AND subdist.PC_SUB_DIST_PRJ_CONT_EMAIL = pc.PC_PRJ_CONT_EMAIL", submittal.OldID);
                    mySqlCmd.CommandText = myQuery;
                    reader = mySqlCmd.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SubmittalDistributionList submittalDistributionList = new SubmittalDistributionList();
                            submittalDistributionList.ID = Guid.NewGuid();

                            submittalDistributionList.OldID = reader["PC_SUB_DIST_ID"].ToString();

                            submittalDistributionList.SubmittalID = submittal.ID;

                            int m_OldSubmittalID;
                            int.TryParse(reader["PC_SUB_DIST_SUB_FK"].ToString(), out m_OldSubmittalID);
                            submittalDistributionList.OldSubmittalID = m_OldSubmittalID;

                            submittalDistributionList.ProjectContactsName = reader["PC_SUB_DIST_PRJ_CONT_NAME"].ToString();
                            submittalDistributionList.ProjectCompanyName = reader["PC_SUB_DIST_PRJ_COMP_NAME"].ToString();
                            submittalDistributionList.ProjectContactsEMail = reader["PC_SUB_DIST_PRJ_CONT_EMAIL"].ToString();

                            submittalDistributionList.DistTo = (reader["PC_SUB_DIST_TO"].ToString() == "Y") ? 1 : 0;

                            submittalDistributionList.BCC = (reader["PC_SUB_DIST_BCC"].ToString() == "Y") ? 1 : 0;

                            submittalDistributionList.CC = (reader["PC_SUB_DIST_CC"].ToString() == "Y") ? 1 : 0;

                            int m_DistType;
                            int.TryParse(reader["PC_SUB_DIST_TYPE"].ToString(), out m_DistType);
                            submittalDistributionList.DistType = m_DistType;

                            Guid m_ProjectContactID;
                            Guid.TryParse(reader["PC_PRJ_CONT_ID"].ToString(), out m_ProjectContactID);
                            submittalDistributionList.ProjectContactsID = m_ProjectContactID;

                            submittalDistributionList.AC1 = "Migrated Data";

                            submittalDistributionLists.Add(submittalDistributionList);

                        }
                    }
                    restoreStatus = RestoreStatus.Success;
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (mySqlCmd.Connection != null || mySqlCmd.Connection.State == ConnectionState.Open)
                {
                    mySqlCmd.Connection.Close();
                    //reader.Close();
                }
            }
            return submittalDistributionLists;
        }
    }
}
