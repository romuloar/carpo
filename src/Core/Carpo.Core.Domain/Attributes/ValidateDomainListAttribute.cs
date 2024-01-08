using Carpo.Core.Interface.Domain;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Transactions;

namespace Carpo.Core.Domain.Attributes
{
    /// <summary>
    /// There must be at least 1 item in the list
    /// </summary>
    public class ValidateDomainListAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value is IList list)
            {
                var listDomain = list.OfType<IValidationDomain>().ToList();

                if (listDomain.Any(item => !item.IsValidDomain))
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
