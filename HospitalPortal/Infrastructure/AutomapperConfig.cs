using AutoMapper;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static HospitalPortal.Controllers.TestApiController;

namespace ProductMini.Infrastructure
{
    public  class AutomapperConfig
    {

        public static void MapIt()
        {
            Mapper.CreateMap<PatientRequest, TestModule>();
            Mapper.CreateMap<TestModule, PatientRequest>();

            Mapper.CreateMap<MainCategory, VehicleList>();
            Mapper.CreateMap<VehicleList, MainCategory>();

            Mapper.CreateMap<Charge, ChargeDTO>();
            Mapper.CreateMap<ChargeDTO, Charge>();

            Mapper.CreateMap<StateMaster,StateMasterDTO>();
            Mapper.CreateMap<StateMasterDTO, StateMaster>();

            Mapper.CreateMap<CityMaster, CityMasterDTO>();
            Mapper.CreateMap<CityMasterDTO, CityMaster>();

            Mapper.CreateMap<Chemist, ChemistDTO>();
            Mapper.CreateMap<ChemistDTO, Chemist>();

            Mapper.CreateMap<Department, DepartmentDTO>();
            Mapper.CreateMap<DepartmentDTO, Department>();

            Mapper.CreateMap<Doctor, DoctorDTO>();
            Mapper.CreateMap<DoctorDTO, Doctor>();

            Mapper.CreateMap<Driver, DriverDTO>();
            Mapper.CreateMap<DriverDTO, Driver>();

            Mapper.CreateMap<Employee, EmployeeDTO>();
            Mapper.CreateMap<EmployeeDTO, Employee>();

            Mapper.CreateMap<Hospital, HospitalDTO>();
            Mapper.CreateMap<HospitalDTO, Hospital>();

            Mapper.CreateMap<Lab, LabDTO>();
            Mapper.CreateMap<LabDTO, Lab>();

            Mapper.CreateMap<HealthCheckupCenter, HealthCheckupCenterDTO>();
            Mapper.CreateMap<HealthCheckupCenterDTO, HealthCheckupCenter>();

            Mapper.CreateMap<Nurse, NurseDTO>();
            Mapper.CreateMap<NurseDTO, Nurse>();

            Mapper.CreateMap<RWA, RWADTO>();
            Mapper.CreateMap<RWADTO, RWA>();

            Mapper.CreateMap<Specialist, SpecialistDTO>();
            Mapper.CreateMap<SpecialistDTO, Specialist>();

            Mapper.CreateMap<Vendor, VendorDTO>();
            Mapper.CreateMap<VendorDTO, Vendor>();

            Mapper.CreateMap<Vehicle, VehicleDTO>();
            Mapper.CreateMap<VehicleDTO, Vehicle>();

            Mapper.CreateMap<VehicleType, VehicleTypeDTO>();
            Mapper.CreateMap<VehicleTypeDTO, VehicleType>();

            Mapper.CreateMap<Patient , PatientDTO>();
            Mapper.CreateMap<PatientDTO, Patient>();

            Mapper.CreateMap<DoctorSkill, DoctorSkillsDTO>();
            Mapper.CreateMap<DoctorSkillsDTO, DoctorSkill>();

            Mapper.CreateMap<HospitalDepartment, HospitalDepartmentDTO>();
            Mapper.CreateMap<HospitalDepartmentDTO, HospitalDepartment>();

            Mapper.CreateMap<HospitalFacilityDTO, HopitalFaciltiy>();
            Mapper.CreateMap<HopitalFaciltiy, HospitalFacilityDTO>();

            Mapper.CreateMap<ChekcupCenterDepartment, CheckupCenterDepartmentDTO>();
            Mapper.CreateMap<CheckupCenterDepartmentDTO, ChekcupCenterDepartment>();

            Mapper.CreateMap<HospitalDoctor, HospitalDoctorDTO>();
            Mapper.CreateMap<HospitalDoctorDTO, HospitalDoctor>();

            Mapper.CreateMap<Location, LocationDTO>();
            Mapper.CreateMap<LocationDTO, Location>();

            Mapper.CreateMap<DoctorRegistrationRequest, Doctor>();
            Mapper.CreateMap<Doctor, DoctorRegistrationRequest>();

            Mapper.CreateMap<DriverRequestParameter, Driver>();
            Mapper.CreateMap<Driver, DriverRequestParameter>();

            Mapper.CreateMap<HospitalRegistrationReq, Hospital>();
            Mapper.CreateMap<Hospital, HospitalRegistrationReq>();

            Mapper.CreateMap<Medicine, MedicineDTO>();
            Mapper.CreateMap<MedicineDTO, Medicine>();

            Mapper.CreateMap<LabTest, TestArray>();
            Mapper.CreateMap<TestArray, LabTest>();

            Mapper.CreateMap<BookTestLab, BookLabClass>();
            Mapper.CreateMap<BookLabClass, BookTestLab>();

            Mapper.CreateMap<LabTest, TestDTO>();
            Mapper.CreateMap<TestDTO, LabTest>();

            Mapper.CreateMap<TestLab, AddTestByLabDTO>();
            Mapper.CreateMap<AddTestByLabDTO, TestLab>();

            Mapper.CreateMap<CheckUpReport, CheckUpReportDTO>();
            Mapper.CreateMap<CheckUpReportDTO, CheckUpReport>();

            Mapper.CreateMap<LabReport, LabReportDTO>();
            Mapper.CreateMap<LabReportDTO, LabReport>();

            Mapper.CreateMap<HealthCheckUp, CompleteCheckup>();
            Mapper.CreateMap<CompleteCheckup, HealthCheckUp > ();

            Mapper.CreateMap<CmpltCheckUp, BookHealthCheckUp>();
            Mapper.CreateMap<BookHealthCheckUp, CmpltCheckUp>();

            Mapper.CreateMap<Content, AddContentVM>();
            Mapper.CreateMap<AddContentVM, Content>();

            Mapper.CreateMap<PatientComplaint, Complaint4Patient>();
            Mapper.CreateMap<Complaint4Patient, PatientComplaint>();

            Mapper.CreateMap<NurseType, NurseTypeAPI>();
            Mapper.CreateMap<NurseTypeAPI, NurseType>();

            Mapper.CreateMap<DoctorComplaint, Doctor_Complaint>();
            Mapper.CreateMap<Doctor_Complaint, DoctorComplaint>();

            Mapper.CreateMap<Mail, MailVM>();
            Mapper.CreateMap<MailVM, Mail>();

            Mapper.CreateMap<DoctorReport, uploadDoc_Report>();
            Mapper.CreateMap<uploadDoc_Report, DoctorReport>();

            Mapper.CreateMap<UplaodReportBase, DoctorReport>();
            Mapper.CreateMap<DoctorReport, UplaodReportBase>();

            Mapper.CreateMap<HospitalNurse, HospitalNurseDTO>();
            Mapper.CreateMap<HospitalNurseDTO, HospitalNurse>();

            Mapper.CreateMap<ExpenseReport, ExpenseReportDTO>();
            Mapper.CreateMap<ExpenseReportDTO, ExpenseReport>();

            Mapper.CreateMap<TDSMaster, TDSMasterDTO>();
            Mapper.CreateMap<TDSMasterDTO, TDSMaster>();

            Mapper.CreateMap<GSTMaster, GSTMasterDTO>();
            Mapper.CreateMap<GSTMasterDTO, GSTMaster>();

            Mapper.CreateMap<CommissionMaster, CommissionDTO>();
            Mapper.CreateMap<CommissionDTO, CommissionMaster>();

            Mapper.CreateMap<BankDetail, UpdateBankDetails>();
            Mapper.CreateMap<UpdateBankDetails, BankDetail>();

            Mapper.CreateMap<BankDetail, UpdateBank>();
            Mapper.CreateMap<UpdateBank, BankDetail>();

            Mapper.CreateMap<TempDepartment, UpdateDepartment>();
            Mapper.CreateMap<UpdateDepartment, TempDepartment>();

            Mapper.CreateMap<MainCategory, MainCategoryDTO>();
            Mapper.CreateMap<MainCategoryDTO, MainCategory>();

            Mapper.CreateMap<VehicleTemp, VehicleTempDTO>();
            Mapper.CreateMap<VehicleTempDTO, VehicleTemp>();

            Mapper.CreateMap<VehicleCharge, VehicleChargesDTO>();
            Mapper.CreateMap<VehicleChargesDTO, VehicleCharge>();

            Mapper.CreateMap<Nurse, NurseRequestedParams>();
            Mapper.CreateMap<NurseRequestedParams, Nurse>();

            Mapper.CreateMap<HospitalReport, HospitalReportDTO>();
            Mapper.CreateMap<HospitalReportDTO, HospitalReport>();

            Mapper.CreateMap<DriverLocation, DriverLocationDTO>();
            Mapper.CreateMap<DriverLocationDTO, DriverLocation>();

            Mapper.CreateMap<PageMaster, PageMasterDTO>();
            Mapper.CreateMap<PageMasterDTO, PageMaster>();

            Mapper.CreateMap<UserWallet, WalletDTO>();
            Mapper.CreateMap<WalletDTO, UserWallet>();

            Mapper.CreateMap<HealthCheckUpPackage, HealthCheckUpPackageDTO>();
            Mapper.CreateMap<HealthCheckUpPackageDTO, HealthCheckUpPackage>();

            Mapper.CreateMap<PaymentMaster, PaymentMasterDTO>();
            Mapper.CreateMap<PaymentMasterDTO, PaymentMaster>();

            Mapper.CreateMap<LabTest, AddTestByLabDTO>();
            Mapper.CreateMap<AddTestByLabDTO, LabTest>();

            Mapper.CreateMap<RWAPayment, rwaPaymentDTO>();
            Mapper.CreateMap<rwaPaymentDTO, RWAPayment>();

            Mapper.CreateMap<Vehicle, VehicleEditDto>();
            Mapper.CreateMap<VehicleEditDto, Vehicle>();

            Mapper.CreateMap<DoctorClinic, EditClinicDTO>();
            Mapper.CreateMap<EditClinicDTO, DoctorClinic>();

            Mapper.CreateMap<HealthPackageMaster, HealthPackageMasterDTO>();
            Mapper.CreateMap<HealthPackageMasterDTO, HealthPackageMaster>();

            Mapper.CreateMap<HealthLabTest, HealthCheckUpPackageDTO>();
            Mapper.CreateMap<HealthCheckUpPackageDTO, HealthLabTest>();


            Mapper.CreateMap<RWA, RWA_Registration>();
            Mapper.CreateMap<RWA_Registration, RWA>();

            Mapper.CreateMap<Lab, LabREgis>();
            Mapper.CreateMap<LabREgis, Lab>();

            Mapper.CreateMap<Patient, PatientDTO>();
            Mapper.CreateMap<PatientDTO, Patient>();

            Mapper.CreateMap<Chemist, ChemistDTO>();
            Mapper.CreateMap<ChemistDTO, Chemist>();

            Mapper.CreateMap<RWATDSMaster, RWATDSDTO>();
            Mapper.CreateMap<RWATDSDTO, RWATDSMaster>();

            Mapper.CreateMap<FranchiseTDSMaster, FranchiseTDSDTO>();
            Mapper.CreateMap<FranchiseTDSDTO, FranchiseTDSMaster>();

            Mapper.CreateMap<PrescriptionAppointment, PrescriptionDTO>();
            Mapper.CreateMap<PrescriptionDTO, PrescriptionAppointment>();

            Mapper.CreateMap<MedicinePrescriptionDetail, MedicinePrescriptionDTO>();
            Mapper.CreateMap<MedicinePrescriptionDTO, MedicinePrescriptionDetail>();

            Mapper.CreateMap<RWAGstMaster, rwaPaymentDTO>();
            Mapper.CreateMap<rwaPaymentDTO, RWAGstMaster>();

            Mapper.CreateMap<FranchiseGstMaster, rwaPaymentDTO>();
            Mapper.CreateMap<rwaPaymentDTO, FranchiseGstMaster>();
        }

    }
}