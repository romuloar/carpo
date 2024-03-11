using Carpo.Core.Domain;
using Carpo.Core.Domain.Attributes;
using Carpo.Data.EFCore.Test.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carpo.Data.EFCore.Test.Domain
{
    /// <summary>
    /// Person entity
    /// </summary>
    /// <groupName>Clean</groupName>
    /// <nameEntity>Person</nameEntity>
    public class PersonDomain : BaseDomain
    {
        /// <summary>
        /// Identity.
        /// </summary>   
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Name Family
        /// </summary>
        [Required]
        public string FamilyName { get; set; }

        /// <summary>
        /// Age
        /// </summary>
        [NoZero]
        public int Age { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        [Idx(typeof(Idx.Person.IdxGender), ErrorMessage = "Invalid Gender.")]
        public string Gender { get; set; }
    }
}
