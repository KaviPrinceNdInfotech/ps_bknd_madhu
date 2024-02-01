using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class WalletDTO
    {
        public int Id { get; set; }
        [Required]
        public Nullable<int> AdminId { get; set; }
        [Required]
        public Nullable<int> UserId { get; set; }
        [Required]
        public Nullable<double> Amount { get; set; }
    }
}