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
    public class InstructionRepository
    {
        RestoreStatus restoreStatus = RestoreStatus.None;

        public int RestoreinstructionData(Guid projectID, MySqlConnection mysqlCon, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In RestoreInstructionData");
            int result = 0;
            try
            {
                PMMigrationLogger.Log("Data retrival started ---------------------", Color.Black, FontStyle.Bold);
                List<Instructions> instructions = new List<Instructions>();
                List<InstructionDistributionList> instructionDistList = new List<InstructionDistributionList>();
                instructions = GetInstructionDataFromOldProcon(projectID, mysqlCon);
                PMMigrationLogger.Log("Number of Instructions Data retrieve : " + instructions.Count);

                if (restoreStatus == RestoreStatus.Success)
                {
                    
                    instructionDistList = GetInstructionDistributionListFromOldProcon(instructions, mysqlCon);
                    PMMigrationLogger.Log("Number of InstructionDistributionList retrieve : " + instructionDistList.Count);
                    PMMigrationLogger.Log("Data retrival completed ---------------------", Color.Black, FontStyle.Bold);
                }

                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Creating Old ProjectArea , project Control-- New Project Area, project contro  Relation ....It might tale several minutes , please be patient", Color.Black, FontStyle.Bold);
                    instructions = ProjectAreaControlMapping(instructions, sqlCon);
                    PMMigrationLogger.Log("Creating Old ProjectArea, project control -- New Project Area, project contro Relation Completed", Color.Black, FontStyle.Bold);
                }


                if (restoreStatus == RestoreStatus.Success)
                {
                    PMMigrationLogger.Log("Instruction Data restore started ................", Color.Black, FontStyle.Bold);
                    InsertInstructionDataTOIDBO(instructions, instructionDistList, sqlCon);
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

        public List<Instructions> GetInstructionDataFromOldProcon(Guid projectId, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetInstructionDataFromOldProcon : Getting Instruction Data started ...", Color.Black, FontStyle.Bold);
            List<Instructions> instructions = new List<Instructions>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                string myGetInstructionlQuery = String.Format("select * from pc_instructions where PC_INS_PRJ_FK = '{0}'", projectId);
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                mySqlCmd.CommandText = myGetInstructionlQuery;
                myReader = mySqlCmd.ExecuteReader();
                if (myReader != null && myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        Instructions instruction = new Instructions();
                        instruction.ID = Guid.NewGuid();

                        int oldID;
                        int.TryParse(myReader["PC_INS_ID"].ToString(), out oldID);
                        instruction.OldID = oldID;

                        DateTime Date;
                        if (myReader["PC_INS_TO_CONTRACTOR"] == DBNull.Value)
                        {
                            instruction.ToContractorDate = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_INS_TO_CONTRACTOR"].ToString(), out Date);
                            instruction.ToContractorDate = Date;
                        }

                        TimeSpan time;
                        if (myReader["PC_INS_RES_TIME"] == DBNull.Value)
                        {
                            instruction.ResponseTime = null;
                        }
                        else
                        {
                            TimeSpan.TryParse(myReader["PC_INS_RES_TIME"].ToString(), out time);
                            instruction.ResponseTime = time;
                        }

                        int days;
                        int.TryParse(myReader["PC_INS_RES_DAYS"].ToString(), out days);
                        instruction.ResponseDays= days;

                        instruction.ProjectID = projectId;
                        instruction.OldProjectID = myReader["PC_INS_PRJ_FK"].ToString();

                        instruction.PIDescription = myReader["PC_INS_PI_DESC"].ToString();
                        instruction.Action = myReader["PC_INS_PI_ACTION"].ToString();
                        instruction.Originator= myReader["PC_INS_ORIGINATOR_FK"].ToString();

                        DateTime OrigDate;
                        if (myReader["PC_INS_ORIG_DATE"] == DBNull.Value)
                        {
                            instruction.ToContractorDate = null;
                        }
                        else
                        {
                            DateTime.TryParse(myReader["PC_INS_ORIG_DATE"].ToString(), out OrigDate);
                            instruction.OriginationDate = OrigDate;
                        }

                        int slNo;
                        int.TryParse(myReader["PC_INS_NO"].ToString(), out slNo);
                        instruction.SerialNumber = slNo;

                        instruction.Suffix = myReader["PC_INS_LETTER"].ToString();
                        instruction.SendStatus = myReader["PC_INS_SEND_STATUS"].ToString();

                        string isSend = myReader["PC_INS_IS_SENT"].ToString();
                        if (isSend == "Y")
                        {
                            instruction.IsSent = 1;
                        }

                        string isActive = myReader["PC_INS_IS_ACTIVE"].ToString();
                        if (isActive == "Y")
                        {
                            instruction.IsActive = 1;
                        }
                        else
                        {
                            instruction.IsActive = 0;
                        }

                        int dupType;
                        int.TryParse(myReader["PC_INS_DUP_TYPE"].ToString(), out dupType);
                        instruction.DuplicateType = dupType;

                        instruction.InstructionDescription = myReader["PC_INS_DOC_DESC"].ToString();
                        instruction.ProjectControlID = Guid.Empty;

                        int projConID;
                        int.TryParse(myReader["PC_INS_CTRL"].ToString(), out projConID);
                        instruction.OldProjectControlID = projConID;

                        int areaID;
                        int.TryParse(myReader["PC_INS_AREA_PKGS"].ToString(), out areaID);
                        instruction.OldAreaID = areaID;

                        instruction.AreaID = Guid.Empty;

                        int areaNo;
                        int.TryParse(myReader["PC_INS_AE_NO"].ToString(), out areaNo);
                        instruction.AENumber = areaNo;

                        instruction.AEDescription = myReader["PC_INS_AE_DESC"].ToString();

                       
                        instruction.AC1 = "Migrated Data";

                        instructions.Add(instruction);
                    }
                }
                if (!myReader.IsClosed)
                {
                    myReader.Close();
                }
                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetInstructionDataFromOldProcon : Getting Instruction Data completed ...", Color.Black, FontStyle.Bold);
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
            return instructions;
        }


        public List<InstructionDistributionList> GetInstructionDistributionListFromOldProcon(List<Instructions> instrs, MySqlConnection mySqlCon)
        {
            PMMigrationLogger.Log("In GetInstructionDistributionListFromOldProcon :Getting Instruction Distribution List  started ...", Color.Black, FontStyle.Bold);
            List<InstructionDistributionList> instructionDistLists = new List<InstructionDistributionList>();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                mySqlCon.Open();
                mySqlCmd.Connection = mySqlCon;
                foreach (var trans in instrs)
                {
                    string myQuery = string.Format("select " +
                       "pcpidlist.PC_PI_DIST_PRJ_CONT_NAME," +
                          "pcpidlist.PC_PI_DIST_PRJ_CONT_EMAIL," +
                          "pcpidlist.PC_PI_DIST_INS_TO," +
                          "pcpidlist.PC_PI_DIST_INS_RES," +
                          "pcpidlist.PC_PI_DIST_INS_ID," +
                          "pcpidlist.PC_PI_DIST_INS_FK," +
                          "prjCont.PC_PRJ_CONT_ID," +
                          "pcpidlist.PC_PI_DIST_INS_CC," +
                          "pcpidlist.PC_PI_DIST_INS_BCC   " +
                          "FROM  pc_pi_distribution_list pcpidlist " +
                          "JOIN  pc_instructions pi ON pcpidlist.PC_PI_DIST_INS_FK = pi.PC_INS_ID  " +
                          "JOIN pc_project_contacts prjCont ON pi.PC_INS_PRJ_FK = prjCont.PC_PRJ_CONT_PRJ_FK " +
                          "WHERE pi.PC_INS_ID = '{0}' "+
                          "AND pcpidlist.PC_PI_DIST_PRJ_CONT_NAME = prjCont.PC_PRJ_CONT_NAME " +
                          "AND pcpidlist.PC_PI_DIST_PRJ_CONT_EMAIL = prjCont.PC_PRJ_CONT_EMAIL " +
                          "AND pcpidlist.PC_PI_DIST_PRJ_COMP_NAME = prjCont.PC_PRJ_CONT_COMP_NAME ", trans.OldID);
                    mySqlCmd.CommandText = myQuery;
                    myReader = mySqlCmd.ExecuteReader();

                    if (myReader != null && myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            InstructionDistributionList instructionDistList = new InstructionDistributionList();

                            instructionDistList.ID = Guid.NewGuid();

                            int oldID;
                            int.TryParse(myReader["PC_PI_DIST_INS_ID"].ToString(), out oldID);
                            instructionDistList.OldID = oldID;

                            //instructionDistList.OldProjectContactID = myReader["PC_PI_DIST_PRJ_CONT_NAME"].ToString();
                            Guid m_ProjectContactID;
                            Guid.TryParse(myReader["PC_PRJ_CONT_ID"].ToString(),out m_ProjectContactID);
                            instructionDistList.ProjectContactID = m_ProjectContactID;

                            instructionDistList.ContactEmail = myReader["PC_PI_DIST_PRJ_CONT_EMAIL"].ToString();
                            string to = myReader["PC_PI_DIST_INS_TO"].ToString();
                            if (to == "Y")
                            {
                                instructionDistList.To = 1;
                            }

                            string cc = myReader["PC_PI_DIST_INS_CC"].ToString();
                            if (cc == "Y")
                            {
                                instructionDistList.Cc = 1;
                            }
                            string bcc = myReader["PC_PI_DIST_INS_BCC"].ToString();
                            if (bcc == "Y")
                            {
                                instructionDistList.Bcc = 1;
                            }
                            string res = myReader["PC_PI_DIST_INS_RES"].ToString();
                            if (res == "Y")
                            {
                                instructionDistList.Res = 1;
                            }

                            int oldInsID;
                            int.TryParse(myReader["PC_PI_DIST_INS_FK"].ToString(), out oldInsID);
                            instructionDistList.OldInstructionID = oldInsID;

                            instructionDistList.InstructionID = trans.ID;
                            

                            instructionDistList.AC1 = "Migrated Data";

                            instructionDistLists.Add(instructionDistList);
                        }
                    }
                    if (!myReader.IsClosed)
                    {
                        myReader.Close();
                    }
                }

                restoreStatus = RestoreStatus.Success;
                PMMigrationLogger.Log("Out GetInstructionDistributionListFromOldProcon :Getting Instruction Distribution List completed ...", Color.Black, FontStyle.Bold);
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
            return instructionDistLists;
        }

        #region Relation
        public List<Instructions> ProjectAreaControlMapping(List<Instructions> unmappedInstructions, SqlConnection sqlCon)
        {
            List<Instructions> mappedInstructions = new List<Instructions>();
            //mappedRfis.AddRange(unmappedRFis);
            try
            {
                sqlCon.Open();
                //int counter = 1;
                foreach (var instruction in unmappedInstructions)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        SqlDataReader myReader = null;
                        cmd.Connection = sqlCon;
                        Instructions tempinstruction = new Instructions();
                        // we can get both projectareaid and projectcontrolid from FinProjectControl
                        string areaQry = String.Format("select ID,ProjectAreaID from FinProjectControl where OldID={0};", instruction.OldProjectControlID);

                        cmd.CommandText = areaQry;
                        myReader = cmd.ExecuteReader();
                        if (myReader != null && myReader.HasRows)
                        {
                            while (myReader.Read())
                            {
                                Guid controlID;
                                Guid.TryParse(myReader["ID"].ToString(), out controlID);
                                instruction.ProjectControlID = controlID;

                                Guid areaId;
                                Guid.TryParse(myReader["ProjectAreaID"].ToString(), out areaId);
                                instruction.AreaID = areaId;
                            }
                        }
                        mappedInstructions.Add(instruction);
                        PMMigrationLogger.Log("Old Area -> New Area : " + instruction.OldAreaID + " -> " + instruction.AreaID + " ##### Old Control -> New Control : " + instruction.OldProjectControlID + " -> " + instruction.ProjectControlID);
                      
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
            return mappedInstructions;
        }
        #endregion

        public void InsertInstructionDataTOIDBO(List<Instructions> instructions, List<InstructionDistributionList> instructionDistLists, SqlConnection sqlCon)
        {
            InsertInstructionToIDBO(instructions, sqlCon);
           
            InsertInstructionDistributionListToIDBO(instructionDistLists, sqlCon);
        }

        public void InsertInstructionToIDBO(List<Instructions> instructions, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertInstructionToIDBO : Restoring Instruction started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var instruction in instructions)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = sqlCon;
                        string qryInstruction = string.Format("INSERT INTO [PMInstruction] ( [ID],[ProjectID],[ProjectControlID],[ProjectAreaID]," +
                                                "[ToContractorDate],[SendStatus],[ResponseTime],[ResponseDays],[PIDescription],[Action],[Originator],[OriginationDate],[SerialNumber],[Suffix]," +
                                                "[IsSent],[DuplicateType],[InstructionDescription],[AC1],[OldID],[OldProjectID],[OldProjectAreaID],[OldProjectControlID])" +
                                                " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}','{10}','{11}',{12},'{13}',{14},{15},'{16}','{17}',{18},'{19}',{20},{21})", instruction.ID
                                                , instruction.ProjectID,  instruction.ProjectControlID, instruction.AreaID, instruction.ToContractorDate, instruction.SendStatus,
                                                instruction.ResponseTime, instruction.ResponseDays, instruction.PIDescription.Replace("'", "''"), instruction.Action, instruction.Originator, instruction.OriginationDate, instruction.SerialNumber, instruction.Suffix, instruction.IsSent, instruction.DuplicateType,
                                                instruction.InstructionDescription.Replace("'", "''"), instruction.AC1.Replace("'", "''"), instruction.OldID, instruction.OldProjectID, instruction.OldAreaID, instruction.OldProjectControlID);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryInstruction;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN INSTRUCTION INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertInstructionToIDBO : Restoring Instruction completed  --------------------", Color.Black, FontStyle.Bold);
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


        public void InsertInstructionDistributionListToIDBO(List<InstructionDistributionList> instructionDistributionList, SqlConnection sqlCon)
        {
            PMMigrationLogger.Log("In InsertInstructionDistributionListToIDBO : Restoring DistributionList  started  --------------------", Color.Black, FontStyle.Bold);
            try
            {
                sqlCon.Open();
                int counter = 1;
                foreach (var instructionDist in instructionDistributionList)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                       
                        cmd.Connection = sqlCon;
                        string qryInstructionDist = string.Format("INSERT INTO [PMInstructionDistributionList] ([ID],[InstructionID],[ProjectContactID],[To],[Res],[Cc],[Bcc]," +
                        "[AC1],[OldID],[OldInstructionID],[OldProjectContactID],[OldContactName],[OldContactEmail],[OldContactCompany])" +
                        "VALUES ('{0}','{1}','{2}',{3},{4},{5},{6},'{7}','{8}',{9},'{10}','{11}','{12}','{13}')", instructionDist.ID, instructionDist.InstructionID, instructionDist.ProjectContactID,
                        instructionDist.To, instructionDist.Res, instructionDist.Cc, instructionDist.Bcc, instructionDist.AC1, instructionDist.OldID, instructionDist.OldInstructionID, instructionDist.OldProjectContactID, instructionDist.OldContactName, instructionDist.OldContactEmail, instructionDist.OldContactCompany);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = qryInstructionDist;


                        PMMigrationLogger.Log("[" + counter++ + "] >> " + cmd.CommandText.ToString());
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the error and don't throw since we want to continue with execution.
                        PMMigrationLogger.Log("Some Error Occured :", Color.Black, FontStyle.Bold);
                        PMMigrationLogger.Log("ERROR IN INSTRUCTION DISTIBUTION INSERT : " + ex.ToString(), Color.Red, FontStyle.Regular);
                        continue;
                    }
                }
                PMMigrationLogger.Log("Out InsertInstructionDistributionListToIDBO : Restoring DistributionList  completed  --------------------", Color.Black, FontStyle.Bold);
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
