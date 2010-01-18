using System;
using System.Linq;
using System.Collections.Generic;
using EnvDTE;
using System.Collections;

namespace Microsoft.Samples.LinqToCodeModel
{
    /// <summary>
    /// CodeElement iterator
    /// </summary>
	public sealed class CodeElementIterator : IEnumerable<CodeElement>
	{
		#region Fields
		private CodeElements codeElements;
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeElementIterator"/> class.
        /// </summary>
        /// <param name="codeElements">The code elements.</param>
		public CodeElementIterator(CodeElements codeElements)
		{
			if(codeElements == null)
			{
				throw new ArgumentNullException("codeElements");
			}

			this.codeElements = codeElements;
		}
		#endregion

		#region Public Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
		public IEnumerator<CodeElement> GetEnumerator()
		{
			return (Enumerate(codeElements)
			    .Select(codeElement => codeElement)).GetEnumerator();
		}

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Private Implementation
		private IEnumerable<CodeElement> Enumerate(CodeElements codeElements)
		{
			foreach(CodeElement element in codeElements)
			{
				yield return element;

				CodeElements childrens;

				try
				{
					childrens = element.Children;
				}
				catch(NotImplementedException)
				{
					childrens = null;
				}

				if(childrens != null)
				{
					foreach(CodeElement subElement in Enumerate(childrens))
					{
						yield return subElement;
					}
				}
			}
		}
		#endregion
	}
}