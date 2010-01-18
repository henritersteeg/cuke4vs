using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.Samples.LinqToCodeModel.Enums;

namespace Microsoft.Samples.LinqToCodeModel.Extensions
{
    /// <summary>
    /// Extensions methods for CodeClass
    /// </summary>
	public static class CodeClassExtensions
	{
		#region Public Implementation
        /// <summary>
        /// Gets IEnumerable elements.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="codeClass">The code class.</param>
        /// <returns></returns>
		public static IEnumerable<TElement> GetIEnumerable<TElement>(this CodeClass codeClass)
		{
			return QueryableCodeModel.GetElements<TElement>(codeClass.Children);
		}

        /// <summary>
        /// Gets class functions.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <returns></returns>
		public static IEnumerable<CodeFunction> GetFunctions(this CodeClass codeClass)
		{
			return QueryableCodeModel.GetElements<CodeFunction>(codeClass.Children);
		}

        /// <summary>
        /// Gets class functions by modifier.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <param name="modifier">The modifier.</param>
        /// <returns></returns>
		public static IEnumerable<CodeFunction> GetFunctionsByModifier(this CodeClass codeClass, FunctionModifier modifier)
		{
			switch(modifier)
			{
				case FunctionModifier.Abstract:
					return GetFunctions(codeClass)
						.Where(codeFunction => codeFunction.MustImplement);
				case FunctionModifier.Virtual:
					return GetFunctions(codeClass)
						.Where(codeFunction => codeFunction.CanOverride);
				default:
					return GetFunctions(codeClass);
			}
		}

        /// <summary>
        /// Gets class functions by function kind.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
		public static IEnumerable<CodeFunction> GetFunctionsByKind(this CodeClass codeClass, FunctionKind kind)
		{
			switch(kind)
			{
				case FunctionKind.Constructor:
					return GetFunctions(codeClass)
						.Where(codeFunction => codeFunction.FunctionKind == vsCMFunction.vsCMFunctionConstructor);
				case FunctionKind.Destructor:
					return GetFunctions(codeClass)
						.Where(codeFunction => codeFunction.FunctionKind == vsCMFunction.vsCMFunctionDestructor);
                case FunctionKind.Function:
                    return GetFunctions(codeClass)
                        .Where(codeFunction => codeFunction.FunctionKind == vsCMFunction.vsCMFunctionFunction);
                case FunctionKind.Sub:
                    return GetFunctions(codeClass)
                        .Where(codeFunction => codeFunction.FunctionKind == vsCMFunction.vsCMFunctionSub);
                case FunctionKind.PropertySet:
					return GetFunctions(codeClass)
						.Where(codeFunction => codeFunction.FunctionKind == vsCMFunction.vsCMFunctionPropertySet);
				case FunctionKind.PropertyGet:
					return GetFunctions(codeClass)
						.Where(codeFunction => codeFunction.FunctionKind == vsCMFunction.vsCMFunctionPropertyGet);
				default:
					return GetFunctions(codeClass);
			}
		}

        /// <summary>
        /// Gets class functions by visibility.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <param name="visibility">The visibility.</param>
        /// <returns></returns>
		public static IEnumerable<CodeFunction> GetFunctionsByVisibility(this CodeClass codeClass, vsCMAccess visibility)
		{
            return QueryableCodeModel.GetElementsWhere<CodeFunction>(
                codeClass.Children,
                codeFunction => codeFunction.Access == visibility);
		}

        /// <summary>
        /// Gets class functions by function name.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
		public static IEnumerable<CodeFunction> GetFunctionsByName(this CodeClass codeClass, string functionName)
		{
			return QueryableCodeModel.GetElementsWhere<CodeFunction>(
				codeClass.Children,
				codeFunction => codeFunction.Name == functionName);
		}

        /// <summary>
        /// Gets class attributes.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <returns></returns>
		public static IEnumerable<CodeAttribute> GetAttributes(this CodeClass codeClass)
		{
			return QueryableCodeModel.GetElements<CodeAttribute>(codeClass.Children);
		}

        /// <summary>
        /// Gets class attributes by type name.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <param name="attributeTypeName">Name of the attribute type.</param>
        /// <returns></returns>
		public static IEnumerable<CodeAttribute> GetAttributesByTypeName(this CodeClass codeClass, string attributeTypeName)
		{
			return QueryableCodeModel.GetElementsWhere<CodeAttribute>(
				codeClass.Children,
				codeAttribute => codeAttribute.Name == attributeTypeName);
		}

        /// <summary>
        /// Gets class variables.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <returns></returns>
		public static IEnumerable<CodeVariable> GetVariables(this CodeClass codeClass)
		{
			return QueryableCodeModel.GetElements<CodeVariable>(codeClass.Children)
				.Where(codeVariable => !codeVariable.IsConstant);
		}

        /// <summary>
        /// Gets class variables by visibility.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <param name="visibility">The visibility.</param>
        /// <returns></returns>
		public static IEnumerable<CodeVariable> GetVariablesByVisibility(this CodeClass codeClass, vsCMAccess visibility)
		{
			return GetVariables(codeClass)
				.Where(codeVariable => codeVariable.Access == visibility);
		}

        /// <summary>
        /// Gets class constants.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <returns></returns>
		public static IEnumerable<CodeVariable> GetConstants(this CodeClass codeClass)
		{
			return QueryableCodeModel.GetElements<CodeVariable>(codeClass.Children)
				.Where(codeVariable => codeVariable.IsConstant);
		}

        /// <summary>
        /// Gets class constants by visibility.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <param name="visibility">The visibility.</param>
        /// <returns></returns>
		public static IEnumerable<CodeVariable> GetConstantsByVisibility(this CodeClass codeClass, vsCMAccess visibility)
		{
			return GetConstants(codeClass)
				.Where(constant => constant.Access == visibility);
		}

        /// <summary>
        /// Gets class properties.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <returns></returns>
		public static IEnumerable<CodeProperty> GetProperties(this CodeClass codeClass)
		{
			return QueryableCodeModel.GetElements<CodeProperty>(codeClass.Children);
		}

        /// <summary>
        /// Gets class properties by visibility.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <param name="visibility">The visibility.</param>
        /// <returns></returns>
		public static IEnumerable<CodeProperty> GetPropertiesByVisibility(this CodeClass codeClass, vsCMAccess visibility)
		{
			return GetProperties(codeClass)
				.Where(codeProperty => codeProperty.Access == visibility);
		}

        /// <summary>
        /// Gets class base classes.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <returns></returns>
		public static IEnumerable<CodeClass> GetBaseClasses(this CodeClass codeClass)
		{
			return QueryableCodeModel.GetElements<CodeClass>(codeClass.Bases);
		}

        /// <summary>
        /// Gets class implemented interfaces.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <returns></returns>
		public static IEnumerable<CodeInterface> GetImplementedInterfaces(this CodeClass codeClass)
		{
			return QueryableCodeModel.GetElements<CodeInterface>(codeClass.ImplementedInterfaces);
		}

        /// <summary>
        /// Gets class members.
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <returns></returns>
		public static IEnumerable<CodeElement> GetMembers(this CodeClass codeClass)
		{
			return QueryableCodeModel.GetElements<CodeElement>(codeClass.Members);
		} 
		#endregion
	}
}