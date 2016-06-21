
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMImportImplementation;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using PMImportImplementation.Entities;
using System.Drawing;
using System.Data;

namespace PMImportImplementation.Repository
{

	public class CertifiedPayRollRepository
	{
        RestoreStatus restoreStatus = RestoreStatus.None;
        public int RestoreCertifiedPayRollData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
        	PMMigrationLogger.Log("In RestoreCertifiedPayRollData");
        	int result = 0;
        	try 
        	{
        		PMMigrationLogger.Log("Data retrival started ---------------------",Color.Black, FontStyle.Bold );
        		
        		List<CertifiedPayRoll> certifiedPayRolls = new List<CertifiedPayRoll>();
                List<CertifiedPayNotes> certifiedPayNotes = new List<CertifiedPayNotes>();
                List<CertifiedPayReports> certifiedPayReports = new List<CertifiedPayReports>();

        		certifiedPayRolls = GetCertifiedPayRollDataFromOldProcon(projectID,mysqlCon);
                PMMigrationLogger.Log("Number of Certified Pay rolls retrieve : " + certifiedPayRolls.Count);
        		if (restoreStatus == RestoreStatus.Success)
        		{
                    certifiedPayRolls = CreateNodeRelationship(certifiedPayRolls);
                    certifiedPayNotes = GetCertifiedPayNotesDataFromOldProcob(certifiedPayRolls, mysqlCon);
                    PMMigrationLogger.Log("Number of Certified Pay Notes retrieve : " + certifiedPayNotes.Count);
        		}
        		if (restoreStatus == RestoreStatus.Success) 
        		{
                    certifiedPayReports = GetCertifiedPayReportsFromOldProcon(certifiedPayRolls, mysqlCon);
                    PMMigrationLogger.Log("Number of Certified Pay Reports retrieve : " + certifiedPayReports.Count);
        			PMMigrationLogger.Log("Data retrival completed ---------------------",Color.Black, FontStyle.Bold );
        		}
                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Certified pay roll Data restore started ................",Color.Black, FontStyle.Bold);
                    InsertCertifiedPayRollDataToIDBO(certifiedPayRolls, certifiedPayNotes, certifiedPayReports,sqlCon);

                    List<CertifiedPayRoll> oprhanCertifiedPayrolls = GetOrphanNodes(certifiedPayRolls);
                    PMMigrationLogger.Log("Number of Orphan Certified Pay Rolls : " + oprhanCertifiedPayrolls.Count + ". See the details below.", Color.Red, FontStyle.Bold);
                    int counter = 1;
                    foreach (var orphanCertifiedPayroll in oprhanCertifiedPayrolls)
                    {
                        PMMigrationLogger.Log("Certified Payroll [" + counter++ +"]");
                        PMMigrationLogger.Log("ID => " + orphanCertifiedPayroll.ID, Color.Red, FontStyle.Regular);
                        PMMigrationLogger.Log("Parent ID => " + orphanCertifiedPayroll.ParentID, Color.Red, FontStyle.Regular);
                        PMMigrationLogger.Log("Tier Number : " + orphanCertifiedPayroll.Level1.ToString() + "." + orphanCertifiedPayroll.Level2.ToString() + "." + orphanCertifiedPayroll.Level3.ToString() + "." + orphanCertifiedPayroll.Level4.ToString() + "." + orphanCertifiedPayroll.Level5.ToString(), Color.Red, FontStyle.Regular);
                    }
                    PMMigrationLogger.Log("Certified pay roll Data restore completed ................", Color.Black, FontStyle.Bold);
                }
				        		
        	} 
        	catch (Exception ex) 
        	{
        		PMMigrationLogger.Log("Some Error Occured :",Color.Black,FontStyle.Bold );
				PMMigrationLogger.Log(ex.ToString(),Color.Red,FontStyle.Regular);
        		throw;
        	}
        	
