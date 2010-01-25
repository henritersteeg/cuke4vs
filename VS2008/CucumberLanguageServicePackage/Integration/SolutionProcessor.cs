using System;
using System.Diagnostics;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using ProductivityPackage;

namespace CucumberLanguageServices.Integration
{
    public class SolutionProcessor 
    {
        public StepProvider StepProvider { get; set; }
        public IVsSolution Solution { get; set; }

        private System.Threading.Thread _thread;
        private bool _stopProcessing;

        public void Process()
        {
            _thread = new System.Threading.Thread(Run);
            _thread.Start();
        }

        private void Run()
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
                if (_stopProcessing)
                    break;
            }
            Debug.Print("SolutionProcessor.Run() is READY!");
        }

        public void Stop()
        {
            if (_thread == null) return;

            _stopProcessing = true;
            _thread.Join();
        }
    }
}
