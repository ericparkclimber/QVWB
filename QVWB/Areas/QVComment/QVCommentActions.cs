using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QVWB.Areas.Base;
using QVWB.Models;
using System.Data.SqlClient;
using System.Data;

namespace QVWB.Areas.QVComment
{
    public class QVCommentActions : QVActions
    {
        QVCommentRequest QVCommentRequest;

        public QVCommentActions(QVCommentRequest qvrequest): base(qvrequest)
        {
            this.QVCommentRequest = qvrequest;
        }

        public void addComment()
        {
            string sSql = "";
            List<SqlParameter> SqlParams = new List<SqlParameter>();

            sSql = "INSERT INTO " + this.QVCommentRequest.Table + "(Comment,DateTimeAdded,OSUser";

            foreach (string FieldName in this.QVCommentRequest.FieldValuePairs.Select(X => X.FieldName))
            {
                sSql += "," + FieldName;
            }

            sSql += ") VALUES(@Comment,@DateTimeAdded,@OSUser";

            for (int i = 0; i < this.QVCommentRequest.FieldValuePairs.Count(); i++)
            {
                sSql += ",@FieldValue" + i.ToString();
                SqlParams.Add(new SqlParameter("@FieldValue" + i.ToString(), SqlDbType.VarChar) { Value = this.QVCommentRequest.FieldValuePairs[i].FieldValue });
            }

            sSql += ")";

            SqlParams.Add(new SqlParameter("@Comment", SqlDbType.VarChar) { Value = this.QVCommentRequest.Comment });
            SqlParams.Add(new SqlParameter("@DateTimeAdded", SqlDbType.DateTime) { Value = DateTime.Now });
            SqlParams.Add(new SqlParameter("@OSUser", SqlDbType.VarChar) { Value = this.QVCommentRequest.OSUser });

            if (!DB.execute(sSql, SqlParams))
                throw new HttpException(500,"Could Not Add Comment");
        }

        public List<Comment> getComments()
        {
            List<Comment> Comments = new List<Comment>();
            string sSql = "";
            List<SqlParameter> SqlParams = new List<SqlParameter>();

            try
            {
                sSql = "SELECT * FROM " + this.QVCommentRequest.Table + " WHERE";

                for (int i = 0; i < this.QVCommentRequest.FieldValuePairs.Count(); i++)
                {
                    sSql += " " + QVCommentRequest.FieldValuePairs[i].FieldName + " = @FieldValue" + i.ToString();
                    SqlParams.Add(new SqlParameter("@FieldValue" + i.ToString(), SqlDbType.VarChar) { Value = QVCommentRequest.FieldValuePairs[i].FieldValue });
                }

                DB.executeReader(sSql, SqlParams);

                while (DB.Read())
                {
                    string OSUser = DB.GetDBString("OSUser");
                    string Comment = DB.GetDBString("Comment");
                    DateTime DateAdded = DB.GetDBDateTime("DateTimeAdded");
                    Comments.Add(new Comment(OSUser, DateAdded, Comment));
                }
            }
            catch
            {
                throw new HttpException(500, "Could not retreive comments");
            }
            finally
            {
                DB.CloseReader();
            }

            

            return Comments;
        }

        public void LogTransaction(string Action)
        {

            string sSql = "INSERT INTO " + this.QVCommentRequest.TransactionTable;
            sSql += " (FieldValues,Message,Action,OSUser,lastModified,APIKey,IP) VALUES(@FieldValues,@Message,@Action,@OSUser,@LastModified,@key,@IP)";

            string FieldValues = "";
            foreach (FieldValuePair FieldValue in this.QVCommentRequest.FieldValuePairs)
            {
                FieldValues += FieldValue.FieldName + ":" + FieldValue.FieldValue + ";";
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter("@FieldValues", SqlDbType.VarChar) { Value = FieldValues });
            SqlParams.Add(new SqlParameter("@Message", SqlDbType.VarChar) { Value = this.QVCommentRequest.Comment });
            SqlParams.Add(new SqlParameter("@Action", SqlDbType.VarChar) { Value = Action });
            SqlParams.Add(new SqlParameter("@OSUser", SqlDbType.VarChar) { Value = this.QVCommentRequest.OSUser });
            SqlParams.Add(new SqlParameter("@LastModified", SqlDbType.DateTime) { Value = DateTime.Now });
            SqlParams.Add(new SqlParameter("@key", SqlDbType.VarChar) { Value = this.QVCommentRequest.APIKey });
            SqlParams.Add(new SqlParameter("@IP", SqlDbType.VarChar) { Value = this.QVCommentRequest.RemoteAddress });

            if (!DB.execute(sSql, SqlParams))
                throw new HttpException(500,"Could not log transaction");
        }

    }
}