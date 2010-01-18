using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace Microsoft.Samples.LinqToCodeModel.Extensions
{
    /// <summary>
    /// Extensions methods for CodeModel
    /// </summary>
	public static class CodelModelExtensions
	{
		#region Constants
		private const string System = "System";
		private const string Microsoft = "Microsoft";
		private const string MS = "MS"; 
		#endregion

		#region Public Implementation
        /// <summary>
        /// Gets IEnumerable elements.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="codeModel">The code model.</param>
        /// <returns></returns>
		public static IEnumerable<TElement> GetIEnumerable<TElement>(this CodeModel codeModel)
		{
			return QueryableCodeModel.GetElements<TElement>(codeModel.CodeElements);
		}

        /// <summary>
        /// Gets codeModel namespaces.
        /// </summary>
        /// <param name="codeModel">The code model.</param>
        /// <returns></returns>
		public static IEnumerable<CodeNamespace> GetNamespaces(this CodeModel codeModel)
		{
			NamespaceComparer comparer = new NamespaceComparer();

			return QueryableCodeModel.GetElements<CodeNamespace>(codeModel.CodeElements)
				.Where(ns => (ns.Name != MS && ns.Name != Microsoft && ns.Name != System))
				.Distinct(comparer);
		}

		#endregion

		#region Private Implementation
		private class NamespaceComparer : IEqualityComparer<CodeNamespace>
		{
			public bool Equals(CodeNamespace x, CodeNamespace y)
			{
				return x.Name == y.Name;
			}

			public int GetHashCode(CodeNamespace obj)
			{
				return 1;
			}
		} 
		#endregion
	}
}