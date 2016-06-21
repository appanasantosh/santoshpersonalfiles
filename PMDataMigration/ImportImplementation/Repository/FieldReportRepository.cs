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
    public class FieldReportRepository
    {

        RestoreStatus restoreStatus = RestoreStatus.None;

        public int RestoreFieldReportData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreFieldReportData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("Data retrival started ---------------------", Color.Black, FontStyle.Bold);
                List<FieldReport> fieldReports = new List<FieldReport>();
                fieldReports = GetFieldReportDataFromOldProcon(projectID, mysqlCon);
                List<FieldReportSubVendors> fieldReportSubvendorList = null;
                PMMigrationLogger.Log("Number of Field Report  Data retrieve : " + fieldReports.Count);

                if (restoreStatus == RestoreStatus.Success)
                {
                    fieldReportSubvendorList = new List<FieldReportSubVendors>();
                    fieldReportSubvendorList = GetFieldReportSubVendorsFromOldProcon(fieldReports, mysqlCon);
                    PMMigrationLogger.Log("Number of Field Report Subvendors retrieve : " + fieldReportSubvendorList.Count);
                }
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("FieldReport Data restore started ................", Color.Black, FontStyle.Bold);
                    InsertFieldReportToIDBO(fieldReports, sqlCon);
                    InsertFieldReportSubvendorToIDBO(fieldReportSubvendorList, sqlCon);
                    PMMigrationLogger.Log("FieldReport Data restore completed ................", Color.Black, FontStyle.Bold);

                }

            }
            catch (Exception ex)
            {
                PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                PMMigrationLogger.Log(ex.ToString(), Color.Red, FontStyle.Regular);
                throw;
            }
            PMMigrationLogger.Log("Out RestoreFieldReportData");
            return result;
        }

        public List<FieldReport> GetFieldReportDataFromOldProcon(Guid projectId, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetFieldReportDataFromOldProcon : Getting Field Report Data started ...", Color.Black, FontStyle.Bold);
            List<FieldReport> fieldReports = new List<FieldReport>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                string myGetTransmittslQuery = String.Format("select * from pc_field_report where PC_FLD_RPT_PRJ_FK = '{0}'", projectId);
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myGetTransmittslQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        FieldReport fieldReport = new FieldReport();
                        fieldReport.ID = Guid.NewGuid();

                        int oldID;
                        int.TryParse(myReader["PC_FLD_RPT_ID"].ToString(), out oldID);
                        fieldReport.OldID = oldID;

                        fieldReport.Comments = myReader["PC_FLD_RPT_COMMENTS"].ToString();
                        //fieldReport.CompletedByID = Guid.Empty;

                        if (myReader["PC_FLD_RPT_COMPLETED_BY"] == DBNull.Value || myReader["PC_FLD_RPT_COMPLETED_BY"] == (object)"")
                            fieldReport.CompletedBy = null;
                        else
                            fieldReport.CompletedBy = myReader["PC_FLD_RPT_COMPLETED_BY"].ToString();

                        DateTime Date;
                        if (myReader["PC_FLD_RPT_DATE"] == DBNull.Value)
                        {
                            fieldReport.Date = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_FLD_RPT_DATE"].ToString(), out Date);
                            fieldReport.Date = Date;
                        }

                        if (myReader["PC_FLD_RPT_DELIVERY_LOG"] == DBNull.Value || myReader["PC_FLD_RPT_DELIVERY_LOG"] == (object)"")
                            fieldReport.DeliveryLog = null;
                        else
                            fieldReport.DeliveryLog = myReader["PC_FLD_RPT_DELIVERY_LOG"].ToString();

                        if (myReader["PC_FLD_RPT_DESC_1"] == DBNull.Value || myReader["PC_FLD_RPT_DESC_1"] == (object)"")
                            fieldReport.Description1 = null;
                        else
                            fieldReport.Description1 = myReader["PC_FLD_RPT_DESC_1"].ToString();

                        if (myReader["PC_FLD_RPT_DESC_2"] == DBNull.Value || myReader["PC_FLD_RPT_DESC_2"] == (object)"")
                            fieldReport.Description2 = null;
                        else
                            fieldReport.Description2 = myReader["PC_FLD_RPT_DESC_2"].ToString();

                        if (myReader["PC_FLD_RPT_DESC_3"] == DBNull.Value || myReader["PC_FLD_RPT_DESC_3"] == (object)"")
                            fieldReport.Description3 = null;
                        else
                            fieldReport.Description3 = myReader["PC_FLD_RPT_DESC_3"].ToString();
                        if (myReader["PC_FLD_RPT_DESC_4"] == DBNull.Value || myReader["PC_FLD_RPT_DESC_4"] == (object)"")
                            fieldReport.Description4 = null;
                        else
                            fieldReport.Description4 = myReader["PC_FLD_RPT_DESC_4"].ToString();

                        if (myReader["PC_FLD_RPT_DESC_5"] == DBNull.Value || myReader["PC_FLD_RPT_DESC_5"] == (object)"")
                            fieldReport.Description5 = null;
                        else
                            fieldReport.Description5 = myReader["PC_FLD_RPT_DESC_5"].ToString();

                        fieldReport.ProjectID = projectId;

                        if (myReader["PC_FLD_RPT_SITE_CONDITIONS"] == DBNull.Value || myReader["PC_FLD_RPT_SITE_CONDITIONS"] == (object)"")
                            fieldReport.SiteConditions = null;
                        else
                            fieldReport.SiteConditions = myReader["PC_FLD_RPT_SITE_CONDITIONS"].ToString();

                        decimal tempHigh;
                        decimal.TryParse(myReader["PC_FLD_RPT_TEMP_HIGH"].ToString(), out tempHigh);
                        fieldReport.TempHigh = tempHigh;


                        decimal tempLow;
                        decimal.TryParse(myReader["PC_FLD_RPT_TEMP_LOW"].ToString(), out tempLow);
                        fieldReport.TempHigh = tempLow;

                        string fieldValue1 = myReader["PC_FLD_RPT_VALUE_1"].ToString();
                        if (fieldValue1 == "Y")
                        {
                            fieldReport.Value1 = 1;
                        }

                        string fieldValue2 = myReader["PC_FLD_RPT_VALUE_2"].ToString();
                        if (fieldValue2 == "Y")
                        {
                            fieldReport.Value2 = 1;
                        }

                        string fieldValue3 = myReader["PC_FLD_RPT_VALUE_3"].ToString();
                        if (fieldValue3 == "Y")
                        {
                            fieldReport.Value3 = 1;
                        }

                        string fieldValue4 = myReader["PC_FLD_RPT_VALUE_4"].ToString();
                        if (fieldValue4 == "Y")
                        {
                            fieldReport.Value4 = 1;
                        }

                        string fieldValue5 = myReader["PC_FLD_RPT_VALUE_5"].ToString();
                        if (fieldValue5 == "Y")
                        {
                            fieldReport.Value5 = 1;
                        }

                        if (myReader["PC_FLD_RPT_WEATHER"] == DBNull.Value || myReader["PC_FLD_RPT_WEATHER"] == (object)"")
                            fieldReport.Weather = null;
                        else
                            fieldReport.Weather = myReader["PC_FLD_RPT_WEATHER"].ToString();

                        if (myReader["PC_FLD_RPT_WORK_CONDITIONS"] == DBNull.Value || myReader["PC_FLD_RPT_WORK_CONDITIONS"] == (object)"")
                            fieldReport.WorkConditions = null;
                        else
                            fieldReport.WorkConditions = myReader["PC_FLD_RPT_WORK_CONDITIONS"].ToString();

                        fieldReport.AC1 = "Migrated Data";

                        fieldReports.Add(fieldReport);
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
            return fieldReports;
        }


        public List<FieldReportSubVendors> GetFieldReportSubVendorsFromOldProcon(List<FieldReport> fldReport, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetFieldReportSubVendorsFromOldProcon : Getting Field Report SubVendors  started ...", Color.Black, FontStyle.Bold);
            List<FieldReportSubVendors> fieldRptSubVendors = new List<FieldReportSubVendors>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var trans in fldReport)
                {
                    string myQuery = string.Format("select " +
                       "pcsub.PC_FLD_RPT_SV_COMMENTS," +
                          "pcsub.PC_FLD_RPT_SV_COMP_NAME," +
                          "pcsub.PC_FLD_RPT_SV_CREW," +
                          "pcsub.PC_FLD_RPT_SV_FLD_RPT_FK," +
                          "pcsub.PC_FLD_RPT_SV_ID," +
                          "pcsub.PC_FLD_RPT_SV_IS_ACTIVE   " +
                          "FROM  pc_fld_rpt_subvendor pcsub " +
                        "JOIN  pc_field_report pf ON pcsub.PC_FLD_RPT_SV_FLD_RPT_FK = pf.PC_FLD_RPT_ID  " +
                          "WHERE pf.PC_FLD_RPT_ID = '{0}' ", trans.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();

                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            FieldReportSubVendors fieldRptSubVendor = new FieldReportSubVendors();

                            fieldRptSubVendor.ID = Guid.NewGuid();
                            int oldID;
                            int.TryParse(myReader["PC_FLD_RPT_SV_ID"].ToString(), out oldID);
                            fieldRptSubVendor.OldID = oldID;

                            fieldRptSubVendor.FieldReportID = trans.ID;
                            int fieldID;
                            int.TryParse(myReader["PC_FLD_RPT_SV_FLD_RPT_FK"].ToString(), out fieldID);
                            fieldRptSubVendor.OldFieldReportID = fieldID;

                            fieldRptSubVendor.Comments = myReader["PC_FLD_RPT_SV_COMMENTS"].ToString();
                            fieldRptSubVendor.CompanyName = myReader["PC_FLD_RPT_SV_COMP_NAME"].ToString();
                            fieldRptSubVendor.Crew = myReader["PC_FLD_RPT_SV_CREW"].ToString();

                            int isActive;
                            int.TryParse(myReader["PC_FLD_RPT_SV_IS_ACTIVE"].ToString(), out isActive);
                            fieldRptSubVendor.IsActive = isActive;

                            fieldRptSubVendor.AC1 = "Migrated Data";

                            fieldRptSubVendors.Add(fieldRptSubVendor);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }

                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetFieldReportSubVendorsFromOldProcon :Getting Field Report SubVendors completed ...", Color.Black, FontStyle.Bold);
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
            return fieldRptSubVendors;
        }

        public void InsertFieldReportToIDBO(List<FieldReport> fieldReportList, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertFieldReportToIDBO : Restoring  started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var fieldReport in fieldReportList)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryFieldReport = string.Format("INSERT INTO [PMFieldReport] ( [ID],[ProjectID],[CompletedBy],[Comments],[Date],[DeliveryLog],[Description1],[Description2]," +
                                                "[Description3],[Description4],[Description5],[SiteConditions],[TempHigh],[TempLow],[Value1],[Value2],[Value3],[Value4]," +
                                                "[Value5],[Weather],[WorkConditions],[AC1],[OldID])" +
                                                " VALUES ('{0}','{1}',{2},{3},'{4}',{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},'{21}',{22})", fieldReport.ID
                                                , fieldReport.ProjectID, fieldReport.CompletedBy == null ? "null" : "'" + fieldReport.CompletedBy + "'", fieldReport.Comments == null ? "NULL" : "'" + fieldReport.Comments.Replace("'", "''") + "'", fieldReport.Date, fieldReport.DeliveryLog == null ? "null" : "'" + fieldReport.DeliveryLog + "'", fieldReport.Description1 == null ? "null" : "'" + fieldReport.Description1.Replace("'", "''") + "'", fieldReport.Description2 == null ? "null" : "'" + fieldReport.Description2.Replace("'", "''") + "'", fieldReport.Description3 == null ? "null" : "'" + fieldReport.Description3.Replace("'", "''") + "'", fieldReport.Description4 == null ? "null" : "'" + fieldReport.Description4.Replace("'", "''") + "'",
                                                fieldReport.Description5 == null ? "null" : "'" + fieldReport.Description5.Replace("'", "''") + "'", fieldReport.SiteConditions == null ? "null" : "'" + fieldReport.SiteConditions + "'", fieldReport.TempHigh == null ? "null" : (object)fieldReport.TempHigh, fieldReport.TempLow == null ? "null" : (object)fieldReport.TempLow, fieldReport.Value1, fieldReport.Value2, fieldReport.Value3, fieldReport.Value4, fieldReport.Value5, fieldReport.Weather == null ? "null" : "'" + fieldReport.Weather + "'",
                                                fieldReport.WorkConditions == null ? "null" : "'" + fieldReport.WorkConditions + "'", fieldReport.AC1.Replace("'", "''"), fieldReport.OldID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryFieldReport;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN FieldReport INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertFieldReportToIDBO : Restoring FieldReport completed  --------------------", Color.Black, FontStyle.Bold);
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


        public void InsertFieldReportSubvendorToIDBO(List<FieldReportSubVendors> fieldReportSubVendorList, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertFieldReportSubvendorToIDBO : Restoring  started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var fieldReportSubvendor in fieldReportSubVendorList)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryFieldReportSubvendor = string.Format("INSERT INTO [PMFieldReportSubvendor] ( [ID],[FieldReportID],[Comments],[CompanyName],[Crew],[IsActive],"+
                                               "[AC1],[OldID],[OldFieldReportID])" +
                                                " VALUES ('{0}','{1}',{2},{3},{4},{5},'{6}',{7},{8})", fieldReportSubvendor.ID
                                                , fieldReportSubvendor.FieldReportID, fieldReportSubvendor.Comments == null ? "NULL" : "'" + fieldReportSubvendor.Comments.Replace("'", "''") + "'",fieldReportSubvendor.CompanyName == null ? "NULL" : "'" + fieldReportSubvendor.CompanyName.Replace("'", "''") + "'", fieldReportSubvendor.Crew == null ? "NULL" : "'" + fieldReportSubvendor.Crew.Replace("'", "''") + "'", fieldReportSubvendor.IsActive,fieldReportSubvendor.AC1, fieldReportSubvendor.OldID,fieldReportSubvendor.OldFieldReportID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryFieldReportSubvendor;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN FieldReportSubvendor INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertFieldReportSubvendorToIDBO : Restoring FieldReportSubvendor completed  --------------------", Color.Black, FontStyle.Bold);
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
