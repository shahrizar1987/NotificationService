using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotificationService.App_Start;
using NotificationService.Data;
using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NotificationService.Controllers
{
    [RoutePrefix("Api/Send")]
    public class SendController : ApiController
    {
        [Route("Terminal")]
        [HttpGet]
        [ModelStateValidation]
        public async Task<IHttpActionResult> Terminal(string TID, string MID, string Title, string Body, int NotificationType)
        {
           
            if (!Request.IsLocal())
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            string SMS = string.Empty;
            string FCM = string.Empty;
           
            if (NotificationType == 1)
            {
                SMS = await new Tools().SMS(MID, Body);
            }
            else if (NotificationType == 2)
            {
                FCM = await new Tools().PushNotification(TID, MID, Title, Body);
            }
            else if (NotificationType == 3)
            {
                SMS = await new Tools().SMS(MID, Body);
                FCM = await new Tools().PushNotification(TID, MID, Title, Body);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Invalid Notification Type");
            }

            JObject obj = new JObject
            {
                { "Status",
                    new JObject
                    {
                        { "SMS", (string.IsNullOrEmpty(SMS)) ? "N/a" : SMS },
                        { "FCM", (string.IsNullOrEmpty(FCM)) ? "N/a" : FCM }
                    }
                }
            };

            return Ok(obj);
        }

        //[Route("Smartphone")]
        //[HttpGet]
        //[ModelStateValidation]
        //public async Task<IHttpActionResult> Smartphone()
        //{

        //    return Ok();
        //}

        [Route("GenericPush")]
        [HttpPost]
        [ModelStateValidation]
        public async Task<IHttpActionResult> GenericPush(GenericPush Req)
        {
            if (!Request.IsLocal())
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            JObject obj = new JObject
            {
                { "to", Req.Receiver },
                { "data",
                    new JObject
                    {
                        { "title", Req.Title },
                        { "body", Req.Body }
                    }
                }
            };

            var (Result, StatusCode) = await new Tools().SendFCM(Req.LegacyServerKey, obj);
            JObject json = JObject.Parse(Result);
            return Ok(json);
        }




    }
}
