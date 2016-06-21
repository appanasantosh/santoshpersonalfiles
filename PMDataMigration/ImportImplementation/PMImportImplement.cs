
using System;
using PMImportContract;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Composition;
using PMImportImplementation.Repository;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
namespace PMImportImplementation
{
    [Export(typeof(IProjectManagementImport))]
    public class PMImportImplement : IProjectManagementImport
    {

        public void Move(Guid projectID, int sectionName, MySqlConnection mySQLCon, SqlConnection sqlServerCon)
        {
            PunchListRepository punchRepo = new PunchListRepository();
            CertifiedPayRollRepository certifiedPayRollRepo = new CertifiedPayRollRepository();
            CloseOutRepository closeOurRepo = new CloseOutRepository();
            RFIRepository rfiRepo = new RFIRepository();
            TransmittalRepository transRepo = new TransmittalRepository();
            LetterRepository letterRepo = new LetterRepository();
            InstructionRepository instructionRepo = new InstructionRepository();
            FieldReportRepository fieldRepo = new FieldReportRepository();
            ConversationRepository conversationRepo = new ConversationRepository();
            SubmittalsRepository submittalsRepo = new SubmittalsRepository();
            COPRRepository coprRepo = new COPRRepository();
            MeetingsRepository meetRepo = new MeetingsRepository();
            DocumentRepository docRepo = new DocumentRepository();
            
            try
            {
                switch (sectionName)
                {
                    case (int)PMEnum.PunchList:
                        punchRepo.RestorePuchListData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.CertifiedPayRoll:
                        certifiedPayRollRepo.RestoreCertifiedPayRollData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.CloseOut:
                        closeOurRepo.RestoreCloseOutData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.RFI:
                        rfiRepo.RestoreRFIData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.Transmittal:
                        transRepo.RestoreTransmittalData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.Letter:
                        letterRepo.RestoreLetterData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.Instruction:
                        instructionRepo.RestoreinstructionData(projectID, mySQLCon, sqlServerCon);
                        break;
                    case (int)PMEnum.FieldReport:
                        fieldRepo.RestoreFieldReportData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.Conversation:
                        conversationRepo.RestoreConversationData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.Submittals:
                        submittalsRepo.RestoreSubmittalsData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.COPR:
                        coprRepo.RestoreCOPRData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.Meetings:
                        meetRepo.RestoreMeetingData(projectID, mySQLCon, sqlServerCon);
                        break;

                    case (int)PMEnum.Documents:
                        docRepo.RestoreDocumentData(mySQLCon, sqlServerCon);
                        break;

                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
