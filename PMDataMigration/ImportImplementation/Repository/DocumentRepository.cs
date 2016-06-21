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

     
  public  class DocumentRepository
    {
      RestoreStatus restoreStatus = RestoreStatus.None;

      public int RestoreDocumentData(MySqlConnection mysqlCon, SqlConnection sqlCon)
      {
          PMMigrationLogger.Log("In RestoreDocumentData");
          int result = 0;
          try
          {
              //PMMigrationLogger.Log("Data retrival started ---------------------", Color.Black, FontStyle.Bold);
              List<Documents> documents = new List<Documents>();
              documents = GetDocDoumetsFromOldProcon(mysqlCon);
              //PMMigrationLogger.Log("Number of Document  Data retrieve : " + documents.Count);


              List<Folders> folders = new List<Folders>();
              folders = GetDocFoldersFromOldProcon(mysqlCon);
              PMMigrationLogger.Log("Number of Folders  Data retrieve : " + folders.Count);

             
              if (restoreStatus == RestoreStatus.Success)
              {
                 // PMMigrationLogger.Log("Document Data restore started ................", Color.Black, FontStyle.Bold);
                  InsertDocumentToIDBO(documents, sqlCon);
                 // PMMigrationLogger.Log("Document Data restore completed ................", Color.Black, FontStyle.Bold);

                 // PMMigrationLogger.Log("Folder Data restore started ................", Color.Black, FontStyle.Bold);
                  InsertFoldersToIDBO(folders, sqlCon);
                 // PMMigrationLogger.Log("Folder Data restore completed ................", Color.Black, FontStyle.Bold);

              }

          }
          catch (Exception ex)
          {
              PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
              PMMigrationLogger.Log(ex.ToString(), Color.Red, FontStyle.Regular);
              throw;
          }
         PMMigrationLogger.Log("Out RestoreDocumentData");
          return result;
      }

      public List<Documents> GetDocDoumetsFromOldProcon(MySqlConnection mySqlCon)
      {
         PMMigrationLogger.Log("In GetFieldReportDataFromOldProcon : Getting Field Report Data started ...", Color.Black, FontStyle.Bold);
          List<Documents> documents = new List<Documents>();
          MySqlCommand mySqlCmd = new MySqlCommand();
          MySqlDataReader myReader = null;
          try
          {
              string myGetTransmittslQuery = String.Format("select * from pc_documents where pc_doc_prj_fk in(SELECT  PC_PRJ_ID  FROM procon_migration_db.pc_project_list WHERE( (pc_prj_pm_status='Active') or (pc_prj_est_status = 'bidding' and  pc_prj_est_year >= 14) and PC_PRJ_CORP_FK not in ('37f561be-6613-4ff2-9d96-6e887e561b48','e65ec224-c0e1-45e9-8636-0b1632c1eeb5')) AND PC_PRJ_IS_DELETED = 'N' AND ((pc_project_list.PC_PRJ_CORP_FK IN   (Select pc_corp_ID from procon_migration_db.pc_Corp where pc_corp_id in( Select  corp.pc_corp_ID FROM procon_migration_db.tempcompany join procon_migration_db.pc_company on tempcompany.ID = pc_company.PC_COMP_ID join procon_migration_db.pc_corp corp on  (corp.PC_CORP_ID = pc_company.PC_COMP_CORP_FK )  where pc_company.PC_COMP_IS_HIDDEN = 'N' and pc_company.PC_COMP_IS_DEFAULT = 'Y'  and corp.PC_CORP_NAME not like 'Synergy')))or pc_project_list.PC_PRJ_CORP_FK IN('37f561be-6613-4ff2-9d96-6e887e561b48','e65ec224-c0e1-45e9-8636-0b1632c1eeb5')))");
              mySqlCon.Open();
              mySqlCmd.Connection = mySqlCon;
              mySqlCmd.CommandText = myGetTransmittslQuery;
              myReader = mySqlCmd.ExecuteReader();
              if (myReader != null && myReader.HasRows)
              {
                  while (myReader.Read())
                  {
                      Documents document = new Documents();
                      document.PC_DOC_ID = myReader["PC_DOC_ID"].ToString();
                      document.PC_DOC_PRJ_FK = myReader["PC_DOC_PRJ_FK"].ToString();
                      document.PC_DOC_ASG_TYPE = myReader["PC_DOC_ASG_TYPE"].ToString();

                      DateTime Date;
                      if (myReader["PC_DOC_REC_DATE"] == DBNull.Value)
                      {
                          document.PC_DOC_REC_DATE = null;
                      }
                      else
                      {
                          DateTime.TryParse(myReader["PC_DOC_REC_DATE"].ToString(), out Date);
                          document.PC_DOC_REC_DATE = Date;
                      }

                      document.PC_DOC_DESC = myReader["PC_DOC_DESC"].ToString();
                      document.PC_DOC_TYPE = myReader["PC_DOC_TYPE"].ToString();
                      document.PC_DOC_TYPE_ID = myReader["PC_DOC_TYPE_ID"].ToString();
                      document.PC_DOC_TYPE_DESC = myReader["PC_DOC_TYPE_DESC"].ToString();
                      document.PC_DOC_PATH = myReader["PC_DOC_PATH"].ToString();
                      document.PC_DOC_IS_ACTIVE = myReader["PC_DOC_IS_ACTIVE"].ToString();
                      document.PC_DOC_CATEGORY = myReader["PC_DOC_CATEGORY"].ToString();

                      DateTime Date1;
                      if (myReader["PC_DOC_DOC_DATE"] == DBNull.Value)
                      {
                          document.PC_DOC_DOC_DATE = null;
                      }
                      else
                      {
                          DateTime.TryParse(myReader["PC_DOC_DOC_DATE"].ToString(), out Date1);
                          document.PC_DOC_DOC_DATE = Date1;
                      }
                      document.PC_DOC_CNTR_DOC = myReader["PC_DOC_CNTR_DOC"].ToString();

                      document.PC_DOC_REFERENCE = myReader["PC_DOC_REFERENCE"].ToString();


                      DateTime Date2;
                      if (myReader["PC_DOC_REV_DOC_DATE"] == DBNull.Value)
                      {
                          document.PC_DOC_REV_DOC_DATE = null;
                      }
                      else
                      {
                          DateTime.TryParse(myReader["PC_DOC_REV_DOC_DATE"].ToString(), out Date2);
                          document.PC_DOC_REV_DOC_DATE = Date2;
                      }

                      document.PC_DOC_SHEETS_PAGES = myReader["PC_DOC_SHEETS_PAGES"].ToString();

                      document.PC_DOC_FOLDER_PATH = myReader["PC_DOC_FOLDER_PATH"].ToString();

                      document.PC_DOC_CONTACT_FK = myReader["PC_DOC_CONTACT_FK"].ToString();

                      int cntrlFk;
                      int.TryParse(myReader["PC_DOC_CONTROL_FK"].ToString(), out cntrlFk);
                      document.PC_DOC_CONTROL_FK = cntrlFk;

                      int prjMode;
                      int.TryParse(myReader["PC_DOC_PRJ_MODE"].ToString(), out prjMode);
                      document.PC_DOC_PRJ_MODE = prjMode;

                      document.PC_DOC_IS_LOCKED = myReader["PC_DOC_IS_LOCKED"].ToString();

                      document.PC_DOC_FLDR_WEB_MARK = myReader["PC_DOC_FLDR_WEB_MARK"].ToString();


                      int nodeID;
                      int.TryParse(myReader["PC_DOC_FLDR_NODE_ID"].ToString(), out nodeID);
                      document.PC_DOC_FLDR_NODE_ID = nodeID;

                      int parentID;
                      int.TryParse(myReader["PC_DOC_FLDR_PARENT_ID"].ToString(), out parentID);
                      document.PC_DOC_FLDR_PARENT_ID = parentID;

                      int rootID;
                      int.TryParse(myReader["PC_DOC_FLDR_ROOT_ID"].ToString(), out rootID);
                      document.PC_DOC_FLDR_ROOT_ID = rootID;





                      documents.Add(document);
                  }
              }
              if (!myReader.IsClosed)
              {
                  myReader.Close();
              }
              restoreStatus = RestoreStatus.Success;
             PMMigrationLogger.Log("Out GetFieldReportDataFromOldProcon : Getting Field Report Data completed ...", Color.Black, FontStyle.Bold);
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
          return documents;
      }

      public void InsertDocumentToIDBO(List<Documents> documentList, SqlConnection sqlCon)
      {
         PMMigrationLogger.Log("In InsertDocumentToIDBO : Restoring  started  --------------------", Color.Black, FontStyle.Bold);
          try
          {
              sqlCon.Open();
              int counter = 1;
              foreach (var document in documentList)
              {
                  try
                  {
                      SqlCommand cmd = new SqlCommand();
                      cmd.Connection = sqlCon;

                      if (document.PC_DOC_REFERENCE.Contains("'"))
                          document.PC_DOC_REFERENCE = document.PC_DOC_REFERENCE.Replace("'", "''");

                      if (document.PC_DOC_ASG_TYPE.Contains("'"))
                          document.PC_DOC_ASG_TYPE = document.PC_DOC_ASG_TYPE.Replace("'", "''");

                      if (document.PC_DOC_DESC.Contains("'"))
                          document.PC_DOC_DESC = document.PC_DOC_DESC.Replace("'", "''");

                      if (document.PC_DOC_TYPE.Contains("'"))
                          document.PC_DOC_TYPE = document.PC_DOC_TYPE.Replace("'", "''");

                      if (document.PC_DOC_TYPE_DESC.Contains("'"))
                          document.PC_DOC_TYPE_DESC = document.PC_DOC_TYPE_DESC.Replace("'", "''");

                      if (document.PC_DOC_PATH.Contains("'"))
                          document.PC_DOC_PATH = document.PC_DOC_PATH.Replace("'", "''");

                      if (document.PC_DOC_CATEGORY.Contains("'"))
                          document.PC_DOC_CATEGORY = document.PC_DOC_CATEGORY.Replace("'", "''");


                      if (document.PC_DOC_SHEETS_PAGES.Contains("'"))
                          document.PC_DOC_SHEETS_PAGES = document.PC_DOC_SHEETS_PAGES.Replace("'", "''");

                      if (document.PC_DOC_FOLDER_PATH.Contains("'"))
                          document.PC_DOC_FOLDER_PATH = document.PC_DOC_FOLDER_PATH.Replace("'", "''");


                      string qryDocument = string.Format("INSERT INTO [dbo].[DocOld_pc_documents] ([PC_DOC_ID],[PC_DOC_PRJ_FK],[PC_DOC_ASG_TYPE],[PC_DOC_REC_DATE],[PC_DOC_DESC],[PC_DOC_TYPE],[PC_DOC_TYPE_ID],[PC_DOC_TYPE_DESC],[PC_DOC_PATH],[PC_DOC_IS_ACTIVE],[PC_DOC_CATEGORY],[PC_DOC_DOC_DATE],[PC_DOC_CNTR_DOC],[PC_DOC_REFERENCE],[PC_DOC_REV_DOC_DATE],[PC_DOC_SHEETS_PAGES],[PC_DOC_FOLDER_PATH],[PC_DOC_CONTACT_FK],[PC_DOC_CONTROL_FK],[PC_DOC_PRJ_MODE],[PC_DOC_IS_LOCKED],[PC_DOC_FLDR_NODE_ID],[PC_DOC_FLDR_PARENT_ID],[PC_DOC_FLDR_ROOT_ID],[PC_DOC_FLDR_WEB_MARK]) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', {18}, {19}, '{20}', {21}, {22}, {23}, '{24}')",
                          document.PC_DOC_ID, document.PC_DOC_PRJ_FK, document.PC_DOC_ASG_TYPE, document.PC_DOC_REC_DATE, document.PC_DOC_DESC, document.PC_DOC_TYPE, document.PC_DOC_TYPE_ID, document.PC_DOC_TYPE_DESC, document.PC_DOC_PATH, document.PC_DOC_IS_ACTIVE, document.PC_DOC_CATEGORY, document.PC_DOC_DOC_DATE, document.PC_DOC_CNTR_DOC, document.PC_DOC_REFERENCE, document.PC_DOC_REV_DOC_DATE, document.PC_DOC_SHEETS_PAGES, document.PC_DOC_FOLDER_PATH, document.PC_DOC_CONTACT_FK,  document.PC_DOC_CONTROL_FK, document.PC_DOC_PRJ_MODE, document.PC_DOC_IS_LOCKED, document.PC_DOC_FLDR_NODE_ID, document.PC_DOC_FLDR_PARENT_ID, document.PC_DOC_FLDR_ROOT_ID, document.PC_DOC_FLDR_WEB_MARK);
                      cmd.CommandType = CommandType.Text;
                      cmd.CommandText = qryDocument;


                     PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                      cmd.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                      // Log the error and don't throw since we want to continue with execution.
                     PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                     PMMigrationLogger.Log("ERROR IN Document INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                      continue;
                  }
              }
              PMMigrationLogger.Log("Out InsertDocumentToIDBO : Restoring Document completed  --------------------", Color.Black, FontStyle.Bold);
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


      public List<Folders> GetDocFoldersFromOldProcon(MySqlConnection mySqlCon)
      {
         PMMigrationLogger.Log("In GetDocFoldersFromOldProcon : Getting DocFolders From OldProcon started ...", Color.Black, FontStyle.Bold);
          List<Folders> folders = new List<Folders>();
          MySqlCommand mySqlCmd = new MySqlCommand();
          MySqlDataReader myReader = null;
          try
          {
             string myGetTransmittslQuery = String.Format("select * from pc_doc_folders where PC_DOC_FLDR_PRJ_ID in (SELECT  PC_PRJ_ID  FROM procon_migration_db.pc_project_list WHERE( (pc_prj_pm_status='Active') or (pc_prj_est_status = 'bidding' and  pc_prj_est_year >= 14) and PC_PRJ_CORP_FK not in ('37f561be-6613-4ff2-9d96-6e887e561b48','e65ec224-c0e1-45e9-8636-0b1632c1eeb5')) AND PC_PRJ_IS_DELETED = 'N' AND ((pc_project_list.PC_PRJ_CORP_FK IN   (Select pc_corp_ID from procon_migration_db.pc_Corp where pc_corp_id in( Select  corp.pc_corp_ID FROM procon_migration_db.tempcompany join procon_migration_db.pc_company on tempcompany.ID = pc_company.PC_COMP_ID join procon_migration_db.pc_corp corp on  (corp.PC_CORP_ID = pc_company.PC_COMP_CORP_FK )  where pc_company.PC_COMP_IS_HIDDEN = 'N' and pc_company.PC_COMP_IS_DEFAULT = 'Y'  and corp.PC_CORP_NAME not like 'Synergy')))or pc_project_list.PC_PRJ_CORP_FK IN('37f561be-6613-4ff2-9d96-6e887e561b48','e65ec224-c0e1-45e9-8636-0b1632c1eeb5')))");
            // string myGetTransmittslQuery = String.Format("select * from pc_doc_folders where PC_DOC_FLDR_PRJ_ID is null  and PC_DOC_FLDR_ID > 35");
              mySqlCon.Open();
              mySqlCmd.Connection = mySqlCon;
              mySqlCmd.CommandText = myGetTransmittslQuery;
              myReader = mySqlCmd.ExecuteReader();
              if (myReader != null && myReader.HasRows)
              {
                  while (myReader.Read())
                  {
                      Folders folder = new Folders();

                      int folderID;
                      int.TryParse(myReader["PC_DOC_FLDR_ID"].ToString(), out folderID);
                      folder.PC_DOC_FLDR_ID = folderID;

                      folder.PC_DOC_FLDR_NAME = myReader["PC_DOC_FLDR_NAME"].ToString();
                      folder.PC_DOC_FLDR_USER_ID = myReader["PC_DOC_FLDR_USER_ID"].ToString();

                      int nodeID;
                      int.TryParse(myReader["PC_DOC_FLDR_NODE_ID"].ToString(), out nodeID);
                      folder.PC_DOC_FLDR_NODE_ID = nodeID;

                      int parentID;
                      int.TryParse(myReader["PC_DOC_FLDR_PARENT_ID"].ToString(), out parentID);
                      folder.PC_DOC_FLDR_PARENT_ID = parentID;

                      folder.PC_DOC_FLDR_PRJ_ID = myReader["PC_DOC_FLDR_PRJ_ID"].ToString();
                      folder.PC_DOC_FLDR_TYPE_ID = myReader["PC_DOC_FLDR_TYPE_ID"].ToString();

                      int projectMode;
                      int.TryParse(myReader["PC_DOC_FLDR_PRJ_MODE"].ToString(), out projectMode);
                      folder.PC_DOC_FLDR_PRJ_MODE = projectMode;

                      folder.PC_DOC_FLDR_IS_LOCKED = myReader["PC_DOC_FLDR_IS_LOCKED"].ToString();
                      folder.PC_DOC_FLDR_WEB_MARK = myReader["PC_DOC_FLDR_WEB_MARK"].ToString();

                      folders.Add(folder);
                  }
              }
              if (!myReader.IsClosed)
              {
                  myReader.Close();
              }
              restoreStatus = RestoreStatus.Success;
             PMMigrationLogger.Log("Out GetDocFoldersFromOldProcon : Getting DocFolders From OldProcon Data completed ...", Color.Black, FontStyle.Bold);
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
          return folders;
      }

      public void InsertFoldersToIDBO(List<Folders> folderList, SqlConnection sqlCon)
      {
          PMMigrationLogger.Log("In InsertDocumentToIDBO : Restoring  started  --------------------", Color.Black, FontStyle.Bold);
          try
          {
              sqlCon.Open();
              int counter = 1;
              foreach (var folder in folderList)
              {
                  try
                  {
                      SqlCommand cmd = new SqlCommand();
                      cmd.Connection = sqlCon;

                      if (folder.PC_DOC_FLDR_NAME.Contains("'"))
                          folder.PC_DOC_FLDR_NAME = folder.PC_DOC_FLDR_NAME.Replace("'", "''");

                      string qryDocument = string.Format("INSERT INTO [dbo].[DocOld_pc_doc_folders]( PC_DOC_FLDR_ID,PC_DOC_FLDR_NAME,PC_DOC_FLDR_USER_ID,PC_DOC_FLDR_NODE_ID,PC_DOC_FLDR_PARENT_ID,PC_DOC_FLDR_PRJ_ID,PC_DOC_FLDR_TYPE_ID,PC_DOC_FLDR_PRJ_MODE,PC_DOC_FLDR_IS_LOCKED,PC_DOC_FLDR_WEB_MARK) VALUES({0}, '{1}', '{2}', {3}, {4}, '{5}', '{6}', {7}, '{8}', '{9}')",
                         folder.PC_DOC_FLDR_ID, folder.PC_DOC_FLDR_NAME, folder.PC_DOC_FLDR_USER_ID, folder.PC_DOC_FLDR_NODE_ID, folder.PC_DOC_FLDR_PARENT_ID, folder.PC_DOC_FLDR_PRJ_ID, folder.PC_DOC_FLDR_TYPE_ID, folder.PC_DOC_FLDR_PRJ_MODE, folder.PC_DOC_FLDR_IS_LOCKED, folder.PC_DOC_FLDR_WEB_MARK);
                      cmd.CommandType = CommandType.Text;
                      cmd.CommandText = qryDocument;


                      PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                      cmd.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                      // Log the error and don't throw since we want to continue with execution.
                    PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                     PMMigrationLogger.Log("ERROR IN Document INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                      continue;
                  }
              }
              PMMigrationLogger.Log("Out InsertDocumentToIDBO : Restoring Document completed  --------------------", Color.Black, FontStyle.Bold);
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
