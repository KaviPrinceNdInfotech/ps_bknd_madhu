using AutoMapper;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Repositories
{
    public class CommonRepository
    {
        DbEntities ent = new DbEntities();

        public IEnumerable<StateMaster> GetAllStates()
        {
             var data = ent.StateMasters.Where(a => !a.IsDeleted).OrderBy(a=> new { a.StateName }).ToList();
            return data;
        }
        public IEnumerable<Medicine> GetMedicine()
        {
            var data = ent.Medicines.Where(a => !a.IsDeleted).OrderBy(a => new { a.MedicineName }).ToList();
            return data;
        }
        public IEnumerable<Driver> GetAllDrivers()
        {
            var data = ent.Drivers.Where(a => !a.IsDeleted).OrderBy(a => new { a.DriverName }).ToList();
            return data;
        }

        public IEnumerable<VehicleType> GetVehicleType()
        {
            var data = ent.VehicleTypes.Where(a => a.IsDeleted==false).OrderBy(a=> new {a.VehicleTypeName });
            return data;
        }

        public IEnumerable<VehicleType> GetVehicleTypes()
        {
            string q = @"select * from VehicleType join MainCategory on VehicleType.Category_Id = MainCategory.Id where VehicleType.IsDeleted  = 0 and MainCategory.IsDeleted  = 0";
            var data = ent.Database.SqlQuery<VehicleType>(q).ToList();
            return data;
        }

        public IEnumerable<CityMaster> GetCitiesByState(int? stateId)
        {
            var data = ent.CityMasters.Where(a => a.StateMaster_Id == stateId && a.IsDeleted==false).OrderBy(a=> new { a.CityName}).ToList();
            return data;
        }

        public IEnumerable<LocationDTO> GetLocationByCity(int? cityId)
        {
            var data = ent.Locations.Where(a => a.City_Id == cityId).OrderBy(a=> new  {a.LocationName }).ToList();
            return Mapper.Map<IEnumerable<LocationDTO>>(data);
        }

        public IEnumerable<Department> GetDepartments()
        {
            var data = ent.Departments.Where(a=> a.IsDeleted==false).OrderBy(a=> new {a.DepartmentName }).ToList();
            return data;
        }

        public IEnumerable<Specialist> GetSpecialistByDepartment(int departmentId)
        {
            var data = ent.Specialists.Where(a => !a.IsDeleted && a.Department_Id == departmentId).OrderBy(a=> new {a.SpecialistName }).ToList();
            return data;
        }

        public IEnumerable<LabTest> GetTests()
        {
            var data = ent.LabTests.Where(a =>a.TestName != null ).OrderBy(a=> new { a.TestName}).ToList();
            return data;
        }

        public List<LabTest> GetTestsName()
        {
            var data = ent.LabTests.Where(a => a.TestName == null).OrderBy(a => new { a.TestName }).ToList();
            return data;
        }

        public IEnumerable<MainCategory> GetCategory()
        {
            var data = ent.MainCategories.Where(a=>a.IsDeleted == false).OrderBy(a => new { a.CategoryName }).ToList();
            return data;
        }
        public IEnumerable<Driver> GetDriver()
        {
            var data = ent.Drivers.Where(a => a.IsDeleted == false).OrderBy(a => new { a.DriverName }).ToList();
            return data;
        }

        public IEnumerable<Patient> GetPatient()
        {
            var data = ent.Patients.Where(a => a.IsDeleted == false).OrderBy(a => new { a.PatientName }).ToList();
            return data;
        }

        public IEnumerable<HealthPackageMaster> GetPackageNames()
        {
            var data = ent.HealthPackageMasters.OrderBy(a => new { a.PackageName }).ToList();
            return data;
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 