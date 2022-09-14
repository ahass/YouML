using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Windows.Controls;
using Task = System.Threading.Tasks.Task;

namespace YouML
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class YouMLToolWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        private uint _toolWindowInstanceId;


        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("42572517-6e7f-4489-8782-6d29ff9de4e2");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;


        private IVsWindowFrame _vsWindowFrame;
        private YouMLToolWindowControl _youMLToolWindowControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="YouMLToolWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private YouMLToolWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(ShowUmlToolWindow, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static YouMLToolWindowCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in YouMLToolWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new YouMLToolWindowCommand(package, commandService);
        }

        ///// <summary>
        ///// Shows the tool window when the menu item is clicked.
        ///// </summary>
        ///// <param name="sender">The event sender.</param>
        ///// <param name="e">The event args.</param>
        //private void Execute(object sender, EventArgs e)
        //{
        //    this.package.JoinableTaskFactory.RunAsync(async delegate
        //    {
        //        ToolWindowPane window = await this.package.ShowToolWindowAsync(typeof(YouMLToolWindow), 0, true, this.package.DisposalToken);
        //        if ((null == window) || (null == window.Frame))
        //        {
        //            throw new NotSupportedException("Cannot create tool window");
        //        }
        //    });
        //}

        /// <summary>
        /// Shows the tool window with result.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ShowUmlToolWindow(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            const string toolWindowGuid = "6BF11835-8A31-496C-B01E-94F731AFC7CE";

            _youMLToolWindowControl = new YouMLToolWindowControl();
            _vsWindowFrame = CreateToolWindow("Uml Diagram " + _toolWindowInstanceId, toolWindowGuid, _youMLToolWindowControl);

            ErrorHandler.ThrowOnFailure(_vsWindowFrame.Show());
        }

        private IVsWindowFrame CreateToolWindow(string caption, string guid, UserControl userControl)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var guidNull = Guid.Empty;
            var position = new int[1];

            var uiShell = (IVsUIShell)package.GetServiceAsync(typeof(SVsUIShell)).Result;

            var toolWindowPersistenceGuid = new Guid(guid);

            var result = uiShell.CreateToolWindow((uint)__VSCREATETOOLWIN.CTW_fMultiInstance,
                ++_toolWindowInstanceId, userControl, ref guidNull, ref toolWindowPersistenceGuid,
                ref guidNull, null, caption, position, out var windowFrame);

            ErrorHandler.ThrowOnFailure(result);

            return windowFrame;
        }
    }
}
