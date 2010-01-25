using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace CucumberLanguageServices.Integration
{
    public class SolutionEventMonitor : IVsSolutionEvents
    {
        public StepProvider StepProvider { get; set; }
        public IVsSolution Solution { get; set; }

        private SolutionProcessor _solutionProcessor;
        private uint _cookie;

        public void ProcessSolution()
        {
            _solutionProcessor = new SolutionProcessor { Solution = Solution, StepProvider = StepProvider };
            _solutionProcessor.Process();
        }

        public void MonitorChanges()
        {
            Solution.AdviseSolutionEvents(this, out _cookie);
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            ProcessSolution();
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            if (_solutionProcessor != null)
                _solutionProcessor.Stop();
            if (StepProvider != null)
                StepProvider.Clear();
            return VSConstants.S_OK;
        }


        #region Not implemented SolutionEvents...

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
