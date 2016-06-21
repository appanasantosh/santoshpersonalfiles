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
    public class MeetingsRepository
    {
        RestoreStatus restoreStatus = RestoreStatus.None;

        public int RestoreMeetingData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreMeetingData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("Data retrival started ---------------------", Color.Black, FontStyle.Bold);
                List<Meetings> meetings = new List<Meetings>();
                List<MeetingTopicAndMeetingItems> meetingTopic = new List<MeetingTopicAndMeetingItems>();
                List<MeetingAttendees> meetingAttendees = new List<MeetingAttendees>();
                meetings = GetMeetingDataFromOldProcon(projectID, mysqlCon);
                PMMigrationLogger.Log("Number of Meetings Data retrieve : " + meetings.Count);

                if (restoreStatus == RestoreStatus.Success)
                {

                    meetingTopic = GetMeetingTopicAndItemFromOldProcon(meetings, mysqlCon);
                    PMMigrationLogger.Log("Number of MeetingTopic retrieve : " + meetingTopic.Count);
                    PMMigrationLogger.Log("Data retrival completed ---------------------", Color.Black, FontStyle.Bold);
                }


                if (restoreStatus == RestoreStatus.Success)
                {

                    meetingAttendees = GetMeetingAttendeesFromOldProcon(meetings, mysqlCon);
                    PMMigrationLogger.Log("Number of MeetingAttendees retrieve : " + meetingTopic.Count);
                    PMMigrationLogger.Log("Data retrival completed ---------------------", Color.Black, FontStyle.Bold);
                }



                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Instruction Data restore started ................", Color.Black, FontStyle.Bold);
                    InsertMeetingDataTOIDBO(meetings, meetingTopic, meetingAttendees, sqlCon);
                    PMMigrationLogger.Log("Instruction Data restore completed ................", Color.Black, FontStyle.Bold);

                }

            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                PMMigrationLogger.Log(ex.ToString(), Color.Red, FontStyle.Regular);
                throw;
            }
            PMMigrationLogger.Log("Out RestoreInstructionData");
            return result;
        }


        public List<Meetings> GetMeetingDataFromOldProcon(Guid projectId, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetMeetingDataFromOldProcon : Getting Meeting Data started ...", Color.Black, FontStyle.Bold);
            List<Meetings> meetings = new List<Meetings>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                string myGetMeetingsQuery = String.Format("select * from pc_meetings where PC_MEET_PRJ_FK = '{0}'", projectId);
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myGetMeetingsQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        Meetings meeting = new Meetings();
                        meeting.ID = Guid.NewGuid();

                        int oldID;
                        int.TryParse(myReader["PC_MEET_ID"].ToString(), out oldID);
                        meeting.OldID = oldID;

                        

                        DateTime Date;
                        if (myReader["PC_MEET_DATE"] == DBNull.Value)
                        {
                            meeting.Date = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_MEET_DATE"].ToString(), out Date);
                            meeting.Date = Date;
                        }

                        TimeSpan time;
                        if (myReader["PC_MEET_TIME"] == DBNull.Value)
                        {
                            meeting.Time = null;
                        }
                        else
                        {
                            TimeSpan.TryParse(myReader["PC_MEET_TIME"].ToString(), out time);
                            meeting.Time = time;
                        }

                        int meetingNumber;
                        int.TryParse(myReader["PC_MEET_NO"].ToString(), out meetingNumber);
                        meeting.Number = meetingNumber;

                        meeting.ProjectID = projectId;
                        Guid m_OldProjectID;
                        Guid.TryParse(myReader["PC_MEET_PRJ_FK"].ToString(), out m_OldProjectID);
                        meeting.OldProjectID = m_OldProjectID;

                        meeting.RecorderName = myReader["PC_MEET_RECORDER"].ToString();
                        meeting.Name = myReader["PC_MEET_NAME"].ToString();
                        meeting.Location = myReader["PC_MEET_LOC"].ToString();

                        DateTime m_NextMeetingDate;
                        if (myReader["PC_MEET_NEXT_MEET"] == DBNull.Value)
                        {
                            meeting.NextMeeting = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_MEET_NEXT_MEET"].ToString(), out m_NextMeetingDate);
                            meeting.NextMeeting = m_NextMeetingDate;
                        }

                        meeting.SpecialNotes2 = myReader["PC_MEET_SPECIAL_NOTES_2"].ToString();

                        meeting.SpecialNotes = myReader["PC_MEET_SPECIAL_NOTES"].ToString();
                        meeting.Status = myReader["PC_MEET_STATUS"].ToString();

                        string locked = myReader["PC_MEET_LOCKED"].ToString();
                        if (locked == "Y")
                        {
                            meeting.Locked = 1;
                        }
                        else
                        {
                            meeting.Locked = 0;
                        }

                        int m_ProjectMode;
                        int.TryParse(myReader["PC_MEET_PROJECT_MODE"].ToString(),out m_ProjectMode);
                        meeting.ProjectMode = m_ProjectMode;

                        meeting.TimeZone = myReader["PC_MEET_TIME_ZONE"].ToString();

                       
                        meeting.AC1 = "Migrated Data";

                        meetings.Add(meeting);
                    }
                }
                if (!myReader.IsClosed)
                {
                    myReader.Close();
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetMeetingDataFromOldProcon : Getting Meeting Data completed ...", Color.Black, FontStyle.Bold);
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
            return meetings;
        }


        public List<MeetingTopicAndMeetingItems> GetMeetingTopicAndItemFromOldProcon(List<Meetings> meetings, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetMeetingItemFromOldProcon :Getting MeetingItem  started ...", Color.Black, FontStyle.Bold);
            List<MeetingTopicAndMeetingItems> meetingTopicAndItems = new List<MeetingTopicAndMeetingItems>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var meeting in meetings)
                {
                    

                    string myQuery = String.Format("select * from pc_meeting_items where PC_MEET_ITM_MEET_FK = '{0}'", meeting.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();

                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            MeetingTopicAndMeetingItems meetingItemList = new MeetingTopicAndMeetingItems();

                            meetingItemList.MeetingTopicID = Guid.NewGuid();
                            meetingItemList.ItemID = Guid.NewGuid();
                            meetingItemList.MeetingID = meeting.ID;
                            meetingItemList.OldMeetingID = meeting.OldID;

                            
                            
                            int m_OldTopicID;
                            int.TryParse(myReader["PC_MEET_ITM_ID"].ToString(), out m_OldTopicID);
                            meetingItemList.OldTopicID = m_OldTopicID;

                            int m_TopicNumber;
                            if (myReader["PC_MEET_ITM_NO"] == DBNull.Value)
                            {
                                meetingItemList.TopicOrder = null;
                            }
                            else
                            {
                                int.TryParse(myReader["PC_MEET_ITM_NO"].ToString(),out m_TopicNumber);
                                meetingItemList.TopicOrder = m_TopicNumber;
                            }

                            meetingItemList.TopicDescription = myReader["PC_MEET_ITM_TOPIC"].ToString();

                            meetingItemList.Status = myReader["PC_MEET_ITM_STATUS"].ToString();

                            decimal m_ItemNumber;
                            if (myReader["PC_MEET_ITM_SUB_NO"] == DBNull.Value)
                            {
                                meetingItemList.Number = null;
                            }
                            else
                            {
                                decimal.TryParse(myReader["PC_MEET_ITM_SUB_NO"].ToString(), out m_ItemNumber);
                                meetingItemList.Number = m_ItemNumber;
                            }

                            DateTime m_Due;
                            if (myReader["PC_MEET_ITM_DUE"] == DBNull.Value)
                            {
                                meetingItemList.Due = null;
                            }
                            else
                            {
                                DateTime.TryParse(myReader["PC_MEET_ITM_DUE"].ToString(), out m_Due);
                                meetingItemList.Due = m_Due;
                            }

                            meetingItemList.MeetingItemsDescription = myReader["PC_MEET_ITM_DESC"].ToString();

                            DateTime m_Date;
                            if (myReader["PC_MEET_ITM_DATE"] == DBNull.Value)
                            {
                                meetingItemList.Date = null;
                            }
                            else
                            {
                                DateTime.TryParse(myReader["PC_MEET_ITM_DATE"].ToString(), out m_Date);
                                meetingItemList.Date = m_Date;
                            }

                            meetingItemList.BIC = myReader["PC_MEET_ITM_BIC"].ToString();

                            

                            meetingItemList.AC1 = "Migrated Data";

                            meetingTopicAndItems.Add(meetingItemList);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }

                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetMeetingItemFromOldProcon :Getting MeetingItem Item completed ...", Color.Black, FontStyle.Bold);
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
            return meetingTopicAndItems;
        }


        public List<MeetingAttendees> GetMeetingAttendeesFromOldProcon(List<Meetings> meetings, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetMeetingAttendeesFromOldProcon :Getting MeetingAttendees  started ...", Color.Black, FontStyle.Bold);
            List<MeetingAttendees> meetingAttendees = new List<MeetingAttendees>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var meeting in meetings)
                {


                    string myQuery = String.Format("select * from pc_meeting_attendees where PC_MEET_ATTD_MEET_FK = '{0}'", meeting.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();

                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            MeetingAttendees meetingAttendeeList = new MeetingAttendees();

                            meetingAttendeeList.ID = Guid.NewGuid();
                            meetingAttendeeList.MeetingID = meeting.ID;
                            
                            meetingAttendeeList.OldMeetingID = meeting.OldID;



                            int m_OldMeetingAttendeeID;
                            int.TryParse(myReader["PC_MEET_ATTD_ID"].ToString(), out m_OldMeetingAttendeeID);
                            meetingAttendeeList.OldID = m_OldMeetingAttendeeID;

                            Guid m_ProjectContactID;
                            Guid.TryParse(myReader["PC_MEET_ATTD_CONT_FK"].ToString(), out m_ProjectContactID);
                            meetingAttendeeList.ProjectContactID = m_ProjectContactID;

                            string ATT = myReader["PC_MEET_ATTD_ATT"].ToString();

                            if (ATT == "Y")
                            {
                                meetingAttendeeList.ATT = 1;
                            }
                            else
                            {
                                meetingAttendeeList.ATT = 0;
                            }

                            string Dist = myReader["PC_MEET_ATTD_DIST"].ToString();

                            if (Dist == "Y")
                            {
                                meetingAttendeeList.Dist = 1;
                            }
                            else
                            {
                                meetingAttendeeList.Dist = 0;
                            }

                            string Attendee = myReader["PC_MEET_ATTENDEE"].ToString();

                            if (Attendee == "Y")
                            {
                                meetingAttendeeList.Attendee = 1;
                            }
                            else
                            {
                                meetingAttendeeList.Attendee = 0;
                            }

                            string IsActive = myReader["PC_MEET_ATTD_IS_ACTIVE"].ToString();

                            if (IsActive == "Y")
                            {
                                meetingAttendeeList.IsActive = 0;
                            }
                            else
                            {
                                meetingAttendeeList.IsActive = 1;
                            }

                        

                            meetingAttendeeList.AC1 = "Migrated Data";

                            meetingAttendees.Add(meetingAttendeeList);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }

                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetMeetingAttendeeFromOldProcon :Getting Meeting Attendee completed ...", Color.Black, FontStyle.Bold);
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
            return meetingAttendees;
        }


        public void InsertMeetingDataTOIDBO(List<Meetings> meetings, List<MeetingTopicAndMeetingItems> meetingTopicandItemLists,List<MeetingAttendees> meetingAttendees, SqlConnection sqlCon)
        {
            InsertMeetingToIDBO(meetings, sqlCon);

            InsertMeetingTopicListToIDBO(meetingTopicandItemLists, sqlCon);

            InsertMeetingItemListToIDBO(meetingTopicandItemLists, sqlCon);

            InsertMeetingAttendeeToIDBO(meetingAttendees, sqlCon);
        }

        public void InsertMeetingToIDBO(List<Meetings> meetings, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertMeetingToIDBO : Restoring Meeting started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var meeting in meetings)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryMeeting = string.Format("INSERT INTO [PMMeetings] ( [ID],[ProjectID]," +
                                                "[Date],[Time],[Number],[Name],[Location],[NextMeeting],[SpecialNotes2],[SpecialNotes],[Status],[Locked]," +
                                                "[ProjectMode],[TimeZone],[RecorderName],[AC1],[OldID],[OldProjectID])" +
                                                " VALUES ('{0}','{1}',{2},'{3}',{4},'{5}','{6}',{7},'{8}','{9}','{10}',{11},{12},'{13}','{14}','{15}',{16},'{17}')", meeting.ID
                                                , meeting.ProjectID, (meeting.Date == null) ? (object)"null" : "'" + meeting.Date + "'", meeting.Time, meeting.Number, meeting.Name.Replace("'", "''"),
                                                meeting.Location.Replace("'", "''"), (meeting.NextMeeting == null) ? (object)"null" : "'" + meeting.NextMeeting + "'", meeting.SpecialNotes2.Replace("'", "''"), meeting.SpecialNotes.Replace("'", "''"), meeting.Status, meeting.Locked, meeting.ProjectMode, meeting.TimeZone, 
                                                 meeting.RecorderName,meeting.AC1.Replace("'", "''"), meeting.OldID, meeting.OldProjectID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryMeeting;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN MEETING INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertMeetingToIDBO : Restoring Meeting completed  --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertMeetingTopicListToIDBO(List<MeetingTopicAndMeetingItems> meetingTopicList, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertMeetingTopicToIDBO : Restoring MeetingTopic  started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var meetingTopic in meetingTopicList)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();

                        cmd.Connection = sqlCon;
                        string qryMeetingTopic = string.Format("INSERT INTO [PMMeetingTopic] ([ID],[MeetingID],[TopicOrder],[TopicDescription]," +
                        "[AC1],[OldTopicID],[OldMeetingID])" +
                        "VALUES ('{0}','{1}',{2},'{3}','{4}',{5},{6})", meetingTopic.MeetingTopicID, meetingTopic.MeetingID, (meetingTopic.TopicOrder == null)? (object)"null":meetingTopic.TopicOrder,
                        meetingTopic.TopicDescription.Replace("'", "''"), meetingTopic.AC1.Replace("'", "''"), meetingTopic.OldTopicID, meetingTopic.OldMeetingID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryMeetingTopic;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN MeetingTopic INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertMeetingTopicToIDBO : Restoring MeetingTopic  completed  --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertMeetingItemListToIDBO(List<MeetingTopicAndMeetingItems> meetingTopicList, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertMeetingItemToIDBO : Restoring MeetingItem  started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var meetingItem in meetingTopicList)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();

                        cmd.Connection = sqlCon;
                        string qryMeetingItem = string.Format("INSERT INTO [PMMeetingItems] ([ID],[MeetingTopicID],[Due],[Date]," +
                        "[Number],[Status],[MeetingItemsDescription],[BIC],[AC1],[OldItemID])" +
                        "VALUES ('{0}','{1}',{2},{3},{4},'{5}','{6}','{7}','{8}',{9})", meetingItem.ItemID, meetingItem.MeetingTopicID, (meetingItem.Due == null) ? (object)"null" :"'"+meetingItem.Due+"'",
                        (meetingItem.Date == null) ? (object)"null" : "'"+meetingItem.Date+"'",meetingItem.Number ,meetingItem.Status, meetingItem.MeetingItemsDescription.Replace("'","''"), meetingItem.BIC, meetingItem.AC1.Replace("'", "''"), meetingItem.OldTopicID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryMeetingItem;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN MeetingItem INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertMeetingItemToIDBO : Restoring MeetingItem  completed  --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertMeetingAttendeeToIDBO(List<MeetingAttendees> meetingAttendees, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertMeetingAttendeeToIDBO : Restoring MeetingAttendee started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var meetingAttendee in meetingAttendees)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryMeetingAttendee = string.Format("INSERT INTO [PMMeetingAttendees] ( [ID],[MeetingID]," +
                                                "[ProjectContactID],[Attendee],[Dist],[Att],[IsActive],[AC1],[OldID],[OldMeetingID])"+
                                                " VALUES ('{0}','{1}','{2}',{3},{4},{5},{6},'{7}',{8},{9})", meetingAttendee.ID
                                                , meetingAttendee.MeetingID, meetingAttendee.ProjectContactID, meetingAttendee.Attendee, meetingAttendee.Dist, meetingAttendee.ATT,
                                                meetingAttendee.IsActive, meetingAttendee.AC1.Replace("'", "''"), meetingAttendee.OldID, meetingAttendee.OldMeetingID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryMeetingAttendee;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN MEETINGATTENDEE INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertMeetingAttendeeToIDBO : Restoring Meeting Attendee completed  --------------------", Color.Black, FontStyle.Bold);
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
