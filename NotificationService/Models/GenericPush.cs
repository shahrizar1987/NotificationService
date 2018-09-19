using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationService.Models
{
    public class GenericPush
    {
        public string LegacyServerKey { get; set; }
        public string Receiver { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}