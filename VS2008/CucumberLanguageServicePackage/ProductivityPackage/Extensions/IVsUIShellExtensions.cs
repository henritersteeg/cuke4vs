using System;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProductivityPackage
{
	public static class IVsUIShellExtensions
	{
		public static IVsUIHierarchyWindow GetSolutionExplorer(this IVsUIShell uiShell)
		{
			Guid tempGuid = new Guid(ToolWindowGuids.SolutionExplorer);
			IVsWindowFrame frame;
			ErrorHandler.ThrowOnFailure(uiShell.FindToolWindow(0, ref tempGuid, out frame));
			frame.Show();
			object win;
			ErrorHandler.ThrowOnFailure(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out win));

			return win as IVsUIHierarchyWindow;
		}

		public static void SelectItemInSolutionExplorer(this IVsUIShell uiShell, HierarchyNode node)
		{
            if (node != null)
            {
                uiShell.GetSolutionExplorer().ExpandItem((node.Hierarchy as IVsUIHierarchy), node.ItemId, EXPANDFLAGS.EXPF_SelectItem);
            }
		}

		public static void SelectItemInSolutionExplorer(this IVsUIShell uiShell, IVsSolution solution, string fileName)
		{
            HierarchyNodeIterator it = new HierarchyNodeIterator(solution);

            HierarchyNode hierarchyNode = it.FirstOrDefault(node => node.FullName == fileName);

            uiShell.SelectItemInSolutionExplorer(hierarchyNode);
		}
	}
}