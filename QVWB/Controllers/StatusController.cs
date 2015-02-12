using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QVWB.Models;
using QVWB.Areas.QVStatus;
namespace QVWB.Controllers
{
    public class StatusController : Controller
    {
        [HttpGet]
        public JsonResult GetStatus(QVStatusRequest QVRequest)
        {
            QVStatusResponse QVResponse = new QVStatusResponse();

            try
            {
                if (QVRequest.isValid(false))
                {
                    QVStatusActions StatusActions =  new QVStatusActions(QVRequest);
                    if(StatusActions.isVerified()){
                        QVResponse.Status = StatusActions.GetStatus();
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
        public JsonResult UpdateStatus(QVStatusRequest QVRequest)
        {
            QVStatusResponse QVResponse = new QVStatusResponse();
            QVRequest.RemoteAddress = Request.ServerVariables["REMOTE_ADDR"];

            try
            {
                if (QVRequest.isValid())
                {
                    QVStatusActions StatusActions = new QVStatusActions(QVRequest);
                    StatusActions.LogTransaction();
                    if (StatusActions.isVerified())
                    {
                        StatusActions.UpdateStatus();
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
