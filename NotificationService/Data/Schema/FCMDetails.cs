using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationService.Data
{
    public class FCMDetails
    {
        public string DeviceRegToken { get; set; }
        public string LegacyServerKey { get; set; }
        public string Topic { get; set; }
    }
}