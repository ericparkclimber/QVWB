using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace QVWB.HelperClasses
{
    public class DBSql
    {
        SqlConnection DBConnection;
        SqlDataReader SqlDr;

        public DBSql()
        {
            try
            {
                DBConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            }
            catch
            {
                throw new Exception("Connection Not Available");
            }
        }

        public DBSql(string ConnectionStringName)
        {
            try
            {
                DBConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString);
            }
            catch
            {
                throw new HttpException(401, "Bad Connection Name");
            }

        }

        public Boolean execute(string SqlString, List<SqlParameter> SqlParams = null)
        {
            try
            {
                DBConnection.Open();
                SqlCommand sqlCmd = new SqlCommand(SqlString, DBConnection);
                if (SqlParams != null)
                    sqlCmd.Parameters.AddRange(SqlParams.ToArray());
                sqlCmd.ExecuteNonQuery();
            }
            catch
            {
                CloseReader();
                return false;
            }
            CloseReader();
            return true;
        }

        public void executeReader(string SqlString, List<SqlParameter> SqlParams = null)
        {
            try
            {
                DBConnection.Open();
                SqlCommand sqlCmd = new SqlCommand(SqlString, DBConnection);
                if (SqlParams != null)
                    sqlCmd.Parameters.AddRange(SqlParams.ToArray());
                SqlDr = sqlCmd.ExecuteReader();
            }
            catch(Exception ex)
            {
                CloseReader();
            }
        }

        public bool Read()
        {
            if (SqlDr != null)
                return SqlDr.Read();
            else
                return false;
        }

        public string GetDBString(string Field)
        {
            if (SqlDr != null)
            {
                for (int i = 0; i < SqlDr.FieldCount; i++)
                {
                    if (SqlDr.GetName(i) == Field)
                    {
                        if (SqlDr.IsDBNull(i))
                            return "";
                        else
                            return SqlDr.GetString(i);
                    }
                }
            }
            return "";
        }

        public DateTime GetDBDateTime(string Field)
        {
            if (SqlDr != null)
            {
                for (int i = 0; i < SqlDr.FieldCount; i++)
                {
                    if (SqlDr.GetName(i) == Field)
                    {
                        if (SqlDr.IsDBNull(i))
                            return DateTime.MinValue;
                        else
                            return SqlDr.GetDateTime(i);
                    }
                }
            }
            return DateTime.MinValue;
        }
        public void CloseReader()
        {
            if (SqlDr != null)
            {
                SqlDr.Close();
            }
            DBConnection.Close();
        }
    }
}
