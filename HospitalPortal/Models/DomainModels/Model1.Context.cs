﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HospitalPortal.Models.DomainModels
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DbEntities : DbContext
    {
        public DbEntities()
            : base("name=DbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<RWAPayment> RWAPayments { get; set; }
        public virtual DbSet<UserWallet> UserWallets { get; set; }
        public virtual DbSet<VehicleTemp> VehicleTemps { get; set; }
        public virtual DbSet<AdminLogin> AdminLogins { get; set; }
        public virtual DbSet<BankDetail> BankDetails { get; set; }
        public virtual DbSet<BookTestLab> BookTestLabs { get; set; }
        public virtual DbSet<CancelledAppointment> CancelledAppointments { get; set; }
        public virtual DbSet<Charge> Charges { get; set; }
        public virtual DbSet<CheckUpReport> CheckUpReports { get; set; }
        public virtual DbSet<ChekcupCenterDepartment> ChekcupCenterDepartments { get; set; }
        public virtual DbSet<ChemistPayOut> ChemistPayOuts { get; set; }
        public virtual DbSet<CityMaster> CityMasters { get; set; }
        public virtual DbSet<CityTemp> CityTemps { get; set; }
        public virtual DbSet<CmpltCheckUp> CmpltCheckUps { get; set; }
        public virtual DbSet<CommissionMaster> CommissionMasters { get; set; }
        public virtual DbSet<Content> Contents { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DepartmentTemp> DepartmentTemps { get; set; }
        public virtual DbSet<DoctorComplaint> DoctorComplaints { get; set; }
        public virtual DbSet<DoctorDepartment> DoctorDepartments { get; set; }
        public virtual DbSet<DoctorPayOut> DoctorPayOuts { get; set; }
        public virtual DbSet<DoctorReport> DoctorReports { get; set; }
        public virtual DbSet<DoctorSkill> DoctorSkills { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<ExpenseReport> ExpenseReports { get; set; }
        public virtual DbSet<Gallery> Galleries { get; set; }
        public virtual DbSet<HealthBooking> HealthBookings { get; set; }
        public virtual DbSet<HealthPayOut> HealthPayOuts { get; set; }
        public virtual DbSet<HopitalFaciltiy> HopitalFaciltiys { get; set; }
        public virtual DbSet<HospitalDepartment> HospitalDepartments { get; set; }
        public virtual DbSet<HospitalDoctor> HospitalDoctors { get; set; }
        public virtual DbSet<HospitalDoctorDepartment> HospitalDoctorDepartments { get; set; }
        public virtual DbSet<HospitalNurse> HospitalNurses { get; set; }
        public virtual DbSet<HospitalReport> HospitalReports { get; set; }
        public virtual DbSet<LabPayOut> LabPayOuts { get; set; }
        public virtual DbSet<LabReport> LabReports { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Mail> Mails { get; set; }
        public virtual DbSet<MainCategory> MainCategories { get; set; }
        public virtual DbSet<Medicine> Medicines { get; set; }
        public virtual DbSet<MedicineCart> MedicineCarts { get; set; }
        public virtual DbSet<MedicineOrder> MedicineOrders { get; set; }
        public virtual DbSet<MedicineOrderDetail> MedicineOrderDetails { get; set; }
        public virtual DbSet<MedicineType> MedicineTypes { get; set; }
        public virtual DbSet<Nurse_Location> Nurse_Location { get; set; }
        public virtual DbSet<NursePayout> NursePayouts { get; set; }
        public virtual DbSet<NurseServiceRequest> NurseServiceRequests { get; set; }
        public virtual DbSet<NurseType> NurseTypes { get; set; }
        public virtual DbSet<PageMaster> PageMasters { get; set; }
        public virtual DbSet<PatientAppointment> PatientAppointments { get; set; }
        public virtual DbSet<PatientComplaint> PatientComplaints { get; set; }
        public virtual DbSet<PatientRequest> PatientRequests { get; set; }
        public virtual DbSet<PaymentMaster> PaymentMasters { get; set; }
        public virtual DbSet<PayoutMaster> PayoutMasters { get; set; }
        public virtual DbSet<RwaPayout> RwaPayouts { get; set; }
        public virtual DbSet<SalaryMaster> SalaryMasters { get; set; }
        public virtual DbSet<Specialist> Specialists { get; set; }
        public virtual DbSet<StateMaster> StateMasters { get; set; }
        public virtual DbSet<TDSMaster> TDSMasters { get; set; }
        public virtual DbSet<TempDepartment> TempDepartments { get; set; }
        public virtual DbSet<TestLab> TestLabs { get; set; }
        public virtual DbSet<TravelMaster> TravelMasters { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<VehicleType> VehicleTypes { get; set; }
        public virtual DbSet<VendorPayOut> VendorPayOuts { get; set; }
        public virtual DbSet<AppLog> AppLogs { get; set; }
        public virtual DbSet<LabBooking> LabBookings { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<RWA> RWAs { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<LabTest> LabTests { get; set; }
        public virtual DbSet<VehicleCharge> VehicleCharges { get; set; }
        public virtual DbSet<DoctorClinic> DoctorClinics { get; set; }
        public virtual DbSet<HealthPackageMaster> HealthPackageMasters { get; set; }
        public virtual DbSet<HealthCheckUpPackage> HealthCheckUpPackages { get; set; }
        public virtual DbSet<HealthLabTest> HealthLabTests { get; set; }
        public virtual DbSet<TravelRecordMaster> TravelRecordMasters { get; set; }
        public virtual DbSet<HealthCheckUp> HealthCheckUps { get; set; }
        public virtual DbSet<Nurse> Nurses { get; set; }
        public virtual DbSet<HealthCheckupCenter> HealthCheckupCenters { get; set; }
        public virtual DbSet<Medium> Media { get; set; }
        public virtual DbSet<MediaHospital> MediaHospitals { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<NurseComplaint> NurseComplaints { get; set; }
        public virtual DbSet<Bannerprofessional> Bannerprofessionals { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<DriverComplaint> DriverComplaints { get; set; }
        public virtual DbSet<AmbulanceType> AmbulanceTypes { get; set; }
        public virtual DbSet<PatientSubject> PatientSubjects { get; set; }
        public virtual DbSet<TimeSlot> TimeSlots { get; set; }
        public virtual DbSet<DoctorChooseDepartment> DoctorChooseDepartments { get; set; }
        public virtual DbSet<RWA_PaymentReport> RWA_PaymentReport { get; set; }
        public virtual DbSet<RWA_Complaints> RWA_Complaints { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Lab> Labs { get; set; }
        public virtual DbSet<LabComplaint> LabComplaints { get; set; }
        public virtual DbSet<chemistcomplaint> chemistcomplaints { get; set; }
        public virtual DbSet<Chemist> Chemists { get; set; }
        public virtual DbSet<FranchiseComplaint> FranchiseComplaints { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<AddFranchiseDepartment> AddFranchiseDepartments { get; set; }
        public virtual DbSet<Franchise_Department> Franchise_Department { get; set; }
        public virtual DbSet<Franchise_Specialist> Franchise_Specialist { get; set; }
        public virtual DbSet<FraAddVehicleType> FraAddVehicleTypes { get; set; }
        public virtual DbSet<franchiseRole> franchiseRoles { get; set; }
        public virtual DbSet<Medicine_Pay> Medicine_Pays { get; set; }
        public virtual DbSet<Medicinedeliver> Medicinedelivers { get; set; }
        public virtual DbSet<Nurse_Rep> Nurse_Rep { get; set; }
        public virtual DbSet<NearDriver> NearDrivers { get; set; }
        public virtual DbSet<DriverLocation> DriverLocations { get; set; }
        public virtual DbSet<TestInLab> TestInLabs { get; set; }
        public virtual DbSet<DoctorBookingMode> DoctorBookingModes { get; set; }
        public virtual DbSet<PFMaster> PFMasters { get; set; }
        public virtual DbSet<GSTMaster> GSTMasters { get; set; }
        public virtual DbSet<NurseService> NurseServices { get; set; }
        public virtual DbSet<PenaltyAmount> PenaltyAmounts { get; set; }
        public virtual DbSet<RWATDSMaster> RWATDSMasters { get; set; }
        public virtual DbSet<FranchiseTDSMaster> FranchiseTDSMasters { get; set; }
        public virtual DbSet<RWAGstMaster> RWAGstMasters { get; set; }
        public virtual DbSet<FranchiseGstMaster> FranchiseGstMasters { get; set; }
        public virtual DbSet<MedicineDeliveryCharge> MedicineDeliveryCharges { get; set; }
        public virtual DbSet<MedicinePrescriptionDetail> MedicinePrescriptionDetails { get; set; }
        public virtual DbSet<PrescriptionAppointment> PrescriptionAppointments { get; set; }
        public virtual DbSet<DriverPayOut> DriverPayOuts { get; set; }
        public virtual DbSet<DoctorTimeSlot> DoctorTimeSlots { get; set; }
        public virtual DbSet<ChemistMedicine> ChemistMedicines { get; set; }
    
        [DbFunction("DbEntities", "udf_AppointmentSlots")]
        public virtual IQueryable<udf_AppointmentSlots_Result> udf_AppointmentSlots(Nullable<int> doctorId)
        {
            var doctorIdParameter = doctorId.HasValue ?
                new ObjectParameter("doctorId", doctorId) :
                new ObjectParameter("doctorId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<udf_AppointmentSlots_Result>("[DbEntities].[udf_AppointmentSlots](@doctorId)", doctorIdParameter);
        }
    
        [DbFunction("DbEntities", "udf_getAppointmentSlots")]
        public virtual IQueryable<udf_getAppointmentSlots_Result> udf_getAppointmentSlots(Nullable<int> doctorId)
        {
            var doctorIdParameter = doctorId.HasValue ?
                new ObjectParameter("doctorId", doctorId) :
                new ObjectParameter("doctorId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<udf_getAppointmentSlots_Result>("[DbEntities].[udf_getAppointmentSlots](@doctorId)", doctorIdParameter);
        }
    
        public virtual ObjectResult<sp_AppointmentHistory_Result> sp_AppointmentHistory(Nullable<int> patientId)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_AppointmentHistory_Result>("sp_AppointmentHistory", patientIdParameter);
        }
    
        public virtual int AddMedicineToCart(Nullable<int> patientId, Nullable<int> medicineId, Nullable<int> qty)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            var medicineIdParameter = medicineId.HasValue ?
                new ObjectParameter("medicineId", medicineId) :
                new ObjectParameter("medicineId", typeof(int));
    
            var qtyParameter = qty.HasValue ?
                new ObjectParameter("qty", qty) :
                new ObjectParameter("qty", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AddMedicineToCart", patientIdParameter, medicineIdParameter, qtyParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> AppointmentTestNurse(Nullable<int> patientId, string mobile, Nullable<System.DateTime> startdate, Nullable<System.DateTime> enddate, Nullable<System.DateTime> requestdate, Nullable<int> nurseTypeId, Nullable<int> locationId, Nullable<System.DateTime> serviceDate, string serviceType, string serviceTime)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            var mobileParameter = mobile != null ?
                new ObjectParameter("mobile", mobile) :
                new ObjectParameter("mobile", typeof(string));
    
            var startdateParameter = startdate.HasValue ?
                new ObjectParameter("startdate", startdate) :
                new ObjectParameter("startdate", typeof(System.DateTime));
    
            var enddateParameter = enddate.HasValue ?
                new ObjectParameter("Enddate", enddate) :
                new ObjectParameter("Enddate", typeof(System.DateTime));
    
            var requestdateParameter = requestdate.HasValue ?
                new ObjectParameter("requestdate", requestdate) :
                new ObjectParameter("requestdate", typeof(System.DateTime));
    
            var nurseTypeIdParameter = nurseTypeId.HasValue ?
                new ObjectParameter("nurseTypeId", nurseTypeId) :
                new ObjectParameter("nurseTypeId", typeof(int));
    
            var locationIdParameter = locationId.HasValue ?
                new ObjectParameter("locationId", locationId) :
                new ObjectParameter("locationId", typeof(int));
    
            var serviceDateParameter = serviceDate.HasValue ?
                new ObjectParameter("serviceDate", serviceDate) :
                new ObjectParameter("serviceDate", typeof(System.DateTime));
    
            var serviceTypeParameter = serviceType != null ?
                new ObjectParameter("serviceType", serviceType) :
                new ObjectParameter("serviceType", typeof(string));
    
            var serviceTimeParameter = serviceTime != null ?
                new ObjectParameter("serviceTime", serviceTime) :
                new ObjectParameter("serviceTime", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("AppointmentTestNurse", patientIdParameter, mobileParameter, startdateParameter, enddateParameter, requestdateParameter, nurseTypeIdParameter, locationIdParameter, serviceDateParameter, serviceTypeParameter, serviceTimeParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> AppointmentWithNurse(Nullable<int> patientId, string mobile, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> nurseTypeId, Nullable<int> locationId, string serviceType)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            var mobileParameter = mobile != null ?
                new ObjectParameter("mobile", mobile) :
                new ObjectParameter("mobile", typeof(string));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var nurseTypeIdParameter = nurseTypeId.HasValue ?
                new ObjectParameter("nurseTypeId", nurseTypeId) :
                new ObjectParameter("nurseTypeId", typeof(int));
    
            var locationIdParameter = locationId.HasValue ?
                new ObjectParameter("locationId", locationId) :
                new ObjectParameter("locationId", typeof(int));
    
            var serviceTypeParameter = serviceType != null ?
                new ObjectParameter("serviceType", serviceType) :
                new ObjectParameter("serviceType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("AppointmentWithNurse", patientIdParameter, mobileParameter, startDateParameter, endDateParameter, nurseTypeIdParameter, locationIdParameter, serviceTypeParameter);
        }
    
        public virtual int DeleteMedicineFromCart(Nullable<int> cartId)
        {
            var cartIdParameter = cartId.HasValue ?
                new ObjectParameter("cartId", cartId) :
                new ObjectParameter("cartId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteMedicineFromCart", cartIdParameter);
        }
    
        public virtual ObjectResult<GetMedicineCart_Result> GetMedicineCart(Nullable<int> patientId)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetMedicineCart_Result>("GetMedicineCart", patientIdParameter);
        }
    
        public virtual ObjectResult<GetNurseAppointmentList_Result> GetNurseAppointmentList(Nullable<int> nurseTypeId, Nullable<int> nurseId)
        {
            var nurseTypeIdParameter = nurseTypeId.HasValue ?
                new ObjectParameter("nurseTypeId", nurseTypeId) :
                new ObjectParameter("nurseTypeId", typeof(int));
    
            var nurseIdParameter = nurseId.HasValue ?
                new ObjectParameter("nurseId", nurseId) :
                new ObjectParameter("nurseId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetNurseAppointmentList_Result>("GetNurseAppointmentList", nurseTypeIdParameter, nurseIdParameter);
        }
    
        public virtual int sp_AddCheckUp(Nullable<int> center_Id, string name, string testDesc, Nullable<double> testAmount)
        {
            var center_IdParameter = center_Id.HasValue ?
                new ObjectParameter("Center_Id", center_Id) :
                new ObjectParameter("Center_Id", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var testDescParameter = testDesc != null ?
                new ObjectParameter("TestDesc", testDesc) :
                new ObjectParameter("TestDesc", typeof(string));
    
            var testAmountParameter = testAmount.HasValue ?
                new ObjectParameter("TestAmount", testAmount) :
                new ObjectParameter("TestAmount", typeof(double));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AddCheckUp", center_IdParameter, nameParameter, testDescParameter, testAmountParameter);
        }
    
        public virtual ObjectResult<sp_getChemistOrder_Result> sp_getChemistOrder(Nullable<int> chemistId)
        {
            var chemistIdParameter = chemistId.HasValue ?
                new ObjectParameter("chemistId", chemistId) :
                new ObjectParameter("chemistId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_getChemistOrder_Result>("sp_getChemistOrder", chemistIdParameter);
        }
    
        public virtual ObjectResult<sp_GetNearestMedicalShop_Result> sp_GetNearestMedicalShop(Nullable<double> userLat, Nullable<double> userLng)
        {
            var userLatParameter = userLat.HasValue ?
                new ObjectParameter("userLat", userLat) :
                new ObjectParameter("userLat", typeof(double));
    
            var userLngParameter = userLng.HasValue ?
                new ObjectParameter("userLng", userLng) :
                new ObjectParameter("userLng", typeof(double));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetNearestMedicalShop_Result>("sp_GetNearestMedicalShop", userLatParameter, userLngParameter);
        }
    
        public virtual ObjectResult<sp_getOrder_Result> sp_getOrder(Nullable<int> patientId)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_getOrder_Result>("sp_getOrder", patientIdParameter);
        }
    
        public virtual ObjectResult<sp_getOrderDetail_Result> sp_getOrderDetail(Nullable<int> orderId)
        {
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("orderId", orderId) :
                new ObjectParameter("orderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_getOrderDetail_Result>("sp_getOrderDetail", orderIdParameter);
        }
    
        public virtual ObjectResult<sp_PatientAppointments_Result> sp_PatientAppointments(Nullable<int> patientId)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_PatientAppointments_Result>("sp_PatientAppointments", patientIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> sp_saveOrder(Nullable<int> patientId, Nullable<int> chemistId, string name, string mobile, Nullable<int> stateId, Nullable<int> cityId, string address, string pincode, Nullable<int> locationId)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            var chemistIdParameter = chemistId.HasValue ?
                new ObjectParameter("chemistId", chemistId) :
                new ObjectParameter("chemistId", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));
    
            var mobileParameter = mobile != null ?
                new ObjectParameter("mobile", mobile) :
                new ObjectParameter("mobile", typeof(string));
    
            var stateIdParameter = stateId.HasValue ?
                new ObjectParameter("stateId", stateId) :
                new ObjectParameter("stateId", typeof(int));
    
            var cityIdParameter = cityId.HasValue ?
                new ObjectParameter("cityId", cityId) :
                new ObjectParameter("cityId", typeof(int));
    
            var addressParameter = address != null ?
                new ObjectParameter("address", address) :
                new ObjectParameter("address", typeof(string));
    
            var pincodeParameter = pincode != null ?
                new ObjectParameter("pincode", pincode) :
                new ObjectParameter("pincode", typeof(string));
    
            var locationIdParameter = locationId.HasValue ?
                new ObjectParameter("LocationId", locationId) :
                new ObjectParameter("LocationId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_saveOrder", patientIdParameter, chemistIdParameter, nameParameter, mobileParameter, stateIdParameter, cityIdParameter, addressParameter, pincodeParameter, locationIdParameter);
        }
    
        public virtual ObjectResult<sp_SearchNurse_Result> sp_SearchNurse(Nullable<int> locationId, Nullable<int> nurseTypeId)
        {
            var locationIdParameter = locationId.HasValue ?
                new ObjectParameter("locationId", locationId) :
                new ObjectParameter("locationId", typeof(int));
    
            var nurseTypeIdParameter = nurseTypeId.HasValue ?
                new ObjectParameter("nurseTypeId", nurseTypeId) :
                new ObjectParameter("nurseTypeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SearchNurse_Result>("sp_SearchNurse", locationIdParameter, nurseTypeIdParameter);
        }
    
        public virtual ObjectResult<spVehicleCategory_Result> spVehicleCategory(string type)
        {
            var typeParameter = type != null ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spVehicleCategory_Result>("spVehicleCategory", typeParameter);
        }
    
        public virtual ObjectResult<spVehicleList_Result> spVehicleList(Nullable<int> categoryId)
        {
            var categoryIdParameter = categoryId.HasValue ?
                new ObjectParameter("CategoryId", categoryId) :
                new ObjectParameter("CategoryId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spVehicleList_Result>("spVehicleList", categoryIdParameter);
        }
    
        public virtual ObjectResult<string> UpdateCartQuantity(Nullable<int> patientId, Nullable<int> medicineId, string type)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            var medicineIdParameter = medicineId.HasValue ?
                new ObjectParameter("medicineId", medicineId) :
                new ObjectParameter("medicineId", typeof(int));
    
            var typeParameter = type != null ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("UpdateCartQuantity", patientIdParameter, medicineIdParameter, typeParameter);
        }
    
        public virtual ObjectResult<usp_AppintmentSlots_Result> usp_AppintmentSlots(Nullable<int> doctorId)
        {
            var doctorIdParameter = doctorId.HasValue ?
                new ObjectParameter("doctorId", doctorId) :
                new ObjectParameter("doctorId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_AppintmentSlots_Result>("usp_AppintmentSlots", doctorIdParameter);
        }
    
        public virtual ObjectResult<usp_getAppintmentSlots_Result> usp_getAppintmentSlots(Nullable<int> doctorId)
        {
            var doctorIdParameter = doctorId.HasValue ?
                new ObjectParameter("doctorId", doctorId) :
                new ObjectParameter("doctorId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_getAppintmentSlots_Result>("usp_getAppintmentSlots", doctorIdParameter);
        }
    
        public virtual ObjectResult<usp_GetBookingList_Result> usp_GetBookingList(Nullable<int> patientId)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("PatientId", patientId) :
                new ObjectParameter("PatientId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_GetBookingList_Result>("usp_GetBookingList", patientIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> droptable()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("droptable");
        }
    
        public virtual ObjectResult<GEtAllMedicines_Result> GEtAllMedicines()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GEtAllMedicines_Result>("GEtAllMedicines");
        }
    
        public virtual ObjectResult<GetNearDriver_Result> GetNearDriver(Nullable<int> driverId, Nullable<int> kM, string name, string dL, Nullable<int> charge, string deviceId, Nullable<int> toatlDistance)
        {
            var driverIdParameter = driverId.HasValue ?
                new ObjectParameter("DriverId", driverId) :
                new ObjectParameter("DriverId", typeof(int));
    
            var kMParameter = kM.HasValue ?
                new ObjectParameter("KM", kM) :
                new ObjectParameter("KM", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var dLParameter = dL != null ?
                new ObjectParameter("DL", dL) :
                new ObjectParameter("DL", typeof(string));
    
            var chargeParameter = charge.HasValue ?
                new ObjectParameter("Charge", charge) :
                new ObjectParameter("Charge", typeof(int));
    
            var deviceIdParameter = deviceId != null ?
                new ObjectParameter("DeviceId", deviceId) :
                new ObjectParameter("DeviceId", typeof(string));
    
            var toatlDistanceParameter = toatlDistance.HasValue ?
                new ObjectParameter("ToatlDistance", toatlDistance) :
                new ObjectParameter("ToatlDistance", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetNearDriver_Result>("GetNearDriver", driverIdParameter, kMParameter, nameParameter, dLParameter, chargeParameter, deviceIdParameter, toatlDistanceParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> AppointmentNurseFor24Hrs(Nullable<int> patientId, string mobile, Nullable<int> nurseTypeId, Nullable<int> locationId, Nullable<System.DateTime> serviceDate, string serviceType)
        {
            var patientIdParameter = patientId.HasValue ?
                new ObjectParameter("patientId", patientId) :
                new ObjectParameter("patientId", typeof(int));
    
            var mobileParameter = mobile != null ?
                new ObjectParameter("mobile", mobile) :
                new ObjectParameter("mobile", typeof(string));
    
            var nurseTypeIdParameter = nurseTypeId.HasValue ?
                new ObjectParameter("nurseTypeId", nurseTypeId) :
                new ObjectParameter("nurseTypeId", typeof(int));
    
            var locationIdParameter = locationId.HasValue ?
                new ObjectParameter("locationId", locationId) :
                new ObjectParameter("locationId", typeof(int));
    
            var serviceDateParameter = serviceDate.HasValue ?
                new ObjectParameter("serviceDate", serviceDate) :
                new ObjectParameter("serviceDate", typeof(System.DateTime));
    
            var serviceTypeParameter = serviceType != null ?
                new ObjectParameter("serviceType", serviceType) :
                new ObjectParameter("serviceType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("AppointmentNurseFor24Hrs", patientIdParameter, mobileParameter, nurseTypeIdParameter, locationIdParameter, serviceDateParameter, serviceTypeParameter);
        }
    
        public virtual int DeleteNearDriver()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteNearDriver");
        }
    
        public virtual int sp_AddCheckUp1(Nullable<int> center_Id, string name, string testDesc, Nullable<double> testAmount)
        {
            var center_IdParameter = center_Id.HasValue ?
                new ObjectParameter("Center_Id", center_Id) :
                new ObjectParameter("Center_Id", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var testDescParameter = testDesc != null ?
                new ObjectParameter("TestDesc", testDesc) :
                new ObjectParameter("TestDesc", typeof(string));
    
            var testAmountParameter = testAmount.HasValue ?
                new ObjectParameter("TestAmount", testAmount) :
                new ObjectParameter("TestAmount", typeof(double));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AddCheckUp1", center_IdParameter, nameParameter, testDescParameter, testAmountParameter);
        }
    }
}
