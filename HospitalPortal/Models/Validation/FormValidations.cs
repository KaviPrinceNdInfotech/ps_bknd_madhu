using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace HospitalPortal.Models.Validation
{
    public class FormValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validateContext)
        {
            if(value != null)
            {
                string email = value.ToString();
                if (Regex.IsMatch(email, @"[A-Za-z0-9._%+-,]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",RegexOptions.IgnoreCase))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Please Enter Valid Email");
                }

            }
            else
            {
                return new ValidationResult("" + validateContext.DisplayName + "is Required");
            }
        }
       
    }
}