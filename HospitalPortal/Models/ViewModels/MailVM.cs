using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class MailVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TelePhone { get; set; }
        public string Email { get; set; }
        public string Subjects { get; set; }
        public string Msg { get; set; }
    }
}