using Carpo.Core.Domain.Idx;
using Carpo.Core.Interface.Idx;

namespace Carpo.Data.EFCore.Test.Generic
{
    /// <summary>
    /// Index 
    /// </summary>
    public struct Idx
    {
        /// <summary>
        /// 
        /// </summary>
        public class Base
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public struct Person
        {
            /// <summary>
            /// IdxCode
            /// </summary>
            public class IdxGender : BaseGenericIdx, IIdxField
            {
                /// <summary>
                ///Male
                /// </summary>
                public struct Male
                {
                    /// <summary>
                    /// 
                    /// </summary>
                    public static readonly string IdxValue = "M";
                    /// <summary>
                    /// 
                    /// </summary>
                    public static readonly string IdxDescription = "Male";
                }

                /// <summary>
                /// Female
                /// </summary>
                public struct Female
                {
                    /// <summary>
                    /// 
                    /// </summary>
                    public static readonly string IdxValue = "F";
                    /// <summary>
                    /// 
                    /// </summary>
                    public static readonly string IdxDescription = "Female";
                }

                /// <summary>
                /// Other
                /// </summary>
                public struct Other
                {
                    /// <summary>
                    /// 
                    /// </summary>
                    public static readonly string IdxValue = "O";
                    /// <summary>
                    /// 
                    /// </summary>
                    public static readonly string IdxDescription = "Other";
                }

                /// <summary>
                /// ListIdx
                /// </summary>
                public List<IIdx> ListIdx => GetListBaseIdc<IdxGender>();

            }

        }
    }
}
