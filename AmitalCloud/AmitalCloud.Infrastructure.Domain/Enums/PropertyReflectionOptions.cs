using System;
namespace AmitalCloud.Infrastructure.Domain.Enums
{
    [Flags]
    public enum PropertyReflectionOptions : int
    {
        /// <summary> 
        /// Take all. 
        /// </summary> 
        All = 0,

        /// <summary> 
        /// Ignores indexer properties. 
        /// </summary> 
        IgnoreIndexer = 1,

        /// <summary> 
        /// Ignores all other IEnumerable properties 
        /// except strings. 
        /// </summary> 
        IgnoreEnumerable = 2
    }
}
