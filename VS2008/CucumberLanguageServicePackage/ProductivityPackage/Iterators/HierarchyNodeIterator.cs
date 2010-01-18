using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProductivityPackage
{
    /// <summary>
    /// Iterator for IVsHierarchy hierarchy
    /// </summary>
    public sealed class HierarchyNodeIterator : IEnumerable<HierarchyNode>
    {
        #region Fields
        private IVsSolution solution;
		private IVsHierarchy hierarchy;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyNodeIterator"/> class.
        /// </summary>
        /// <param name="solution">The solution.</param>
        public HierarchyNodeIterator(IVsSolution solution)
        {
            if(solution == null)
            {
                throw new ArgumentNullException("solution");
            }

            this.solution = solution;
        }

		public HierarchyNodeIterator(IVsHierarchy hierarchy)
		{
			if(hierarchy == null)
			{
				throw new ArgumentNullException("hierarchy");
			}

			this.hierarchy = hierarchy;
		}
        #endregion

        #region Public Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<HierarchyNode> GetEnumerator()
        {
			if(this.hierarchy == null)
			{
				IEnumHierarchies penum;
				Guid nullGuid = Guid.Empty;
				int hr = solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_ALLINSOLUTION, ref nullGuid, out penum);
				Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
				if((VSConstants.S_OK == hr) && (penum != null))
				{
					uint fetched;
					IVsHierarchy[] rgelt = new IVsHierarchy[1];
					while(penum.Next(1, rgelt, out fetched) == 0 && fetched == 1)
					{
						foreach(HierarchyNode hier in Enumerate(rgelt[0], VSConstants.VSITEMID_ROOT, 0))
						{
							yield return hier;
						}
					}
				}
			}
			else
			{
				foreach(HierarchyNode hier in Enumerate(this.hierarchy, VSConstants.VSITEMID_ROOT, 0))
				{
					yield return hier;
				}
			}
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Private Implementation
        private IEnumerable<HierarchyNode> Enumerate(IVsHierarchy hierarchy, uint itemid, int recursionLevel)
        {
            yield return new HierarchyNode(hierarchy, itemid);

            int hr;
            object pVar;
            recursionLevel++;

            hr = hierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_FirstChild, out pVar);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);

            if(VSConstants.S_OK == hr)
            {
                uint childId = GetItemId(pVar);
                while(childId != VSConstants.VSITEMID_NIL)
                {
                    foreach(HierarchyNode nestedNode in Enumerate(hierarchy, childId, recursionLevel))
                    {
                        yield return nestedNode;
                    }

                    hr = hierarchy.GetProperty(childId, (int)__VSHPROPID.VSHPROPID_NextSibling, out pVar);

                    if(VSConstants.S_OK == hr)
                    {
                        childId = GetItemId(pVar);
                    }
                    else
                    {
                        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
                        break;
                    }
                }
            }
        }

        private uint GetItemId(object pvar)
        {
            if(pvar == null) return VSConstants.VSITEMID_NIL;
            if(pvar is int) return (uint)(int)pvar;
            if(pvar is uint) return (uint)pvar;
            if(pvar is short) return (uint)(short)pvar;
            if(pvar is ushort) return (uint)(ushort)pvar;
            if(pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }
        #endregion
    }
}