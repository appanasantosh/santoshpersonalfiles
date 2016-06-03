using AutomaticEmail.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticEmail.Repository
{


    public class SubmittalsCriticalPathRepository
    {
        
        string connectionString = ConfigurationManager.ConnectionStrings["CriticalPath"].ConnectionString;
        public List<SubmittalCriticalPathItem> GetSubmittalCriticalPathRptData()
        {
            SqlCommand cmd = new SqlCommand();
            List<SubmittalCriticalPathItem> criticalPathItems = null;
            try
            {
                cmd.Connection = new SqlConnection(connectionString);
                cmd.Connection.Open();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "IDBO_PM_RPT_SUBMITTALALL_GET";
                cmd.Parameters.Add("@pProjectID", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@pProjectID"].Value = Guid.Empty;
                cmd.Parameters.Add("@pUserID", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@pUserID"].Value = Guid.Empty;
                cmd.Parameters.Add("@pMode", SqlDbType.Int);
                cmd.Parameters["@pMode"].Value = 4;

                SqlDataReader reader = cmd.ExecuteReader();
                if(reader != null && reader.HasRows == true)
                {
                    criticalPathItems = new List<SubmittalCriticalPathItem>();
                    while (reader.Read())
                    {
                        SubmittalCriticalPathItem item = new SubmittalCriticalPathItem();
                        item.ProjectNumber = reader["ProjectNumber"].ToString();
                        item.SpecNumber = reader["SpecNumber"].ToString();
                        item.SpecPara = reader["SpecPara"].ToString();
                        item.SpecTitle = reader["SpecTitle"].ToString();

                        if (reader["ActualBuyOutDate"] == DBNull.Value)
                            item.ActualBuyOutDate = null;
                        else
                        {
                            DateTime m_ActualBuyOutDate;
                            DateTime.TryParse(reader["ActualBuyOutDate"].ToString(), out m_ActualBuyOutDate);
                            item.ActualBuyOutDate = m_ActualBuyOutDate;
                        }

                        item.SubmittalsDescription = reader["SubmittalsDescription"].ToString();

                        if (reader["AEReviewAndProcessingDays"] == DBNull.Value)
                            item.AEReviewAndProcessingDays = null;
                        else
                        {
                            int m_AEReviewAndProcessingDays;
                            int.TryParse(reader["AEReviewAndProcessingDays"].ToString(), out m_AEReviewAndProcessingDays);
                            item.AEReviewAndProcessingDays = m_AEReviewAndProcessingDays;
                        }

                        if (reader["ToAEDate"] == DBNull.Value)
                            item.ToAEDate = null;
                        else
                        {
                            DateTime m_ToAEDate;
                            DateTime.TryParse(reader["ToAEDate"].ToString(), out m_ToAEDate);
                            item.ToAEDate = m_ToAEDate;
                        }

                        if (reader["DateReturnedFromAE"] == DBNull.Value)
                            item.DateReturnedFromAE = null;
                        else
                        {
                            DateTime m_DateReturnedFromAE;
                            DateTime.TryParse(reader["DateReturnedFromAE"].ToString(), out m_DateReturnedFromAE);
                            item.DateReturnedFromAE = m_DateReturnedFromAE;
                        }

                        item.CompanyName = reader["CompanyName"].ToString();

                        if (reader["AEDue"] == DBNull.Value)
                            item.AEDue = null;
                        else
                        {
                            DateTime m_AEDue;
                            DateTime.TryParse(reader["AEDue"].ToString(), out m_AEDue);
                            item.AEDue = m_AEDue;
                        }

                        item.SubmittalNumber = reader["SubmittalNumber"].ToString();
                        item.SpecNumberPara = reader["SpecNumberPara"].ToString();

                        if (reader["ActualToAEDate"] == DBNull.Value)
                            item.ActualToAEDate = null;
                        else
                        {
                            DateTime m_ActualToAEDate;
                            DateTime.TryParse(reader["ActualToAEDate"].ToString(), out m_ActualToAEDate);
                            item.ActualToAEDate = m_ActualToAEDate;
                        }

                        item.SendStatus = reader["SendStatus"].ToString();
                        item.SubVendorName = reader["SubVendorName"].ToString();
                        item.AccountingNumber = reader["AccountingNumber"].ToString();


                        // submittalPrintlItem.AC4 = reader["AC4"].ToString();
                        if (reader["AEComments"] == DBNull.Value)
                        {
                            item.AEComments = null;
                        }
                        else
                        {
                            item.AEComments = reader["AEComments"].ToString();
                        }

                        if (reader["BuyOutStatus"].ToString() == "RED" || reader["AEStatus"].ToString() == "RED" || reader["GovernmentStatus"].ToString() == "RED" || reader["SubVendorStatus"].ToString() == "RED" || reader["DeliveryStatus"].ToString() == "RED" || reader["BuyOutStatus"].ToString() == "YELLOW" || reader["AEStatus"].ToString() == "YELLOW" || reader["GovernmentStatus"].ToString() == "YELLOW" || reader["SubVendorStatus"].ToString() == "YELLOW" || reader["DeliveryStatus"].ToString() == "YELLOW")
                        {
                            if (item.AEComments == "Submittal Not Required" && reader["BuyOutStatus"].ToString() != "RED" && reader["GovernmentStatus"].ToString() != "RED" && reader["SubVendorStatus"].ToString() != "RED" && reader["DeliveryStatus"].ToString() != "RED" && reader["BuyOutStatus"].ToString() != "YELLOW" && reader["GovernmentStatus"].ToString() != "YELLOW" && reader["SubVendorStatus"].ToString() != "YELLOW" && reader["DeliveryStatus"].ToString() != "YELLOW" && (reader["AEStatus"].ToString() == "RED" || reader["AEStatus"].ToString() == "YELLOW"))
                            {
                                item.AC4 = "";
                            }
                            else
                            {
                                item.AC4 = "0";
                            }
                        }
                        else
                        {
                            item.AC4 = "";

                        }
                        if (reader["SubmittalDate"] == DBNull.Value)
                        {
                            item.SubmittalDate = null;
                        }
                        else
                        {
                            DateTime m_SubmittalDate;
                            DateTime.TryParse(reader["SubmittalDate"].ToString(), out m_SubmittalDate);
                            item.SubmittalDate = m_SubmittalDate;
                        }
                        if (reader["ProjectName"] == DBNull.Value)
                        {
                            item.ProjectName = null;
                        }
                        else
                        {
                            item.ProjectName = reader["ProjectName"].ToString();
                        }

                        criticalPathItems.Add(item);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
            }
            return criticalPathItems;
        }

        public Task<List<SubmittalCriticalPathItem>> GetSubmittalCriticalPathRptDataAsync()
        {
            return Task.Run(() => this.GetSubmittalCriticalPathRptData());
        }



    }
}
