using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMImportImplementation;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using PMImportImplementation.Entities;
using System.Drawing;
using System.Data;

namespace PMImportImplementation.Repository
{
    public class RFIRepository
    {
        RestoreStatus restoreStatus = RestoreStatus.None;

        public int RestoreRFIData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreRFIData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("Data retrival started ---------------------",Color.Black, FontStyle.Bold );
                List<RFI> RFIs = new List<RFI>();
                List<RFIDistributionList> RFIDistributionList = new List<RFIDistributionList>();
                RFIs = GetRFIDataFromOldProcon(projectID, mysqlCon);
                PMMigrationLogger.Log("Number of RFI retrieve : " + RFIs.Count);
                if (restoreStatus == RestoreStatus.Success)
                {
                    RFIDistributionList = GetRFIDistributionListFromOldProcon(RFIs, mysqlCon);
                    PMMigrationLogger.Log("Number of RFI Distribution list retrieve : " + RFIDistributionList.Count);
                    PMMigrationLogger.Log("Data retrival completed ---------------------", Color.Black, FontStyle.Bold);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                     PMMigrationLogger.Log("Creating Old ProjectArea , project Control-- New Project Area, project contro  Relation ....It might tale several minutes , please be patient",Color.Black, FontStyle.Bold);
                     RFIs = ProjectAreaControlMapping(RFIs, sqlCon);
                     PMMigrationLogger.Log("Creating Old ProjectArea, project control -- New Project Area, project contro Relation Completed", Color.Black, FontStyle.Bold);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("RFI Data restore started ................", Color.Black, FontStyle.Bold);
                    InsertRFIDataTOIDBO(RFIs, RFIDistributionList, sqlCon);
                    PMMigrationLogger.Log("RFI Data restore completed ................", Color.Black, FontStyle.Bold);

                }
          
            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :",Color.Black,FontStyle.Bold );
				PMMigrationLogger.Log(ex.ToString(),Color.Red,FontStyle.Regular);
                throw;
            }
            PMMigrationLogger.Log("Out RestoreRFIData");
            return result;
        }

        public List<RFI> GetRFIDataFromOldProcon(Guid projectId, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetRFIDataFromOldProcon : Getting RFIs started ...", Color.Black, FontStyle.Bold);
            List<RFI> rfis = new List<RFI>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                string myGetRFIQuery = String.Format( "select * from pc_rfi where PC_RFI_PRJ_FK = '{0}'",projectId);
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myGetRFIQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        RFI rfi = new RFI();
                        rfi.ID = Guid.NewGuid();

                        int oldId;
                        int.TryParse(myReader["PC_RFI_ID"].ToString(), out oldId);
                        rfi.OldID = oldId;


                        rfi.ProjectID = projectId;
                        //rfi.OriginatorID = Guid.Empty;
                        //rfi.ReceipentID = Guid.Empty;
                        if (myReader["PC_RFI_ORIGINATOR_FK"] == DBNull.Value)
                            rfi.OriginatorID = Guid.Empty;
                        else
                        {
                            Guid m_OriginatorID;
                            Guid.TryParse(myReader["PC_RFI_ORIGINATOR_FK"].ToString(), out m_OriginatorID);
                            rfi.OriginatorID = m_OriginatorID;
                        }
                        if (myReader["PC_RFI_SENDER_FK"] == DBNull.Value)
                            rfi.SenderID = Guid.Empty;
                        else
                        {
                            Guid m_SenderID;
                            Guid.TryParse(myReader["PC_RFI_SENDER_FK"].ToString(), out m_SenderID);
                            rfi.SenderID = m_SenderID;
                        }

                        if (myReader["PC_RFI_RECIPIENT_FK"] == DBNull.Value)
                            rfi.ReceipentID = Guid.Empty;
                        else
                        {
                            Guid m_RecipientID;
                            Guid.TryParse(myReader["PC_RFI_RECIPIENT_FK"].ToString(), out m_RecipientID);
                            rfi.ReceipentID = m_RecipientID;
                        }
                        rfi.DepartmentID = Guid.Empty;
                        rfi.ProjectAreaID = Guid.Empty;
                        rfi.ProjectControlID = Guid.Empty;

                        if (myReader["PC_RFI_DEPT_ID"] == DBNull.Value)
                            rfi.OldDepartmentID = null;
                        else
                        {
                            int m_OldDepartmentID;
                            int.TryParse(myReader["PC_RFI_DEPT_ID"].ToString(), out m_OldDepartmentID);
                            rfi.OldDepartmentID = m_OldDepartmentID;
                        }

                        //Guid oldProjectID;
                        //Guid.TryParse(myReader["PC_RFI_PRJ_FK"].ToString(), out oldProjectID);
                        rfi.OldProjectID = myReader["PC_RFI_PRJ_FK"].ToString();

                        int slNo;
                        int.TryParse(myReader["PC_RFI_NO"].ToString(), out slNo);
                        rfi.SerialNumber = slNo;

                        //rfi.OldOriginatorID = myReader["PC_RFI_ORIGINATOR_FK"].ToString();
                        //rfi.OldRecipientID = myReader["PC_RFI_RECIPIENT_FK"].ToString();
                        //rfi.OldSenderID = myReader["PC_RFI_SENDER_FK"].ToString();

                        int oldControlId;
                        int.TryParse(myReader["PC_RFI_CTRL"].ToString(), out oldControlId);
                        rfi.OldProjectControlID = oldControlId;

                        int oldProjectAreaID;
                        int.TryParse(myReader["PC_RFI_AREA_PKGS"].ToString(), out oldProjectAreaID);
                        rfi.OldProjectAreaID = oldProjectAreaID;


                        DateTime originationDate;
                        if (myReader["PC_RFI_ORIG_DATE"] == DBNull.Value)
                        {
                            rfi.OriginationDate = null;
                        }
                        else
	                    {
                            DateTime.TryParse(myReader["PC_RFI_ORIG_DATE"].ToString(), out originationDate);
                            rfi.OriginationDate = originationDate;
	                    }

                        int responseDays;
                        int.TryParse(myReader["PC_RFI_RESPONSE_DAYS"].ToString(), out responseDays);
                        rfi.ResponseDays = responseDays;

                        DateTime aeRespondedDate;
                        if (myReader["PC_RFI_AE_RESPONDED"] == DBNull.Value)
                        {
                            rfi.AERespondedDate = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_RFI_AE_RESPONDED"].ToString(), out aeRespondedDate);
                            rfi.AERespondedDate = aeRespondedDate;
                        }
                        rfi.RequestNotes = myReader["PC_RFI_REQ_NOTES"].ToString();
                        rfi.Request = myReader["PC_RFI_REQ"].ToString();
                        rfi.Response = myReader["PC_RFI_RESPONSE"].ToString();

                        if (myReader["PC_RFI_SPEC"] == DBNull.Value)
                            rfi.Spec = null;
                        else
                        {
                            double spec;
                            double.TryParse(myReader["PC_RFI_SPEC"].ToString(), out spec);
                            rfi.Spec = spec;
                        }

                        rfi.Section = myReader["PC_RFI_SECTION"].ToString();

                        rfi.RFIDescription = myReader["PC_RFI_DESCRIPTION"].ToString();

                        if (myReader["PC_RFI_TYPE"] == DBNull.Value)
                        {
                            rfi.Type = "Other";
                        }
                        else
                        {
                            rfi.Type = myReader["PC_RFI_TYPE"].ToString();
                        }

                        rfi.Suffix = myReader["PC_RFI_LETTER"].ToString();

                        int dupType;
                        int.TryParse(myReader["PC_RFI_DUP_TYPE"].ToString(), out dupType);
                        rfi.DuplicateType = dupType;

                        rfi.AC1 = "Migrated Data";

                        rfis.Add(rfi);

                    }
                    
                }
                if (!myReader.IsClosed)
                {
                    myReader.Close();
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetRFIDataFromOldProcon : Getting RFIs completed ...", Color.Black, FontStyle.Bold);
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
                    //myReader.Close();
                }
            }
            return rfis;
        }

        public List<RFIDistributionList> GetRFIDistributionListFromOldProcon(List<RFI> rfis, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetRFIDistributionListFromOldProcon : Getting RFI distribution lists started ...", Color.Black, FontStyle.Bold);
            List<RFIDistributionList> rfiDisLists = new List<RFIDistributionList>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var rfi in rfis)
                {
                    string myQuery = string.Format("select " +
                        "rfiDist.PC_RFI_DIST_ID," +
                        "prjCont.PC_PRJ_CONT_ID," +
                        "rfiDist.PC_RFI_DIST_RFI_FK," +
                        "rfi.PC_RFI_PRJ_FK," +
                        "rfiDist.PC_RFI_DIST_PRJ_COMP_NAME," +
                        "rfiDist.PC_RFI_DIST_PRJ_CONT_EMAIL," +
                        "rfiDist.PC_RFI_DIST_PRJ_CONT_NAME," +
                        "rfiDist.PC_RFI_DIST_BCC," +
                        "rfiDist.PC_RFI_DIST_CC," +
                        "rfiDist.PC_RFI_DIST_TO," +
                        "rfiDist.PC_RFI_DIST_TYPE " +
                        "FROM pc_rfi_distribution_list rfiDist " +
                        "JOIN pc_rfi rfi ON rfiDist.PC_RFI_DIST_RFI_FK = rfi.PC_RFI_ID " +
                        "JOIN pc_project_contacts prjCont ON rfi.PC_RFI_PRJ_FK = prjCont.PC_PRJ_CONT_PRJ_FK " +
                        "WHERE rfi.PC_RFI_ID = '{0}' " +
                        "AND rfiDist.PC_RFI_DIST_PRJ_CONT_NAME = prjCont.PC_PRJ_CONT_NAME " +
                        "AND rfiDist.PC_RFI_DIST_PRJ_CONT_EMAIL = prjCont.PC_PRJ_CONT_EMAIL " +
                        "AND rfiDist.PC_RFI_DIST_PRJ_COMP_NAME = prjCont.PC_PRJ_CONT_COMP_NAME ",rfi.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();

                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            RFIDistributionList rfidistList = new RFIDistributionList();
                            rfidistList.ID = Guid.NewGuid();

                            rfidistList.RfiID = rfi.ID;
                            //rfidistList.ProjectContactID = Guid.NewGuid();

                            rfidistList.OldID = myReader["PC_RFI_DIST_ID"].ToString();

                            rfidistList.OldprojectContactID = myReader["PC_PRJ_CONT_ID"].ToString();

                            Guid m_ProjectContactID;
                            Guid.TryParse(myReader["PC_PRJ_CONT_ID"].ToString(),out m_ProjectContactID);
                            rfidistList.ProjectContactID = m_ProjectContactID;

                            int oldRFIId;
                            int.TryParse(myReader["PC_RFI_DIST_RFI_FK"].ToString(), out oldRFIId);
                            rfidistList.OldRfiID = oldRFIId;

                            //Guid oldProjectId;
                            //Guid.TryParse(myReader["PC_RFI_PRJ_FK"].ToString(), out oldProjectId);
                            rfidistList.OldProjectId = myReader["PC_RFI_PRJ_FK"].ToString();

                            rfidistList.OldContatCompany = myReader["PC_RFI_DIST_PRJ_COMP_NAME"].ToString();

                            rfidistList.OldContactEmail = myReader["PC_RFI_DIST_PRJ_CONT_EMAIL"].ToString();

                            rfidistList.OldContactName = myReader["PC_RFI_DIST_PRJ_CONT_NAME"].ToString();

                            string to = myReader["PC_RFI_DIST_TO"].ToString();
                            string cc = myReader["PC_RFI_DIST_CC"].ToString();
                            string bcc = myReader["PC_RFI_DIST_BCC"].ToString();

                            if (to == "Y")
                            {
                                rfidistList.To = 1;
                            }
                            if (cc == "Y")
                            {
                                rfidistList.CC = 1;
                            }
                            if (bcc == "Y")
                            {
                                rfidistList.BCC = 1;
                            }

                            int type;
                            int.TryParse(myReader["PC_RFI_DIST_TYPE"].ToString(), out type);
                            rfidistList.Type = type;

                            rfidistList.AC1 = "Migrated Data";

                            rfiDisLists.Add(rfidistList);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }
                
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetRFIDistributionListFromOldProcon : Getting RFI distribution lists completed ...", Color.Black, FontStyle.Bold);
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
                    //myReader.Close();
                }
            }
            return rfiDisLists;
        }

        public void InsertRFIDataTOIDBO(List<RFI> rfis, List<RFIDistributionList> rfiDistLists,  SqlConnection sqlCon)
        {
            InsertRFIToIDBO(rfis, sqlCon);
            InsertRFIDistributionListToIDBO(rfiDistLists, sqlCon);
        }

        public void InsertRFIToIDBO(List<RFI> rfis, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertRFIToIDBO : Restoring RFI started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var rfi in rfis)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
						cmd.Connection = sqlCon;
                        string qryRfi = string.Format("INSERT INTO [PMRfi] ( [ID],[ProjectID],[OriginatorID],[RecipientID],[SenderID],[ProjectControlID],[ProjectAreaID],[DepartmentID]," +
                                                "[SerialNumber],[OriginationDate],[ResponseDays],[AERespondedDate],[RequestNotes],[Request],[Response],[Spec],[Section],[RFIDescription]," +
                                                "[Type],[Suffix],[DuplicateType],[AC1],[OldID],[OldProjectID],[OldOriginatorID],[OldRecipientID],[OldSenderID],[OldProjectAreaID],[OldProjectControlID])" +
                                                " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},'{9}',{10},{11},'{12}','{13}','{14}',{15},'{16}','{17}','{18}','{19}',{20},'{21}',{22},'{23}','{24}','{25}','{26}',{27},{28})", rfi.ID
                                                ,rfi.ProjectID,rfi.OriginatorID,rfi.ReceipentID,rfi.SenderID,rfi.ProjectControlID,rfi.ProjectAreaID,rfi.DepartmentID,rfi.SerialNumber,rfi.OriginationDate,
                                                rfi.ResponseDays,rfi.AERespondedDate==null?"null":"'"+rfi.AERespondedDate+"'", rfi.RequestNotes.Replace("'", "''"), rfi.Request.Replace("'", "''"), rfi.Response.Replace("'", "''"), rfi.Spec==null?"null":(object)rfi.Spec, rfi.Section.Replace("'", "''"), rfi.RFIDescription.Replace("'", "''"), rfi.Type.Replace("'", "''"), rfi.Suffix.Replace("'", "''"),
                                                rfi.DuplicateType, rfi.AC1.Replace("'", "''"), rfi.OldID, rfi.OldProjectID, rfi.OldOriginatorID, rfi.OldRecipientID, rfi.OldSenderID, rfi.OldProjectAreaID, rfi.OldProjectControlID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryRfi;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN RFI INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertRFIToIDBO : Restoring RFI completed  --------------------", Color.Black, FontStyle.Bold);
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
        }

        public void InsertRFIDistributionListToIDBO(List<RFIDistributionList> rfiDists, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertRFIDistributionListToIDBO : Restoring DistributionList  started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var rfiDist in rfiDists)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryRfiDist = string.Format("INSERT INTO [PMRfiDistributionList] ([ID],[RfiID],[ProjectContactID],[To],[BCC],[CC],[Type]," +
                        "[AC1],[OldID],[OldRfiID],[OldprojectContactID],[OldProjectId],[OldContactName],[OldContactEmail],[OldContatCompany]) VALUES "+
                        "('{0}','{1}','{2}',{3},{4},{5},{6},'{7}','{8}',{9},'{10}','{11}','{12}','{13}','{14}')",rfiDist.ID,rfiDist.RfiID,rfiDist.ProjectContactID,
                        rfiDist.To, rfiDist.BCC, rfiDist.CC, rfiDist.Type, rfiDist.AC1, rfiDist.OldID, rfiDist.OldRfiID, rfiDist.OldprojectContactID, rfiDist.OldProjectId, rfiDist.OldContactName.Replace("'", "''"), rfiDist.OldContactEmail.Replace("'", "''"), rfiDist.OldContatCompany.Replace("'", "''"));
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryRfiDist;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN RFI DISTIBUTION INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertRFIDistributionListToIDBO : Restoring DistributionList  completed  --------------------", Color.Black, FontStyle.Bold);
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
        }

        #region Relation
        public List<RFI> ProjectAreaControlMapping(List<RFI> unmappedRFis, SqlConnection sqlCon)
        {
            List<RFI> mappedRfis = new List<RFI>();
            //mappedRfis.AddRange(unmappedRFis);
            try
            {
                sqlCon.Open();
                //int counter = 1;
                foreach (var rfi in unmappedRFis)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        SqlDataReader myReader = null;
                        cmd.Connection = sqlCon;
                        RFI temprfi = new RFI();
                        // we can get both projectareaid and projectcontrolid from FinProjectControl
                        string areaQry = String.Format("select ID,ProjectAreaID from FinProjectControl where OldID={0};", rfi.OldProjectControlID);

                        cmd.CommandText = areaQry;
                        myReader = cmd.ExecuteReader();
                        if (myReader != null && myReader.HasRows)
                        {
                            while (myReader.Read())
                            {
                                Guid controlID;
                                Guid.TryParse(myReader["ID"].ToString(), out controlID);
                                rfi.ProjectControlID = controlID;

                                Guid areaId;
                                Guid.TryParse(myReader["ProjectAreaID"].ToString(), out areaId);
                                rfi.ProjectAreaID = areaId;
                            }
                        }
                        mappedRfis.Add(rfi);
                        PMMigrationLogger.Log("Old Area -> New Area : " + rfi.OldProjectAreaID + " -> " + rfi.ProjectAreaID + " ##### Old Control -> New Control : " + rfi.OldProjectControlID + " -> " + rfi.ProjectControlID);
                        //PMMigrationLogger.Log("Old Control -> New Control : " + rfi.OldProjectControlID + " -> " + rfi.ProjectControlID);
                        if (!myReader.IsClosed)
                        {
                            myReader.Close();
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
            return mappedRfis;
        }
        #endregion
    }

}
