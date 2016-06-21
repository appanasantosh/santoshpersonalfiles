
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PMImportImplementation;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using PMImportImplementation.Entities;
using PMImportImplementation;
using System.Drawing;

namespace PMImportImplementation.Repository
{
    public class PunchListRepository
    {
        RestoreStatus restoreStatus = RestoreStatus.None;
        public int RestorePuchListData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestorePuchListData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("DATA RETRIEVAL STARTED ---------------------", Color.Black, FontStyle.Bold);
                List<PunchList> punchLists = new List<PunchList>();
                punchLists = GetPunchListDataFromOldProcon(projectID, mysqlCon);
                PMMigrationLogger.Log("Number of Punch Lists retrived are : " + punchLists.Count);
                List<PunchListItem> punchListItems = new List<PunchListItem>();
                if (restoreStatus == RestoreStatus.Success)
                {
                    punchListItems = GetPunchListItemDataFromOldProcon(punchLists, mysqlCon);
                    PMMigrationLogger.Log("Number of Punch Items retrived are : " + punchListItems.Count);
                }

                List<PunchListItemCompany> punchListItemCompanies = new List<PunchListItemCompany>();
                if (restoreStatus == RestoreStatus.Success)
                {
                    punchListItemCompanies = GetPunchListItemCompanyDataFromOldProcon(punchListItems, mysqlCon);
                    PMMigrationLogger.Log("Number of Punch Items Companies retrived are : " + punchListItemCompanies.Count);
                    PMMigrationLogger.Log("DATA RETRIEVAL COMPLETED ---------------------", Color.Black, FontStyle.Bold);
                }


                // If Data retrive successful then ony proceed to insert. 
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("DATA INSERTION INTO IDBO STARTED  ---------------------", Color.Black, FontStyle.Bold);
                    InsertPunchListDataToIDBO(punchLists, punchListItems, punchListItemCompanies, sqlCon);
                    PMMigrationLogger.Log("DATA INSERTION INTO IDBO COMPLETED  ---------------------", Color.Black, FontStyle.Bold);
                }

                //PMMigrationLogger.Log("Building Relationship ........." );

                // TODO :  Project Conatct RelationShip building pending since it depends on General teams migration scame.


