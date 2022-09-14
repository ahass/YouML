using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YouML.Annotations;
using YouML.Parser;
using YouML.Renderer;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using PlantUml.Net;

namespace YouML
{
    public class YouMLToolWindowViewModel : INotifyPropertyChanged
    {
        public YouMLToolWindowViewModel()
        {            
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var fileCode = GetFileContent();

                if (fileCode.Equals(string.Empty)) return;

                var classCode = ClassParser.Parse(fileCode);

                var plantCode = UmlRenderer.Render(classCode);

                var renderFactory = new RendererFactory();

                var plantUmlRenderer = renderFactory.CreateRenderer();

                using (var mStream = new MemoryStream(plantUmlRenderer.Render(plantCode, OutputFormat.Png)))
                {
                    UmlOutput = BitmapFrame.Create(mStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
        }

        private ImageSource _umlOutput;

        public ImageSource UmlOutput
        {
            get => _umlOutput;
            set
            {
                _umlOutput = value;
                OnPropertyChanged(nameof(UmlOutput));
            }
        }

        private string GetFileContent()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsHierarchy hierarchy = null;
            uint itemid = VSConstants.VSITEMID_NIL;

            if (!IsSingleProjectItemSelection(out hierarchy, out itemid)) return String.Empty;
            // Get the file path
            string itemFullPath = null;
            ((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);
            var transformFileInfo = new FileInfo(itemFullPath);

            // then check if the file is named 'web.config'
            bool isCsFile = string.Compare(".cs", transformFileInfo.Extension, StringComparison.OrdinalIgnoreCase) == 0;

            if (isCsFile)
                return File.ReadAllText(itemFullPath);
            else
            {
                return String.Empty;
            }

            //    menuCommand.Visible = true;
            //    menuCommand.Enabled = true;
            //}
        }
        public static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemId)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            hierarchy = null;
            itemId = VSConstants.VSITEMID_NIL;
            int hr = VSConstants.S_OK;

            var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            IVsMultiItemSelect multiItemSelect = null;
            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainerPtr = IntPtr.Zero;

            try
            {
                hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemId, out multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemId == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                // multiple items are selected
                if (multiItemSelect != null) return false;

                // there is a hierarchy root node selected, thus it is not a single item inside a project

                if (itemId == VSConstants.VSITEMID_ROOT) return false;

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null) return false;

                Guid guidProjectId = Guid.Empty;

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out guidProjectId)))
                {
                    return false; // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
                }

                // if we got this far then there is a single project item selected
                return true;
            }
            finally
            {
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}