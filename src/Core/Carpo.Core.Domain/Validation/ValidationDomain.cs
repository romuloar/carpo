using Carpo.Core.Interface.Domain;
using System.ComponentModel.DataAnnotations;

namespace Carpo.Core.Domain.Validation
{
    /// <summary>
    /// Classe estática para validar a classe de domínio
    /// </summary>
    public static class ValidationDomain
    {
        /// <summary>
        /// Valida todos os atributos da classe de domínio
        /// </summary>        
        public static List<ValidationResult> ValidateDomain(IValidationDomain objectDomain)
        {
            ValidationContext context = new ValidationContext(objectDomain, null, null);
            List<ValidationResult> listValidationResult = new List<ValidationResult>();
            Validator.TryValidateObject(objectDomain, context, listValidationResult, true);

            return listValidationResult;
        }
    }
}
