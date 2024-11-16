using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class IncvoiceController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: Incvoice
        public ActionResult DoctorInvoiceReport()
        {
            string qry = "select pa.Id,d.DoctorId as UniqueId,d.DoctorName as ServiceProviderName,sm.StateName+','+cm.CityName+','+d.Location+','+ d.PinCode as Location,pa.AppointmentDate,pa.PaymentDate,p.PatientName,p.PatientRegNo,dm.DepartmentName,s.SpecialistName,pa.InvoiceNumber from PatientAppointment as pa join Doctor as d on d.Id=pa.Doctor_Id join StateMaster as sm on sm.Id=d.StateMaster_Id join CityMaster as cm on cm.Id=d.CityMaster_Id join Patient as p on p.Id=pa.Patient_Id join Department as dm on dm.Id=d.Department_Id join Specialist as s on s.Id=d.Specialist_Id where pa.IsCancelled=0 and pa.AppointmentIsDone=1";
            var data = ent.Database.SqlQuery<InvoiceDTO>(qry).ToList();
            return View(data);
           
        }

        public ActionResult DoctorInvoice(string InvoiceNumber)
        {
            try
            {
                var model = new InvoiceDTO();
                var gst = ent.GSTMasters.FirstOrDefault(x => x.IsDeleted == false && x.Name == "Doctor");
                if (gst == null)
                {
                    TempData["msg"] = "GST data not found for Doctors.";
                    return RedirectToAction("Index", "Home");
                }

                var invoiceData = (from d in ent.Doctors
                                   join pa in ent.PatientAppointments on d.Id equals pa.Doctor_Id
                                   join p in ent.Patients on pa.Patient_Id equals p.Id
                                   join sm in ent.StateMasters on d.StateMaster_Id equals sm.Id
                                   join cm in ent.CityMasters on d.CityMaster_Id equals cm.Id
                                   join psm in ent.StateMasters on d.StateMaster_Id equals psm.Id
                                   join pcm in ent.CityMasters on d.CityMaster_Id equals pcm.Id
                                   where pa.InvoiceNumber == InvoiceNumber
                                   select new InvoiceDTO
                                   {
                                       Patient_Id = (int)pa.Patient_Id,
                                       DoctorName = d.DoctorName,
                                       PAN = d.PAN,
                                       ClinicName = d.ClinicName,
                                       Fee = d.Fee,
                                       TotalFee = pa.TotalFee,
                                       GST = (double)gst.Amount,
                                       OrderId = pa.OrderId,
                                       InvoiceNumber = pa.InvoiceNumber,
                                       OrderDate = (DateTime)pa.OrderDate,
                                       PatientName=p.PatientName,
                                       PatientRegNo=p.PatientRegNo,
                                       EmailId=d.EmailId,
                                       PatientEmailId=p.EmailId,
                                       MobileNumber=p.MobileNumber,
                                       PinCode=d.PinCode,
                                       PatientPinCode = p.PinCode,
                                       Qualification=d.Qualification,
                                       StateName=sm.StateName,
                                       CityName=cm.CityName,
                                       Location = d.Location,
                                       PatientStateName = psm.StateName,
                                       PatientCityName = pcm.CityName,
                                       PatientLocation = p.Location,
                                       
                                   }).ToList();

                if (invoiceData.Any())
                {
                    decimal grandTotal = (decimal)invoiceData.Sum(item => item.TotalFee);
                    double totalAmount = invoiceData.Sum(item => item.Fee);
                    double gstAmount = (double)((totalAmount * gst.Amount) / 100);
                    double finalAmount = (double)grandTotal - gstAmount;
                    foreach(var item in invoiceData)
                    {
                        item.FinalAmount = (decimal)(double)finalAmount;
                        item.TotalFee = totalAmount;
                        item.GSTAmount = (decimal)gstAmount;
                        item.GrandTotal = grandTotal;
                        item.GST = (double)gst.Amount;
                    }

                    //int? patientId = invoiceData[0].Patient_Id;

                    //var patientData = ent.Patients.FirstOrDefault(x => x.Id == patientId);

                    //if (patientData != null)
                    //{
                    //    var viewModel = new InvoiceDTO
                    //    {
                    //        PatientName = patientData.PatientName,
                    //        EmailId = patientData.EmailId,
                    //        PinCode = patientData.PinCode,
                    //        MobileNumber = patientData.MobileNumber,
                    //        Location = patientData.Location,
                    //        InvoiceNumber = InvoiceNumber,
                    //        OrderId = invoiceData[0].OrderId,
                    //        OrderDate = invoiceData[0].OrderDate,
                    //        GST = (double)gst.Amount,
                    //        GSTAmount = gstAmount,
                    //        GrandTotal = grandTotal,
                    //        FinalAmount = finalAmount
                    //    };

                    //    return View(viewModel);
                    //}
                    //else
                    //{
                    //    TempData["msg"] = "Patient data not found";
                    //    return RedirectToAction("DoctorInvoiceReport");
                    //}
                    return View(invoiceData);
                }
                else
                {
                    TempData["msg"] = "No Invoice data found";
                    return RedirectToAction("DoctorInvoiceReport");
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server Error.";
                // Log the exception for further investigation
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult NurseInvoiceReport()
        {
            string qry = "select NS.Id,Nurse.NurseId as UniqueId,Nurse.NurseName as ServiceProviderName,Nurse.MobileNumber,(sm.StateName+','+cm.CityName+','+Nurse.Location+''+Nurse.PinCode)as Location,Nurse.Fee,NS.PaymentDate,NS.TotalFee,ns.Startdate, ns.Enddate, DATEDIFF(day, ns.Startdate,ns.Enddate) AS TotalNumberofdays,ns.InvoiceNumber,ns.OrderId,p.PatientName,p.PatientRegNo,p.MobileNumber,NS.InvoiceNumber from Nurse join NurseService as ns on ns.Nurse_Id=Nurse.Id join StateMaster as sm on sm.Id=Nurse.StateMaster_Id join CityMaster as cm on cm.Id=Nurse.CityMaster_Id join Patient as p on p.Id=ns.Patient_Id where NS.ServiceStatus='Approved' order by NS.Id desc";
            var data = ent.Database.SqlQuery<InvoiceDTO>(qry).ToList();
            return View(data);

        }
        public ActionResult NurseInvoice(string InvoiceNumber)
        {
            try
            {
                var model = new InvoiceDTO();
                var gst = ent.GSTMasters.Where(x => x.IsDeleted == false).FirstOrDefault(x => x.Name == "Nurse");
                var invoiceData = (from n in ent.Nurses
                                   join ns in ent.NurseServices on n.Id equals ns.Nurse_Id
                                   join p in ent.Patients on ns.Patient_Id equals p.Id
                                   join sm in ent.StateMasters on n.StateMaster_Id equals sm.Id
                                   join cm in ent.CityMasters on n.CityMaster_Id equals cm.Id
                                   join psm in ent.StateMasters on n.StateMaster_Id equals psm.Id
                                   join pcm in ent.CityMasters on n.CityMaster_Id equals pcm.Id
                                   join nt in ent.NurseTypes on n.NurseType_Id equals nt.Id
                                   where ns.InvoiceNumber == InvoiceNumber
                                   select new InvoiceDTO
                                   {
                                       Patient_Id = (int)ns.Patient_Id,
                                       ServiceProviderName = n.NurseName,
                                       NurseTypeName = nt.NurseTypeName,
                                       PAN = n.PAN, 
                                       Fee = n.Fee, 
                                       GST = (double)gst.Amount,
                                       OrderId = ns.OrderId,
                                       InvoiceNumber = ns.InvoiceNumber,
                                       OrderDate = (DateTime)ns.OrderDate,
                                       PatientName = p.PatientName,
                                       PatientRegNo = p.PatientRegNo,
                                       EmailId = n.EmailId,
                                       PatientEmailId = p.EmailId,
                                       MobileNumber = p.MobileNumber,
                                       PinCode = n.PinCode,
                                       PatientPinCode = p.PinCode, 
                                       StateName = sm.StateName,
                                       CityName = cm.CityName,
                                       Location = n.Location,
                                       PatientStateName = psm.StateName,
                                       PatientCityName = pcm.CityName,
                                       PatientLocation = p.Location,                                       
                                       TotalFee = (n.Fee) * DbFunctions.DiffDays(ns.StartDate, ns.EndDate) + (n.Fee * (gst.Amount / 100)), 
                                       TotalNumberofdays = (int)DbFunctions.DiffDays(ns.StartDate, ns.EndDate)
                                   }).ToList();

                if (invoiceData.Any())
                {
                    decimal grandTotal = (decimal)invoiceData.Sum(item => item.TotalFee);
                    double totalAmount = (double)invoiceData.Sum(item => item.TotalFee);
                    double gstAmount = (double)((totalAmount * gst.Amount) / 100);
                    double finalAmount = (double)grandTotal - gstAmount;
                    foreach (var item in invoiceData)
                    {
                        item.FinalAmount = (decimal)(double)finalAmount;
                        item.TotalFee = totalAmount;
                        item.GSTAmount = (decimal)gstAmount;
                        item.GrandTotal = grandTotal;
                        item.GST = (double)gst.Amount;
                    }
                     
                    return View(invoiceData);
                }
                else
                {
                    TempData["msg"] = "No Invoice data found";
                    return RedirectToAction("NurseInvoiceReport");
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server Error.";
                // Log the exception for further investigation
                return RedirectToAction("Error", "Home");
            }
        }

    }
}