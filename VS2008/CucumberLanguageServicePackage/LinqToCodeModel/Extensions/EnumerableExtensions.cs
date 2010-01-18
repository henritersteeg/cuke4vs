using System;
using System.Collections.Generic;

namespace Microsoft.Samples.LinqToCodeModel.Extensions
{
    /// <summary>
    /// Extensions methods for IEnumerable
    /// </summary>
	public static class EnumerableExtensions
	{
		#region Public Implementation
        /// <summary>
        /// Foreach implementation.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
		public static void ForEach<TItem>(this IEnumerable<TItem> source, Action<TItem> action)
		{
			foreach(var item in source)
			{
				action(item);
			}
		} 
		#endregion
	}
}