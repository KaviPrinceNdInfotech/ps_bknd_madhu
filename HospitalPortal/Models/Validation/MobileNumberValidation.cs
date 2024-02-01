using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.Validation
{
    public class MobileNumberValidation : ValidationAttribute
    {
        private int MobileNo = 10;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string val = (string)value;
                int count = val.Length;
                if (count == MobileNo)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Kindly Insert only 10 Digit");
                }
            }
            else
            {
                return new ValidationResult("" + validationContext.DisplayName + "is Required");
            }
        }
    }
}