
using System;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace PMImportContract
{
	
	public interface IProjectManagementImport
	{
		void Move(Guid projectID,int sectionName,MySqlConnection mySQLCon, SqlConnection sqlServerCon);
	}
}
