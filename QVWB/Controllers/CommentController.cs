using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QVWB.Models;
using QVWB.Areas.QVComment;

namespace QVWB.Controllers
{
    public class CommentController : Controller
    {
        [HttpGet]
        public JsonResult getComments(QVCommentRequest QVRequest, string Table)
        {
             QVCommentResponse QVResponse = new QVCommentResponse();

            try
            {
                if (QVRequest.isValid(false,false))
                {
                    QVCommentActions CommentActions = new QVCommentActions(QVRequest);
                    if (CommentActions.isVerified())
                    {
                        QVResponse.Comments = CommentActions.getComments();
                    }
                }
            }
            catch (HttpException ex)
            {
                QVResponse.Error = new ResponseError(ex);
            }

            JsonResult JsonResponse = new JsonResult();
            JsonResponse.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            JsonResponse.Data = QVResponse;
            return JsonResponse;
        }

        [HttpPost]
        public JsonResult addComments(QVCommentRequest QVRequest, string Table)
        {
            QVCommentResponse QVResponse = new QVCommentResponse();
            QVRequest.RemoteAddress = Request.ServerVariables["REMOTE_ADDR"];

            try
            {
                if (QVRequest.isValid(true))
                {
                    QVCommentActions CommentActions = new QVCommentActions(QVRequest);
                    CommentActions.LogTransaction("Add");
                    if (CommentActions.isVerified())
                    {
                        CommentActions.addComment();
                    }
                }
            }
            catch (HttpException ex)
            {
                QVResponse.Error = new ResponseError(ex);
            }

            JsonResult JsonResponse = new JsonResult();
            JsonResponse.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            JsonResponse.Data = QVResponse;
            return JsonResponse;
        }

    }
}
