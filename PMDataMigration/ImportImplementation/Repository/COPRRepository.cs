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
    public class COPRRepository
    {
        RestoreStatus restoreStatus = RestoreStatus.None;

        public int RestoreCOPRData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreCOPRData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("Data retrival started ---------------------", Color.Black, FontStyle.Bold);
                List<COPR> coprlist = new List<COPR>();
                List<COPRDistributionList> coprDistributionList = new List<COPRDistributionList>();
                coprlist = GetCOPRDataFromOldProcon(projectID, mysqlCon);
                PMMigrationLogger.Log("Number of COPR retrieve : " + coprlist.Count);
                if (restoreStatus == RestoreStatus.Success)
                {
                    coprDistributionList = GetCOPRDistributionListFromOldProcon(coprlist, mysqlCon);
                    PMMigrationLogger.Log("Number of COPR Distribution list retrieve : " + coprDistributionList.Count);
                    PMMigrationLogger.Log("Data retrival completed ---------------------", Color.Black, FontStyle.Bold);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Creating Old ProjectArea , project Control-- New Project Area, project contro  Relation ....It might tale several minutes , please be patient", Color.Black, FontStyle.Bold);
                    coprlist = ProjectAreaControlMapping(coprlist, sqlCon);
                    PMMigrationLogger.Log("Creating Old ProjectArea, project control -- New Project Area, project contro Relation Completed", Color.Black, FontStyle.Bold);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("COPR Data restore started ................", Color.Black, FontStyle.Bold);
                    //InsertCOPRDataTOIDBO(coprlist, COPRDistributionList, sqlCon);
                    InsertCOPRToIDBO(coprlist, sqlCon);
                    InsertCOPRDistributionListToIDBO(coprDistributionList, sqlCon);
                    PMMigrationLogger.Log("COPR Data restore completed ................", Color.Black, FontStyle.Bold);

                }

            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                PMMigrationLogger.Log(ex.ToString(), Color.Red, FontStyle.Regular);
                throw;
            }
            PMMigrationLogger.Log("Out RestoreCOPRData");
            return result;
        }

        public List<COPRDistributionList> GetCOPRDistributionListFromOldProcon(List<COPR> coprlist, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetCOPORDistributionListFromOldProcon : Getting COPR distribution lists started ...", Color.Black, FontStyle.Bold);
            List<COPRDistributionList> coprDisLists = new List<COPRDistributionList>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var copr in coprlist)
                {
                    string myQuery = string.Format("select " +
                        "coprDist.PC_COPR_DIST_ID," +
                        "prjCont.PC_PRJ_CONT_ID," +
                        "coprDist.PC_COPR_DIST_COPR_FK," +
                        "copr.PC_COPR_PRJ_FK," +
                        "coprDist.PC_COPR_DIST_PRJ_COMP_NAME," +
                        "coprDist.PC_COPR_DIST_PRJ_CONT_EMAIL," +
                        "coprDist.PC_COPR_DIST_PRJ_CONT_NAME," +
                        "coprDist.PC_COPR_DIST_BCC," +
                        "coprDist.PC_COPR_DIST_CC," +
                        "coprDist.PC_COPR_DIST_TO " +
                        "FROM pc_copr_distribution_list coprDist " +
                        "JOIN pc_copr copr ON coprDist.PC_COPR_DIST_COPR_FK = copr.PC_COPR_ID " +
                        "JOIN pc_project_contacts prjCont ON copr.PC_COPR_PRJ_FK = prjCont.PC_PRJ_CONT_PRJ_FK " +
                        "WHERE copr.PC_COPR_ID = '{0}' " +
                        "AND coprDist.PC_COPR_DIST_PRJ_CONT_NAME = prjCont.PC_PRJ_CONT_NAME " +
                        "AND coprDist.PC_COPR_DIST_PRJ_CONT_EMAIL = prjCont.PC_PRJ_CONT_EMAIL " +
                        "AND coprDist.PC_COPR_DIST_PRJ_COMP_NAME = prjCont.PC_PRJ_CONT_COMP_NAME ", copr.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();

                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            COPRDistributionList coprdistList = new COPRDistributionList();
                            coprdistList.ID = Guid.NewGuid();

                            coprdistList.CoprID = copr.ID;
                            //coprdistList.ProjectContactID = Guid.NewGuid();

                            coprdistList.OldID = myReader["PC_COPR_DIST_ID"].ToString();

                            coprdistList.OldprojectContactID = myReader["PC_PRJ_CONT_ID"].ToString();

                            Guid m_ProjectContactID;
                            Guid.TryParse(myReader["PC_PRJ_CONT_ID"].ToString(), out m_ProjectContactID);
                            coprdistList.ProjectContactID = m_ProjectContactID;

                            int oldRFIId;
                            int.TryParse(myReader["PC_COPR_DIST_COPR_FK"].ToString(), out oldRFIId);
                            coprdistList.OldCoprID = oldRFIId;

                            //Guid oldProjectId;
                            //Guid.TryParse(myReader["PC_RFI_PRJ_FK"].ToString(), out oldProjectId);
                            coprdistList.OldProjectId = myReader["PC_COPR_PRJ_FK"].ToString();

                            coprdistList.OldContatCompany = myReader["PC_COPR_DIST_PRJ_COMP_NAME"].ToString();

                            coprdistList.OldContactEmail = myReader["PC_COPR_DIST_PRJ_CONT_EMAIL"].ToString();

                            coprdistList.OldContactName = myReader["PC_COPR_DIST_PRJ_CONT_NAME"].ToString();

                            string to = myReader["PC_COPR_DIST_TO"].ToString();
                            string cc = myReader["PC_COPR_DIST_CC"].ToString();
                            string bcc = myReader["PC_COPR_DIST_BCC"].ToString();

                            if (to == "Y")
                            {
                                coprdistList.To = 1;
                            }
                            if (cc == "Y")
                            {
                                coprdistList.CC = 1;
                            }
                            if (bcc == "Y")
                            {
                                coprdistList.BCC = 1;
                            }

                            coprdistList.AC1 = "Migrated Data";

                            coprDisLists.Add(coprdistList);
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
            return coprDisLists;
        }

        public List<COPR> GetCOPRDataFromOldProcon(Guid projectId, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetCOPRDataFromOldProcon : Getting COPRs started ...", Color.Black, FontStyle.Bold);
            List<COPR> coprlist = new List<COPR>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                string myGetCOPRQuery = String.Format("select * from pc_copr where PC_COPR_PRJ_FK = '{0}'", projectId);
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myGetCOPRQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        COPR copr = new COPR();
                        copr.ID = Guid.NewGuid();

                        int oldId;
                        int.TryParse(myReader["PC_COPR_ID"].ToString(), out oldId);
                        copr.OldID = oldId;

                        copr.ProjectID = projectId;
                        if (myReader["PC_COPR_EST_FK"] == DBNull.Value)
                            copr.EstimateID = Guid.Empty;
                        else
                        {
                            Guid m_EstimateID;
                            Guid.TryParse(myReader["PC_COPR_EST_FK"].ToString(), out m_EstimateID);
                            copr.EstimateID = m_EstimateID;
                        }
                        copr.ProjectAreaID = Guid.Empty;
                        copr.ProjectControlID = Guid.Empty;

                        copr.ProjectID = projectId;
                        copr.OldProjectID = projectId.ToString();

                        int oldControlId;
                        int.TryParse(myReader["PC_COPR_CONTROL_FK"].ToString(), out oldControlId);
                        copr.OldProjectControlID = oldControlId;

                        int oldProjectAreaID;
                        int.TryParse(myReader["PC_COPR_AREA_FK"].ToString(), out oldProjectAreaID);
                        copr.OldProjectAreaID = oldProjectAreaID;

                        if(myReader["PC_COPR_ACCT_MRK"].ToString() == "N")
                            copr.AcctMark = 0;
                        else
                            copr.AcctMark = 1;

                        decimal m_Approved;
                        decimal.TryParse(myReader["PC_COPR_APPROVED"].ToString(), out m_Approved);
                        copr.Approved = m_Approved;

                        int m_BaseMark;
                        int.TryParse(myReader["PC_COPR_BASEMARK"].ToString(), out m_BaseMark);
                        copr.BaseMark = m_BaseMark;

                        if (myReader["PC_COPR_COPR_LETTER"] == DBNull.Value)
                            copr.COPRLetter = null;
                        else
                            copr.COPRLetter = myReader["PC_COPR_COPR_LETTER"].ToString();

                        if (myReader["PC_COPR_DATE"] == DBNull.Value)
                            copr.Date = null;
                        else
                        { 
                            DateTime m_Date;
                            DateTime.TryParse(myReader["PC_COPR_DATE"].ToString(), out m_Date);
                            copr.Date = m_Date;
                        }

                        if (myReader["PC_COPR_DAYS_ADDED"] == DBNull.Value)
                            copr.DaysAdded = null;
                        else
                        {
                            int m_DaysAdded;
                            int.TryParse(myReader["PC_COPR_DAYS_ADDED"].ToString(), out m_DaysAdded);
                            copr.DaysAdded = m_DaysAdded;
                        }

                        if (myReader["PC_COPR_DAYS_VALID"] == DBNull.Value)
                            copr.DaysValid = null;
                        else
                        {
                            int m_DaysValid;
                            int.TryParse(myReader["PC_COPR_DAYS_VALID"].ToString(), out m_DaysValid);
                            copr.DaysValid = m_DaysValid;
                        }

                        if (myReader["PC_COPR_DESCRIPTION"] == DBNull.Value)
                            copr.COPRDescription = null;
                        else
                            copr.COPRDescription = myReader["PC_COPR_DESCRIPTION"].ToString();

                        int m_DupType;
                        int.TryParse(myReader["PC_COPR_DUP_TYPE"].ToString(), out m_DupType);
                        copr.DupType = m_DupType;

                        if (myReader["PC_COPR_FLDR_NAME"] == DBNull.Value)
                            copr.FolderName = null;
                        else
                            copr.FolderName = myReader["PC_COPR_FLDR_NAME"].ToString();

                        if (myReader["PC_COPR_IS_CTRL_FRM_BUDGET"].ToString() == "N")
                            copr.IsControlFromBudget = 0;
                        else
                            copr.IsControlFromBudget = 1;

                        if (myReader["PC_COPR_IS_LOCKED"].ToString() == "N")
                            copr.IsLocked = 0;
                        else
                            copr.IsLocked = 1;

                        int slNo;
                        int.TryParse(myReader["PC_COPR_NO"].ToString(), out slNo);
                        copr.SerialNumber = slNo;

                        decimal m_Pending;
                        decimal.TryParse(myReader["PC_COPR_PENDING"].ToString(), out m_Pending);
                        copr.Pending = m_Pending;

                        decimal m_Rejected;
                        decimal.TryParse(myReader["PC_COPR_REJECTED"].ToString(), out m_Rejected);
                        copr.Rejected = m_Rejected;

                        if (myReader["PC_COPR_STATUS"] == DBNull.Value)
                            copr.Status = null;
                        else
                            copr.Status = myReader["PC_COPR_STATUS"].ToString();

                        copr.AC1 = "Migrated Data";

                        coprlist.Add(copr);

                    }

                }
                if (!myReader.IsClosed)
                {
                    myReader.Close();
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetCOPRDataFromOldProcon : Getting COPRs completed ...", Color.Black, FontStyle.Bold);
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
            return coprlist;
        }

        public void InsertCOPRToIDBO(List<COPR> coprlist, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertCOPRToIDBO : Restoring COPR started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var copr in coprlist)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryCOPR = string.Format("INSERT INTO [PMCOPR] ( [ID],[ProjectID],[ProjectControlID],[ProjectAreaID],[EstimateID]," +
                                                "[AcctMark],[Approved],[BaseMark],[CoprLetter],[Date],[DaysAdded],[DaysValid],[CoprDescription],[DupType],[FolderName],[IsControlFromBudget]," +
                                                "[IsLocked],[SerialNumber],[Pending],[Rejected],[Status],[AC1],[OldID])" +
                                                " VALUES ('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},'{20}','{21}',{22})", copr.ID
                                                , copr.ProjectID, copr.ProjectControlID, copr.ProjectAreaID, copr.EstimateID, copr.AcctMark, copr.Approved,
                                                copr.BaseMark, copr.COPRLetter==null?"null":"'"+copr.COPRLetter.Replace("'", "''")+"'", copr.Date==null?"null":"'"+copr.Date+"'", copr.DaysAdded == null ? "null" : (object)copr.DaysAdded, copr.DaysValid == null ? "null" : (object)copr.DaysValid, 
                                                copr.COPRDescription==null?"null":"'"+copr.COPRDescription.Replace("'", "''")+"'", copr.DupType, copr.FolderName==null?"null":"'"+copr.FolderName.Replace("'", "''")+"'", copr.IsControlFromBudget, copr.IsLocked, copr.SerialNumber,
                                                copr.Pending, copr.Rejected,copr.Status,copr.AC1.Replace("'", "''"),copr.OldID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryCOPR;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN COPR INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertCOPRToIDBO : Restoring COPR completed  --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertCOPRDistributionListToIDBO(List<COPRDistributionList> coprDists, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertCOPRDistributionListToIDBO : Restoring DistributionList  started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var coprDist in coprDists)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryCOPRDist = string.Format("INSERT INTO [PMCOPRDistributionList] ([ID],[CoprID],[ProjectContactID],[To],[BCC],[CC]," +
                        "[AC1],[OldID],[OldCOPRID],[OldprojectContactID],[OldProjectId],[OldContactName],[OldContactEmail],[OldContatCompany]) VALUES " +
                        "('{0}','{1}','{2}',{3},{4},{5},'{6}','{7}',{8},'{9}','{10}','{11}','{12}','{13}')", coprDist.ID, coprDist.CoprID, coprDist.ProjectContactID,
                        coprDist.To, coprDist.BCC, coprDist.CC, coprDist.AC1, coprDist.OldID, coprDist.OldCoprID, coprDist.OldprojectContactID, coprDist.OldProjectId, coprDist.OldContactName.Replace("'", "''"), coprDist.OldContactEmail.Replace("'", "''"), coprDist.OldContatCompany.Replace("'", "''"));
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryCOPRDist;


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
                PMMigrationLogger.Log("Out InsertCOPRDistributionListToIDBO : Restoring DistributionList  completed  --------------------", Color.Black, FontStyle.Bold);
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
        public List<COPR> ProjectAreaControlMapping(List<COPR> unmappedCOPRs, SqlConnection sqlCon)
        {
            List<COPR> mappedCOPRs = new List<COPR>();
            try
            {
                sqlCon.Open();
                foreach (var copr in unmappedCOPRs)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        SqlDataReader myReader = null;
                        cmd.Connection = sqlCon;
                        COPR tempCOPR = new COPR();
                        // we can get both projectareaid and projectcontrolid from FinProjectControl
                        string areaQry = String.Format("select ID,ProjectAreaID from FinProjectControl where OldID={0};", copr.OldProjectControlID);

                        cmd.CommandText = areaQry;
                        myReader = cmd.ExecuteReader();
                        if (myReader != null && myReader.HasRows)
                        {
                            while (myReader.Read())
                            {
                                Guid controlID;
                                Guid.TryParse(myReader["ID"].ToString(), out controlID);
                                copr.ProjectControlID = controlID;

                                Guid areaId;
                                Guid.TryParse(myReader["ProjectAreaID"].ToString(), out areaId);
                                copr.ProjectAreaID = areaId;
                            }
                        }
                        mappedCOPRs.Add(copr);
                        PMMigrationLogger.Log("Old Area -> New Area : " + copr.OldProjectAreaID + " -> " + copr.ProjectAreaID + " ##### Old Control -> New Control : " + copr.OldProjectControlID + " -> " + copr.ProjectControlID);
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
            return mappedCOPRs;
        }
        #endregion
    }
}
