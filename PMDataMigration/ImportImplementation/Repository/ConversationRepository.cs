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
    public class ConversationRepository
    {

        RestoreStatus restoreStatus = RestoreStatus.None;


        public int RestoreConversationData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreConversationData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("Data retrival started ---------------------", Color.Black, FontStyle.Bold);
                List<Conversations> conversations = new List<Conversations>();
                conversations = GetConversationDataFromOldProcon(projectID, mysqlCon);
                PMMigrationLogger.Log("Number of Conversation Data retrieve : " + conversations.Count);
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("RFI Data restore started ................", Color.Black, FontStyle.Bold);
                    InsertConversationToIDBO(conversations, sqlCon);
                    PMMigrationLogger.Log("RFI Data restore completed ................", Color.Black, FontStyle.Bold);
                }

            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                PMMigrationLogger.Log(ex.ToString(), Color.Red, FontStyle.Regular);
                throw;
            }
            PMMigrationLogger.Log("Out RestoreConversationData");
            return result;
        }

        public List<Conversations> GetConversationDataFromOldProcon(Guid projectId, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetConversationDataFromOldProcon : Getting Conversation  Data started ...", Color.Black, FontStyle.Bold);
            List<Conversations> conversations = new List<Conversations>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                string myGetTransmittslQuery = String.Format("select * from pc_conversation where PC_CONV_PRJ_FK = '{0}'", projectId);
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myGetTransmittslQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        Conversations conversation = new Conversations();
                        conversation.ID = Guid.NewGuid();

                        int oldID;
                        int.TryParse(myReader["PC_CONV_ID"].ToString(), out oldID);
                        conversation.OldID = oldID;

                        conversation.Comments = myReader["PC_CONV_COMMENTS"].ToString();
                        conversation.ProjectID = projectId;

                        if (myReader["PC_CONV_PERSONNEL_FK"] == DBNull.Value)
                        {
                            conversation.OldPersonID = null;
                            conversation.PersonID = Guid.Empty;
                        }
                        else
                        {
                            conversation.OldPersonID = myReader["PC_CONV_PERSONNEL_FK"].ToString();
                            Guid m_PersonID;
                            Guid.TryParse(myReader["PC_CONV_PERSONNEL_FK"].ToString(), out m_PersonID);
                            conversation.PersonID = m_PersonID;
                        }

                        if (myReader["PC_CONV_CONT_FK"] == DBNull.Value)
                        {
                            conversation.OldProjectContactID = null;
                            conversation.ProjectContactID = Guid.Empty;
                        }
                        else
                        {
                            Guid m_ProjectContactID;
                            Guid.TryParse(myReader["PC_CONV_CONT_FK"].ToString(), out m_ProjectContactID);
                            conversation.ProjectContactID = m_ProjectContactID;
                            //conversation.ProjectContactID = Guid.Empty;
                            conversation.OldProjectContactID = myReader["PC_CONV_CONT_FK"].ToString();
                        }

                        DateTime Date;
                        if (myReader["PC_CONV_DATE"] == DBNull.Value)
                        {
                            conversation.Date = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_CONV_DATE"].ToString(), out Date);
                            conversation.Date = Date;
                        }

                        int isActive;
                        int.TryParse(myReader["PC_CONV_IS_ACTIVE"].ToString(), out isActive);
                        conversation.IsActive = isActive;

                        int prjMode;
                        int.TryParse(myReader["PC_CONV_PRJ_MODE"].ToString(), out prjMode);
                        conversation.ProjectMode = prjMode;

                        conversation.AC1 = "Migrated Data";

                        conversations.Add(conversation);
                    }
                }
                if (!myReader.IsClosed)
                {
                    myReader.Close();
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetConversationDataFromOldProcon : Getting Conversation Data completed ...", Color.Black, FontStyle.Bold);
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
            return conversations;
        }

        public void InsertConversationToIDBO(List<Conversations> conversationList, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertConversationToIDBO : Restoring Conversation started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var conversation in conversationList)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryConversation = string.Format("INSERT INTO [PMConversation] ( [ID],[ProjectID],[ProjectcontactID],[PersonID]," +
                                                "[Comments],[Date],[IsActive],[ProjectMode],[AC1],[OldID])" +
                                                " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},'{8}',{9})", conversation.ID
                                                , conversation.ProjectID, conversation.ProjectContactID, conversation.PersonID, conversation.Comments.Replace("'", "''"),
                                                conversation.Date, conversation.IsActive, conversation.ProjectMode, conversation.AC1.Replace("'", "''"), conversation.OldID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryConversation;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN Conversation INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertConversationToIDBO : Restoring Conversation completed  --------------------", Color.Black, FontStyle.Bold);
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


    }
}
