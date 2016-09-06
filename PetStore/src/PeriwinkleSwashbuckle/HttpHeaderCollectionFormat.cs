using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Periwinkle.Swashbuckle
{
    public enum HttpHeaderCollectionFormat
    {
        /// <summary>
        /// comma separated values foo, bar.
        /// </summary>
        csv,        
        /// <summary>
        /// space separated values foo bar.
        /// </summary>
        ssv,
        /// <summary>
        /// tab separated values foo\tbar.
        /// </summary>
        tsv,
        /// <summary>
        /// pipe separated values foo|bar.
        /// </summary>
        pipes,
    }
}
