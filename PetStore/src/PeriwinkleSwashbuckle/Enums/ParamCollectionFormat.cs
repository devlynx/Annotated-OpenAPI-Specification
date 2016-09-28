

namespace Periwinkle.Swashbuckle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public enum ParamCollectionFormat
    {
        /// <summary>
        /// comma separated values: foo, bar.
        /// </summary>
        csv,

        /// <summary>
        /// space separated values: foo bar.  
        /// </summary>
        ssv,

        /// <summary>
        /// tab separated values: foo\tbar.  
        /// </summary>
        tsv,

        /// <summary>
        /// pipe separated values: foo|bar.
        /// </summary>
        pipes,

        /// <summary>
        /// corresponds to multiple parameter instances instead of multiple values 
        /// for a single instance: foo = bar & foo = baz. This is valid only for 
        /// parameters in "query" or "formData".
        /// </summary>
        multi,
    }
}
