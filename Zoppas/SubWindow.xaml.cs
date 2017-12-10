namespace Zoppas
{
    using System.Windows;
    using Zoppas.ViewModel;

    /// <summary>
    /// Interaction logic for SubWindow.xaml
    /// </summary>
    public partial class SubWindow : Window
    {
        public SubWindow()
        {
            DataContext = new SubViewModel();
            InitializeComponent();
        }
    }
}