                PMMigrationLogger.Log("Out RestorePuchListData");
            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                PMMigrationLogger.Log(ex.ToString(), Color.Red, FontStyle.Regular);
                throw;
            }
            return result;

        }


        #region Getting all punch list related Data
        public List<PunchList> GetPunchListDataFromOldProcon(Guid projectID, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetPunchListDataFromOldProcon : GETTING PUNCH LIST  STARTED ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            List<PunchList> punchLists = new List<PunchList>();


            try
            {
                //string.Format("Select * from pc_punch_list where pc_pun_lst_prj_fk = '{0}'", projectID);
                string query = string.Format("select * from pc_punch_list where pc_pun_lst_prj_fk IN (SELECT  PC_PRJ_ID FROM pc_project_list where PC_PRJ_PM_STATUS='Active' and PC_PRJ_IS_DELETED='N'  and " +
                                            "pc_project_list.PC_PRJ_CORP_FK not in(Select pc_corp_ID from procon_migration_db.pc_Corp where pc_corp_id not in(" +
                                            "Select  corp.pc_corp_ID FROM procon_migration_db.temp_Company join procon_migration_db.pc_company on temp_Company.ID = pc_company.PC_COMP_ID " +
                                            "join procon_migration_db.pc_corp corp on  (corp.PC_CORP_ID = pc_company.PC_COMP_CORP_FK )  where pc_company.PC_COMP_IS_HIDDEN = 'N' and pc_company.PC_COMP_IS_DEFAULT = 'Y'and corp.PC_CORP_NAME not like 'Synergy')))");
                string myQuery = query;

                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        PunchList punchList = new PunchList();

                        punchList.ID = Guid.NewGuid();
                        //PMMigrationLogger.Log("punchList.ID => " + punchList.ID.ToString());

                        int oldId;
                        int.TryParse(myReader["PC_PUN_LST_ID"].ToString(), out oldId);
                        punchList.OldID = oldId;
                        //PMMigrationLogger.Log("punchList.OldID => " + punchList.OldID.ToString());

                        Guid oldProjectId = Guid.Empty;
                        Guid.TryParse(myReader["PC_PUN_LST_PRJ_FK"].ToString(), out oldProjectId);
                        punchList.OldProjectID = oldProjectId;
                        //PMMigrationLogger.Log("punchList.OldProjectID => " + punchList.OldProjectID.ToString());

                        // We are considering we will keep the same project id in migrated project data.But still we need to have oldProjectID for migration
                        punchList.ProjectID = oldProjectId;


                        punchList.Title = myReader["PC_PUN_LST_TITLE"].ToString();
                        //PMMigrationLogger.Log("punchList.Title => " + punchList.Title);

                        int status;
                        int.TryParse(myReader["PC_PUN_LST_SEND_STAT"].ToString(), out status);
                        punchList.Status = status;
                        //PMMigrationLogger.Log("punchList.Status => " + punchList.Status.ToString());

                        int number;
                        int.TryParse(myReader["PC_PUN_LST_NO"].ToString(), out number);
                        punchList.Number = number;
                        //PMMigrationLogger.Log("punchList.Number => " + punchList.Number.ToString());

                        //						int inActive;
                        //						int.TryParse( myReader["PC_PUN_LST_IS_ACTIVE"].ToString(), out inActive);
                        //						punchList.IsActive = inActive;

                        punchList.Description = myReader["PC_PUN_LST_DESC"].ToString();
                        //PMMigrationLogger.Log("punchList.Description => " + punchList.Description);

                        if (myReader["PC_PUN_LST_DATE"] == DBNull.Value)
                            punchList.Date = null;

                        else
                        {
                            DateTime date;
                            DateTime.TryParse(myReader["PC_PUN_LST_DATE"].ToString(), out date);
                            punchList.Date = date;
                        }
                        //PMMigrationLogger.Log("punchList.Date => " + punchList.Date );
                        if (myReader["PC_PUN_LST_COMP_REQ_DATE"] == DBNull.Value)
                            punchList.RequiredDate = null;
                        else
                        {
                            DateTime requiredDate;
                            DateTime.TryParse(myReader["PC_PUN_LST_COMP_REQ_DATE"].ToString(), out requiredDate);
                            punchList.RequiredDate = requiredDate;
                        }
                        //PMMigrationLogger.Log("punchList.RequiredDate => " + punchList.RequiredDate);

                        if (myReader["PC_PUN_LST_ACT_COMP_DATE"] == DBNull.Value)
                            punchList.CompletionDate = null;
                        else
                        {
                            DateTime completionDate;
                            DateTime.TryParse(myReader["PC_PUN_LST_ACT_COMP_DATE"].ToString(), out completionDate);
                            punchList.CompletionDate = completionDate;
                        }
                        //PMMigrationLogger.Log("punchList.CompletionDate => " + punchList.CompletionDate );

                        //punchList.Created = DateTime.UtcNow;
                        punchList.AC1 = "Migrated Data";
                        //PMMigrationLogger.Log("punchList.AC1 => " + punchList.AC1 );
                        punchLists.Add(punchList);

                        //PMMigrationLogger.Log("PUNCHLIST["+ punchLists.Count +"] RETRIEVE SUCCESSFULLY..........................");

                    }
                }
                restoreStatus = RestoreStatus.Success;
                if (!myReader.IsClosed)
                {
                    myReader.Close();
                }
                PMMigrationLogger.Log("Out GetPunchListDataFromOldProcon : GETTING PUNCH LIST  ENDED ...", Color.Black, FontStyle.Bold);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (mySqlCmd.Connection != null || mySqlCmd.Connection.State == ConnectionState.Open)
                {
                    //if (!myReader.IsClosed)
                    //{
                    //    myReader.Close();
                    //}
                    mySqlCmd.Connection.Close();

                }
            }
            return punchLists;

        }

        public List<PunchListItem> GetPunchListItemDataFromOldProcon(List<PunchList> punchLists, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetPunchListItemDataFromOldProcon : GETTING PUNCH LIST ITEM STARTED ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            List<PunchListItem> punchListItems = new List<PunchListItem>();

            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var punchList in punchLists)
                {
                    string myQuery = string.Format("Select * from pc_punch_items where pc_pun_itm_pl_fk = {0}", punchList.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();
                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            PunchListItem PunchListItem = new PunchListItem();
                            PunchListItem.ID = Guid.NewGuid();
                            //PMMigrationLogger.Log("PunchListItem.ID => " + PunchListItem.ID.ToString());


                            int oldId;
                            int.TryParse(myReader["PC_PUN_ITM_ID"].ToString(), out oldId);
                            PunchListItem.OldID = oldId;
                            //PMMigrationLogger.Log("PunchListItem.OldID => " + PunchListItem.OldID.ToString());

                            int oldPunchListId;
                            int.TryParse(myReader["PC_PUN_ITM_PL_FK"].ToString(), out oldPunchListId);
                            PunchListItem.OldPunchlistId = oldPunchListId;
                            //PMMigrationLogger.Log("PunchListItem.OldPunchlistId => " + PunchListItem.OldPunchlistId.ToString());

                            //Building relation
                            PunchListItem.PunchListID = punchList.ID;

                            if (myReader["PC_PUN_ITM_COMP_DATE"] == DBNull.Value)
                                PunchListItem.CompletionDate = null;

                            else
                            {
                                DateTime completionDate;
                                DateTime.TryParse(myReader["PC_PUN_ITM_COMP_DATE"].ToString(), out completionDate);
                                PunchListItem.CompletionDate = completionDate;
                            }
                            //PMMigrationLogger.Log("PunchListItem.CompletionDate => " + PunchListItem.CompletionDate.ToString() );

                            int completionMark;
                            int.TryParse(myReader["PC_PUN_ITM_COMP_MARK"].ToString(), out completionMark);
                            PunchListItem.CompletionMark = completionMark;
                            //PMMigrationLogger.Log("PunchListItem.CompletionMark => " + PunchListItem.CompletionMark.ToString() );

                            PunchListItem.Description = myReader["PC_PUN_ITM_DESC"].ToString();
                            //PMMigrationLogger.Log("PunchListItem.Description => " + PunchListItem.Description);

                            PunchListItem.Location = myReader["PC_PUN_ITM_LOC"].ToString();
                            //PMMigrationLogger.Log("PunchListItem.Location" + PunchListItem.Location );

                            int number;
                            int.TryParse(myReader["PC_PUN_ITM_NO"].ToString(), out number);
                            PunchListItem.Number = number;
                            //PMMigrationLogger.Log("PunchListItem.Number => " + PunchListItem.Number.ToString() );

                            //int isActive;
                            //int.TryParse(myReader["PC_PUN_ITM_IS_ACTIVE"].ToString(), out isActive);
                            // In old procon table the value for this field is null for all records. so I just set it as 0.
                            //PunchListItem.IsActive = 0;


                            PunchListItem.AC1 = "Migrated Data";
                            //PMMigrationLogger.Log("PunchListItem.AC1 => " + PunchListItem.AC1 );

                            //PunchListItem.Created = DateTime.UtcNow;
                            //PMMigrationLogger.Log("PunchListItem.Created" + PunchListItem.Created.ToString() );


                            punchListItems.Add(PunchListItem);
                            //PMMigrationLogger.Log("PUNCHLISTITEM["+ punchListItems.Count+"] RETRIEVE SUCCESSFULLY ................................");
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }

                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetPunchListItemDataFromOldProcon : GETTING PUNCH LIST ITEM ENDED ...", Color.Black, FontStyle.Bold);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (mySqlCmd.Connection != null || mySqlCmd.Connection.State == ConnectionState.Open)
                {
                    //if (!myReader.IsClosed)
                    //{
                    //    myReader.Close();
                    //}
                    mySqlCmd.Connection.Close();


                }
            }
            return punchListItems;
        }

        public List<PunchListItemCompany> GetPunchListItemCompanyDataFromOldProcon(List<PunchListItem> punchListItems, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetPunchListItemCompanyDataFromOldProcon : GETTING PUNCH LIST ITEM COMPANY STARTED ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            List<PunchListItemCompany> punchListItemCompanies = new List<PunchListItemCompany>();
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var punchListItem in punchListItems)
                {
                    string myQuery = string.Format("Select * from pc_comp_responsible where PC_COMP_RESP_PI_FK = {0}", punchListItem.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();
                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            PunchListItemCompany punchListItemCompany = new PunchListItemCompany();
                            punchListItemCompany.ID = Guid.NewGuid();
                            //PMMigrationLogger.Log("punchListItemCompany.ID => " + punchListItemCompany.ID.ToString());

                            int oldId;
                            int.TryParse(myReader["PC_COMP_RESP_ID"].ToString(), out oldId);
                            punchListItemCompany.OldID = oldId;
                            //PMMigrationLogger.Log("punchListItemCompany.OldID => " + punchListItemCompany.OldID.ToString() );

                            int oldPunchItemId;
                            int.TryParse(myReader["PC_COMP_RESP_PI_FK"].ToString(), out oldPunchItemId);
                            punchListItemCompany.OldPunchItemID = oldPunchItemId;
                            //PMMigrationLogger.Log("punchListItemCompany.OldPunchItemID => " + punchListItemCompany.OldPunchItemID.ToString() );

                            //Guid oldProjectContactId;
                            //Guid.TryParse(myReader["PC_COMP_RESP_CONT_FK"].ToString(), out oldProjectContactId);
                            // Some project contact are not Guid in old table, they are numbers. so I take this field's datatype  as string in entity.
                            punchListItemCompany.OldProjectContactID = myReader["PC_COMP_RESP_CONT_FK"].ToString();
                            //PMMigrationLogger.Log("punchListItemCompany.OldProjectContactID => " + punchListItemCompany.OldProjectContactID.ToString());

                            // since genera team is keeping same project contact id, we are mapping to the same
                            Guid projectContactId;
                            Guid.TryParse(myReader["PC_COMP_RESP_CONT_FK"].ToString(), out projectContactId);
                            punchListItemCompany.ProjectContactID = projectContactId;


                            //Building Relations
                            punchListItemCompany.PunchItemID = punchListItem.ID;
                            // project contact relation need to done later using project contact table

                            string completed = myReader["PC_COMP_RESP_COMP"].ToString();
                            if (completed == "Y")
                            {
                                punchListItemCompany.Completed = 1; // Swarnalata confirms Y means 1
                            }
                            if (completed == "N")
                            {
                                punchListItemCompany.Completed = 0;
                            }
                            //PMMigrationLogger.Log("punchListItemCompany.Completed => " + punchListItemCompany.Completed.ToString() );

                            if (myReader["PC_COMP_RESP_COMP_DATE"] == DBNull.Value)
                                punchListItemCompany.CompletionDate = null;
                            else
                            {
                                DateTime completionDate;
                                DateTime.TryParse(myReader["PC_COMP_RESP_COMP_DATE"].ToString(), out completionDate);
                                punchListItemCompany.CompletionDate = completionDate;
                            }
                            //PMMigrationLogger.Log("punchListItemCompany.CompletionDate => " + punchListItemCompany.CompletionDate.ToString());

                            punchListItemCompany.AC1 = "Migrated Data";
                            //PMMigrationLogger.Log("punchListItemCompany.AC1 => " + punchListItemCompany.AC1);

                            //punchListItemCompany.Created = DateTime.UtcNow;

                            punchListItemCompanies.Add(punchListItemCompany);
                            //PMMigrationLogger.Log("PUNCHLISTITEMCOMPANY["+ punchListItemCompanies.Count+"] RETRIEVE SUCCESSFULLY ....................");

                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }

                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetPunchListItemCompanyDataFromOldProcon : GETTING PUNCH LIST ITEM COMPANY ENDED ...", Color.Black, FontStyle.Bold);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (mySqlCmd.Connection != null || mySqlCmd.Connection.State == ConnectionState.Open)
                {
                    //if (!myReader.IsClosed)
                    //{
                    //    myReader.Close();
                    //}
                    mySqlCmd.Connection.Close();

                }
            }

            return punchListItemCompanies;
        }
        #endregion

        #region Inserting all punch list related Data
        public void InsertPunchListDataToIDBO(List<PunchList> punchLists, List<PunchListItem> punchItems, List<PunchListItemCompany> punchListItemCompanies, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertPunchListDataToIDBO");
            try
            {
                InsertPunchListToIDBO(punchLists, sqlCon);
                InsertPunchItemsToIDBO(punchItems, sqlCon);
                InsertPunchItemCompaniesToIDBO(punchListItemCompanies, sqlCon);
                //if (punchLists.Count != 0)
                //{
                //    InsertPunchListToIDBO(punchLists, sqlCon);
                //}
                //if (punchItems.Count != 0)
                //{
                //    InsertPunchItemsToIDBO(punchItems, sqlCon);
                //}
                //if (punchListItemCompanies.Count != 0)
                //{
                //    InsertPunchItemCompaniesToIDBO(punchListItemCompanies, sqlCon);
                //}

                PMMigrationLogger.Log("Out InsertPunchListDataToIDBO");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void InsertPunchListToIDBO(List<PunchList> punchLists, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertPunchListToIDBO : RESTORING PUNCHLIST STARTED --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var punchList in punchLists)
                {
                    try
                    {

                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;

                        string qryPunchList = string.Format("INSERT INTO PMPunchList(Id,ProjectID,CompletionDate,RequiredDate,Date,PunchListDescription,Status,Title,Number,AC1,OldID,OldProjectID) " +
                                                            "VALUES ('{0}','{1}',{2},{3},{4},'{5}',{6},'{7}',{8},'{9}',{10},'{11}')", punchList.ID,
                                                            punchList.ProjectID, (punchList.CompletionDate == null) ? (object)"null" : "'" + punchList.CompletionDate + "'", (punchList.RequiredDate == null) ? (object)"null" : "'" + punchList.RequiredDate + "'", (punchList.Date == null) ? (object)"null" : "'" + punchList.Date + "'", punchList.Description.Replace("'", "''"),
                                                            punchList.Status, punchList.Title.Replace("'", "''"), punchList.Number, punchList.AC1.Replace("'", "''"), punchList.OldID, punchList.OldProjectID);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryPunchList;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();


                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN PUNCH LIST INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertPunchListToIDBO : RESTORING PUNCHLIST COMPLETED --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertPunchItemsToIDBO(List<PunchListItem> punchItems, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertPunchItemsToIDBO : RESTORING PUNCHLIST ITEM STARTED --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var punchItem in punchItems)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;

                        string qryPunchList = string.Format("INSERT INTO PMPunchListItems(ID,PunchListID,CompletionDate,Description,Location,Number,CompletionMark,AC1,OldID,OldPunchListID) " +
                                                            "VALUES ('{0}','{1}',{2},'{3}','{4}',{5},{6},'{7}',{8},{9})", punchItem.ID,
                                                            punchItem.PunchListID, (punchItem.CompletionDate == null) ? (object)"null" : "'" + punchItem.CompletionDate + "'", punchItem.Description.Replace("'", "''"), punchItem.Location.Replace("'", "''"), punchItem.Number,
                                                            punchItem.CompletionMark, punchItem.AC1.Replace("'", "''"), punchItem.OldID, punchItem.OldPunchlistId);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryPunchList;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN PUNCH ITEM INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertPunchItemsToIDBO : RESTORING PUNCHLIST ITEM COMPLETED --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertPunchItemCompaniesToIDBO(List<PunchListItemCompany> punchItemCompanies, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertPunchItemCompaniesToIDBO : RESTORING PUNCHLIST ITEM  COMPANIES STARTED --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var punchItemCompany in punchItemCompanies)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;

                        string qryPunchList = string.Format("INSERT INTO PMPunchListItemCompany(ID,PunchItemID,ProjectContactID,CompletionDate,Completed,AC1,OldID,OldPunchItemID,OldProjectContactID) " +
                                                            "VALUES ('{0}','{1}','{2}',{3},{4},'{5}',{6},{7},'{8}' )", punchItemCompany.ID,
                                                            punchItemCompany.PunchItemID, punchItemCompany.ProjectContactID, (punchItemCompany.CompletionDate == null) ? (object)"null" : "'" + punchItemCompany.CompletionDate + "'", punchItemCompany.Completed, punchItemCompany.AC1.Replace("'", "''"),
                                                            punchItemCompany.OldID, punchItemCompany.OldPunchItemID, punchItemCompany.OldProjectContactID);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryPunchList;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN PUNCH ITEM COMPANIES INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }

                PMMigrationLogger.Log("Out InsertPunchItemCompaniesToIDBO : RESTORING PUNCHLIST ITEM  COMPANIES COMPLETED --------------------", Color.Black, FontStyle.Bold);
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

        #region Making relation

        #endregion
    }
}
