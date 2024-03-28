using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class PayOutVM
    {
        public string LabName { get; set; }
        public string DoctorName { get; set; }
        public int Id { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public SelectList NurseTypeList { get; set; }
        public IEnumerable<PayOutNurseHistory> NurseHistory { get; set; }
        public IEnumerable<PayOutDocHistroy> PayHistory { get; set; }
        public IEnumerable<PayOutLabHistoty> LabHistory { get; set; }
        public IEnumerable<PayOutHealthHistoty> HealthHistory { get; set; }
        public IEnumerable<PayOutChemistHistoty> ChemistHistory { get; set; }
    }

    public class PayOutDocHistroy
    {
        public double NetAmount { get; set; }
        public int Doctor_Id { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public bool? IsPaid { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> Amountwithrazorpaycomm { get; set; }
    }

    public class PayOutNurseHistory
    {
        public string NurseTypeName { get; set; }
        public int Id { get; set; }
        public SelectList NurseTypeList { get; set; }
        public int NurseType_Id { get; set; }
        public double NetAmount { get; set; }
        public int Nurse_Id { get; set; }
        public string NurseId { get; set; }
        public string NurseName { get; set; }
        public bool? IsPaid { get; set; }
        public double Amount { get; set; }
		public double? Amountwithrazorpaycomm { get; set; }
	}


    public class PayOutLabHistoty
    {
        public double NetAmount { get; set; }
        public int Lab_Id { get; set; }
        public string lABId { get; set; }
        public string LabName { get; set; }
        public bool? IsPaid { get; set; }
        public double Amount { get; set; }
        public double Amountwithrazorpaycomm { get; set; }
    }

    public class PayOutHealthHistoty
    {
        public double NetAmount { get; set; }
        public int Center_Id { get; set; }
        public string LabName { get; set; }
        public bool? IsPaid { get; set; }
        public double Amount { get; set; }
    }


    public class PayOutChemistHistoty
    {
        public double NetAmount { get; set; }
        public int Chemist_Id { get; set; }
        public string ChemistId { get; set; }
        public string ChemistName { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime OrderDate { get; set; }
        public double Amount { get; set; }
        public double Amountwithrazorpaycomm { get; set; }
    }
}