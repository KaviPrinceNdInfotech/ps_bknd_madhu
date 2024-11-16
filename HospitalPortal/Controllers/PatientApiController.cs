using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Web.Http; 
using System.Xml.Linq;
using static HospitalPortal.Controllers.TestApiController;
using static HospitalPortal.Utilities.EmailOperations;
using DriverDetail = HospitalPortal.Controllers.TestApiController.DriverDetail;
using ReturnMessage = HospitalPortal.Controllers.TestApiController.ReturnMessage;

namespace HospitalPortal.Controllers
{

    public class PatientApiController : ApiController
    {
        DbEntities ent = new DbEntities();
        ReturnMessage rm = new ReturnMessage();
        Rmwithparm rwithprm = new Rmwithparm();
        ILog log = LogManager.GetLogger(typeof(PatientApiController));

        [HttpPost]
        public IHttpActionResult UpdateProfile(PatientUpdateReq model)
        {
            
            try
            { 
                var data = ent.Patients.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "No record found";
                    return Ok(rm);
                }
                data.PatientName = model.PatientName;
                data.EmailId = model.EmailId;
                data.MobileNumber = model.MobileNumber;
                data.CityMaster_Id = model.CityMaster_Id;
                data.Location = model.Location;
                data.StateMaster_Id = model.StateMaster_Id;
                data.PinCode = model.PinCode;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Profile has updated.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        [HttpGet]
        public IHttpActionResult GetDoctorInfo(int cityId, int specialistId)
        {
            //var data = ent.Doctors.Where(a => !a.IsDeleted && a.IsApproved && a.CityMaster_Id == cityId && a.Specialist_Id == specialistId)
            var data = (from doctor in ent.Doctors
                        join state in ent.StateMasters on doctor.StateMaster_Id equals state.Id
                        join city in ent.CityMasters on doctor.CityMaster_Id equals city.Id
                        join specialist in ent.Specialists on doctor.Specialist_Id equals specialist.Id
                        join dept in ent.Departments on doctor.Department_Id equals dept.Id
                        join hospital in ent.Hospitals on doctor.HospitalId equals hospital.Id
                        into hospitalInfo
                        from Hospital in hospitalInfo.DefaultIfEmpty()
                        where doctor.CityMaster_Id == cityId && doctor.Specialist_Id == specialistId /*&& !doctor.IsDeleted*/
                        select new DoctorSearchResult
                        {
                            DoctorId = doctor.Id,
                            ClinicName = (doctor.HospitalId == null) ? doctor.ClinicName : Hospital.HospitalName,
                            DoctorName = (doctor.DoctorName),
                            EmailId = doctor.EmailId,
                            PhoneNumber = doctor.PhoneNumber,
                            MobileNumber = doctor.MobileNumber,
                            StateName = state.StateName,
                            CityName = city.CityName,
                            DepartmentName = dept.DepartmentName,
                            SpecialistName = specialist.SpecialistName,
                            Availability1 = doctor.StartTime,
                            Availability2 = doctor.EndTime,
                            Fee = doctor.Fee,
                            Location = doctor.Location,
                        }).ToList();
            foreach (var d in data)
            {
                var skills = ent.DoctorSkills.Where(a => a.Doctor_Id == d.DoctorId).Select(a => a.SkillName).ToList();
                if (skills.Count() == 0)
                {
                    d.Skills = "NA";
                }
                else
                {
                    d.Skills = string.Join(",", skills);
                }
            }
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["doctors"] = data;
            return Ok(dict);
        }

      
        [HttpPost]
        public IHttpActionResult MakeAppointment(PatientAppointmentVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }

                //DateTime data = Convert.ToDateTime(model.AppointmentDate);
                //DayOfWeek day = data.DayOfWeek;
                //if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                //{
                //    rwithprm.Status = 0;
                //    rwithprm.Message = "Not a Valid Request. Kindly try in weekdays only between MON-FRI.";
                //    return Ok(rwithprm);
                //}
                //var KalaHit = ent.Doctors.ToList();
                //TimeSpan? StartTime = KalaHit.FirstOrDefault().StartTime;
                //TimeSpan? EndTime = KalaHit.FirstOrDefault().EndTime;
                //TimeSpan? startSlot = model.StartSlotTime;
                //TimeSpan? EndSlot = model.EndSlotTime;
                //if (StartTime < startSlot && EndTime < EndSlot)
                //{
                //    msg.Status = 0;
                //    msg.Message = "Please Select Available Doctor Time.";
                //    return Ok(msg);
                //}
                //Hello New World


                string time1 = model.TimeSlot;
                string[] s = time1.Split('-');
                string StartTime = s[0];
                string EndTime = s[1];
                DateTime dt1 = DateTime.Parse(StartTime);
                DateTime dt2 = DateTime.Parse(EndTime);
                TimeSpan StartSlotTime = new TimeSpan(dt1.Hour, dt1.Minute, dt1.Second);
                TimeSpan EndSlotTime = new TimeSpan(dt2.Hour, dt2.Minute, dt2.Second);
                if (model != null)
                {
                    var Appointment = new PatientAppointment();
                    Appointment.Doctor_Id = model.Doctor_Id;
                    Appointment.Hospital_Id = model.Hospital_Id;
                    Appointment.HospitalDoc_Id = model.HospitalDoc_Id;
                    Appointment.AppointmentDate = model.AppointmentDate;
                    Appointment.Patient_Id = model.Patient_Id;
                    Appointment.Specialist_Id = model.Specialist_Id;
                    Appointment.EndSlotTime = EndSlotTime;
                    Appointment.StartSlotTime = StartSlotTime;
                    Appointment.TotalFee = model.Fee;
                    Appointment.IsBooked = true;
                    Appointment.IsCancelled = false;
                    ent.PatientAppointments.Add(Appointment);
                    ent.SaveChanges();
                    rwithprm.AppointmentId = Appointment.Id;
                    rwithprm.Message = "Your Booking has been done";
                    rwithprm.Status = 1;
                    return Ok(rwithprm);
                }
                else
                {
                    rwithprm.Status = 0;
                    rwithprm.Message = "Some Error Occurred";
                    return Ok(rwithprm);
                }
            }
            catch (Exception ex)
            {
                rwithprm.Status = 0;
                rwithprm.Message = "Server Error";
                return Ok(rwithprm);
            }
        }

        //View Appointments by Patient
        [HttpGet]
        public IHttpActionResult ShowAppointMent(int PatientId, DateTime? date = null)
        {
            var query = @"execute [sp_PatientAppointments] @patientId=" + PatientId;
            var data = ent.Database.SqlQuery<AppointmentSearchBy_Patient>(query).ToList();
            if (date != null)
            {
                string date1 = date.Value.ToString("yyyy/MM/dd");
                data = data.Where(a => a.AppointmentDate.Value.ToString("yyyy/MM/dd") == date1).ToList();
                if (data.Count() == 0)
                {
                    rm.Message = "No Records for Selected Date";
                    rm.Status = 0;
                    return Ok(rm);
                }
            }
            //var data1 = (from appointment in data
            //             join Doctors in ent.Doctors on appointment equals Doctors.Id
            //             join Speciality in ent.Specialists on appointment equals Speciality.Id
            //             where appointment == PatientId
            //             select new AppointmentSearchBy_Patient
            //             {
            //                 DoctornName = Doctors .DoctorName,
            //                 AppointmentDate = appointment,
            //                 MobileNo = Doctors.MobileNumber,
            //                 PhoneNo = Doctors.PhoneNumber,
            //                 ClinicAddress = Doctors.Location,
            //                 ClinicName = Doctors.ClinicName,
            //                 Specility = Speciality.SpecialistName,
            //                 //AppointmentEndTime = appointment.EndSlotTime,
            //                 //AppointmentStartTime = appointment.StartSlotTime,
            //                 AppointedTime = appointment.StartSlotTime + " - " + appointment.EndSlotTime,
            //             }).ToList();

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["Appointment"] = data;
            return Ok(dict);
        }


