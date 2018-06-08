using System.Linq;
using System.Collections.Generic;

namespace HidGlobal.OK.Readers.Utilities
{
    public static class LinqExtension
    {
        /// <summary>
        /// Splits data into parts of given length. 
        /// Last batch of data can be smaller.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="batchSize">Size of the batches of data.</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> enumerable, int batchSize)
        {
            return enumerable.Select((byteValue, index) => new { Value = byteValue, Index = index })
                .GroupBy(x => x.Index / batchSize)
                .Select(g => g.Select(x => x.Value));
        }
    }
}
