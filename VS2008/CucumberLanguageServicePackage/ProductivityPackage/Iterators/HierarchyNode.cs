using System;
using System.Drawing;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProductivityPackage
{
	/// <summary>
	/// IVsHierarchy Wrapper
	/// </summary>
	public class HierarchyNode : IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HierarchyNode"/> class.
		/// </summary>
		/// <param name="hierarchy">The hierarchy.</param>
		/// <param name="itemId">The item id.</param>
		public HierarchyNode(IVsHierarchy hierarchy, uint itemId)
		{
			this.hierarchy = hierarchy;
			this.itemId = itemId;
		}

		private uint itemId;

		/// <summary>
		/// Gets the item id.
		/// </summary>
		/// <value>The item id.</value>
		public uint ItemId
		{
			get { return itemId; }
		}

		private IVsHierarchy hierarchy;

		/// <summary>
		/// Gets the hierarchy.
		/// </summary>
		/// <value>The hierarchy.</value>
		public IVsHierarchy Hierarchy
		{
			get { return hierarchy; }
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return GetProperty<string>(__VSHPROPID.VSHPROPID_Name); }
		}

		private string fullName;

		/// <summary>
		/// Gets the full name.
		/// </summary>
		/// <value>The full name.</value>
		public string FullName
		{
			get
			{
				if(string.IsNullOrEmpty(fullName))
				{
					try
					{
						IVsProject prj = Hierarchy as IVsProject;
						ErrorHandler.ThrowOnFailure(prj.GetMkDocument(itemId, out fullName));
					}
					catch
					{
						fullName = string.Empty;
					}
				}

				return fullName;
			}
		}

		/// <summary>
		/// Gets the ext object.
		/// </summary>
		/// <value>The ext object.</value>
		public object ExtObject
		{
			get { return GetProperty<object>(__VSHPROPID.VSHPROPID_ExtObject); }
		}

		public object BrowsableObject
		{
			get
			{
				return GetProperty<object>(__VSHPROPID.VSHPROPID_BrowseObject);
			}
		}

		/// <summary>
		/// Gets the subtype.
		/// </summary>
		/// <value>The subtype.</value>
		public string Subtype
		{
			get
			{
				return GetProperty<string>(__VSHPROPID.VSHPROPID_ItemSubType);
			}
		}

		private Icon icon = null;

		/// <summary>
		/// Gets the icon.
		/// </summary>
		/// <value>The icon.</value>
		public Icon Icon
		{
			get
			{
				if(icon == null)
				{
					IVsProject project = Hierarchy as IVsProject;
					string file;

					ErrorHandler.ThrowOnFailure(project.GetMkDocument(ItemId, out file));

					icon = NativeMethods.GetIcon(file);
				}

				return icon;
			}
		}

		public override string ToString()
		{
			return this.Name;
		}

		public void Open()
		{
			ProjectItem item = this.ExtObject as ProjectItem;

			if(item != null)
			{
				Window window = item.DTE.OpenFile(EnvDTE.Constants.vsViewKindPrimary, this.FullName);

				window.Activate();
			}
		}

		/// <summary>
		/// Gets the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetObject<T>()
			where T : class
		{
			return (hierarchy as T);
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propId">The prop id.</param>
		/// <returns></returns>
		public T GetProperty<T>(__VSHPROPID propId)
		{
			return GetProperty<T>(propId, this.itemId);
		}

		private T GetProperty<T>(__VSHPROPID propId, uint itemid)
		{
			object value = null;
			int hr = hierarchy.GetProperty(itemid, (int)propId, out value);
			if(hr != VSConstants.S_OK || value == null)
			{
				return default(T);
			}
			return (T)value;
		}

		private static uint GetItemId(object pvar)
		{
			if(pvar == null) return VSConstants.VSITEMID_NIL;
			if(pvar is int) return (uint)(int)pvar;
			if(pvar is uint) return (uint)pvar;
			if(pvar is short) return (uint)(short)pvar;
			if(pvar is ushort) return (uint)(ushort)pvar;
			if(pvar is long) return (uint)(long)pvar;
			return VSConstants.VSITEMID_NIL;
		}

		#region IDisposable Members

		private bool disposed;
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if(!this.disposed)
			{
				if(disposing)
				{
					// Dispose managed resources.
				}
			}
			disposed = true;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="HierarchyNode"/> is reclaimed by garbage collection.
		/// </summary>
		~HierarchyNode()
		{
			Dispose(false);
		}

		#endregion
	}
}