        //View Appointments of Hospitals Doctors by Patient
        [HttpGet]
        public IHttpActionResult ShowAppointMentOfDoctor(int PatientId, DateTime? date = null)
        {
            var query = @"select PatientAppointment.Id as AppointmentId, AppointmentDate, Doctor.MobileNumber as MobileNo ,isnull(PatientAppointment.HospitalDoc_Id,0) as HospitalDoc_Id, isnull(PatientAppointment.Hospital_Id,0) as Hospital_Id, 
PatientAppointment.Specialist_Id, Specialist.SpecialistName as Specility,
Doctor.DoctorName, h.Location as Address, h.HospitalName, Doctor.DoctorName as DoctornName, { fn concat(CONVERT(varchar(15),CAST(StartSlotTime AS TIME),100),
{fn concat ('-', CONVERT(varchar(15),CAST(EndSlotTime AS TIME),100))})} AS AppointedTime from PatientAppointment 
join HospitalDoctor Doctor on Doctor.Id = PatientAppointment.HospitalDoc_Id join Specialist on Doctor.Specialist_Id = Specialist.Id
join Hospital h on h.Id = Doctor.Hospital_Id 
where Patient_Id='" + PatientId + "' and MONTH(AppointmentDate) =  MONTH(GETDATE()) order by AppointmentDate desc";
            var data = ent.Database.SqlQuery<AppointmentOFhospital>(query).ToList();
            if (date != null)
            {
                data = data.Where(a => a.AppointmentDate == date).ToList();
                if (data.Count() == 0)
                {
                    rm.Message = "No Records for Selected Date";
                    rm.Status = 0;
                    return Ok(rm);
                }
            }
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["Appointment"] = data;
            return Ok(dict);
        }

