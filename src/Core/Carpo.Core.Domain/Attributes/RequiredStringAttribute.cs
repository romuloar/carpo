using System.ComponentModel.DataAnnotations;

namespace Carpo.Core.Domain.Attributes
{
    public class RequiredStringAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var str = value as String;

            return !string.IsNullOrEmpty(str);
        }
    }
}
