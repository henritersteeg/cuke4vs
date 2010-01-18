using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace Microsoft.Samples.LinqToCodeModel.Extensions
{
    /// <summary>
    /// Extensions methods for CodeInterface
    /// </summary>
	public static class CodeInterfaceExtensions
	{
		#region Public Implementation
        /// <summary>
        /// Gets IEnumerable elements.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="codeInterface">The code interface.</param>
        /// <returns></returns>
		public static IEnumerable<TElement> GetIEnumerable<TElement>(this CodeInterface codeInterface)
		{
			return QueryableCodeModel.GetElements<TElement>(codeInterface.Children);
		}

        /// <summary>
        /// Gets interface functions.
        /// </summary>
        /// <param name="codeInterface">The code interface.</param>
        /// <returns></returns>
		public static IEnumerable<CodeFunction> GetFunctions(this CodeInterface codeInterface)
		{
			return QueryableCodeModel.GetElements<CodeFunction>(codeInterface.Children);
		}

        /// <summary>
        /// Gets interface functions by name.
        /// </summary>
        /// <param name="codeInterface">The code interface.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
		public static IEnumerable<CodeFunction> GetFunctionsByName(this CodeInterface codeInterface, string functionName)
		{
			return QueryableCodeModel.GetElementsWhere<CodeFunction>(
				codeInterface.Children,
				codeFunction => codeFunction.Name == functionName);
		}

        /// <summary>
        /// Gets interface properties.
        /// </summary>
        /// <param name="codeInterface">The code interface.</param>
        /// <returns></returns>
		public static IEnumerable<CodeProperty> GetProperties(this CodeInterface codeInterface)
		{
			return QueryableCodeModel.GetElements<CodeProperty>(codeInterface.Children);
		}

        /// <summary>
        /// Gets interface attributes.
        /// </summary>
        /// <param name="codeInterface">The code interface.</param>
        /// <returns></returns>
		public static IEnumerable<CodeAttribute> GetAttributes(this CodeInterface codeInterface)
		{
			return QueryableCodeModel.GetElements<CodeAttribute>(codeInterface.Children);
		}

        /// <summary>
        /// Gets interface implemented interfaces.
        /// </summary>
        /// <param name="codeInterface">The code interface.</param>
        /// <returns></returns>
		public static IEnumerable<CodeInterface> GetImplementedInterfaces(this CodeInterface codeInterface)
		{
			return QueryableCodeModel.GetElements<CodeInterface>(codeInterface.Bases);
		}

        /// <summary>
        /// Gets interface members.
        /// </summary>
        /// <param name="codeInterface">The code interface.</param>
        /// <returns></returns>
		public static IEnumerable<CodeElement> GetMembers(this CodeInterface codeInterface)
		{
			return QueryableCodeModel.GetElements<CodeElement>(codeInterface.Members);
		} 
		#endregion
	}
}