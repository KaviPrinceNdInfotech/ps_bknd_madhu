using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ExpenseReportDTO
    {
        public int Id { get; set; }
        [Required]
        public string Expense { get; set; }
        [Required]
        public string Type_Of_Expense { get; set; }
        [Required]
        public string Transaction_Type { get; set; }
        [Required]
        public string Expense_Description { get; set; }
        [Required]
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<ExpenseList> ExpenselList { get; set; }
    }


    public class ExpenseList
    {

        public int Id { get; set; }
        [Required]
        public string Expense { get; set; }
        [Required]
        public string Type_Of_Expense { get; set; }
        [Required]
        public string Transaction_Type { get; set; }
        [Required]
        public string Expense_Description { get; set; }
        [Required]
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
    }
}