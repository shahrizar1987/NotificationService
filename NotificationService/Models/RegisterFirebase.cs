using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationService.Models
{
    public class RegisterFirebase
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string Channel { get; set; }
        public string DeviceRegToken { get; set; }
        public string Topic { get; set; }
        public string LegacyServerKey { get; set; }
        public string SenderID { get; set; }
    }
}