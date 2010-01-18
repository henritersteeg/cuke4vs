using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace Microsoft.Samples.LinqToCodeModel
{
    /// <summary>
    /// Helper class for querying codemodel hierarchies
    /// </summary>
	public static class QueryableCodeModel
	{
		#region Public Implementation
        /// <summary>
        /// Gets elements.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
		public static IEnumerable<TElement> GetElements<TElement>(CodeElements source)
		{
			CodeElementIterator iterator = new CodeElementIterator(source);

			return iterator
				.OfType<TElement>();
		}

        /// <summary>
        /// Gets element filtered by where condition.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
		public static TElement GetElementWhere<TElement>(CodeElements source, Func<TElement, bool> predicate)
		{
			return GetElements<TElement>(source)
				.SingleOrDefault(predicate);
		}

        /// <summary>
        /// Gets elements filtered by where condition.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
		public static IEnumerable<TElement> GetElementsWhere<TElement>(CodeElements source, Func<TElement, bool> predicate)
		{
			return GetElements<TElement>(source)
				.Where(predicate)
				.Select(element => element);
		} 
		#endregion
	}
}