using HospitalPortal.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.BL
{
    public class GenerateBookingId
    {
        public string GenerateBooking4HC()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.CmpltCheckUps.OrderByDescending(a => a.Id).Select(a => a.BookingId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(2); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "HC000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "HC00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "HC0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "HC" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("Checkup ID overflow");
                    }
                }
                else
                {
                    return "HC0001";
                }
            }
        }
        public string GeneratePatientRegNo()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.Patients.OrderByDescending(a => a.Id).Select(a => a.PatientRegNo).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(2); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "PS000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "PS00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "PS0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "PS" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100000)
                    {
                        return "PS" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("Patient ID overflow");
                    }
                }
                else
                {
                    return "PS000";
                }
            }
        }
        public string GenerateDriverId()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.Drivers.OrderByDescending(a => a.Id).Select(a => a.DriverId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(3); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "DVR000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "DVR00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "DVR0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "DVR" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100000)
                    {
                        return "DVR" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("Driver ID overflow");
                    }
                }
                else
                {
                    return "DVR0001"; //id not exit 
                }
            }
        }
        public string GenerateNurseId()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.Nurses.OrderByDescending(a => a.Id).Select(a => a.NurseId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(2); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "N000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "N00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "N0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "N" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100000)
                    {
                        return "N" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("Nurse ID overflow");
                    }
                }
                else
                {
                    return "N0001";
                }
            }
        }
        public string GenerateLabId()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.Labs.OrderByDescending(a => a.Id).Select(a => a.lABId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(3); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "LAB000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "LAB00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "LAB0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "LAB" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100000)
                    {
                        return "LAB" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("Lab ID overflow");
                    }
                }
                else
                {
                    return "LAB0001"; //id not exit 
                }
            }
        }
        public string GenerateHealthCenterId()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.HealthCheckupCenters.OrderByDescending(a => a.Id).Select(a => a.HealthCheckUpId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(2); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "CH000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "CH00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "CH0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "CH" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("HealthCheckup ID overflow");
                    }
                }
                else
                {
                    return "CH0001";
                }
            }
        }
        public string GenerateHospitalId()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.Hospitals.OrderByDescending(a => a.Id).Select(a => a.HospitalId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(2); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "H000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "H00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "H0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "H" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("Hospital ID overflow");
                    }
                }
                else
                {
                    return "H0001";
                }
            }
        }
        public string GenerateDoctorId()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.Doctors.OrderByDescending(a => a.Id).Select(a => a.DoctorId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(2); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "DR000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "DR00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "DR0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "DR" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100000)
                    { 
                        return "DR" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("Doctor ID overflow");
                    }
                }
                else
                {
                    return "DR0001";  
                }
            }
        }
        public string GenerateChemistId()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.Chemists.OrderByDescending(a => a.Id).Select(a => a.ChemistId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(2); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "CH000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "CH00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "CH0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "CH" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100000)
                    {
                        return "CH" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("Chemist ID overflow");
                    }
                }
                else
                {
                    return "CH0001";
                }
            }
        }
        public string GenerateVenderId()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.Vendors.OrderByDescending(a => a.Id).Select(a => a.UniqueId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(2); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "FR000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "FR00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "FR0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "FR" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100000)
                    {
                        return "FR" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("Franchise ID overflow");
                    }
                }
                else
                {
                    return "FR0001";
                }
            }
        }
        public string GenerateRWA_Id()
        {
            using (DbEntities ent = new DbEntities())
            {
                string data = ent.RWAs.OrderByDescending(a => a.Id).Select(a => a.RWAId).FirstOrDefault();

                if (data != null)
                {
                    string PartitionValue = data.Substring(3); // Get the numeric part of the existing ID
                    int IncrementedVal = Convert.ToInt32(PartitionValue) + 1;

                    if (IncrementedVal < 10)
                    {
                        return "FRRWA000" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100)
                    {
                        return "FRRWA00" + IncrementedVal;
                    }
                    else if (IncrementedVal < 1000)
                    {
                        return "FRRWA0" + IncrementedVal;
                    }
                    else if (IncrementedVal < 10000)
                    {
                        return "FRRWA" + IncrementedVal;
                    }
                    else if (IncrementedVal < 100000)
                    {
                        return "FRRWA" + IncrementedVal;
                    }
                    else
                    {
                        throw new Exception("RWA ID overflow");
                    }
                }
                else
                {
                    return "FRRWA0001"; //id not exit 
                }
            }
        }
    }
}