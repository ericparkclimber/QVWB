using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QVWB.Models
{

    public class QVStatusResponse : QVResponse
    {
        public string Status;
    }

    public class QVCommentResponse : QVResponse
    {
        public List<Comment> Comments = new List<Comment>();
        public bool hasComments{
            get{
                if(this.Comments != null)
                    return Comments.Count() > 0;

                return false;
            }
        }
    }

    public class Comment{
        public string User {get;set;}
        public DateTime DateAdded {get;set;}
        public string Message {get;set;}

        public Comment(string user, DateTime dateadded, string message)
        {
            this.User = user;
            this.DateAdded = dateadded;
            this.Message = message;
        }
    }

    public abstract class QVResponse
    {
        public bool Success {
            get
            {
                if (this.Error == null)
                    return true;
                return false;
            }
        }
        public ResponseError Error = null;


    }

    public class ResponseError
    {
        public ResponseError(HttpException httpException)
        {
            this.Message = httpException.Message;
            this.ErrorCode = httpException.ErrorCode;
        }
        public ResponseError(int errorCode, string errorMessage)
        {
            this.Message = errorMessage;
            this.ErrorCode = errorCode;
        }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }
}