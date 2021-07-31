namespace ChilledWindows
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Windows;

    public class App : Application
    {
        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }

        [STAThread, DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            App app1 = new App();
            app1.InitializeComponent();
            app1.Run();
        }
    }
}

