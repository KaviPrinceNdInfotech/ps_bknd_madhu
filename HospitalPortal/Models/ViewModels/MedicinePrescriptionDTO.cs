using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class MedicinePrescriptionDTO
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public Nullable<int> Doctor_Id { get; set; }
        public int? Patient_Id { get; set; }  // Property to hold the selected patient ID
        public SelectList Patient { get; set; }  // List of patient options
        public string Weight { get; set; }
        public string PresentComplaint { get; set; }
        public string Allergies { get; set; }
        public string Primarydiagnosis { get; set; }
        public string MedicineName1 { get; set; }
        public string Dosage1 { get; set; }
        public string Instruction1 { get; set; }
        public string MedicineName2 { get; set; }
        public string Dosage2 { get; set; }
        public string Instruction2 { get; set; }
        public string MedicineName3 { get; set; }
        public string Dosage3 { get; set; }
        public string Instruction3 { get; set; }
        public string MedicineName4 { get; set; }
        public string Dosage4 { get; set; }
        public string Instruction4 { get; set; }
        public string MedicineName5 { get; set; }
        public string Dosage5 { get; set; }
        public string Instruction5 { get; set; }
        public string MedicineName6 { get; set; }
        public string Dosage6 { get; set; }
        public string Instruction6 { get; set; }
        public string TestPrescribed { get; set; }
        public string MedicineName7 { get; set; }
        public string Dosage7 { get; set; }
        public string Instruction7 { get; set; }
        public string MedicineName8 { get; set; }
        public string Dosage8 { get; set; }
        public string Instruction8 { get; set; }
        public string MedicineName9 { get; set; }
        public string Dosage9 { get; set; }
        public string Instruction9 { get; set; }
        public string MedicineName10 { get; set; }
        public string Dosage10 { get; set; }
        public string Instruction10 { get; set; }
        public string PastMedical_SurgicalHistory { get; set; }
        public string Furtherrefferal_Recommendations { get; set; }
        public string PatientRegNo { get; set; }
        public string PatientName { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public int AgeValue { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
    }



    public class PrescriptionPdfModel
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string PatientRegNo { get; set; }
        public string PatientName { get; set; }
        public string EmailId { get; set; }
        public string DoctorEmailId { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string Weight { get; set; }
        public string PresentComplaint { get; set; }
        public string PastMedical_SurgicalHistory { get; set; }
        public string Allergies { get; set; }
        public string Primarydiagnosis { get; set; }
        public string TestPrescribed { get; set; }
        public string Furtherrefferal_Recommendations { get; set; }

        // Medicine-related information
        public string MedicineName1 { get; set; }
        public string Dosage1 { get; set; }
        public string Instruction1 { get; set; }
        public string MedicineName2 { get; set; }
        public string Dosage2 { get; set; }
        public string Instruction2 { get; set; }
        public string MedicineName3 { get; set; }
        public string Dosage3 { get; set; }
        public string Instruction3 { get; set; }
        public string MedicineName4 { get; set; }
        public string Dosage4 { get; set; }
        public string Instruction4 { get; set; }
        public string MedicineName5 { get; set; }
        public string Dosage5 { get; set; }
        public string Instruction5 { get; set; }
        public string MedicineName6 { get; set; }
        public string Dosage6 { get; set; }
        public string Instruction6 { get; set; }
        public string MedicineName7 { get; set; }
        public string Dosage7 { get; set; }
        public string Instruction7 { get; set; }
        public string MedicineName8 { get; set; }
        public string Dosage8 { get; set; }
        public string Instruction8 { get; set; }
        public string MedicineName9 { get; set; }
        public string Dosage9 { get; set; }
        public string Instruction9 { get; set; }
        public string MedicineName10 { get; set; }
        public string Dosage10 { get; set; }
        public string Instruction10 { get; set; }
        public  DateTime DOB { get; set; }
        public DateTime  EntryDate { get; set; }
        public string Qualification { get; set; }
        public string RegistrationNumber { get; set; }
        public string SignaturePic { get; set; }
    }



}