using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
namespace QVWB.Models
{
    public abstract class QVRequest
    {
        public string OSUser { get; set; }
        public string Table { get; set; }
        public string TransactionTable { get; set; }
        public string APIKey { get; set; }
        public string ConnectionName { get; set; }
        public string RemoteAddress { get; set; }

        public bool baseIsValid(bool checkTransactionTable = true)
        {
            if (string.IsNullOrEmpty(this.APIKey) || string.IsNullOrEmpty(this.ConnectionName))
                return false;

            if (!isTableNameAlphaNum())
                return false;

            if (checkTransactionTable)
                if (!isTransactionTableAlphaNum())
                    return false;

            return true;
        }

        private bool isTableNameAlphaNum()
        {
            if (string.IsNullOrEmpty(this.Table))
                return false;

            return Regex.IsMatch(this.Table, "^[a-zA-Z0-9]+$");
        }

        private bool isTransactionTableAlphaNum()
        {
            if (string.IsNullOrEmpty(this.TransactionTable))
                return false;

            return Regex.IsMatch(this.TransactionTable, "^[a-zA-Z0-9]+$");
        }
    }

    public class QVStatusRequest : QVRequest
    {
        public string Id { get; set; }
        public string Status { get; set; }

        public bool isValid(bool checkTransactionTable = true)
        {
            if (!this.baseIsValid(checkTransactionTable))
                throw new HttpException(401, "Invalid Request");

            if (string.IsNullOrEmpty(this.Id))
                throw new HttpException(401, "Invalid Request");

            return true;
        }
    }

    public class QVCommentRequest : QVRequest
    {
        public List<FieldValuePair> FieldValuePairs { get; set; }
        public string Comment { get; set; }

        private bool isFieldNamesValid()
        {
            foreach (FieldValuePair pair in this.FieldValuePairs)
            {
                if (!Regex.IsMatch(pair.FieldName, "^[a-zA-Z0-9]+$"))
                    return false;
            }
            return true;
        }

        public bool isValid(bool checkComment,bool checkTransActionTable = true)
        {

            if (!this.baseIsValid(checkTransActionTable))
                throw new HttpException(401, "Invalid Request");

            if(FieldValuePairs.Count <= 0)
                throw new HttpException(401, "No Field specified Request");

            if (!isFieldNamesValid())
                throw new HttpException(401, "Invalid Request");

            if (checkComment)
                if (string.IsNullOrEmpty(this.Comment))
                    throw new HttpException(401, "Comment must not be blank");
            return true;
        }
    }

    public class FieldValuePair
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}