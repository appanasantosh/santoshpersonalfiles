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
using System.Data.SqlTypes;

namespace PMImportImplementation.Repository
{
    public class CloseOutRepository
    {
        RestoreStatus restoreStatus = RestoreStatus.None;

        public int RestoreCloseOutData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreCloseOutData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("Data retrival started ---------------------", Color.Black, FontStyle.Bold);
                List<CloseOut> closeOuts = new List<CloseOut>();
                List<CloseOutDistributionList> closeOutDistributionLists = new List<CloseOutDistributionList>();
                closeOuts = GetCloseOutDataFromOldProcon(projectID, mysqlCon);
                PMMigrationLogger.Log("Number of close outs retrieve : " + closeOuts.Count);
                if (restoreStatus == RestoreStatus.Success)
                {
                    closeOutDistributionLists = GetCloseOutDistributionListDataFromOldProcon(closeOuts, mysqlCon);
                    PMMigrationLogger.Log("Number of close out distribution list retrieve : " + closeOutDistributionLists.Count);
                    PMMigrationLogger.Log("Data retrival completed ---------------------", Color.Black, FontStyle.Bold);
                }

                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Creating Old ProjectArea -- New Project Area  Relation ....It might tale several minutes , please be patient", Color.Black, FontStyle.Bold);
                    closeOuts = ProjectAreaMapping(closeOuts, sqlCon);
                    PMMigrationLogger.Log("Creating Old ProjectArea -- New Project Area, Relation Completed", Color.Black, FontStyle.Bold);
                }

                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Close Out Data restore started ................", Color.Black, FontStyle.Bold);
                    InsertCloseOutDataToIDBO(closeOuts, closeOutDistributionLists, sqlCon);
                    PMMigrationLogger.Log("Close Out Data restore completed ................", Color.Black, FontStyle.Bold);
                }

            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :",Color.Black,FontStyle.Bold );
				PMMigrationLogger.Log(ex.ToString(),Color.Red,FontStyle.Regular);
                throw;
            }
            PMMigrationLogger.Log("Out RestoreCloseOutData");
            return result;
        }

        #region Get All Close out Related Data

        public List<CloseOut> GetCloseOutDataFromOldProcon(Guid projectID, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetCloseOutDataFromOldProcon : Getting Close Out started ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            List<CloseOut> closeOuts = new List<CloseOut>();
            try
            {
                string myQuery = string.Format("select * from pc_closeout where PC_CLSOUT_PRJ_FK = '{0}'", projectID);
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        CloseOut closeOut = new CloseOut();
                        closeOut.ID = Guid.NewGuid();

                        int oldID;
                        int.TryParse(myReader["PC_CLSOUT_ID"].ToString(), out oldID);
                        closeOut.OldID = oldID;

                        Guid oldProjectID;
                        Guid.TryParse(myReader["PC_CLSOUT_PRJ_FK"].ToString(), out oldProjectID);
                        closeOut.OldProjectID = oldProjectID;
                        closeOut.ProjectID = projectID;

                        //closeOut.OldProjectContactID = myReader["PC_CLSOUT_REPRESNTATIVE"].ToString();
                        Guid m_ProjectContactID;
                        Guid.TryParse(myReader["PC_CLSOUT_REPRESNTATIVE"].ToString(), out m_ProjectContactID);
                        closeOut.ProjectContactID = m_ProjectContactID;

                        int OldProjectAreaID;
                        int.TryParse(myReader["PC_CLSOUT_AREA_PKGS"].ToString(), out OldProjectAreaID);
                        closeOut.OldProjectAreaID = OldProjectAreaID;

                        closeOut.AreaID = Guid.Empty;

                        DateTime DateToAEO;
                        if (myReader["PC_CLSOUT_DATE_TO_AEO"] == DBNull.Value )
                        {
                            closeOut.DateToAEO = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_CLSOUT_DATE_TO_AEO"].ToString(), out DateToAEO);
                            closeOut.DateToAEO = DateToAEO;
                        }
                        
                        

                        DateTime dueDate;
                        if (myReader["PC_CLSOUT_DUE_DATE"] == DBNull.Value)
                        {
                            closeOut.DueDate = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_CLSOUT_DUE_DATE"].ToString(), out dueDate);
                            closeOut.DueDate = dueDate;
                        }
                        

                        closeOut.Description = myReader["PC_CLSOUT_DESC"].ToString();

                        closeOut.DocumentDescription = myReader["PC_CLSOUT_DOC_DESC"].ToString();

                        int number;
                        int.TryParse(myReader["PC_CLSOUT_NO"].ToString(), out number);
                        closeOut.Number = number;

                        if (myReader["PC_CLSOUT_SPEC_NO"] == DBNull.Value)
                        {
                            closeOut.SpecNo = null;
                        }
                        else
                        {
                            int specNo;
                            int.TryParse(myReader["PC_CLSOUT_SPEC_NO"].ToString(), out specNo);
                            closeOut.SpecNo = specNo;
                        }

                        closeOut.SpecPara = myReader["PC_CLSOUT_SPEC_PARA"].ToString();

                        closeOut.SpecTitle = myReader["PC_CLSOUT_SPEC_TITLE"].ToString();

                        closeOut.Status = myReader["PC_CLSOUT_STATUS"].ToString();

                        closeOut.AC1 = "Migrated Data";

                        closeOuts.Add(closeOut);
                    }
                }
                restoreStatus = RestoreStatus.Success;
                if (!myReader.IsClosed)
                {
                    myReader.Close();
                }
                PMMigrationLogger.Log("Out GetCloseOutDataFromOldProcon : Getting Close Out completed ...", Color.Black, FontStyle.Bold);
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
            return closeOuts;
        }

        public List<CloseOutDistributionList> GetCloseOutDistributionListDataFromOldProcon(List<CloseOut> closeOuts, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetCloseOutDistributionListDataFromOldProcon : Getting Close Out distribution lists started ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            List<CloseOutDistributionList> closeOutDistributionLists = new List<CloseOutDistributionList>();
            try
            {
                
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var closeOut in closeOuts)
                {
                    string myQuery = string.Format("SELECT " +
                                            "cld.PC_CLS_OUT_DIST_ID," +
                                            "pc.PC_PRJ_CONT_ID," +
                                            "cld.PC_CLS_OUT_DIST_CLS_OUT_FK," +
                                            "cl.PC_CLSOUT_PRJ_FK," +
                                            "cld.PC_CLS_OUT_DIST_PRJ_COMP_NAME," +
                                            "cld.PC_CLS_OUT_DIST_PRJ_CONT_EMAIL," +
                                            "cld.PC_CLS_OUT_DIST_PRJ_CONT_NAME," +
                                            "cld.PC_CLS_OUT_DIST_TO," +
                                            "cld.PC_CLS_OUT_DIST_BCC," +
                                            "cld.PC_CLS_OUT_DIST_CC," +
                                            "cld.PC_CLS_OUT_DIST_TYPE " +
                                            "FROM pc_closeout_distribution_list cld " +
                                            "JOIN pc_closeout cl ON cld.PC_CLS_OUT_DIST_CLS_OUT_FK = cl.PC_CLSOUT_ID " +
                                            "JOIN pc_project_contacts pc ON cl.PC_CLSOUT_PRJ_FK = pc.PC_PRJ_CONT_PRJ_FK " +
                                            "WHERE cl.PC_CLSOUT_ID = {0} and cld.PC_CLS_OUT_DIST_PRJ_CONT_NAME = pc.PC_PRJ_CONT_NAME and " +
                                            "cld.PC_CLS_OUT_DIST_PRJ_CONT_EMAIL = pc.PC_PRJ_CONT_EMAIL and cld.PC_CLS_OUT_DIST_PRJ_COMP_NAME = pc.PC_PRJ_CONT_COMP_NAME", closeOut.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();
                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            CloseOutDistributionList distributionList = new CloseOutDistributionList();
                            distributionList.ID = Guid.NewGuid();

                            int oldID;
                            int.TryParse(myReader["PC_CLS_OUT_DIST_ID"].ToString(), out oldID);
                            distributionList.OldID = oldID;

                            int oldCloseOutId;
                            int.TryParse(myReader["PC_CLS_OUT_DIST_CLS_OUT_FK"].ToString(), out oldCloseOutId);
                            distributionList.OldCloseOutID = oldCloseOutId;

                            //distributionList.OldProjectContactID = myReader["PC_PRJ_CONT_ID"].ToString();
                            Guid m_ProjectContactID;
                            Guid.TryParse(myReader["PC_PRJ_CONT_ID"].ToString(), out m_ProjectContactID);
                            distributionList.ProjectContactID = m_ProjectContactID;

                            distributionList.OldProjectContactName = myReader["PC_CLS_OUT_DIST_PRJ_CONT_NAME"].ToString();

                            distributionList.OldProjectContactEMail = myReader["PC_CLS_OUT_DIST_PRJ_CONT_EMAIL"].ToString();

                            distributionList.OldProjectContactContactCompany = myReader["PC_CLS_OUT_DIST_PRJ_COMP_NAME"].ToString();

                            string to = myReader["PC_CLS_OUT_DIST_TO"].ToString();
                            string cc = myReader["PC_CLS_OUT_DIST_CC"].ToString();
                            string bcc = myReader["PC_CLS_OUT_DIST_BCC"].ToString();

                            if (to == "Y")
                            {
                                distributionList.To = 1;
                            }
                            if (cc == "Y")
                            {
                                distributionList.CC = 1;
                            }
                            if (bcc == "Y")
                            {
                                distributionList.BCC = 1;
                            }

                            int type;
                            int.TryParse(myReader["PC_CLS_OUT_DIST_TYPE"].ToString(), out type);
                            distributionList.Type = type;

                            // making relation
                            distributionList.CloseOutID = closeOut.ID;

                            distributionList.AC1 = "Migrated Data";
                            closeOutDistributionLists.Add(distributionList);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetCloseOutDistributionListDataFromOldProcon : Getting Close Out distribution lists completed ...", Color.Black, FontStyle.Bold);
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
            return closeOutDistributionLists;
        }
        #endregion

        #region Insert All Close Out Related Data

        public void InsertCloseOutDataToIDBO(List<CloseOut> closeOuts, List<CloseOutDistributionList> closeOutDistributionLists ,SqlConnection sqlCon)
        {
            try
            {
                InsertCloseOutToIDBO(closeOuts,sqlCon);
                InsertCloseOutDistributionToIDBO(closeOutDistributionLists,sqlCon);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InsertCloseOutToIDBO(List<CloseOut> closeOuts, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertCloseOutToIDBO : Restoring Close Out started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var closeOut in closeOuts)
                {
                    try
                    {
                        
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        
                        string qryCloseout = string.Format("INSERT INTO PMCloseOut(ID,ProjectID,ProjectContactID,ProjectAreaID,DateToAEO,DueDate,Number,SpecNo,DocumentDescription,SpecPara,SpecTitle,CloseOutDescription,AC1,OldID,OldProjectID,OldProjectContactID,OldProjectAreaID) " +
                                                            "VALUES ('{0}','{1}','{2}','{3}',{4},{5},{6},{7},'{8}','{9}','{10}','{11}','{12}',{13},'{14}','{15}',{16})", closeOut.ID,
                                                            closeOut.ProjectID, closeOut.ProjectContactID, closeOut.AreaID,(closeOut.DateToAEO == null) ?(object)"null" : "'"+closeOut.DateToAEO+"'", (closeOut.DueDate == null) ?(object)"null":"'"+closeOut.DueDate+"'", closeOut.Number,
                                                            (closeOut.SpecNo == null) ? (object)"null":closeOut.SpecNo  , closeOut.DocumentDescription.Replace("'", "''"), closeOut.SpecPara.Replace("'", "''"), closeOut.SpecTitle.Replace("'", "''"),
                                                            closeOut.Description.Replace("'", "''"), closeOut.AC1.Replace("'", "''"), closeOut.OldID,closeOut.OldProjectID,closeOut.OldProjectContactID,closeOut.OldProjectAreaID);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryCloseout;


                        PMMigrationLogger.Log("["+ counter++ +"] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                       // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN CLOSE OUT INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }

                PMMigrationLogger.Log("Out InsertCloseOutToIDBO : Restoring Close Out completed  --------------------", Color.Black, FontStyle.Bold);
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
                                                                    //closeOut.DateToAEO == null ? DBNull.Value:closeOut.DateToAEO

        public void InsertCloseOutDistributionToIDBO(List<CloseOutDistributionList> closeOutDistributionLists, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertCloseOutDistributionToIDBO : Restoring Close out distribution list started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open(); 
                int counter = 1;
                foreach (var closeOutDistributionList in closeOutDistributionLists)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryCloseoutDistributionList = string.Format("INSERT INTO PMCloseOutDistributionList(ID,CloseOutID,ProjectContactID,BCC,CC,[To],Type,AC1,OldID,OldCloseOutID,OldProjectContactID,OldProjectContactName,OldProjectContactEMail,OldProjectContactCompany) " +
                                                            "VALUES ('{0}','{1}','{2}',{3},{4},{5},{6},'{7}',{8},{9},'{10}','{11}','{12}','{13}')", closeOutDistributionList.ID,
                                                            closeOutDistributionList.CloseOutID, closeOutDistributionList.ProjectContactID, closeOutDistributionList.BCC, closeOutDistributionList.CC, closeOutDistributionList.To, closeOutDistributionList.Type,
                                                            closeOutDistributionList.AC1, closeOutDistributionList.OldID, closeOutDistributionList.OldCloseOutID, closeOutDistributionList.OldProjectContactID,
                                                            closeOutDistributionList.OldProjectContactName, closeOutDistributionList.OldProjectContactEMail, closeOutDistributionList.OldProjectContactContactCompany);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryCloseoutDistributionList;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN CLOSE OUT DISTRIBUTION LIST INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }

                PMMigrationLogger.Log("Out InsertCloseOutDistributionToIDBO : Restoring Close out distribution list completed  --------------------", Color.Black, FontStyle.Bold);
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
        #endregion

        #region Relation
        public List<CloseOut> ProjectAreaMapping(List<CloseOut> unmappedCloseOuts, SqlConnection sqlCon)
        {
            List<CloseOut> mappedCloseOuts = new List<CloseOut>();
            
            try
            {
                sqlCon.Open();
                //int counter = 1;
                foreach (var closeout in unmappedCloseOuts)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        SqlDataReader myReader = null;
                        cmd.Connection = sqlCon;
                        Instructions tempinstruction = new Instructions();
                        
                        string areaQry = String.Format("select ID from FinProjectArea where OldID={0};", closeout.OldProjectAreaID);

                        cmd.CommandText = areaQry;
                        myReader = cmd.ExecuteReader();
                        if (myReader != null && myReader.HasRows)
                        {
                            while (myReader.Read())
                            {
                                if (myReader["ID"] == DBNull.Value)
                                    closeout.AreaID = Guid.Empty;
                                else
                                {
                                    Guid m_AreaID;
                                    Guid.TryParse(myReader["ID"].ToString(), out m_AreaID);
                                    closeout.AreaID = m_AreaID;
                                }

                                
                            }
                        }
                        mappedCloseOuts.Add(closeout);
                        PMMigrationLogger.Log("Old Area -> New Area : " + closeout.OldProjectAreaID + " -> " + closeout.AreaID );

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
            return mappedCloseOuts;
        }
        #endregion
    }
}
