using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Carpo.Core.Domain.Attributes
{
    /// <summary>
    /// There must be at least 1 item in the list
    /// </summary>
    public class RequiredGuidAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if(value is Guid)
            {
                var gd = (Guid)value;
                return gd != Guid.Empty;
            }

            return false;

        }
    }
}
