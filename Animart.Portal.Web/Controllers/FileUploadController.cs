using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Animart.Portal.Web.Controllers
{
    public class FileUploadController : ApiController
    {
        [HttpPost()]
        public bool UploadFiles()
        {
            int iUploadedCnt = 0;

            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UserImage/");
            var webContext = System.Web.HttpContext.Current.Request;

            var id = webContext.Form.Get("id");
            System.Web.HttpFileCollection hfc = webContext.Files;

            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {
                    if (!File.Exists(sPath + Path.GetFileName(id + ".jpg")))
                    {
                        hpf.SaveAs(sPath + Path.GetFileName(id + ".jpg"));
                        iUploadedCnt = iUploadedCnt + 1;
                    }
                }
            }

            if (iUploadedCnt > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

