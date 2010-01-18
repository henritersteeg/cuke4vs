using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.Samples.LinqToCodeModel.Enums;

namespace Microsoft.Samples.LinqToCodeModel.Extensions
{
    /// <summary>
    /// Extensions methods for CodeNamespace
    /// </summary>
	public static class CodeNamespaceExtensions
	{
		#region Public Implementation
        /// <summary>
        /// Gets IEnumerable elements.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <returns></returns>
		public static IEnumerable<TElement> GetIEnumerable<TElement>(this CodeNamespace codeNamespace)
		{
			return QueryableCodeModel.GetElements<TElement>(codeNamespace.Children);
		}

        /// <summary>
        /// Gets classes.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <returns></returns>
		public static IEnumerable<CodeClass> GetClasses(this CodeNamespace codeNamespace)
		{
			return QueryableCodeModel.GetElements<CodeClass>(codeNamespace.Children);
		}

        /// <summary>
        /// Gets class by name.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
		public static CodeClass GetClassByName(this CodeNamespace codeNamespace, string className)
		{
			return QueryableCodeModel.GetElementWhere<CodeClass>(
				codeNamespace.Children,
				codeClass => codeClass.Name == className);
		}

        /// <summary>
        /// Gets classes by modifier.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="modifier">The modifier.</param>
        /// <returns></returns>
		public static IEnumerable<CodeClass> GetClassesByModifier(this CodeNamespace codeNamespace, ClassModifier modifier)
		{
			switch(modifier)
			{
				case ClassModifier.Abstract:
					return GetClasses(codeNamespace)
						.Where(codeClass => codeClass.IsAbstract);
				case ClassModifier.Concrete:
					return GetClasses(codeNamespace)
					   .Where(codeClass => !codeClass.IsAbstract);
				default:
					return GetClasses(codeNamespace);
			}
		}

        /// <summary>
        /// Gets classes by visibility.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="visibility">The visibility.</param>
        /// <returns></returns>
		public static IEnumerable<CodeClass> GetClassesByVisibility(this CodeNamespace codeNamespace, vsCMAccess visibility)
		{
			return GetClasses(codeNamespace)
				.Where(codeClass => codeClass.Access == visibility);
		}

        /// <summary>
        /// Gets classes by location.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="location">The location.</param>
        /// <returns></returns>
		public static IEnumerable<CodeClass> GetClassesByLocation(this CodeNamespace codeNamespace, vsCMInfoLocation location)
		{
			return GetClasses(codeNamespace)
					.Where(codeClass => codeClass.InfoLocation == location);
		}

        /// <summary>
        /// Gets interfaces.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <returns></returns>
		public static IEnumerable<CodeInterface> GetInterfaces(this CodeNamespace codeNamespace)
		{
			return QueryableCodeModel.GetElements<CodeInterface>(codeNamespace.Children);
		}

        /// <summary>
        /// Gets interface by name.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <returns></returns>
		public static CodeInterface GetInterfaceByName(this CodeNamespace codeNamespace, string interfaceName)
		{
			return QueryableCodeModel.GetElementWhere<CodeInterface>(
				codeNamespace.Children,
				codeInterface => codeInterface.Name == interfaceName);
		}

        /// <summary>
        /// Gets interfaces by visibility.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="visibility">The visibility.</param>
        /// <returns></returns>
		public static IEnumerable<CodeInterface> GetInterfacesByVisibility(this CodeNamespace codeNamespace, vsCMAccess visibility)
		{
			return GetInterfaces(codeNamespace)
				.Where(codeInterface => codeInterface.Access == visibility);
		}

        /// <summary>
        /// Gets interfaces by location.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="location">The location.</param>
        /// <returns></returns>
		public static IEnumerable<CodeInterface> GetInterfacesByLocation(this CodeNamespace codeNamespace, vsCMInfoLocation location)
		{
			return GetInterfaces(codeNamespace)
					.Where(codeInterface => codeInterface.InfoLocation == location);
		} 
		#endregion	
	}
}