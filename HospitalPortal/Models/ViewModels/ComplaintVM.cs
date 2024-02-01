using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ComplaintVM
    {
        public IEnumerable<ComplaintList> ComplaintList { get; set; }
        public IEnumerable<Complaint_Doc> Complaint_Doc { get; set; }
        
        public IEnumerable<Complaint_Hospital> Complaint_Hospital { get; set; }
        public IEnumerable<Complaint_Driver> Complaint_Driver { get; set; }
        public IEnumerable<Complaint_Pateint> Complaint_Patient { get; set; }
        public IEnumerable<Complaint_Ambulance> Complaint_Ambulance { get; set; }
        public int? Rating { get; set; }
        public string Name { get; set; }
        public string Others { get; set; }
        public Nullable<System.DateTime> ComplaintDate { get; set; }
    }

    public class ComplaintList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Complaint { get; set; }
        public string Others { get; set; }
        public bool IsResolved { get; set; }
        public Nullable<System.DateTime> ComplaintDate { get; set; }
    }
    public class Complaint_Doc
    {
        public Nullable<System.DateTime> ComplaintDate { get; set; }
        public int Id { get; set; }
        public string Subjects { get; set; }
        public int Login_Id { get; set; }
        public string DoctorName { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Complaints { get; set; }
        public string Others { get; set; }
        public bool IsResolved { get; set; }
    }


   









    public class Complaint_Hospital
    {
        public int Id { get; set; }
        public string Subjects { get; set; }
        public int Login_Id { get; set; }
        public string HospitalName { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Complaints { get; set; }
        public string Others { get; set; }
        public bool IsResolved { get; set; }
        public Nullable<System.DateTime> ComplaintDate { get; set; }
    }

    public class Complaint_Driver
    {
        public int Id { get; set; }
        public string Subjects { get; set; }
        public int Login_Id { get; set; }
        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Complaints { get; set; }
        public string Others { get; set; }
        public bool IsResolved { get; set; }
        public Nullable<System.DateTime> ComplaintDate { get; set; }
    }

    public class Complaint_Pateint
    {
        public int Id { get; set; }
        public string Subjects { get; set; }
        public int Login_Id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Complaints { get; set; }
        public string Others { get; set; }
        public bool IsResolved { get; set; }
        public Nullable<System.DateTime> ComplaintDate { get; set; }
    }

    public class Complaint_Ambulance
    {
        public int Id { get; set; }
        public string Subjects { get; set; }
        public int Login_Id { get; set; }
        public string VehicleNumber { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
       
        public string Role { get; set; }
        public string Complaints { get; set; }
        public string Others { get; set; }
        public bool IsResolved { get; set; }
        public Nullable<System.DateTime> ComplaintDate { get; set; }
    }
}