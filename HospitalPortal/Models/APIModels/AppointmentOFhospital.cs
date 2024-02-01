using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class AppointmentOFhospital
    {
        public int Doctor_Id { get; set; }
        public int Id { get; set; }
        public string DoctornName { get; set; }
        public string HospitalName { get; set; }
        public string Address { get; set; }
        public string MobileNo { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Specility { get; set; }
        public string AppointedTime { get; set; }
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string PatientMobileNumber { get; set; }
    }


    public class DoctorAppointmentByPatient
    {

        public string DoctorName { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        public string SpecialistName { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string Location { get; set; }
        public Nullable<double> TotalFee { get; set; }
        public string SlotTime { get; set; }
        public int Id { get; set; }

        public string MobileNumber { get; set; }
        public string DeviceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderId { get; set; }

    }
}