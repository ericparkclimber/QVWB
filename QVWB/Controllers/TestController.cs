using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QVWB.Controllers
{
    public class TestController : Controller
    {
        public string index()
        {
            return "";
        }

        [HttpGet]
        public string isReachable()
        {
            return "Qlik Writeback";
        }
    }
}
