﻿using HospitalPortal.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class DriverComplaints
    {
        public int Id { get; set; }
        public Nullable<int> Login_Id { get; set; }
        public string Subjects { get; set; }
        public string Complaints { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsResolved { get; set; }
        public string Others { get; set; }

        public virtual Driver Driver { get; set; }
    }
}