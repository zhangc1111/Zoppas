namespace Zoppas
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            foreach (var screen in screens)
            {
                if (screen.Primary)
                {
                    MainWindow main = new MainWindow();
                    main.Left = screen.WorkingArea.Left;
                    main.Top = screen.WorkingArea.Top;
                    main.Width = screen.WorkingArea.Width;
                    main.Height = screen.WorkingArea.Height;
                    main.Show();
                    main.WindowState = WindowState.Maximized;
                }
                else
                {
                    SubWindow sub = new SubWindow();
                    sub.Left = screen.WorkingArea.Left;
                    sub.Top = screen.WorkingArea.Top;
                    sub.Width = screen.WorkingArea.Width;
                    sub.Height = screen.WorkingArea.Height;
                    sub.Show();
                    sub.WindowState = WindowState.Maximized;
                }
            }
        }
    }
}
