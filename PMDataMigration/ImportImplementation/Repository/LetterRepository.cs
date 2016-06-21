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
    public class LetterRepository
    {
        RestoreStatus restoreStatus = RestoreStatus.None;

        public int RestoreLetterData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreLetterData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("Data retrival started ---------------------", Color.Black, FontStyle.Bold);
                List<Letter> Letters = new List<Letter>();
                Letters = GetLetterDataFromOldProcon(projectID, mysqlCon);
                List<LetterDistributionList> letterDistList = null;
                PMMigrationLogger.Log("Number of Letter Data retrieve : " + Letters.Count);

                if (restoreStatus == RestoreStatus.Success)
                {
                    letterDistList = new List<LetterDistributionList>();
                    letterDistList = GetLetterDistributionListFromOldProcon(Letters, mysqlCon);
                    PMMigrationLogger.Log("Number of LetterDistributionList retrieve : " + letterDistList.Count);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Creating Old ProjectArea , project Control-- New Project Area, project contro  Relation ....It might tale several minutes , please be patient", Color.Black, FontStyle.Bold);
                    Letters = ProjectAreaControlMapping(Letters, sqlCon);
                    PMMigrationLogger.Log("Creating Old ProjectArea, project control -- New Project Area, project contro Relation Completed", Color.Black, FontStyle.Bold);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Letter Data restore started ................", Color.Black, FontStyle.Bold);
                    InsertLetterDataTOIDBO(Letters, letterDistList, sqlCon);
                    PMMigrationLogger.Log("Letter Data restore completed ................", Color.Black, FontStyle.Bold);

                }
              
            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                PMMigrationLogger.Log(ex.ToString(), Color.Red, FontStyle.Regular);
                throw;
            }
            PMMigrationLogger.Log("Out RestoreLetterData");
            return result;
        }

        public List<Letter> GetLetterDataFromOldProcon(Guid projectId, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetLetterDataFromOldProcon : Getting Letter started ...", Color.Black, FontStyle.Bold);
            List<Letter> letters = new List<Letter>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                string myGetTransmittslQuery = String.Format("select l.*,pc.PC_PRJ_CONT_ID from pc_Letters l LEFT JOIN pc_project_contacts pc on l.PC_LET_PRJ_FK=pc.PC_PRJ_CONT_PRJ_FK and l.PC_LET_SENDER_FK=pc.PC_PRJ_CONT_USER_ID where PC_LET_PRJ_FK = '{0}'", projectId);
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myGetTransmittslQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        Letter letter = new Letter();
                        letter.ID = Guid.NewGuid();

                        int oldID;
                        int.TryParse(myReader["PC_LET_ID"].ToString(), out oldID);
                        letter.OldID = oldID;
                        letter.ProjectID = projectId;
                        letter.OldProjectID = projectId.ToString();

                        int m_SerialNumber;
                        int.TryParse(myReader["PC_LET_NO"].ToString(), out m_SerialNumber);
                        letter.SerialNumber = m_SerialNumber;
                        letter.Description = myReader["PC_LET_TITLE"].ToString();
                        //letter.SenderID = Guid.Empty;

                        if (myReader["PC_PRJ_CONT_ID"] == DBNull.Value)
                            letter.SenderID =Guid.Empty;
                        else{
                            Guid m_SenderID;
                            Guid.TryParse(myReader["PC_PRJ_CONT_ID"].ToString(), out m_SenderID);
                            letter.SenderID = m_SenderID;
                        }

                        int projMode;
                        int.TryParse(myReader["PC_LET_PRJ_MODE"].ToString(), out projMode);
                        letter.ProjectMode = projMode;

                       
                        letter.Regarding = myReader["PC_LET_REGARDING"].ToString();

                        letter.Enclosure = myReader["PC_LET_ENCLOSURE"].ToString();


                        DateTime Date;
                        if (myReader["PC_LET_DATE"] == DBNull.Value)
                        {
                            letter.Date = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_LET_DATE"].ToString(), out Date);
                            letter.Date = Date;
                        }

                        letter.ProjectControlID = Guid.Empty;

                        int CntNo;
                        int.TryParse(myReader["PC_LET_CTRL_NO"].ToString(), out CntNo);
                        letter.OldProjectControlID = CntNo;

                        //letter.RecipientID = Guid.Empty;
                        //letter.OldRecipientID = myReader["PC_LET_CONTACT_FK"].ToString();
                        if (myReader["PC_LET_CONTACT_FK"] == DBNull.Value)
                            letter.RecipientID = Guid.Empty;
                        else
                        {
                            Guid m_RecipientID;
                            Guid.TryParse(myReader["PC_LET_CONTACT_FK"].ToString(), out m_RecipientID);
                            letter.RecipientID = m_RecipientID;
                        }
                        letter.Closing = myReader["PC_LET_CLOSING"].ToString();
                        letter.Body = myReader["PC_LET_BODY"].ToString();

                        letter.ProjectAreaID = Guid.Empty;
                        int prjAreaID;
                        int.TryParse(myReader["PC_LET_AREA_PKGS"].ToString(), out prjAreaID);
                        letter.OldProjectAreaID = prjAreaID;

                        letter.AC1 = "Migrated Data";

                        letters.Add(letter);
                    }
                }
                if (!myReader.IsClosed)
                {
                    myReader.Close();
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetLetterDataFromOldProcon : Getting Letter Data completed ...", Color.Black, FontStyle.Bold);
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
                    myReader.Close();
                }
            }
            return letters;
        }

        public List<LetterDistributionList> GetLetterDistributionListFromOldProcon(List<Letter> letters, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetLetterDistributionListFromOldProcon :Getting Letter Distribution List  started ...", Color.Black, FontStyle.Bold);
            List<LetterDistributionList> letterDistLists = new List<LetterDistributionList>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var trans in letters)
                {
                    string myQuery = string.Format("select " +
                       "pcldlist.PC_LTR_DIST_ID," +
                       "prjCont.PC_PRJ_CONT_ID," +
                        "pcldlist.PC_LTR_DIST_TO," +
                        "pcldlist.PC_LTR_DIST_CC," +
                        "pcldlist.PC_LTR_DIST_BCC," +
                        "pcldlist.PC_LTR_DIST_PRJ_CONT_NAME," +
                        "pcldlist.PC_LTR_DIST_PRJ_CONT_EMAIL," +
                        "pcldlist.PC_LTR_DIST_PRJ_COMP_NAME," +
                        "pcldlist.PC_LTR_DIST_LTR_FK " +
                        "FROM pc_letter_distributionlist pcldlist " +
                        "JOIN  pc_Letters pl ON pcldlist.PC_LTR_DIST_LTR_FK = pl.PC_LET_ID  " +
                        "JOIN pc_project_contacts prjCont ON pl.PC_LET_PRJ_FK = prjCont.PC_PRJ_CONT_PRJ_FK " +
                        "WHERE pl.PC_LET_ID = '{0}' "+
                        "AND pcldlist.PC_LTR_DIST_PRJ_CONT_NAME = prjCont.PC_PRJ_CONT_NAME " +
                        "AND pcldlist.PC_LTR_DIST_PRJ_CONT_EMAIL = prjCont.PC_PRJ_CONT_EMAIL " +
                        "AND pcldlist.PC_LTR_DIST_PRJ_COMP_NAME = prjCont.PC_PRJ_CONT_COMP_NAME ", trans.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();

                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            LetterDistributionList letterDistList = new LetterDistributionList();

                            letterDistList.ID = Guid.NewGuid();
                            letterDistList.LetterID = trans.ID;
                            Guid m_ProjectContactID;
                            Guid.TryParse(myReader["PC_PRJ_CONT_ID"].ToString(),out m_ProjectContactID);
                            letterDistList.ProjectContactID = m_ProjectContactID;
                            letterDistList.OldID = myReader["PC_LTR_DIST_ID"].ToString();
                            letterDistList.OldProjectContactID = myReader["PC_PRJ_CONT_ID"].ToString();
                            string to = myReader["PC_LTR_DIST_TO"].ToString();
                            if (to == "Y")
                            {
                                letterDistList.To = 1;
                            }

                            string cc = myReader["PC_LTR_DIST_CC"].ToString();
                            if (cc == "Y")
                            {
                                letterDistList.Cc = 1;
                            }

                            string bcc = myReader["PC_LTR_DIST_BCC"].ToString();
                            if (bcc == "Y")
                            {
                                letterDistList.Bcc = 1;
                            }
                            //letterDistList.ProjectContactID = Guid.Empty;

                            int ltrId;
                            int.TryParse(myReader["PC_LTR_DIST_LTR_FK"].ToString(), out ltrId);
                            letterDistList.OldLetterID = ltrId;

                            //letterDistList.LetterID = Guid.Empty;
                            letterDistList.CompanyName = myReader["PC_LTR_DIST_PRJ_COMP_NAME"].ToString();
                            letterDistList.ContactName = myReader["PC_LTR_DIST_PRJ_CONT_NAME"].ToString();
                            letterDistList.ContactEmail = myReader["PC_LTR_DIST_PRJ_CONT_EMAIL"].ToString();

                            letterDistList.AC1 = "Migrated Data";

                            letterDistLists.Add(letterDistList);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }

                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetLetterDistributionListFromOldProcon :Getting Letter Distribution List completed ...", Color.Black, FontStyle.Bold);
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
            return letterDistLists;
        }

        public void InsertLetterDataTOIDBO(List<Letter> letters, List<LetterDistributionList> letterDistLists, SqlConnection sqlCon)
        {
            InsertLetterToIDBO(letters, sqlCon);
            InsertLetterDistributionListToIDBO(letterDistLists, sqlCon);
        }

        public void InsertLetterToIDBO(List<Letter> letters, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertLetterToIDBO : Restoring Letter started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var letter in letters)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryLetter = string.Format("INSERT INTO [PMLetters] ( [ID],[ProjectID],[RecipientID],[SenderID],[ProjectControlID],[ProjectAreaID]," +
                                                "[Description],[Regarding],[Greeting],[Enclosure],[Closing],[Body],[SerialNumber],[ProjectMode],[Date]," +
                                                "[OldID],[OldProjectID],[OldRecipientID],[OldSenderID],[OldProjectControlID],[OldProjectAreaID])" +
                                                " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12},{13},'{14}',{15},'{16}','{17}','{18}',{19},{20})", letter.ID
                                                , letter.ProjectID, letter.RecipientID, letter.SenderID, letter.ProjectControlID, letter.ProjectAreaID, (letter.Description!=null)?letter.Description.Replace("'", "''"):"", (letter.Regarding!=null)?letter.Regarding.Replace("'", "''"):"", (letter.Greeting!=null)?letter.Greeting.Replace("'", "''"):"",
                                                (letter.Enclosure!=null)?letter.Enclosure.Replace("'", "''"):"", (letter.Closing!=null)?letter.Closing.Replace("'", "''"):"", (letter.Body!=null)?letter.Body.Replace("'", "''"):"", letter.SerialNumber, letter.ProjectMode, letter.Date,
                                                letter.OldID, letter.OldProjectID, letter.OldRecipientID, letter.OldSenderID, letter.OldProjectControlID, letter.OldProjectAreaID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryLetter;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN Letter INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertLetterToIDBO : Restoring Letter completed  --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertLetterDistributionListToIDBO(List<LetterDistributionList> letterDists, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertLetterDistributionListToIDBO : Restoring DistributionList  started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var letterDist in letterDists)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryLetterDist = string.Format("INSERT INTO [PMLetterDistributionList] ([ID],[LetterID],[ProjectContactID],[To],[BCC],[CC]," +
                        "[OldID],[OldLetterID],[OldProjectContactID],[OldContactName],[OldContactEmail],[OldCompanyName]) VALUES " +
                        "('{0}','{1}','{2}',{3},{4},{5},'{6}',{7},'{8}','{9}','{10}','{11}')", letterDist.ID, letterDist.LetterID, letterDist.ProjectContactID,
                        letterDist.To, letterDist.Bcc, letterDist.Cc, letterDist.OldID, letterDist.OldLetterID, letterDist.OldProjectContactID, letterDist.ContactName.Replace("'", "''"), letterDist.ContactEmail.Replace("'", "''"), letterDist.CompanyName.Replace("'", "''"));
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryLetterDist;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN Letter DISTIBUTION INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertLetterDistributionListToIDBO : Restoring DistributionList  completed  --------------------", Color.Black, FontStyle.Bold);
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
        public List<Letter> ProjectAreaControlMapping(List<Letter> unmappedLetters, SqlConnection sqlCon)
        {
            List<Letter> mappedLetters = new List<Letter>();
            //mappedLetters.AddRange(unmappedLetters);
            try
            {
                
                //int counter = 1;
                foreach (var letter in unmappedLetters)
                {
                    try
                    {
                        sqlCon.Open();
                        SqlCommand cmd = new SqlCommand();
                        SqlDataReader myReader = null;
                        cmd.Connection = sqlCon;
                        Letter tempLetter = new Letter();
                        // we can get both projectareaid and projectcontrolid from FinProjectControl
                        string areaQry = String.Format("select ID from FinProjectArea fpa where OldID={0};", letter.OldProjectAreaID);

                        cmd.CommandText = areaQry;
                        myReader = cmd.ExecuteReader();
                        if (myReader != null && myReader.HasRows)
                        {
                            while (myReader.Read())
                            {
                                //Guid controlID;
                                //Guid.TryParse(myReader["ID"].ToString(), out controlID);
                                //letter.ProjectControlID = controlID;

                                Guid areaId;
                                if (myReader["ID"] == DBNull.Value)
                                    letter.ProjectAreaID = Guid.Empty;
                                else
                                {
                                    Guid.TryParse(myReader["ID"].ToString(), out areaId);
                                    letter.ProjectAreaID = areaId;
                                }
                            }
                        }
                        mappedLetters.Add(letter);
                        PMMigrationLogger.Log("Old Area -> New Area : " + letter.OldProjectAreaID + " -> " + letter.ProjectAreaID );
                        //PMMigrationLogger.Log("Old Control -> New Control : " + letter.OldProjectControlID + " -> " + letter.ProjectControlID);
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
                    finally
                    {
                        if (sqlCon.State == ConnectionState.Open)
                            sqlCon.Close();
                    }
                }
                unmappedLetters = mappedLetters;
                mappedLetters = new List<Letter>();
                foreach (var letter in unmappedLetters)
                {
                    try
                    {
                        sqlCon.Open();
                        SqlCommand cmd = new SqlCommand();
                        SqlDataReader myReader = null;
                        cmd.Connection = sqlCon;
                        Letter tempLetter = new Letter();
                        // we can get both projectareaid and projectcontrolid from FinProjectControl
                        string areaQry = String.Format("select ID from FinProjectControl  where OldID={0};", letter.OldProjectControlID);

                        cmd.CommandText = areaQry;
                        myReader = cmd.ExecuteReader();
                        if (myReader != null && myReader.HasRows)
                        {
                            while (myReader.Read())
                            {
                                Guid controlID;
                                if (myReader["ID"] == DBNull.Value)
                                    letter.ProjectControlID = Guid.Empty;
                                else
                                {
                                    Guid.TryParse(myReader["ID"].ToString(), out controlID);
                                    letter.ProjectControlID = controlID;
                                }

                                //Guid areaId;
                                //Guid.TryParse(myReader["ProjectAreaID"].ToString(), out areaId);
                                //letter.ProjectAreaID = areaId;
                            }
                        }
                        mappedLetters.Add(letter);
                        PMMigrationLogger.Log("Old Area -> New Area : " + letter.OldProjectAreaID + " -> " + letter.ProjectAreaID + " ##### Old Control -> New Control : " + letter.OldProjectControlID + " -> " + letter.ProjectControlID);
                        //PMMigrationLogger.Log("Old Control -> New Control : " + letter.OldProjectControlID + " -> " + letter.ProjectControlID);
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
                    finally
                    {
                        if (sqlCon.State == ConnectionState.Open)
                            sqlCon.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
           
            return mappedLetters;
        }
        #endregion
    }
}
