using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QVWB.HelperClasses;
using QVWB.Models;
using System.Data.SqlClient;
using System.Data;

namespace QVWB.Areas.Base
{
    public abstract class QVActions
    {
        public DBSql DB;
        public QVRequest QVRequest;

        public QVActions(QVRequest qvrequest)
        {
            this.QVRequest = qvrequest;
            this.DB = new DBSql(qvrequest.ConnectionName);
        }

        public bool isVerified()
        {
            bool isVerified = false;
            string sSql = "";
            List<SqlParameter> SqlParams = new List<SqlParameter>();
            try
            {
                sSql = "SELECT * FROM ApiKeys WHERE ApiKey = @Key";
                SqlParams.Add(new SqlParameter("@Key", SqlDbType.VarChar) { Value = QVRequest.APIKey });

                DB.executeReader(sSql, SqlParams);

                if (DB.Read())
                    isVerified =  true;


            }
            catch
            {
                throw new HttpException(401, "Could Not Verify");
            }
            finally
            {
                DB.CloseReader();
            }

            if(!isVerified)
                throw new HttpException(401, "Could Not Verify");

            return isVerified;
        }
    }
}