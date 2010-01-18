using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using ProductivityPackage;

namespace CucumberLanguageServices.Integration
{
    public class SolutionProcessor
    {
        public StepProvider StepProvider { get; set; }
        public IVsSolution Solution { get; set; }
        public void Process()
        {
            // We don't use Linq here, since this fails for some mystical reason... 
            foreach (var node in new HierarchyNodeIterator(Solution))
            {
                var projectItem = node.ExtObject as ProjectItem;
                if (projectItem == null)
                    continue;

                if (projectItem.Kind != EnvDTE.Constants.vsProjectItemKindPhysicalFile)
                    continue;
                StepProvider.ProcessItem(projectItem);
            }
        }
    }
}
