using System;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;

namespace CucumberLanguageServices.Integration
{
    public class CodeModelEventMonitor : IDisposable
    {
        public StepProvider StepProvider { get; set; }
        public DTE DTE { get; set; }

        private CodeModelEvents _codeModelEvents;

        public void MonitorChanges()
        {
            if (DTE == null) return;

            _codeModelEvents = DTE.Events.GetObject("CodeModelEvents") as CodeModelEvents;
            if (_codeModelEvents == null) return;

            _codeModelEvents.ElementAdded += CodeElementAdded;
            _codeModelEvents.ElementChanged += CodeElementChanged;
            _codeModelEvents.ElementDeleted += CodeElementDeleted;
        }

        private void CodeElementAdded(CodeElement element)
        {
            ProcessElement(element);
        }

        private void CodeElementChanged(CodeElement element, vsCMChangeKind change)
        {
            ProcessElement(element);
        }

        private void CodeElementDeleted(object parent, CodeElement element)
        {
            ProcessElement(element);
        }

        private void ProcessElement(CodeElement element)
        {
            if (StepProvider == null || element == null) return;
            Debug.Print("CodeModelEventMonitor: processing {0}: {1}", element.Kind, element.Name);

            StepProvider.ProcessItem(element.ProjectItem);
        }

        public void Dispose()
        {
            if (_codeModelEvents == null) return;

            _codeModelEvents.ElementAdded -= CodeElementAdded;
            _codeModelEvents.ElementChanged -= CodeElementChanged;
            _codeModelEvents.ElementDeleted -= CodeElementDeleted;
        }
    }
}
