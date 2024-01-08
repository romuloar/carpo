using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Carpo.Core.Domain.Attributes
{
    /// <summary>
    /// There must be at least 1 item in the list
    /// </summary>
    public class RequiredListAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                return list.Count > 0;
            }
            return false;
        }
    }
}