        [HttpGet]
        //Show SlotTiming choosing DoctorId
        public IHttpActionResult SlotTiming(int DoctorId)
        {
            var model = new AppointmentClass();
            //            var query = @";WITH divide (n) AS ( SELECT SlotTime*(ROW_NUMBER() OVER (ORDER BY (SELECT Null))-1)
            //    FROM Doctor) SELECT Doctor.Id, TSStart=DATEADD(minute, n, StartTime),TSEnd=DATEADD(minute,n+ SlotTime, StartTime),Timeslot=CONVERT(VARCHAR(100), DATEADD(minute, n, StartTime), 0) + ' - ' +
            //        CONVERT(VARCHAR(100), DATEADD(minute, n+SlotTime, StartTime), 0)FROM Doctor 
            //CROSS APPLY( SELECT n FROM  divide WHERE n BETWEEN 0 AND DATEDIFF(minute, StartTime, DATEADD(minute, SlotTime, EndTime)) and Doctor.Id='" + DoctorId + "') a ORDER BY Doctor.Id, TSStart,TSEnd";
            var query = "execute usp_AppintmentSlots @doctorId=" + DoctorId;
            var data = ent.Database.SqlQuery<SlotTimingVM>(query).ToList();
            model.slottiming = data;
            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult TestBookingHistory(int PatientId, DateTime? date = null)
        {
            var model = new BookedTests();
            var query = @"select BookTestLab.Id, TestDate, Lab.LabName, Lab.PhoneNumber, Lab.MobileNumber, Lab.Location,
TestLab.TestDescription as TestName,  TestLab.TestAmount,{ fn concat(CONVERT(varchar(15),CAST(AvailabelTime1 AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(AvailableTime2 AS TIME),100))})} AS AvailableTime from BookTestLab 
join Lab on Lab.Id = BookTestLab.Lab_Id join LabTest on LabTest.Id = BookTestLab.Test_Id join TestLab on TestLab.Test_Id = LabTest.Id  where Patient_Id='" + PatientId + "'order by TestDate desc";
            var data = ent.Database.SqlQuery<BookingTestHistory>(query).ToList();
            if (date != null)
            {
                var q = @"select BookTestLab.Id, TestDate, Lab.LabName, Lab.PhoneNumber, Lab.MobileNumber, Lab.Location,
TestLab.TestDescription as TestName, TestLab.TestAmount,{ fn concat(CONVERT(varchar(15), CAST(AvailabelTime1 AS TIME), 100),{ fn concat ('-', CONVERT(varchar(15), CAST(AvailableTime2 AS TIME), 100))})}
                AS AvailableTime from BookTestLab
join Lab on Lab.Id = BookTestLab.Lab_Id join LabTest on LabTest.Id = BookTestLab.Test_Id join TestLab on TestLab.Test_Id = LabTest.Id  where Patient_Id = " + PatientId + " and Convert(Date, TestDate)= '" + date + "'order by TestDate desc";
                var data1 = ent.Database.SqlQuery<BookingTestHistory>(q).ToList();
                if (data1.Count() == 0)
                {
                    rm.Message = "No Record";
                    return Ok(rm);
                }
                model.BookingHistory = data1;
                return Ok(model);
            }
            model.BookingHistory = data;
            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult PaymentStatus(int AppointmentId)
        {
            try
            {
                string query = @"update PatientAppointment set PaymentDate=getdate(), IsPaid=1 where Id=" + AppointmentId;
                int Id = ent.Database.SqlQuery<int>(@"select Patient_Id from PatientAppointment where Id='" + AppointmentId + "'").FirstOrDefault();
                DateTime Date = ent.Database.SqlQuery<DateTime>(@"select Convert(date,AppointmentDate) from PatientAppointment where Id='" + AppointmentId + "'").FirstOrDefault();
                DateTime data = Convert.ToDateTime(Date);
                string Time = ent.Database.SqlQuery<string>(@"select { fn concat(CONVERT(varchar(15), CAST(StartSlotTime AS TIME), 100),{ fn concat ('-', CONVERT(varchar(15), CAST(EndSlotTime AS TIME), 100))})} AS AvailableTime from PatientAppointment where Id =" + AppointmentId + "").FirstOrDefault();
                string mobile = ent.Database.SqlQuery<string>(@"select MobileNumber from Patient where Id='" + Id + "'").FirstOrDefault();
                string Name = ent.Database.SqlQuery<string>(@"select PatientName from Patient where Id='" + Id + "'").FirstOrDefault();
                string msg = " You have been appointed through PSWELLNESS on " + data + ". Please visit application for more details.";
                ent.Database.ExecuteSqlCommand(query);
                Message.SendSms(mobile, msg);
                rm.Status = 1;
                rm.Message = "Successfully updated";
                return Ok(rm);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult CartCount(int id)
        {
            var model = new kartCount();
            string q = @"select Count(mc.Quantity) as kart from MedicineCart mc  where Patient_Id=" + id;
            var data = ent.Database.SqlQuery<kartCount>(q).ToList();
            model.kart = data.FirstOrDefault().kart;
            return Ok(model);
        }

        //Show Hospital Name using AutoComplete Technique
        [HttpGet]
        public IHttpActionResult SearchHospital(string term)
        {
            try
            {
                term = term.ToLower();
                var hospital = ent.Hospitals.Where(a => !a.IsDeleted && a.HospitalName.ToLower().Contains(term)).ToList();
                var data = (from m in hospital
                            join t in ent.HospitalDoctors
                            on m.Id equals t.Hospital_Id into tm
                            from t1 in tm.DefaultIfEmpty()
                            select new
                            {
                                m.Id,
                                m.HospitalName,
                            }).ToList();
                dynamic obj = new ExpandoObject();
                obj.Hospitals = data;
                return Ok(obj);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult CancelAppointment_ByDoctor(/*CancelAppointementDTO*/CancelAppointent model)
        {
            //string time1 = model.TimeSlot;
            //string[] s = time1.Split('-');
            //string StartTime = s[0];
            //string EndTime = s[1];
            //DateTime dt1 = DateTime.Parse(StartTime);
            //DateTime dt2 = DateTime.Parse(EndTime);
            //TimeSpan StartSlotTime = new TimeSpan(dt1.Hour, dt1.Minute, dt1.Second);
            //TimeSpan EndSlotTime = new TimeSpan(dt2.Hour, dt2.Minute, dt2.Second);
            //if (model != null)
            //{
            //    var Appointment = new CancelledAppointment();
            //    Appointment.Doctor_Id = model.Doctor_Id;
            //    Appointment.Hospital_Id = model.Hospital_Id;
            //    Appointment.HospitalDoc_Id = model.HospitalDoc_Id;
            //    Appointment.AppointmentDate = model.AppointmentDate;
            //    Appointment.Patient_Id = model.Patient_Id;
            //    Appointment.Specialist_Id = model.Specialist_Id;
            //    Appointment.EndSlotTime = EndSlotTime;
            //    Appointment.StartSlotTime = StartSlotTime;
            //    Appointment.Amount = model.Fee;
            //    Appointment.IsCancelled = true;
            //    Appointment.CancelledId = model.Id;
            //    ent.CancelledAppointments.Add(Appointment);
            //    ent.SaveChanges();
            //    var UserData = ent.Patients.Find(model.Id);
            //    double AppointmentCancelCharges = ent.Database.SqlQuery<double>(@"select Amount from Charges where Role='Doctor'").FirstOrDefault();
            //    double RestAmount = model.Fee - AppointmentCancelCharges;
            //    var Wallet = new UserWallet
            //    {
            //        UserId = model.Patient_Id,
            //        Amount = RestAmount,
            //        AdminId = UserData.AdminLogin_Id,
            //        TransactionType = "cr"
            //    };
            //    ent.UserWallets.Add(Wallet);
            //    ent.SaveChanges();

            if (model.Id != 0)
            {
                
                string q = @"Update PatientAppointment set IsCancelled=1 where Id=" + model.Id;
                ent.Database.ExecuteSqlCommand(q);
                double Amount = ent.Database.SqlQuery<double>(@"select TotalFee from PatientAppointment where Id =" + model.Id + "").FirstOrDefault();
                int Pateintid = ent.Database.SqlQuery<int>(@"select Patient_Id from PatientAppointment where Id =" + model.Id + "").FirstOrDefault();

                if (Amount != null)
                {
                    var emp = ent.Patients.FirstOrDefault(a => a.Id == Pateintid);

                    emp.walletAmount = (decimal?)((double?)emp.walletAmount + Amount);
                    ent.SaveChanges();
                }
                else
                {
                    return BadRequest("Please enter the amount");
                }

                var Wallet = new PenaltyAmount
                {
                    Patient_Id = Pateintid,
                    Amount = ((decimal?)Amount*10)/100,
                    UserWalletAmount = (decimal?)Amount,
                    CancelDate = DateTime.Now,
                    Pro_Id = model .Pro_Id,

                };
                ent.PenaltyAmounts.Add(Wallet);
                ent.SaveChanges();
                //rwithprm.Message = "Successfully Cancelled Your Appointment";
                //rwithprm.Message = "Successfully Cancelled Your Appointment And Your Amount Is Credited Your wallet!!!";
                rwithprm.Status = 1;
                return Ok("Successfully Cancelled Your Appointment And Your Amount Is Credited Your wallet!!!");
            }
            //}
            else
            {
                rwithprm.Status = 0;
                rwithprm.Message = "Some Error Occurred";
                return Ok(rwithprm);
            }
        }
        [HttpPost]
        public IHttpActionResult CancelAppointment_ByNurse(/*CancelAppointementDTO*/CancelAppointent model)
        {
            //string time1 = model.TimeSlot;
            //string[] s = time1.Split('-');
            //string StartTime = s[0];
            //string EndTime = s[1];
            //DateTime dt1 = DateTime.Parse(StartTime);
            //DateTime dt2 = DateTime.Parse(EndTime);
            //TimeSpan StartSlotTime = new TimeSpan(dt1.Hour, dt1.Minute, dt1.Second);
            //TimeSpan EndSlotTime = new TimeSpan(dt2.Hour, dt2.Minute, dt2.Second);
            //if (model != null)
            //{
            //    var Appointment = new CancelledAppointment();
            //    Appointment.Doctor_Id = model.Doctor_Id;
            //    Appointment.Hospital_Id = model.Hospital_Id;
            //    Appointment.HospitalDoc_Id = model.HospitalDoc_Id;
            //    Appointment.AppointmentDate = model.AppointmentDate;
            //    Appointment.Patient_Id = model.Patient_Id;
            //    Appointment.Specialist_Id = model.Specialist_Id;
            //    Appointment.EndSlotTime = EndSlotTime;
            //    Appointment.StartSlotTime = StartSlotTime;
            //    Appointment.Amount = model.Fee;
            //    Appointment.IsCancelled = true;
            //    Appointment.CancelledId = model.Id;
            //    ent.CancelledAppointments.Add(Appointment);
            //    ent.SaveChanges();
            //    var UserData = ent.Patients.Find(model.Id);
            //    double AppointmentCancelCharges = ent.Database.SqlQuery<double>(@"select Amount from Charges where Role='Doctor'").FirstOrDefault();
            //    double RestAmount = model.Fee - AppointmentCancelCharges;
            //    var Wallet = new UserWallet
            //    {
            //        UserId = model.Patient_Id,
            //        Amount = RestAmount,
            //        AdminId = UserData.AdminLogin_Id,
            //        TransactionType = "cr"
            //    };
            //    ent.UserWallets.Add(Wallet);
            //    ent.SaveChanges();

            if (model.Id != 0)
            {

                string q = @"update NurseService set ServiceStatus='Cancelled' where Id=" + model.Id;
                ent.Database.ExecuteSqlCommand(q);
                double Amount = ent.Database.SqlQuery<double>(@"select TotalFee from NurseService where Id=" + model.Id + "").FirstOrDefault();
                int Pateintid = ent.Database.SqlQuery<int>(@"select Patient_Id from NurseService where Id =" + model.Id + "").FirstOrDefault();

                if (Amount != null)
                {
                    var emp = ent.Patients.FirstOrDefault(a => a.Id == Pateintid);

                    emp.walletAmount = (decimal?)((double?)emp.walletAmount + Amount);
                    ent.SaveChanges();
                }
                else
                {
                    return BadRequest("Please enter the amount");
                }

                var Wallet = new PenaltyAmount
                {
                    Patient_Id = Pateintid,
                    Amount = ((decimal?)Amount * 0) / 100,
                    UserWalletAmount = (decimal?)Amount,
                    CancelDate = DateTime.Now,
                    Pro_Id = model.Pro_Id,

                };
                ent.PenaltyAmounts.Add(Wallet);
                ent.SaveChanges();
                //rwithprm.Message = "Successfully Cancelled Your Appointment";
                //rwithprm.Message = "Successfully Cancelled Your Appointment And Your Amount Is Credited Your wallet!!!";
                rwithprm.Status = 1;
                return Ok("Successfully Cancelled Your Appointment And Your Amount Is Credited Your wallet!!!");
            }
            //}
            else
            {
                rwithprm.Status = 0;
                rwithprm.Message = "Some Error Occurred";
                return Ok(rwithprm);
            }
        }


        [HttpPost]
        [Route("api/PatientApi/CancelNurseAppointment")]
        public IHttpActionResult CancelNurseAppointment(NurseCancelApp model)
        {
            if (model.Id != 0)
            {
                string q = @"update NurseService set ServiceStatus='Cancelled' where Id=" + model.Id;
                ent.Database.ExecuteSqlCommand(q);
                double Amount = ent.Database.SqlQuery<double>(@"select TotalFee from NurseService where Id=" + model.Id+"").FirstOrDefault();
                
                //                var ServiceData = ent.NurseServices.Find(model.Id);
                //                var UserData = ent.Patients.Where(a => a.Id == ServiceData.Patient_Id).ToList();
                //                double AppointmentCancelCharges = ent.Database.SqlQuery<double>(@"select Amount from Charges where Role='Nurse'").FirstOrDefault();
                //                var TotalFee = ent.Database.SqlQuery<double>(@"select IsNull(Datediff(day,ns.StartDate,ns.EndDate)*ns.PerDayAmount,0) as TotalFee
                // from NurseService ns 
                //left join Nurse n on ns.Nurse_Id=n.Id
                //where ns.Id=" + model.Id).FirstOrDefault();
                //                double RestAmount = TotalFee - AppointmentCancelCharges;
                if (Amount != null)
                {
                    var emp = ent.Patients.FirstOrDefault(a => a.Id == model.Patient_Id);
                   
                        emp.walletAmount = (decimal?)((double?)emp.walletAmount + Amount);
                    ent.SaveChanges();
                }
                else
                {
                    return BadRequest("Please enter the amount");
                }
            }
            else
            {
                rwithprm.Status = 0;
                rwithprm.Message = "Some Error Occurred";
                return Ok(rwithprm);
            }
            rm.Status = 1;
            return Ok("Successfully Cancelled Your Appointment");
        }


        //new(from User Side)
        [HttpPost]
        [Route("api/PatientApi/CancelDoctorAppointment")]
        public IHttpActionResult CancelDoctorAppointment(NurseCancelApp model)
        {
            if (model.Id != 0)
            {
                string q = @"Update PatientAppointment set IsCancelled=1 where Id=" + model.Id;
                ent.Database.ExecuteSqlCommand(q);
                double Amount = ent.Database.SqlQuery<double>(@"select TotalFee from PatientAppointment where Id =" + model.Id + "").FirstOrDefault();
               
                if (Amount != null)
                {
                    var emp = ent.Patients.FirstOrDefault(a => a.Id == model.Patient_Id);

                    emp.walletAmount = (decimal?)((double?)emp.walletAmount + Amount);
                    ent.SaveChanges();
                }
                else
                {
                    return BadRequest("Please enter the amount");
                }
            }
            else
            {
                rwithprm.Status = 0;
                rwithprm.Message = "Some Error Occurred";
                return Ok(rwithprm);
            }
            return Ok("Successfully Cancelled Your Appointment And Your Amount Is Credited Your wallet!!!");
        }

        [HttpPost]
        [Route("api/PatientApi/PatientRegistration")]
        public IHttpActionResult PatientRegistration(PatientDTO model)
        {
            GenerateBookingId Patient = new GenerateBookingId();
            if (ent.Patients.Any(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber))
            {
                var data = ent.Patients.Where(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                var logdata = ent.AdminLogins.Where(a => a.UserID == data.PatientRegNo).FirstOrDefault();
                string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                Message.SendSms(logdata.PhoneNumber, mssg);
                return Ok("you are already registered with pswellness");
            }
            else
            {
                var admin = new AdminLogin
                {
                    Username = model.EmailId.ToLower(),
                    PhoneNumber = model.MobileNumber,
                    Password = model.Password,
                    Confirmpassword = model.ConfirmPassword,
                    Role = "patient"
                };
                ent.AdminLogins.Add(admin);
                ent.SaveChanges();

                var domainModel = Mapper.Map<Patient>(model);
                domainModel.AdminLogin_Id = admin.Id;
                domainModel.Reg_Date = DateTime.Now;
                domainModel.IsApproved = true;
                domainModel.EmailId = model.EmailId.ToLower();
                domainModel.PatientRegNo = Patient.GeneratePatientRegNo();
                admin.UserID = domainModel.PatientRegNo;
                ent.Patients.Add(domainModel);
                ent.SaveChanges();
                string msg = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.PatientRegNo + "), Password : " + admin.Password + ".";
                Message.SendSms(domainModel.MobileNumber, msg);
                string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + admin.UserID + "), Password : " + admin.Password + ".";

                Utilities.EmailOperations.SendEmail1(model.EmailId, "Ps Wellness", msg1, true);
                return Ok("SuccessFul");
            }


        }

        //View ShowAppointmentOfDoctor by Patient (physical report)
        [HttpGet]

        //physical booking detail
        public IHttpActionResult DoctorAptP(int PatientId)
        {
            //var model = new LabVM();
            var query = @"select  IsNull(ca.AppointmentDate,PatientAppointment.AppointmentDate) as AppointmentDate,PatientAppointment.PaymentDate,CONCAT(CONVERT(NVARCHAR, TS.StartTime, 8), ' To ', CONVERT(NVARCHAR, TS.EndTime, 8)) AS SlotTime,Specialist.SpecialistName,Doctor.Location,PatientAppointment.TotalFee,PatientAppointment.Id,Doctor.DoctorName,Doctor.MobileNumber,AL.DeviceId,PatientAppointment.InvoiceNumber,PatientAppointment.OrderId from PatientAppointment
join Doctor on Doctor.Id = PatientAppointment.Doctor_Id 
left join Specialist on Doctor.Specialist_Id = Specialist.Id 
join DoctorTimeSlot as TS on PatientAppointment.Slot_id=TS.Id
left join CancelledAppointment ca on ca.CancelledId = PatientAppointment.Id 
join AdminLogin as AL on AL.Id=Doctor.AdminLogin_Id 
where PatientAppointment.Patient_Id=" + PatientId + " and PatientAppointment.IsCancelled=0 and PatientAppointment.BookingMode_Id=1 and PatientAppointment.IsBooked=1 order by PatientAppointment.Id desc";
            var data = ent.Database.SqlQuery<DoctorAppointmentByPatient>(query).ToList();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["Appointment"] = data;
            return Ok(dict);
        }

        [HttpGet]
        [Route("api/PatientApi/DoctorVirtualAptByPatient")]
        public IHttpActionResult DoctorVirtualAptByPatient(int PatientId)
        {
            var query = @"select  IsNull(ca.AppointmentDate,PatientAppointment.AppointmentDate) as AppointmentDate,PatientAppointment.PaymentDate,CONCAT(CONVERT(NVARCHAR, TS.StartTime, 8), ' To ', CONVERT(NVARCHAR, TS.EndTime, 8)) AS SlotTime,Specialist.SpecialistName,Doctor.Location,PatientAppointment.TotalFee,PatientAppointment.Id,Doctor.DoctorName,Doctor.MobileNumber,AL.DeviceId,PatientAppointment.InvoiceNumber,PatientAppointment.OrderId from PatientAppointment
join Doctor on Doctor.Id = PatientAppointment.Doctor_Id 
left join Specialist on Doctor.Specialist_Id = Specialist.Id 
join DoctorTimeSlot as TS on PatientAppointment.Slot_id=TS.Id
left join CancelledAppointment ca on ca.CancelledId = PatientAppointment.Id 
join AdminLogin as AL on AL.Id=Doctor.AdminLogin_Id 
where PatientAppointment.Patient_Id=" + PatientId + " and PatientAppointment.IsCancelled=0 and PatientAppointment.BookingMode_Id=2 and PatientAppointment.IsBooked=1 order by PatientAppointment.Id desc";
            var data = ent.Database.SqlQuery<DoctorAppointmentByPatient>(query).ToList();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["Appointment"] = data;
            return Ok(dict);
        }


        [HttpGet]

        public IHttpActionResult LabDetailsByPatient(int PatientId)
        {
            var LabHistory = ent.Database.SqlQuery<LabModel1>(@"select distinct lbt.Id,L.LabName,LT.TestDesc as TestName,lbt.TestDate,lbt.Amount as TestAmount,L.Location,lbt.InvoiceNumber,lbt.OrderId from BookTestLab AS lbt with(nolock)
join LabTest AS LT with(nolock) ON lbt.Test_Id = LT.Id
join TestInLab as TL on TL.Test_Id=LT.Id
join Lab as L on L.Id=lbt.Lab_Id
WHERE lbt.Patient_Id =" + PatientId + "order by lbt.id desc").ToList();
            return Ok(new { kk = LabHistory });
        }


        [HttpGet]
        public IHttpActionResult PatientProfiledetail(int PatientId)
        {
            string query = @"select patient.Id,PatientName,EmailId ,MobileNumber,Location,PinCode,CityName,StateName,Patient.StateMaster_Id,Patient.CityMaster_Id from Patient
join CityMaster on CityMaster.Id=Patient.CityMaster_Id
join StateMaster on StateMaster.Id=Patient.StateMaster_Id
where Patient.Id = " + PatientId + "";
            var data = ent.Database.SqlQuery<PatientprofileDetail>(query).FirstOrDefault();
            //model.Patientprofile = data;
            return Ok(data);
        }

        //========================Nurse History=======================//
        [HttpGet]
        public IHttpActionResult AppoinmentHistory(int Id)  
        {
            if (Id != 0)
            {
                string query = @"select NS.Id,Nurse.NurseName,Nurse.MobileNumber,Nurse.Location,Nurse.Fee,NS.PaymentDate,NS.TotalFee,ns.Startdate, ns.Enddate, DATEDIFF(day, ns.Startdate,ns.Enddate) AS TotalNumberofdays,AL.DeviceId,ns.InvoiceNumber,ns.OrderId from Nurse
left join NurseService as NS on Nurse.Id=ns.Nurse_Id  
left join AdminLogin as AL on AL.Id=Nurse.AdminLogin_Id  
        where  NS.Patient_Id=" + Id + " and NS.ServiceStatus='Approved' order by NS.Id desc";
                var data = ent.Database.SqlQuery<NurseAppointmentDetail>(query).ToList();
                return Ok(new { data });
            }

            else
            {
                return Ok(new { Message = "No record found" });
            }
        }


        [HttpPost]
        [Route("api/PatientApi/UpdateProfilebyPatient")]
        public IHttpActionResult UpdateProfilebyPatient(PatientUpdate model)
        {

            try
            {
                var data = ent.Patients.Where(a => a.Id == model.Id).FirstOrDefault();
                data.PatientName = model.patientName;
                data.MobileNumber = model.MobileNumber;
                data.StateMaster_Id = model.StateMaster_Id;
                data.CityMaster_Id = model.CityMaster_Id;
                data.Location = model.Location;
                data.PinCode = model.PinCode;
                //var d = ent.BankDetails.Where(a => a.Login_Id == model.adminLogin_id).FirstOrDefault();
                //d.AccountNo = model.AccountNo;
                //d.BranchName = model.BranchName;
                //d.IFSCCode = model.IFSCCode;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Profile has updated.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);

        }
        // ==============================Address post api ============================================         ////////////////////////////////////////
        [HttpPost]
        public IHttpActionResult MedicineAddress(AddAddressMedicine Model)
        {
            try
            {
                
                var data = new Medicinedeliver();
                {
                    data.Name = Model.Name;
                    data.Email = Model.Email;
                    data.MobileNumber = Model.MobileNumber;
                    data.StateMaster_Id = Model.StateMaster_Id;
                    data.CityMaster_Id = Model.CityMaster_Id;
                    data.DeliveryAddress = Model.DeliveryAddress;
                    data.PinCode = Model.PinCode;
                    data.Patient_Id = Model.Patient_Id;
                };
                ent.Medicinedelivers.Add(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " data Inserted Successfully";

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }


        [HttpGet]
        [Route("api/PatientApi/GetMedicineAddress")]
        public IHttpActionResult GetMedicineAddress(int Patient_id)
        {
            var model = new AddAddress();
            string query = @"select Medicinedeliver.id,Medicinedeliver.Name,Medicinedeliver.Email,Medicinedeliver.MobileNumber,Medicinedeliver.DeliveryAddress,Medicinedeliver.PinCode,
                sm.StateName,cm.CityName from Medicinedeliver
             join stateMaster as sm on StateMaster_Id = sm.Id
            join CityMaster cm on cm.Id = Medicinedeliver .CityMaster_Id where Medicinedeliver.Patient_id=" + Patient_id+" ";
            var data = ent.Database.SqlQuery<AddAddressMedicines>(query).ToList();
            model.AddAddressMediciness = data;
            return Ok(model);
        }
         
        //================= RWA ADD patient ============//
        
        [HttpPost, Route("api/PatientApi/AddPatient")]
        public IHttpActionResult AddPatient(PatientDTO model)
        { 
            GenerateBookingId Patient = new GenerateBookingId();
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                        rm.Message = message;
                        rm.Status = 0;
                        return Ok(rm);
                    }

                    if (ent.Patients.Any(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Patients.Where(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        //var logData = ent.AdminLogins.Where(a => a.UserID == model.PatientRegNo).FirstOrDefault();
                        //string mssg = "Welcome to PSWELLNESS. Your User Name : " + logData.Username + "(" + logData.UserID + "), Password : " + logData.Password + ".";
                        //Message.SendSms(logData.PhoneNumber, mssg);
                        return Ok("You are already registered with pswellness.");
                    }
                    else
                    {
                        var admin = new AdminLogin
                        {
                            Username = model.EmailId.ToLower(),
                            PhoneNumber = model.MobileNumber,
                            Password = model.Password,
                            Confirmpassword = model.ConfirmPassword,
                            Role = "patient",
                        };
                        ent.AdminLogins.Add(admin);
                        ent.SaveChanges();

                        var domenModel = new Patient()
                        {
                            AdminLogin_Id = admin.Id,
                            Rwa_Id = model.Rwa_Id,
                            PatientName = model.PatientName,
                            EmailId = model.EmailId.ToLower(),
                            MobileNumber = model.MobileNumber,
                            Password = model.Password,
                            Location = model.Location,
                            PinCode = model.PinCode,
                            IsApproved = true,
                            Reg_Date = DateTime.Now,
                            PatientRegNo = Patient.GeneratePatientRegNo(),
                        };
                        admin.UserID = domenModel.PatientRegNo;
                        ent.Patients.Add(domenModel);
                        ent.SaveChanges();
                        tran.Commit();

                        string sms = "Welcome to PSWELLNESS. Your UserName : " + domenModel.EmailId + "(" + domenModel.PatientRegNo + "), Password : " + admin.Password + ".";
                        Message.SendSms(domenModel.MobileNumber, sms);

                        //string EmailMsg = "Welcome to PSWELLNESS. Your UserName : " + domenModel.EmailId + "(" + domenModel.PatientRegNo + "), Password : " + admin.Password + ".";
                        //Utilities.EmailOperations.SendEmail(domenModel.EmailId, "PS-Wellness", EmailMsg, true);

                        string msg = @"<!DOCTYPE html>
<html>
<head>
    <title>PS Wellness Registration Confirmation</title>
</head>
<body>
    <h1>Welcome to PS Wellness!</h1>
    <p>Your signup is complete. To finalize your registration, please use the following login credentials:</p>
    <ul>
        <li><strong>User ID:</strong> " + admin.UserID + @"</li>
        <li><strong>Password:</strong> " + admin.Password + @"</li>
    </ul>
    <p>Thank you for choosing PS Wellness. We look forward to assisting you on your wellness journey.</p>
</body>
</html>";

                        EmailEF ef = new EmailEF()
                        {
                            EmailAddress = model.EmailId,
                            Message = msg,
                            Subject = "PS Wellness Registration Confirmation"
                        };

                        EmailOperations.SendEmainew(ef);

                        var message = "Patient Added successfully.";
                        rm.Message = message;
                        rm.Status = 200;
                        return Ok(rm);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }
        }
       
       //===================update profile===================//
       
        [HttpPost, Route("api/PatientApi/UpdateRWA_Data")]
        public IHttpActionResult UpdateRWA_Data(RWA_Registration model)
        { 
            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
             
                try
                {
                    var RWA_Data = ent.RWAs.Where(a => a.Id == model.ID).FirstOrDefault();
                    if (RWA_Data != null)
                    {
                        //var img = FileOperation.UploadFileWithBase64("Images", model.CertificateImage, model.CertificateImagebase64, allowedExtensions);
                        //if (img == "not allowed")
                        //{
                        //    rm.Status = 0;
                        //    rm.Message = "Only png,jpg,jpeg files are allowed.";
                           
                        //    return Ok(rm);
                        //}
                        //model.CertificateImage = img;

                        var query = @"UPDATE RWA SET AuthorityName= '" + model.AuthorityName + "', LandlineNumber='" + model.LandlineNumber + "', StateMaster_Id=" + model.StateMaster_Id + ", CityMaster_Id=" + model.CityMaster_Id + ", Location='" + model.Location + "', EmailId='" + model.EmailId + "', Pincode='" + model.Pincode + "' WHERE Id=" + model.ID + " ";
                        ent.Database.ExecuteSqlCommand(query);
                        ent.SaveChanges();  
                        rm.Message = "Updated Successfully.";
                        rm.Status = 200;
                        return Ok(rm);
                    }
                    else
                    {
                        rm.Message = "Not Found.";
                        rm.Status = 404;
                        return Ok(rm);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message); 
                    return InternalServerError(ex);
                }
        }
        //Get-RWA-Profile-Data==== complete ================
        [HttpGet]
        [Route("api/PatientApi/GetRWA_ProfileDetails")]
        public IHttpActionResult GetRWA_ProfileDetails(int RWA_Id)
        { 
            try
            {
                var RWALogin = ent.RWAs.FirstOrDefault(a => a.Id == RWA_Id);
                if (RWALogin == null)
                {
                    var msg = "RWA_Id is not correct.";
                    rm.Status = 0;
                    rm.Message = msg;
                    return Ok(rm);
                }
                else
                {
                    var query ="SELECT A.Id, A.AuthorityName, A.PhoneNumber, A.EmailId, A.Location, A.PinCode,A.CityMaster_Id,A.StateMaster_Id, (SELECT StateName FROM StateMaster WHERE Id=A.StateMaster_Id) AS StateName, (SELECT CityName FROM CityMaster WHERE ID=A.CityMaster_Id) AS CityName FROM RWA A WHERE A.Id= " + RWA_Id + "";

                    var data = ent.Database.SqlQuery<RWA_ProfileDetails>(query).FirstOrDefault();
                   // Dictionary<string, object> dict = new Dictionary<string, object>();
                    //dict["RWA_ProfileDetails"] = data;
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("api/PatientApi/GetPatientList")]
        public IHttpActionResult GetPatientList(int RWA_Id)
        { 
            try
            {
                var RWALogin = ent.RWAs.FirstOrDefault(a => a.Id == RWA_Id);
                if (RWALogin == null)
                {
                    var msg = "RWA_Id is not correct.";
                    rm.Status = 0;
                    rm.Message = msg;
                    return Ok(rm);
                }
                else
                {
                    var query = "SELECT Id,PatientName,MobileNumber,EmailId,Location,PinCode FROM Patient WHERE Rwa_Id = " + RWA_Id + "";

                    var data = ent.Database.SqlQuery<PatientData>(query).ToList();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    dict["Patient"] = data;
                    return Ok(dict);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }

        }
        [HttpGet]
        [Route("api/PatientApi/GetRWA_PayoutList")]
        public IHttpActionResult GetRWA_PayoutList(int RWA_Id)
        { 
            try
            {
                var RWALogin = ent.RWAs.FirstOrDefault(a => a.Id == RWA_Id);
                if (RWALogin == null)
                {
                    var msg = "RWA_Id is not correct.";
                    rm.Status = 0;
                    rm.Message = msg;
                    return Ok(rm);
                }
                else
                {
                    var query = "SELECT RP.Id,RP.Amount,RP.PaymentDate, R.AuthorityName FROM RwaPayout RP INNER JOIN RWA R ON R.Id=RP.Rwa_Id WHERE RP.RWA_Id =" + RWA_Id + "";

                    var data = ent.Database.SqlQuery<GetRWA_Payout>(query).ToList();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    dict["RWA_PayoutList"] = data;
                    return Ok(dict);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }


        //================Get-RWA-Payment-Report-Data====================//

        [HttpGet]
        [Route("api/PatientApi/GetRWA_PaymentReport")]
        public IHttpActionResult GetRWA_PaymentReport(int RWA_Id)
        { 
                try
                {
                    var RWALogin = ent.RWAs.FirstOrDefault(a => a.Id == RWA_Id);
                    if (RWALogin == null)
                    {
                        var msg = "RWA_Id is not correct.";
                        rm.Status = 0;
                        rm.Message = msg;
                        return Ok(rm);
                    }
                    else
                    {
                        //                       var query = @"SELECT PR.Id, PR.Bank_Id, PR.User_Id, PR.RWA_Id, PR.PaymentStatus, CONVERT(VARCHAR(11), PR.PaymentDate, 103) AS PaymentDate, FORMAT(PR.PaymentDate, 'hh:mm') AS PaymentTime, PR.PaidAmount,
                        //(SELECT BranchName FROM BankDetails WHERE Id = PR.Bank_Id) AS BankName, (SELECT PatientName FROM Patient WHERE Id = PR.[User_Id]) AS PatientName FROM RWA_PaymentReport PR WHERE PR.RWA_Id = " + RWA_Id + "";
                        var query = @"SELECT  PR.Id,CONVERT(VARCHAR(11), PR.PaymentDate, 103) AS PaymentDate, FORMAT(PR.PaymentDate, 'hh:mm') AS PaymentTime, PR.PaidAmount,
                                        (SELECT BranchName FROM BankDetails WHERE Id = PR.Bank_Id) AS BankName, (SELECT PatientName FROM Patient WHERE Id = PR.[User_Id]) AS PatientName FROM RWA_PaymentReport PR WHERE PR.RWA_Id =" + RWA_Id + "";
                        var data = ent.Database.SqlQuery<RWA_PaymentReports>(query).ToList();
                        Dictionary<string, object> dict = new Dictionary<string, object>();
                        dict["RWA_PaymentReport"] = data;
                        return Ok(dict);
                    }

                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    return InternalServerError(ex);
                }
           
        }

        
        //================ Add-RWA-Complaint====Complete============//
        
        [HttpPost, Route("api/PatientApi/Add_RWAComplaint")]
        public IHttpActionResult Add_RWAComplaint(RWAComplaint model)
        {
            var rm = new returnMessage();
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var RWA_Data = ent.RWAs.Where(a => a.Id == model.RWA_Id).FirstOrDefault();

                    if (RWA_Data == null)
                    {
                        var msg = "RWA_Id is not correct.";
                        rm.Message = msg;
                        rm.Status = 101;
                        return Ok(rm);
                    }
                    else
                    {
                        var domenModel = new RWA_Complaints()
                        {
                            RWA_Id = model.RWA_Id,
                            Subjects = model.Subjects,
                            Complaints = model.Complaints,
                            Others = model.Others,
                        };
                        ent.RWA_Complaints.Add(domenModel);
                        ent.SaveChanges();
                        tran.Commit();
                        var msg = "Complaint Added Successfully.";
                        rm.Message = msg;
                        rm.Status = 200;
                        return Ok(rm);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    tran.Rollback();
                    rm.Status = 101;
                    return Ok(rm);
                }
            }
        }

        
        //======================[Doctor Rating Review]===============//

        [HttpPost]
        [Route("api/PatientApi/DoctorRatingReview")]
        public IHttpActionResult DoctorRatingReview(getRating model)
        { 


            try
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                if (model.ImageBase != "")
                {
                    var Img1 = FileOperation.UploadFileWithBase64("Images", model.Image, model.ImageBase, allowedExtensions);
                    if (Img1 == "not allowed")
                    {
                        return Ok("Only png,jpg,jpeg files are allowed");
                    }
                    model.Image = Img1;

                    Review emp = new Review()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Image = model.Image,
                        Rating1 = model.Rating1,
                        Rating2 = model.Rating2,
                        Rating3 = model.Rating3,
                        Rating4 = model.Rating4,
                        Rating5 = model.Rating5,
                        pro_Id = model.pro_Id,
                        Patient_Id =model.Patient_Id,
                        Professional = model.Professional

                    };
                    ent.Reviews.Add(emp);
                    ent.SaveChanges();
                    rm.Message = "data save successfully";
                    rm.Status = 200;
                    return Ok(rm);

                }

                else if (model.ImageBase == "")
                {
                    
                    Review emp = new Review()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        //Image = model.Image,
                        Rating1 = model.Rating1,
                        Rating2 = model.Rating2,
                        Rating3 = model.Rating3,
                        Rating4 = model.Rating4,
                        Rating5 = model.Rating5,
                        pro_Id = model.pro_Id,
                        Professional = model.Professional


                    };

                    ent.Reviews.Add(emp);
                    ent.SaveChanges();
                    rm.Message = "data save successfully";
                    rm.Status = 200;
                    return Ok(rm);
                } 
                else 
                {
                    rm.Message = "Not Found.";
                    rm.Status = 404;
                    return Ok(rm);
                } 

            }


            catch (Exception ex)
            {
                throw new Exception("Server Error" + ex.Message);
            }
        }
       // =======get rating List  ==============================//

        [HttpGet]
        [Route("api/PatientApi/GETDoctorRatingReview")]
        public IHttpActionResult GETDoctorRatingReview(string Professional)
        {

            // select AVG(Rating5+Rating4 + Rating3 + Rating2 + Rating1) as Avrerage from[Review] where  and Professional = 'Doctor'

            //string query = @"SELECT Name,Description,Image,pro_Id,Professional,(SELECT AVG(c)FROM   (VALUES(Rating1+Rating2+Rating3+Rating4+Rating5)) T (c)) AS [Rating] FROM   Review where pro_Id ='" + Pro_Id + "' and Professional='" + Professional + "'";
            string query = @"select Avg(rating1 + rating2 + rating3 + rating4 + rating5) As rating, Name, Description, Image, pro_Id, Professional  from[Review] where Professional ='" + Professional + "'group by Name,Description,Image,pro_Id,Professional";
            var rating = ent.Database.SqlQuery<DModelRatings>(query).ToList();

            return Ok(new { rating });

        }

        [HttpGet]
        [Route("api/PatientApi/GetTotalRating")]
        public IHttpActionResult GetTotalRating(int Pro_Id, string Professional)

        {
            string query = @"select sum(rating1 + rating2 + rating3 + rating4 + rating5) As rating, Name, Description, Image, pro_Id, Professional , Patient_Id  from[Review] where Professional ='" + Professional + "' and pro_Id ='" + Pro_Id + "' group by Name,Description,Image,pro_Id,Professional, Patient_Id ";
            //only one rating 
            //string query = @"select Id,AVG(Rating1+Rating2+Rating3+Rating4+Rating5) as rating,Id from [Review] where pro_Id ='" + Pro_Id + "' and Professional='" + Professional + "'group by Id";
            var rating = ent.Database.SqlQuery<DModelRating>(query).ToList();
            return Ok(new { rating });
        }


        //=============RWA ABOUT=================//

        [HttpGet]
        [Route("api/PatientApi/RWA_About")]
        public IHttpActionResult RWA_About(int Id)
        {
            string qry = @"select Id,About from RWA Where IsDeleted=0";
            var About = ent.Database.SqlQuery<RWAAbout>(qry).FirstOrDefault();
            return Ok(About);
        }

        //=============USER(PATIENT) ABOUT=================//

        [HttpGet]
        [Route("api/PatientApi/User_About")]
        public IHttpActionResult User_About()
        {
            string qry = @"select Id,About from Patient where IsDeleted=0";
            var About = ent.Database.SqlQuery<UserAbout>(qry).FirstOrDefault();
            return Ok(About);
        }

        //=============VIEW LAB REPORT BY PATIENT============//

        [HttpGet]
        [Route("api/PatientApi/ViewLabReportByPatient")]
        public IHttpActionResult ViewLabReportByPatient(int PatientId)
        {
            string qry = @"select LR.Id,L.LabName,LT.TestName,BTL.TestDate,LR.[File] from Patient as P left join LabReport as LR on LR.Patient_Id=P.Id left join LabTest as LT on LT.Id=LR.Test left join Lab as L on L.Id=LR.Lab_Id left join BookTestLab as BTL on LT.Id=BTL.Test_Id where LR.Patient_Id=" + PatientId + " order by LR.Id desc";
            var ViewLabReport_ByPatient = ent.Database.SqlQuery<LabViewReport_ByPatient>(qry).ToList();
            return Ok(new { ViewLabReport_ByPatient });
        }
         
        //==========================LabViewReportFile=====================//

        [HttpGet]
        [Route("api/PatientApi/LabReport_File")]
        public IHttpActionResult LabReport_File(int Id)
        {
            var data = new LabReport();
            string qry = @"select [File] from LabReport where Id =" + Id + "";
            var LabViewReport_file = ent.Database.SqlQuery<Lab_View_Report_File>(qry).ToList();

            return Ok(new { LabViewReport_file });
        }

        //======================DoctorViewReport By Patient==========================//
        [HttpGet]
        [Route("api/PatientApi/DoctorViewReportByPatient")]
        public IHttpActionResult DoctorViewReportByPatient(int PatientId)
        {
            string qry = @"select DR.Id,D.DoctorName,DR.Image1 from Patient as P left join DoctorReports as DR on DR.Patient_Id=P.Id left join Doctor as D on D.Id=Dr.Doctor_Id where DR.Patient_Id=" + PatientId + " order by Dr.Id Desc";
            var DoctorReportByPatient = ent.Database.SqlQuery<DoctorViewReport_bypatient>(qry).ToList();
            return Ok(new { DoctorReportByPatient });
        }

        //===================DoctorViewReportFile======================//
        [HttpGet]
        [Route("api/PatientApi/DoctorViewReportFile")]
        public IHttpActionResult DoctorViewReportFile(int Id)
        {
            string qry = @"select Image1 from DoctorReports where Id=" + Id + "";
            var DoctorViewReport_file = ent.Database.SqlQuery<DoctorViewReportFile>(qry).ToList();
            return Ok(new { DoctorViewReport_file });
        }

        //============================NurseViewReport By Patient====================================//

        [HttpGet]
        [Route("api/PatientApi/NurseViewReportByPatient")]
        public IHttpActionResult NurseViewReportByPatient(int PatientId)
        {
            string qry = @"select NR.Id,N.NurseName,NR.[File] from Patient as P left join Nurse_Rep as NR on NR.Patient_Id=P.Id left join Nurse as N on N.Id=NR.Nurse_Id where NR.Patient_Id=" + PatientId + " order by NR.Id desc";
            var NurseViewReport = ent.Database.SqlQuery<Nurse_View_ReportBypatient>(qry).ToList();
            return Ok(new { NurseViewReport });
        }

        [HttpGet, Route("api/PatientApi/GetDriverList_ByLatLong")]

        public IHttpActionResult GetDriverList_ByLatLong(int VehType_Id, double StartLat, double StartLong)
        {
            string qry = @"select D.Id,D.DriverName,D.DriverImage,D.AadharNumber,D.DlNumber,D.MobileNumber from DriverLocation as DL left join Driver as D on D.Id=DL.Driver_Id where DL.VehicleType_id=" + VehType_Id + " and start_Lat=" + StartLat + " and start_Long=" + StartLong + "";
            var DriverListByLatLong = ent.Database.SqlQuery<DriverList_ByLatLong>(qry).ToList();
            return Ok(new { DriverListByLatLong });

        }

        [HttpGet, Route("api/PatientApi/GetDriverDetail")]

        public IHttpActionResult GetDriverDetail(int DriverId)
        {
            string qry = @"select D.Id,D.DriverName,D.MobileNumber,D.DriverImage,D.EmailId,SM.StateName,CM.CityName,D.Location,D.DlNumber from Driver as D left join StateMaster as SM on SM.Id=D.StateMaster_Id left join CityMaster as CM on CM.Id=D.CityMaster_Id where D.Id=" + DriverId + "";
            var Driver_Detail = ent.Database.SqlQuery<DriverDetail>(qry).FirstOrDefault();
            return Ok( Driver_Detail );
        }
        //Post Api send request single driver
        [HttpPost, Route("api/PatientApi/BookDriver")]

        public IHttpActionResult BookDriver(DriverLocationDT model)
        {
            try
            {
                var checkalreadybooking = ent.DriverLocations.Where(d => d.PatientId == model.Patient_Id && d.AmbulanceType_id == 2 && d.IsBooked == true && d.RideComplete==false).OrderByDescending(d => d.Id).FirstOrDefault();
                
                var checkamb = ent.DriverLocations.Where(d => d.PatientId == model.Patient_Id && d.AmbulanceType_id == 2).OrderByDescending(d => d.Id).FirstOrDefault();
                //var data=ent.DriverLocations.Where(d=>d.PatientId==model.Patient_Id).OrderByDescending(d=>d.Id).FirstOrDefault();

                var data = ent.DriverLocations.Where(d => d.PatientId == model.Patient_Id && d.IsPay == "N" && d.Status == "0").OrderByDescending(d => d.Id).FirstOrDefault();
                
 
                if(checkamb==null)
                {
                    data.IsBooked = true;
                    data.Driver_Id = model.Driver_Id;
                    ent.SaveChanges();

                    var booking = new DriverBooking
                    {
                        Driver_Id = model.Driver_Id,
                        Patient_Id = model.Patient_Id,
                        BookingDate = DateTime.Now,
                        RideComplete = false
                    };

                    ent.DriverBookings.Add(booking);
                    ent.SaveChanges();
                }
                else
                {
                    if (checkalreadybooking == null)
                    {
                        var driverdetail = ent.Drivers.Where(d => d.Id == model.Driver_Id).FirstOrDefault();
                        driverdetail.IsBooked = true;
                        data.IsBooked = true;
                        data.Driver_Id = model.Driver_Id;
                        ent.SaveChanges();

                        var booking = new DriverBooking
                        {
                            Driver_Id = model.Driver_Id,
                            Patient_Id = model.Patient_Id,
                            BookingDate = DateTime.Now,
                            RideComplete = false
                        };

                        ent.DriverBookings.Add(booking);
                        ent.SaveChanges();
                    }
                    else
                    {
                        return BadRequest("You already book a ambulance.");
                    }
                } 
                return Ok(new { Message = "Driver booked successfully" });

            }
            catch (Exception)
            {
                return BadRequest("Server Error");
            }
        }

        //Post Api send request all driver//
        [HttpPost, Route("api/PatientApi/RequestToAll")]
        public IHttpActionResult RequestToAll(DriverLocationDT model)
        {
            try
            {
                List<DriverListNearByUser> driverListNearByUser = new List<DriverListNearByUser>();

                var Driver = ent.Database.SqlQuery<DriverLocationDT>(@"select Id, DriverId as Driver_Id from NearDriver with(nolock) where KM <= 100 order by KM asc").ToList();
                var data = ent.DriverLocations.Where(d => d.PatientId == model.Patient_Id).OrderByDescending(d => d.Id).FirstOrDefault();
                data.IsBooked = true;
                ent.SaveChanges();

                foreach (var item in Driver)
                {
                    var booking = new DriverBooking
                    {
                        Driver_Id = item.Driver_Id,
                        Patient_Id = model.Patient_Id,
                        BookingDate = DateTime.Now,
                        RideComplete=false
                    };

                    ent.DriverBookings.Add(booking);
                    ent.SaveChanges();
                }
                rm.Status = 1;
                rm.Message = "Request send successfully.";
                return Ok(rm);

            }
            catch (Exception)
            {
                return BadRequest("Server Error");
            }
        }

        [HttpPost, Route("api/PatientApi/UpdateUserLocation")]
        public IHttpActionResult UpdateUserLocation(UpdatelocationPatient model)
        {
            try
            {
                var data = ent.Patients.FirstOrDefault(a => a.Id == model.PatientId);
                data.Lat = model.Lat;
                data.Lang = model.Lang;
                ent.SaveChanges();
                return Ok(new { Message = "Update Patient Location SuccessFully" });
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }

        [HttpGet, Route("api/DriverApi/GetRoadAccidendAmbOngoingRide")]
        public IHttpActionResult GetRoadAccidendAmbOngoingRide(int PatientId)
        {
            var driverDetails = (from d in ent.Drivers
                                 join dl in ent.DriverLocations on d.Id equals dl.Driver_Id
                                 join v in ent.Vehicles on d.Vehicle_Id equals v.Id 
                                 where dl.PatientId == PatientId && dl.AmbulanceType_id == 2 && dl.RideComplete == false
                                 orderby dl.Id descending
                                 select new getdriverbookinglist
                                 {
                                     Id = dl.Id,
                                     DriverId = d.Id,
                                     DriverName = d.DriverName,
                                     MobileNumber = d.MobileNumber,
                                     DlNumber = d.DlNumber,
                                     DriverImage = d.DriverImage,
                                     VehicleNumber = v.VehicleNumber, 
                                     ToatlDistance = dl.ToatlDistance,
                                     PaidAmount = dl.Amount,
                                     TotalPrice = dl.TotalPrice,
                                     RemainingAmount = dl.TotalPrice - dl.Amount,
                                     UserLat = dl.start_Lat,
                                     UserLong = dl.start_Long,
                                     end_Lat = dl.end_Lat,
                                     end_Long = dl.end_Long,
                                     Lat_Driver = d.Lat,
                                     Lang_Driver = d.Lang,
                                     PaymentDate = dl.PaymentDate,
                                 }).FirstOrDefault();



            if (driverDetails != null)
            {
                // Driver
                var lat1 = (double)driverDetails.Lat_Driver;
                var lon1 = (double)driverDetails.Lang_Driver;

                // User
                var lat2 = (double)driverDetails.UserLat;
                var lon2 = (double)driverDetails.UserLong;

                double rlat1 = Math.PI * lat1 / 180;
                double rlat2 = Math.PI * lat2 / 180;
                double theta = lon1 - lon2;
                double rtheta = Math.PI * theta / 180;

                double dist = Math.Sin(rlat1) * Math.Sin(rlat2) +
                              Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);

                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515; // Convert miles to kilometers
                dist = dist * 1.609344;   // Convert miles to kilometers

                driverDetails.DriverUserDistance = dist;

                // Calculate expected time
                double expectedTimeMinutes = dist * 2; // 2 minutes per kilometer

                // Convert the expectedTimeMinutes to an integer
                int expectedTimeMinutesInt = Convert.ToInt32(expectedTimeMinutes);

                driverDetails.ExpectedTime = expectedTimeMinutesInt;
            }

            return Ok(driverDetails);
        }
    }
    
}


