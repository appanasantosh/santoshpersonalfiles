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
   public  class TransmittalRepository
    {
        RestoreStatus restoreStatus = RestoreStatus.None;

        public int RestoreTransmittalData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreTransmittalData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("Data retrival started ---------------------", Color.Black, FontStyle.Bold);
                List<Transmittal> Transmittals = new List<Transmittal>();
                List<TransmittalItem> transItemList = new List<TransmittalItem>();
                List<TransmittalDistributionList> transDistList = new List<TransmittalDistributionList>();
                Transmittals = GetTransmittalDataFromOldProcon(projectID, mysqlCon);
                PMMigrationLogger.Log("Number of Transmittal Data retrieve : " + Transmittals.Count);

                if (restoreStatus == RestoreStatus.Success)
                {
                    
                    transDistList = GetTransmittalDistributionListFromOldProcon(Transmittals, mysqlCon);
                    PMMigrationLogger.Log("Number of TransmittalDistributionList retrieve : " + transDistList.Count);
                    PMMigrationLogger.Log("Data retrival completed ---------------------", Color.Black, FontStyle.Bold);

                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    
                    transItemList = GetTransmittalItemFromOldProcon(Transmittals, mysqlCon);
                    PMMigrationLogger.Log("Number of TransmittalItem retrieve : " + transItemList.Count);
                    PMMigrationLogger.Log("Data retrival completed ---------------------", Color.Black, FontStyle.Bold);

                }

                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Transmittal Data restore started ................", Color.Black, FontStyle.Bold);
                    InsertTransmittalDataToIDBO(Transmittals, transItemList, transDistList, sqlCon);
                    PMMigrationLogger.Log("Transmittal Data restore completed ................", Color.Black, FontStyle.Bold);
                }
                


            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                PMMigrationLogger.Log(ex.ToString(), Color.Red, FontStyle.Regular);
                throw;
            }
            PMMigrationLogger.Log("Out RestoreTransmittalData");
            return result;
        }


        public List<Transmittal> GetTransmittalDataFromOldProcon(Guid projectId, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetTransmittalDataFromOldProcon : Getting Transmittal Data started ...", Color.Black, FontStyle.Bold);
            List<Transmittal> transmittals = new List<Transmittal>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                string myGetTransmittslQuery = String.Format("select * from pc_govt_transmittals where PC_TRANS_PRJ_FK = '{0}'", projectId);
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myGetTransmittslQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        Transmittal transmittal = new Transmittal();
                        transmittal.ID = Guid.NewGuid();

                        int oldID;
                        int.TryParse(myReader["PC_TRANS_ID"].ToString(), out oldID);
                        transmittal.OldId = oldID;

                        //transmittal.ToContactID = Guid.Empty;
                        //transmittal.OldToContactID = myReader["PC_TRANS_TO_ID"].ToString();
                        if (myReader["PC_TRANS_TO_ID"] == DBNull.Value)
                            transmittal.ToContactID = Guid.Empty;
                        else
                        {
                            Guid m_ToContactID;
                            Guid.TryParse(myReader["PC_TRANS_TO_ID"].ToString(), out m_ToContactID);
                            transmittal.ToContactID = m_ToContactID;
                        }

                        transmittal.ToContactName = myReader["PC_TRANS_TO"].ToString();
                        transmittal.Specification = myReader["PC_TRANS_SPECIFICATIONS"].ToString();

                        //transmittal.OldFromContactID = myReader["PC_TRANS_FROM_ID"].ToString();
                        //transmittal.FromContactID = Guid.Empty;
                        if (myReader["PC_TRANS_FROM_ID"] == DBNull.Value)
                            transmittal.FromContactID = Guid.Empty;
                        else
                        {
                            Guid m_FromContactID;
                            Guid.TryParse(myReader["PC_TRANS_FROM_ID"].ToString(), out m_FromContactID);
                            transmittal.FromContactID = m_FromContactID;
                        }


                        string isSubTrans = myReader["PC_TRANS_RESUB_TRANS"].ToString();
                        if (isSubTrans == "Y")
                        {
                            transmittal.IsResubmittedTransmittal = 1;
                        }

                        transmittal.Remarks = myReader["PC_TRANS_REMARKS"].ToString();
                        transmittal.ProjectMode = myReader["PC_TRANS_PRJ_MODE"].ToString();

                        transmittal.ProjectID = projectId;
                        Guid m_OldprojectID;
                        Guid.TryParse(myReader["PC_TRANS_PRJ_FK"].ToString(), out m_OldprojectID);
                        transmittal.OldProjectID = m_OldprojectID;

                        int slNo;
                        int.TryParse(myReader["PC_TRANS_NO"].ToString(), out slNo);
                        transmittal.SerialNumber = slNo;

                        string isNewTrans = myReader["PC_TRANS_NEW_TRANS"].ToString();
                        if (isNewTrans == "Y")
                        {
                            transmittal.IsNewTransmittal = 1;
                        }

                        string isGovtApp = myReader["PC_TRANS_GOVT_APPROVAL"].ToString();
                        if (isGovtApp == "Y")
                        {
                            transmittal.IsGovtApproval = 1;
                        }

                       

                        string isFlo = myReader["PC_TRANS_FIO"].ToString();
                        if (isFlo == "Y")
                        {
                            transmittal.IsFIOTransmittal = 1;
                        }


                        DateTime Date;
                        if (myReader["PC_TRANS_DATE"] == DBNull.Value)
                        {
                            transmittal.Date = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_TRANS_DATE"].ToString(), out Date);
                            transmittal.Date = Date;
                        }
                        transmittal.FromContactName = myReader["PC_TRANS_FROM"].ToString();
                        transmittal.ContractNumber = myReader["PC_TRANS_CONTRACT_NO"].ToString();

                        transmittal.AC1 = "Migrated Data";

                        transmittals.Add(transmittal);
                    }
                }
                if (!myReader.IsClosed)
                {
                    myReader.Close();
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetTransmittalDataFromOldProcon :Getting Transmittal Data completed ...", Color.Black, FontStyle.Bold);
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
            return transmittals;
        }

        public List<TransmittalDistributionList> GetTransmittalDistributionListFromOldProcon(List<Transmittal> transmittalDtls, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetTransmittalDistributionListFromOldProcon :Getting Transmittal Distribution List  started ...", Color.Black, FontStyle.Bold);
            List<TransmittalDistributionList> transDistLists = new List<TransmittalDistributionList>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var trans in transmittalDtls)
                {
                    string myQuery = string.Format("select " +
                       "trndlist.PC_TRANS_DIST_ID," +
                          "trndlist.PC_TRANS_DIST_TRANS_FK," +
                          "prjCont.PC_PRJ_CONT_ID,"+
                          "trndlist.PC_TRANS_DIST_CC," +
                          "trndlist.PC_TRANS_DIST_BCC," +
                          "trndlist.PC_TRANS_DIST_TO," +
                          "trndlist.PC_TRANS_DIST_PRJ_CONT_EMAIL," +
                          "trndlist.PC_TRANS_DIST_PRJ_COMP_NAME," +
                          "trndlist.PC_TRANS_DIST_PRJ_CONT_NAME " +
                          "FROM pc_transmittal_distributionlist trndlist " +
                        "JOIN  pc_govt_transmittals pgt ON trndlist.PC_TRANS_DIST_TRANS_FK = pgt.PC_TRANS_ID  " +
                         "JOIN pc_project_contacts prjCont ON pgt.PC_TRANS_PRJ_FK = prjCont.PC_PRJ_CONT_PRJ_FK " +
                          "WHERE pgt.PC_TRANS_ID = '{0}' "+
                          "AND trndlist.PC_TRANS_DIST_PRJ_CONT_NAME = prjCont.PC_PRJ_CONT_NAME " +
                          "AND trndlist.PC_TRANS_DIST_PRJ_CONT_EMAIL = prjCont.PC_PRJ_CONT_EMAIL " +
                          "AND trndlist.PC_TRANS_DIST_PRJ_COMP_NAME = prjCont.PC_PRJ_CONT_COMP_NAME ", trans.OldId);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();

                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            TransmittalDistributionList transDistList = new TransmittalDistributionList();

                            transDistList.ID = Guid.NewGuid();
                            transDistList.OldID = myReader["PC_TRANS_DIST_ID"].ToString();
                            string to = myReader["PC_TRANS_DIST_TO"].ToString();
                            if (to == "Y")
                            {
                                transDistList.To = 1;
                            }

                            string cc = myReader["PC_TRANS_DIST_CC"].ToString();
                            if (cc == "Y")
                            {
                                transDistList.Cc = 1;
                            }

                            string bcc = myReader["PC_TRANS_DIST_BCC"].ToString();
                            if (bcc == "Y")
                            {
                                transDistList.Bcc = 1;
                            }
                            
                            int m_OldtransID;
                            int.TryParse(myReader["PC_TRANS_DIST_TRANS_FK"].ToString(), out m_OldtransID);
                            transDistList.OldTransmittalID = m_OldtransID;
                            transDistList.TransmittalID = trans.ID;
                            Guid m_ProjectContactID;
                            Guid.TryParse(myReader["PC_PRJ_CONT_ID"].ToString(), out m_ProjectContactID);
                            transDistList.ProjectContactID = m_ProjectContactID;

                            transDistList.CompanyName = myReader["PC_TRANS_DIST_PRJ_COMP_NAME"].ToString();
                            transDistList.ContactName = myReader["PC_TRANS_DIST_PRJ_CONT_NAME"].ToString();
                            transDistList.ContactEmail = myReader["PC_TRANS_DIST_PRJ_CONT_EMAIL"].ToString();

                            transDistList.AC1 = "Migrated Data";

                            transDistLists.Add(transDistList);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }

                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetTransmittalDistributionListFromOldProcon :Getting Transmittal Distribution List completed ...", Color.Black, FontStyle.Bold);
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
            return transDistLists;
        }

        public List<TransmittalItem> GetTransmittalItemFromOldProcon(List<Transmittal> transmittalDtls, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetTransmittalItemFromOldProcon :Getting Transmittal Item  started ...", Color.Black, FontStyle.Bold);
            List<TransmittalItem> transItemLists = new List<TransmittalItem>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var trans in transmittalDtls)
                {
                    string myQuery = string.Format("select " +
                       "trndlist.PC_LTR_TRANS_VAR," +
                          "trndlist.PC_LTR_TRANS_USE_CODE," +
                          "trndlist.PC_LTR_TRANS_SHT_NO," +
                          "trndlist.PC_LTR_TRANS_PARA_NO," +
                          "trndlist.PC_LTR_TRANS_NO," +
                          "trndlist.PC_LTR_TRANS_LTR_FK," +
                          "trndlist.PC_LTR_TRANS_ID," +
                          "trndlist.PC_LTR_TRANS_DESC, " +
                           "trndlist.PC_LTR_TRANS_COPIES, " +
                            "trndlist.PC_LTR_TRANS_BROCH_NO " +
                          "FROM pc_letter_transmittal trndlist " +
                        "JOIN  pc_govt_transmittals pgt ON trndlist.PC_LTR_TRANS_LTR_FK = pgt.PC_TRANS_ID  " +
                          "WHERE pgt.PC_TRANS_ID = '{0}' ", trans.OldId);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();

                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            TransmittalItem transItemList = new TransmittalItem();

                            transItemList.ID = Guid.NewGuid();

                            int ID;
                            int.TryParse(myReader["PC_LTR_TRANS_ID"].ToString(), out ID);
                            transItemList.oldID = ID;

                            transItemList.ItemVersion = myReader["PC_LTR_TRANS_VAR"].ToString();
                            transItemList.UseCode = myReader["PC_LTR_TRANS_USE_CODE"].ToString();
                            transItemList.ShtNumber = myReader["PC_LTR_TRANS_SHT_NO"].ToString();
                            transItemList.SpecParaNumber = myReader["PC_LTR_TRANS_PARA_NO"].ToString();

                            int slNo;
                            int.TryParse(myReader["PC_LTR_TRANS_NO"].ToString(), out slNo);
                            transItemList.SerialNumber = slNo;

                            transItemList.TransmittalID = trans.ID;

                            int transID;
                            int.TryParse(myReader["PC_LTR_TRANS_LTR_FK"].ToString(), out transID);
                            transItemList.oldTransmittalID = transID;

                            //string copies = myReader["PC_LTR_TRANS_COPIES"].ToString();
                            //if (copies == "Y")
                            //{
                            //    transItemList.Copies = 1;
                            //}
                            if (myReader["PC_LTR_TRANS_COPIES"] == DBNull.Value)
                            {
                                transItemList.Copies = null;
                            }
                            else
                            {
                                int m_Copies;
                                int.TryParse(myReader["PC_LTR_TRANS_COPIES"].ToString(), out m_Copies);
                                transItemList.Copies = m_Copies;
                            }

                            transItemList.BrochNumber = myReader["PC_LTR_TRANS_BROCH_NO"].ToString();
                            transItemList.ItemDescription = myReader["PC_LTR_TRANS_DESC"].ToString();
                            
                            transItemList.AC1 = "Migrated Data";

                            transItemLists.Add(transItemList);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }

                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetTransmittalItemFromOldProcon :Getting Transmittal Item completed ...", Color.Black, FontStyle.Bold);
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
            return transItemLists;
        }

        #region Insert All Transmittal Out Related Data

        public void InsertTransmittalDataToIDBO(List<Transmittal> transmittals, List<TransmittalItem> transmittalItems, List<TransmittalDistributionList> transmittalDistributionLists, SqlConnection sqlCon)
        {
            try
            {
                InsertTransmittalToIDBO(transmittals, sqlCon);
               InsertTransmittalItemToIDBO(transmittalItems, sqlCon);
               InsertTransmittalDistributionListToIDBO(transmittalDistributionLists, sqlCon);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InsertTransmittalToIDBO(List<Transmittal> transmittals, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertTransmittalToIDBO : Restoring Transmittals started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var transmittal in transmittals)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryTransmittal = string.Format("INSERT INTO PMTransmittals(ID,ProjectID,FromContactID,ToContactID,Specification,IsResubmittedTransmittal,Remarks,ProjectMode,SerialNumber,IsNewTransmittal,IsGovtApproval,IsFIOTransmittal,Date,AC1,OldID,OldProjectID,OldToContactID,OldFromContactID) " +
                                                            "VALUES ('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}',{8},{9},{10},{11},'{12}','{13}',{14},'{15}','{16}','{17}')", transmittal.ID,
                                                            transmittal.ProjectID, transmittal.FromContactID, transmittal.ToContactID, transmittal.Specification.Replace("'", "''"), transmittal.IsResubmittedTransmittal, transmittal.Remarks.Replace("'", "''"),
                                                            transmittal.ProjectMode, transmittal.SerialNumber, transmittal.IsNewTransmittal, transmittal.IsGovtApproval,
                                                            transmittal.IsFIOTransmittal,transmittal.Date, transmittal.AC1.Replace("'", "''"), transmittal.OldId, transmittal.OldProjectID, transmittal.OldToContactID, transmittal.OldFromContactID);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryTransmittal;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN Transmittal INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }

                PMMigrationLogger.Log("Out InsertTransmittalToIDBO : Restoring Transmittals completed  --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertTransmittalItemToIDBO(List<TransmittalItem> transmittalItemLists, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertTransmittalItemToIDBO : Restoring Transmittal Item list started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var transmittalItem in transmittalItemLists)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryTransmittalItem = string.Format("INSERT INTO PMTransmittalItem(ID,TransmittalID,ItemVersion,UseCode,ShtNumber,SpenParaNumber,SerialNumber,TransmittalItemDescription,Copies,BrochNumber,AC1,OldID,OldTransmittalID) " +
                                                            "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}',{8},'{9}','{10}',{11},{12})", transmittalItem.ID,
                                                            transmittalItem.TransmittalID, transmittalItem.ItemVersion, transmittalItem.UseCode, transmittalItem.ShtNumber, transmittalItem.SpecParaNumber, transmittalItem.SerialNumber, transmittalItem.ItemDescription.Replace("'", "''"),
                                                            (transmittalItem.Copies == null) ? (Object)"null" : transmittalItem.Copies, transmittalItem.BrochNumber, transmittalItem.AC1.Replace("'", "''"), transmittalItem.oldID, transmittalItem.oldTransmittalID);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryTransmittalItem;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN TransmittalItem INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }

                PMMigrationLogger.Log("Out InsertTransmittalItemToIDBO : Restoring Transmittal Item List completed  --------------------", Color.Black, FontStyle.Bold);
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


        public void InsertTransmittalDistributionListToIDBO(List<TransmittalDistributionList> transmittalDistributionList, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertTransmittalDistributionListToIDBO : Restoring DistributionList  started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var transmittalDist in transmittalDistributionList)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();

                        cmd.Connection = sqlCon;
                        string qryTransmittalDist = string.Format("INSERT INTO [PMTransmittalDistributionList] ([ID],[TransmittalID],[ProjectContactID],[To],[Cc],[Bcc]," +
                        "[AC1],[OldID],[OldTransmittalID],[OldProjectContactID],[OldContactName],[OldContactEmail],[OldContactCompany])" +
                        "VALUES ('{0}','{1}','{2}',{3},{4},{5},'{6}','{7}',{8},'{9}','{10}','{11}','{12}')", transmittalDist.ID, transmittalDist.TransmittalID, transmittalDist.ProjectContactID,
                        transmittalDist.To, transmittalDist.Cc, transmittalDist.Bcc, transmittalDist.AC1, transmittalDist.OldID, transmittalDist.OldTransmittalID, transmittalDist.OldProjectContactID, transmittalDist.OldContactName, transmittalDist.OldContactEmail, transmittalDist.OldContactCompany);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryTransmittalDist;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN TRANSMITTAL DISTIBUTION INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertTransmittalDistributionListToIDBO : Restoring DistributionList  completed  --------------------", Color.Black, FontStyle.Bold);
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
    }
}
