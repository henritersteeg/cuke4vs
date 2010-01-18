using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace Microsoft.Samples.LinqToCodeModel.Extensions
{
    /// <summary>
    /// Extensions methods for CodeFunction
    /// </summary>
	public static class CodeFunctionExtensions
	{
		#region Public Implementation
        /// <summary>
        /// Gets IEnumerable elements.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="codeFunction">The code function.</param>
        /// <returns></returns>
		public static IEnumerable<TElement> GetIEnumerable<TElement>(this CodeFunction codeFunction)
		{
			return QueryableCodeModel.GetElements<TElement>(codeFunction.Children);
		}

        /// <summary>
        /// Gets function parameters.
        /// </summary>
        /// <param name="codeFunction">The code function.</param>
        /// <returns></returns>
		public static IEnumerable<CodeParameter> GetParameters(this CodeFunction codeFunction)
		{
			return QueryableCodeModel.GetElements<CodeParameter>(codeFunction.Children);
		}

        /// <summary>
        /// Gets function parameters by name.
        /// </summary>
        /// <param name="codeFunction">The code function.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
		public static CodeParameter GetParameterByName(this CodeFunction codeFunction, string parameterName)
		{
			return QueryableCodeModel.GetElementWhere<CodeParameter>(
				codeFunction.Children,
				parameter => parameter.Name == parameterName);
		}

        /// <summary>
        /// Gets function attributes.
        /// </summary>
        /// <param name="codeFunction">The code function.</param>
        /// <returns></returns>
		public static IEnumerable<CodeAttribute> GetAttributes(this CodeFunction codeFunction)
		{
			return QueryableCodeModel.GetElements<CodeAttribute>(codeFunction.Children);
		}

        /// <summary>
        /// Gets function attributes by type name.
        /// </summary>
        /// <param name="codeFunction">The code function.</param>
        /// <param name="attributeTypeName">Name of the attribute type.</param>
        /// <returns></returns>
		public static IEnumerable<CodeAttribute> GetAttributesByTypeName(this CodeFunction codeFunction, string attributeTypeName)
		{
			return QueryableCodeModel.GetElementsWhere<CodeAttribute>(
				codeFunction.Children,
				attribute => attribute.Name == attributeTypeName);
		}

        /// <summary>
        /// Gets function overloads.
        /// </summary>
        /// <param name="codeFunction">The code function.</param>
        /// <returns></returns>
		public static IEnumerable<CodeFunction> GetOverloads(this CodeFunction codeFunction)
		{
			return QueryableCodeModel.GetElements<CodeFunction>(codeFunction.Overloads);
		} 
		#endregion
	}
}