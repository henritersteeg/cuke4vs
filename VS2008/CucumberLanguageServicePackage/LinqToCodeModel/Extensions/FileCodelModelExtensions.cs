using System.Collections.Generic;
using System.Linq;
using System.IO;
using EnvDTE;

namespace Microsoft.Samples.LinqToCodeModel.Extensions
{
    /// <summary>
    /// Extensions methods for FileCodeModel
    /// </summary>
	public static class FileCodelModelExtensions
	{
		#region Public Implementation
        /// <summary>
        /// Gets Inumerable elements.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="fileCodeModel">The file code model.</param>
        /// <returns></returns>
		public static IEnumerable<TElement> GetIEnumerable<TElement>(this FileCodeModel fileCodeModel)
		{
			return QueryableCodeModel.GetElements<TElement>(fileCodeModel.CodeElements);
		}

        /// <summary>
        /// Gets namespaces.
        /// </summary>
        /// <param name="fileCodeModel">The file code model.</param>
        /// <returns></returns>
		public static IEnumerable<CodeNamespace> GetNamespaces(this FileCodeModel fileCodeModel)
		{
			return QueryableCodeModel.GetElements<CodeNamespace>(fileCodeModel.CodeElements);
		}

        /// <summary>
        /// Gets namespace by name.
        /// </summary>
        /// <param name="fileCodeModel">The file code model.</param>
        /// <param name="nsName">Name of the ns.</param>
        /// <returns></returns>
		public static CodeNamespace GetNamespaceByName(this FileCodeModel fileCodeModel, string nsName)
		{
			return QueryableCodeModel.GetElementWhere<CodeNamespace>(
				fileCodeModel.CodeElements,
				ns => ns.Name == nsName);
		}

        /// <summary>
        /// Gets classes by name.
        /// </summary>
        /// <param name="fileCodeModel">The file code model.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
		public static IEnumerable<CodeClass> GetClassesByName(this FileCodeModel fileCodeModel, string className)
		{
			return QueryableCodeModel.GetElementsWhere<CodeClass>(
				fileCodeModel.CodeElements,
				codeClass => codeClass.Name == className);
		}

        /// <summary>
        /// Gets interfaces by name.
        /// </summary>
        /// <param name="fileCodeModel">The file code model.</param>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <returns></returns>
		public static IEnumerable<CodeInterface> GetInterfacesByName(this FileCodeModel fileCodeModel, string interfaceName)
		{
			return QueryableCodeModel.GetElementsWhere<CodeInterface>(
				fileCodeModel.CodeElements,
				codeInterface => codeInterface.Name == interfaceName);
		}

        /// <summary>
        /// Gets class by full name.
        /// </summary>
        /// <param name="fileCodeModel">The file code model.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
		public static CodeClass GetClassByFullName(this FileCodeModel fileCodeModel, string fullName)
		{
			string className;
			string nsName;

			ExtractNames(fullName, out className, out nsName);

			CodeNamespace codeNamespace = null;
			CodeClass codeClass = null;

			codeNamespace = GetNamespaceByName(fileCodeModel, nsName);

			if(codeNamespace != null)
			{
				codeClass = codeNamespace.GetClassByName(className);
			}

			return codeClass;
		}

        /// <summary>
        /// Gets interface by full name.
        /// </summary>
        /// <param name="fileCodeModel">The file code model.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
		public static CodeInterface GetInterfaceByFullName(this FileCodeModel fileCodeModel, string fullName)
		{
			string interfaceName;
			string nsName;

			ExtractNames(fullName, out interfaceName, out nsName);

			CodeNamespace codeNamespace = null;
			CodeInterface codeInterface = null;

			codeNamespace = GetNamespaceByName(fileCodeModel, nsName);

			if(codeNamespace != null)
			{
				codeInterface = codeNamespace.GetInterfaceByName(interfaceName);
			}

			return codeInterface;
		} 
		#endregion

		#region Private Implementation
		private static void ExtractNames(string fullName, out string className, out string nsName)
		{
			className = Path.GetExtension(fullName).Replace(".", string.Empty);
			nsName = fullName.Replace(Path.GetExtension(fullName), string.Empty);
		} 
		#endregion
	}
}