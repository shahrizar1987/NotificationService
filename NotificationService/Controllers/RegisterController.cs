using NotificationService.Data;
using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NotificationService.Controllers
{
    [RoutePrefix("Api/Register")]
    public class RegisterController : ApiController
    {
        [Route("FCM")]
        [HttpPost]
        public async Task<IHttpActionResult> Firebase(RegisterFirebase Reg)
        {
            if (!Request.IsLocal())
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var Result = await new Mcard().PushNotificationInfo_Add("FCM", Reg.DeviceId, Reg.DeviceType, Reg.Channel, Reg.DeviceRegToken, Reg.Topic, Reg.LegacyServerKey, Reg.SenderID,
                "", "", "", "", "", "", "", "", "", "");

            if (Result == 0)
            {
                return Json(new { code = 200, message = "REGISTER SUCCESS" });
            }
            else if (Result == -1)
            {
                return Json(new { code = 998, message = "DATABASE ERROR" });
            }
            else
            {
                return Json(new { code = 999, message = "FAIL WITH ERROR TICKET " + Result });
            }
            
        }
    }
}
