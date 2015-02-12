using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QVWB.Areas.Base;
using QVWB.Models;
using System.Data.SqlClient;
using System.Data;

namespace QVWB.Areas.QVStatus
{
    public class QVStatusActions : QVActions
    {
        QVStatusRequest QVStatusRequest;

        public QVStatusActions(QVStatusRequest qvrequest) : base( qvrequest)
        {
            this.QVStatusRequest = qvrequest;
        }

        public void LogTransaction()
        {
            string sSql = "INSERT INTO " + this.QVStatusRequest.TransactionTable;
            sSql += "(Id,OSUser,lastModified,Status,APIKey,IP) ";
            sSql += "VALUES(@Id,@OSUser,@LastModified,@Status,@key,@IP)";

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter("@Id", SqlDbType.VarChar) { Value = this.QVStatusRequest.Id });
            SqlParams.Add(new SqlParameter("@OSUser", SqlDbType.VarChar) { Value = this.QVStatusRequest.OSUser });
            SqlParams.Add(new SqlParameter("@LastModified", SqlDbType.DateTime) { Value = DateTime.UtcNow });
            SqlParams.Add(new SqlParameter("@Key", SqlDbType.VarChar) { Value = this.QVStatusRequest.APIKey });
            SqlParams.Add(new SqlParameter("@IP", SqlDbType.VarChar) { Value = this.QVStatusRequest.RemoteAddress });
            if(String.IsNullOrEmpty(this.QVStatusRequest.Status))
                SqlParams.Add(new SqlParameter("@Status", SqlDbType.VarChar) { Value =  DBNull.Value, IsNullable = true});
            else
                SqlParams.Add(new SqlParameter("@Status", SqlDbType.VarChar) { Value = this.QVStatusRequest.Status });

            if (!DB.execute(sSql, SqlParams))
                throw new HttpException(500,"Could not log transactions");
        }

        public void UpdateStatus()
        {
            string sSql = "";
            List<SqlParameter> SqlParams = new List<SqlParameter>();

            try
            {
                sSql = "SELECT * FROM " + this.QVStatusRequest.Table + " WHERE Id = @Id";
                SqlParams.Add(new SqlParameter("@Id", SqlDbType.VarChar) { Value = this.QVStatusRequest.Id });
                DB.executeReader(sSql, SqlParams);
            }
            catch
            {
                throw new HttpException(500,"Could not retrieve status");
            }

            if(DB.Read())
                sSql = "UPDATE " + QVRequest.Table + " SET status = @Status Where Id = @Id";
            else
                sSql = "INSERT INTO " + QVRequest.Table + "(Id,Status) VALUES(@Id,@Status)";

            DB.CloseReader();

            SqlParams.Clear();
            SqlParams.Add(new SqlParameter("@Id", SqlDbType.VarChar) {Value = this.QVStatusRequest.Id});
            if(String.IsNullOrEmpty(this.QVStatusRequest.Status))
                SqlParams.Add(new SqlParameter("@Status", SqlDbType.VarChar) {Value = DBNull.Value, IsNullable = true});
            else
                SqlParams.Add(new SqlParameter("@Status", SqlDbType.VarChar) {Value = this.QVStatusRequest.Status});

            if(!DB.execute(sSql,SqlParams))
                throw new HttpException(500, "Could not retrieve status");
        }

        public string GetStatus()
        {
            string status = "";
            string sSql = "";
            List<SqlParameter> SqlParams = new List<SqlParameter>();

            try
            {
                sSql = "SELECT TOP 1 Status FROM " + QVRequest.Table + " WHERE Id = @Id";
                SqlParams.Add(new SqlParameter("@Id", SqlDbType.VarChar) {Value = this.QVStatusRequest.Id});

                DB.executeReader(sSql, SqlParams);

                if(DB.Read())
                    status = DB.GetDBString("Status");

                DB.CloseReader();
            }
            catch
            {
                throw new HttpException(500,"Could not retrieve status");
            }
            finally
            {
                DB.CloseReader();
            }

            return status;
        }

    }
}