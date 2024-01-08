using System.ComponentModel.DataAnnotations;

namespace Carpo.Core.Domain.Attributes
{
    /// <summary>
    /// Does not accept zero in numeric field
    /// </summary>
    public class NoZeroAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate value
        /// </summary>        
        public override bool IsValid(object value)
        {
            try
            {
                long vl = Convert.ToInt64(value);
                return vl != 0;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
