using Carpo.Core.Domain.DomainDescription;
using Carpo.Core.Domain.Validation;
using Carpo.Core.Interface.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carpo.Core.Domain
{
    /// <summary>
    /// Classe base para as classes de domínio
    /// </summary>
    public abstract class BaseDomain : IValidationDomain
    {
        /// <summary>
        /// Key default
        /// </summary>
        //[Key]
        //public virtual Guid Id { get; set; }
        /// <summary>
        /// Informa se as propriedades da classe de domínio estão válidas. 
        /// </summary>
        [NotMapped]
        public bool IsValidDomain { get { return !((List<ValidationResult>)ListValidationResult).Any(); } }

        /// <summary>
        /// Lista o resultado da validação
        /// </summary>
        internal List<ValidationResult> _listCustomValidationResult;

        /// <summary>
        /// Lista o resultado da validação de cada propriedade da classe de domínio
        /// </summary>
        [NotMapped]
        public object ListValidationResult
        {
            get
            {
                var listResult = ValidationDomain.ValidateDomain(this);
                if (_listCustomValidationResult != null)
                {
                    listResult.AddRange(_listCustomValidationResult);
                }

                return listResult;
            }
        }

        /// <summary>
        /// Busca a descrição da classe de domínio e de suas propriedades.
        /// </summary>
        /// <returns></returns>
        public EntityDescriptionDataTransfer GetDomainClassDescription()
        {
            return EntityDocumentationCore.GetDomainClassDescription(this.GetType());
        }

        /// <summary>
        /// Adicionar um objeto ValidationResult personalizado na lista de validação
        /// </summary>
        private void AddValidationResult(ValidationResult validationResult)
        {
            if (_listCustomValidationResult == null)
            {
                _listCustomValidationResult = new List<ValidationResult>();
            }
            _listCustomValidationResult.Add(validationResult);
        }

        /// <summary>
        /// Adicionar um objeto ValidationResult na lista de validação informando a mensagem e os campos relacionados.
        /// </summary>
        public void AddValidationResult(List<string> listFieldName, string message)
        {
            AddValidationResult(new ValidationResult(message, listFieldName));
        }

        /// <summary>
        /// Adicionar um objeto ValidationResult na lista de validação sem a necessidade de informar um campo específico.
        /// </summary>
        public void AddValidationResult(string message)
        {
            AddValidationResult(new ValidationResult(message));
        }

    }
}