        	PMMigrationLogger.Log("Out RestoreCertifiedPayRollData");
        	return result;
        }
        
        
        #region Getting All Certified Pay roll related data
        
        public List<CertifiedPayRoll> GetCertifiedPayRollDataFromOldProcon(Guid projectId,MySqlConnection mySqlCon)
        {
        	PMMigrationLogger.Log("In GetCertifiedPayRollDataFromOldProcon : Getting Certified Pay roll started ...", Color.Black, FontStyle.Bold);
        	MySqlCommand mySqlCmd = new MySqlCommand();
			MySqlDataReader myReader = null;
        	List<CertifiedPayRoll> certifiedPayRolls = new List<CertifiedPayRoll>();
        	try
        	{
        		string myQuery = string.Format("select * from pc_cert_pay_tier " +
        		                               "JOIN pc_certified_payroll ON pc_cert_pay_tier.PC_CERT_PAY_TIER_PAYROLL_FK = pc_certified_payroll.PC_CERT_PAY_ID" +
											   " where pc_cert_pay_tier.PC_CERT_PAY_TIER_PRJ_FK = '{0}'",projectId);
				mySqlCon.Open();
				mySqlCmd.Connection = mySqlCon;
				mySqlCmd.CommandText = myQuery;
				myReader = mySqlCmd.ExecuteReader();
				if (myReader != null && myReader.HasRows) 
				{
					while(myReader.Read())
					{
						CertifiedPayRoll certifiedPayRoll = new CertifiedPayRoll();
						certifiedPayRoll.ID = Guid.NewGuid();
						
						int oldCertifiedPayRollID;
						int.TryParse(myReader["PC_CERT_PAY_ID"].ToString(), out oldCertifiedPayRollID);
						certifiedPayRoll.OldID = oldCertifiedPayRollID;

                        // Creating relation ship asuming old project id will not be changed during project migration
                        certifiedPayRoll.ProjectID = projectId;

						Guid oldProjectID;
						Guid.TryParse(myReader["PC_CERT_PAY_PRJ_FK"].ToString(), out oldProjectID);
						certifiedPayRoll.OldProjectID = oldProjectID;

                        //certifiedPayRoll.CompanyID = Guid.Empty;
                        // Same company id kept during company migration. so the older id is used.
                        Guid companyID;
                        Guid.TryParse(myReader["PC_CERT_PAY_COMP_FK"].ToString(), out companyID);
                        certifiedPayRoll.CompanyID = companyID;

						Guid oldCompanyID;
						Guid.TryParse( myReader["PC_CERT_PAY_COMP_FK"].ToString(), out oldCompanyID);
						certifiedPayRoll.OldCompanyID = oldCompanyID;

                        certifiedPayRoll.ParentID = Guid.Empty;

						int level1;
						int.TryParse( myReader["PC_CERT_PAY_TIER_LEVEL1"].ToString(), out level1);
						certifiedPayRoll.Level1 = level1;
						
						int level2;
						int.TryParse( myReader["PC_CERT_PAY_TIER_LEVEL2"].ToString(), out level2);
						certifiedPayRoll.Level2 = level2;
						
						int level3;
						int.TryParse( myReader["PC_CERT_PAY_TIER_LEVEL3"].ToString(), out level3);
						certifiedPayRoll.Level3 = level3;
						
						int level4;
						int.TryParse( myReader["PC_CERT_PAY_TIER_LEVEL4"].ToString(), out level4);
						certifiedPayRoll.Level4 = level4;
						
						int level5;
						int.TryParse( myReader["PC_CERT_PAY_TIER_LEVEL5"].ToString(), out level5);
						certifiedPayRoll.Level5 = level5;
						
						certifiedPayRoll.WorkDescription = myReader["PC_CERT_PAY_WORK_DESC"].ToString();
						
						string sf1413 = myReader["PC_CERT_PAY_SF1413"].ToString();
						if (sf1413 == "Y")
						{
							certifiedPayRoll.SF1413 = 1;
							// we don't need to write else part as in DB default value is 0
						}
						
						string swfp1185 = myReader["PC_CERT_PAY_SWFP1185"].ToString();
						if (swfp1185 == "Y") 
						{
							certifiedPayRoll.SWFP1185 = 1;
						}
						
						string training = myReader["PC_CERT_PAY_TRAINING"].ToString();
						if (training == "Y") 
						{
							certifiedPayRoll.Training =1;
						}
						
						string onsite = myReader["PC_CERT_PAY_ONSITE"].ToString();
						if (onsite == "Y")
						{
							certifiedPayRoll.Onsite = 1;
						}
						
                        

						certifiedPayRoll.AC1 = "Migrated Data";
						
						certifiedPayRolls.Add(certifiedPayRoll);
						
					}
				}
        		restoreStatus = RestoreStatus.Success;
				if (!myReader.IsClosed)
				{
					myReader.Close();
				}
				PMMigrationLogger.Log("Out GetCertifiedPayRollDataFromOldProcon : Getting Certified Pay Roll  ended ...",Color.Black, FontStyle.Bold );
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
				}
        	}
        	
        	return certifiedPayRolls;
        }

        public List<CertifiedPayNotes> GetCertifiedPayNotesDataFromOldProcob(List<CertifiedPayRoll> certifiedPayrolls, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetCertifiedPayNotesDataFromOldProcob : Getting Certified Pay Notes Strated ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            List<CertifiedPayNotes> certifiedPayNotes = new List<CertifiedPayNotes>();
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var certifiedPayroll in certifiedPayrolls)
                {
                    string myQuery = string.Format("select * from pc_cert_pay_notes where PC_CERT_PAY_NOTE_PAYROLL_FK = {0}", certifiedPayroll.OldID );
					mySqlCmd.CommandText = myQuery;
					myReader = mySqlCmd.ExecuteReader();
                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            CertifiedPayNotes certifiedPayNote = new CertifiedPayNotes();
                            certifiedPayNote.ID = Guid.NewGuid();

                            int oldID;
                            int.TryParse(myReader["PC_CERT_PAY_NOTE_ID"].ToString(), out oldID);
                            certifiedPayNote.OldID = oldID;

                            int oldCertifiedPayRollID;
                            int.TryParse(myReader["PC_CERT_PAY_NOTE_PAYROLL_FK"].ToString(), out oldCertifiedPayRollID);
                            certifiedPayNote.OldCertifiedPayRollID = oldCertifiedPayRollID;

                            DateTime date;
                            DateTime.TryParse(myReader["PC_CERT_PAY_NOTE_DATE"].ToString(), out date);
                            certifiedPayNote.Date = date;

                            certifiedPayNote.Description = myReader["PC_CERT_PAY_NOTE_DESC"].ToString();

                            int number;
                            int.TryParse(myReader["PC_CERT_PAY_NOTES_NO"].ToString(), out number);
                            certifiedPayNote.Number = number;

                            certifiedPayNote.AC1 = "Migrated Data";

                            // Building Relation
                            certifiedPayNote.CertifiedPayRollID = certifiedPayroll.ID;

                            certifiedPayNotes.Add(certifiedPayNote);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("In GetCertifiedPayNotesDataFromOldProcob : Getting Certified Pay Notes Completed ...", Color.Black, FontStyle.Bold);
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

            return certifiedPayNotes;
        }

        public List<CertifiedPayReports> GetCertifiedPayReportsFromOldProcon(List<CertifiedPayRoll> certifiedPayrolls, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetCertifiedPayReportsFromOldProcon : Getting Certified Pay Report Strated ...", Color.Black, FontStyle.Bold);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            List<CertifiedPayReports> certifiedPayReports = new List<CertifiedPayReports>();

            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;

                foreach (var certifiedPayroll in certifiedPayrolls)
                {
                    string myQuery = string.Format("select * from pc_cert_pay_report where PC_CERT_PAY_REP_PAYROLL_FK = {0}", certifiedPayroll.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();
                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            CertifiedPayReports certifiedPayReport = new CertifiedPayReports();

                            certifiedPayReport.ID = Guid.NewGuid();

                            int oldID;
                            int.TryParse(myReader["PC_CERT_PAY_REP_ID"].ToString(), out oldID);
                            certifiedPayReport.OldID = oldID;

                            int oldCertifiedPayrollID;
                            int.TryParse(myReader["PC_CERT_PAY_REP_PAYROLL_FK"].ToString(), out oldCertifiedPayrollID);
                            certifiedPayReport.OldCertifiedPayRollID = oldCertifiedPayrollID;

                            DateTime beginDate;
                            DateTime.TryParse(myReader["PC_CERT_PAY_REP_BEGIN_DATE"].ToString(), out beginDate);
                            certifiedPayReport.BeginDate = beginDate;

                            DateTime approvaDate;
                            DateTime.TryParse(myReader["PC_CERT_PAY_REP_DATE_APPROVED"].ToString(), out approvaDate);
                            certifiedPayReport.ApprovedDate = approvaDate;

                            DateTime sendDate;
                            DateTime.TryParse(myReader["PC_CERT_PAY_REP_DATE_SEND"].ToString(), out sendDate);
                            certifiedPayReport.DateSend = sendDate;

                            DateTime endDate;
                            DateTime.TryParse(myReader["PC_CERT_PAY_REP_END_DATE"].ToString(), out endDate);
                            certifiedPayReport.EndDate = endDate;

                            DateTime receivedDate;
                            
                            if (myReader["PC_CERT_PAY_REP_RECEIVED_DATE"] == DBNull.Value)
                            {
                                certifiedPayReport.ReceivedDate = null;
                            }
                            else
                            {
                                DateTime.TryParse(myReader["PC_CERT_PAY_REP_RECEIVED_DATE"].ToString(), out receivedDate);
                                certifiedPayReport.ReceivedDate = receivedDate;
                            }
                            
                            string empDed = myReader["PC_CERT_PAY_REP_EMP_DED"].ToString();
                            if (empDed == "Y")
                            {
                                certifiedPayReport.EmpDed = 1;
                                // No need write code for else part as in db default ios 0
                            }

                            int number;
                            int.TryParse(myReader["PC_CERT_PAY_REP_NO"].ToString(), out number);
                            certifiedPayReport.Number = number;

                            certifiedPayReport.Notes = myReader["PC_CERT_PAY_REP_NOTES"].ToString();

                            int sendStatus;
                            int.TryParse(myReader["PC_CERT_PAY_REP_SEND_STAT"].ToString(), out sendStatus);
                            certifiedPayReport.SendStatus = sendStatus;

                            certifiedPayReport.AC1 = "Migrated Data";

                            // Making relationship
                            certifiedPayReport.CertifiedPayRollID = certifiedPayroll.ID;

                            certifiedPayReports.Add(certifiedPayReport);

                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("In GetCertifiedPayReportsFromOldProcon : Getting Certified Pay Report Completed ...", Color.Black, FontStyle.Bold);
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
            return certifiedPayReports;
        }

        #endregion
        
        #region Inserting all Certified pay roll related data
        public void InsertCertifiedPayRollDataToIDBO(List<CertifiedPayRoll> certifiedPayRolls, List<CertifiedPayNotes> certifiedPayNotes, List<CertifiedPayReports> certifiedPayReports, SqlConnection sqlCon)
        {
            try
            {
                InsertCertifiedPayRollToIDBO(certifiedPayRolls, sqlCon);
                InsertCertifiedPayNotesToIDBO(certifiedPayNotes, sqlCon);
                InsertCertifiedPayReportsToIDBO(certifiedPayReports, sqlCon);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InsertCertifiedPayRollToIDBO(List<CertifiedPayRoll> certifiedPayRolls, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertCertifiedPayRollToIDBO : Restoring Certified Pay rolls started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var certifiedPayRoll in certifiedPayRolls)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
						cmd.Connection = sqlCon;
                        string oldTierNumbers = certifiedPayRoll.Level1.ToString() + "." + certifiedPayRoll.Level2.ToString() + "." + certifiedPayRoll.Level3.ToString() + "." + certifiedPayRoll.Level4.ToString() + "." + certifiedPayRoll.Level5;
                        string qryCertifiedPayRoll = string.Format("INSERT INTO PMCertifiedPayRoll(ID,CompanyID,ProjectID,Onsite,SF1413,SWFP1185,Training,WorkDescription,AC1,ParentID,OldID,OldCompanyID,OldProjectID,Level1,Level2,Level3,Level4,Level5,AC2) " +
                                                            "VALUES ('{0}','{1}','{2}',{3},{4},{5},{6},'{7}','{8}','{9}',{10},'{11}','{12}',{13},{14},{15},{16},{17},'{18}')", certifiedPayRoll.ID,
                                                            certifiedPayRoll.CompanyID, certifiedPayRoll.ProjectID, certifiedPayRoll.Onsite, certifiedPayRoll.SF1413, certifiedPayRoll.SWFP1185,certifiedPayRoll.Training,
                                                            certifiedPayRoll.WorkDescription.Replace("'", "''") , certifiedPayRoll.AC1.Replace("'", "''"), certifiedPayRoll.ParentID, certifiedPayRoll.OldID,
                                                            certifiedPayRoll.OldCompanyID, certifiedPayRoll.OldProjectID, certifiedPayRoll.Level1,certifiedPayRoll.Level2,certifiedPayRoll.Level3,certifiedPayRoll.Level4,certifiedPayRoll.Level5,
                                                            oldTierNumbers);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryCertifiedPayRoll;


                        PMMigrationLogger.Log("["+ counter++ +"] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
						
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN CERTIFIED PAY ROLL INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertCertifiedPayRollToIDBO : Restoring Certified Pay rolls completed  --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertCertifiedPayNotesToIDBO(List<CertifiedPayNotes> certifiedPayNotes, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertCertifiedPayNotesToIDBO : Restoring Certified Pay notes started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var certifiedPayNote in certifiedPayNotes)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
						cmd.Connection = sqlCon;

                        string qryCertifiedPayNotes = string.Format("INSERT INTO PMCertifiedPayNotes(ID,CertifiedPayRollID,Date,SerialNumber,Description,AC1,OldID,[OldCertifiedPayRollID]) " +
                                                            "VALUES ('{0}','{1}','{2}',{3},'{4}','{5}',{6},{7})", certifiedPayNote.ID,
                                                            certifiedPayNote.CertifiedPayRollID, certifiedPayNote.Date, certifiedPayNote.Number, certifiedPayNote.Description.Replace("'", "''"), certifiedPayNote.AC1.Replace("'", "''"),
                                                            certifiedPayNote.OldID,certifiedPayNote.OldCertifiedPayRollID);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryCertifiedPayNotes;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN CERTIFIED PAY NOTES INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("In InsertCertifiedPayNotesToIDBO : Restoring Certified Pay notes completed  --------------------", Color.Black, FontStyle.Bold);
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

        public void InsertCertifiedPayReportsToIDBO(List<CertifiedPayReports> certifiedPayReports, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertCertifiedPayNotesToIDBO : Restoring Certified Pay reports started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var certifiedPayReport in certifiedPayReports)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
						cmd.Connection = sqlCon;

                        //object receivedDate;
                        string qryCertifiedPayReport = string.Format("INSERT INTO PMCertifiedPayReport(ID,CertifiedPayRollID,BeginDate,ApprovedDate,DateSend,EndDate,ReceivedDate,EmpDed,SerialNumber,SendStatus,Notes,AC1,OldID,OldCertifiedPayrollID) " +
                                                            "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},{9},'{10}','{11}',{12},{13})", certifiedPayReport.ID,
                                                            certifiedPayReport.CertifiedPayRollID, certifiedPayReport.BeginDate, certifiedPayReport.ApprovedDate, certifiedPayReport.DateSend, certifiedPayReport.EndDate, certifiedPayReport.ReceivedDate,
                                                            certifiedPayReport.EmpDed, certifiedPayReport.Number, certifiedPayReport.SendStatus, certifiedPayReport.Notes.Replace("'", "''"),
                                                            certifiedPayReport.AC1.Replace("'", "''"), certifiedPayReport.OldID, certifiedPayReport.OldCertifiedPayRollID);

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryCertifiedPayReport;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN CERTIFIED PAY REPORTS INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("In InsertCertifiedPayNotesToIDBO : Restoring Certified Pay reports completed  --------------------", Color.Black, FontStyle.Bold);
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
        
        #region Creating Relation Ships
        public List<CertifiedPayRoll> CreateNodeRelationship(List<CertifiedPayRoll> certifiedPayrolls)
        {
            List<CertifiedPayRoll> hierchicalCertifiedPayrolls = new List<CertifiedPayRoll>();
            try
            {
                // in our db there is only one root
                var rootLevelNode = (from cf in certifiedPayrolls where cf.Level1 != 0 && cf.Level2 == 0 && cf.Level3 == 0 && cf.Level4 == 0 && cf.Level5 == 0 select cf);

                
                
                var firstLevelNodes = (from cf in certifiedPayrolls where cf.Level1 != 0 && cf.Level2 != 0 &&  cf.Level3 == 0 && cf.Level4 == 0 && cf.Level5 == 0 select cf);
                if (rootLevelNode.Count() != 0) // Some times there are no root node in procon DB. Thing may start with 1.2.0.0.0
                {
                    foreach (var item in firstLevelNodes)
                    {
                        // Since there is a single root node
                        item.ParentID = rootLevelNode.First().ID;
                    }
                }
                

                var secondLevelNodes = (from cf in certifiedPayrolls where cf.Level1 != 0 && cf.Level2 != 0 && cf.Level3 != 0  && cf.Level4 == 0 && cf.Level5 == 0 select cf);
                if (firstLevelNodes.Count() != 0) // Some times first level node in procon DB is not present. Thing may start with 1.2.3.0.0
                {
                    foreach (var cf in secondLevelNodes)
                    {
                        foreach (var cfFirstLevel in firstLevelNodes)
                        {
                            if (cf.Level2 == cfFirstLevel.Level2)
                            {
                                cf.ParentID = cfFirstLevel.ID;
                            }
                        }
                    }
                }
                

                var thirdLevelNodes = (from cf in certifiedPayrolls where cf.Level1 != 0 && cf.Level2 != 0 && cf.Level3 != 0 && cf.Level4 != 0 && cf.Level5 == 0 select cf);
                if (secondLevelNodes.Count() != 0) // Some times second level node in procon DB is not present. Thing may start with 1.2.3.4.0
                {
                    foreach (var cf in thirdLevelNodes)
                    {
                        foreach (var cfSecondLevel in secondLevelNodes)
                        {
                            if (cf.Level3 == cfSecondLevel.Level3 && cf.Level2 == cfSecondLevel.Level2)
                            {
                                cf.ParentID = cfSecondLevel.ID;
                            }
                        }
                    }
                }
                

                var fourthLevelNodes = (from cf in certifiedPayrolls where cf.Level1 != 0 && cf.Level2 != 0 && cf.Level3 != 0 && cf.Level4 != 0 && cf.Level5 != 0 select cf);
                if (thirdLevelNodes.Count() != 0) // Some times third level node in procon DB is not present. Thing may start with 1.2.3.4.5 . Every thing is possible with procon data.
                {
                    foreach (var cf in fourthLevelNodes)
                    {
                        foreach (var cfThirdLevel in thirdLevelNodes)
                        {
                            if (cf.Level4 == cfThirdLevel.Level4 && cf.Level3 == cfThirdLevel.Level3 && cf.Level2 == cfThirdLevel.Level2)
                            {
                                cf.ParentID = cfThirdLevel.ID;
                            }

                        }
                    }
                }
                

                hierchicalCertifiedPayrolls.AddRange(rootLevelNode);
                hierchicalCertifiedPayrolls.AddRange(firstLevelNodes);
                hierchicalCertifiedPayrolls.AddRange(secondLevelNodes);
                hierchicalCertifiedPayrolls.AddRange(thirdLevelNodes);
                hierchicalCertifiedPayrolls.AddRange(fourthLevelNodes);
            }
            catch (Exception ex)
            {
                throw;
            }


            return hierchicalCertifiedPayrolls;
        }

        public List<CertifiedPayRoll> GetOrphanNodes(List<CertifiedPayRoll> certifiedPayrolls)
        {
            List<CertifiedPayRoll> orphanCertifiedPayRolls = new List<CertifiedPayRoll>();
            try
            {
                // in our db there is only one root
                var rootLevelNode = (from cf in certifiedPayrolls where cf.Level1 != 0 && cf.Level2 == 0 && cf.Level3 == 0 && cf.Level4 == 0 && cf.Level5 == 0 select cf);
                //var orphan = orphanCertifiedPayRolls;
                if (rootLevelNode.Count() != 0 )
                {
                   var orphan = (from cf in certifiedPayrolls where cf.ParentID == Guid.Empty && cf.ID != rootLevelNode.First().ID select cf);
                   orphanCertifiedPayRolls.AddRange(orphan);
                }
                else
                {
                    var orphan = (from cf in certifiedPayrolls where cf.ParentID == Guid.Empty  select cf);
                    orphanCertifiedPayRolls.AddRange(orphan);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return orphanCertifiedPayRolls;
        }
        #endregion
	}
}
