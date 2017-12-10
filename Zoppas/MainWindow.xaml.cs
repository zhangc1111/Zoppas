namespace Zoppas
{
    using System;
    using System.Windows;
    using Zoppas.ViewModel;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }

        /// <summary>
        /// 主窗口退出
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Environment.Exit(0);
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void btn_hide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void button_amplify_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            button_amplify.Visibility = Visibility.Collapsed;
            button_narrow.Visibility = Visibility.Visible;
        }

        private void button_narrow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            button_narrow.Visibility = Visibility.Collapsed;
            button_amplify.Visibility = Visibility.Visible;
        }

        private void btn_amplify_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (this.WindowState)
            {
                case (WindowState.Normal):
                    button_amplify_Click(sender, e);
                    break;
                case (WindowState.Maximized):
                    button_narrow_Click(sender, e);
                    break;
            }
        }

        private void btn_amplify_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ScrollViewerParameter_GotFocus(object sender, RoutedEventArgs e)
        {
            borderPara.BorderThickness = new Thickness(0, 1, 0, 1);
        }

        private void ScrollViewerParameter_LostFocus(object sender, RoutedEventArgs e)
        {
            borderPara.BorderThickness = new Thickness(0);
        }


        private void btn_to_login_Click(object sender, RoutedEventArgs e)
        {
            //backorregister backorregister = new backorregister();
            //Application.Current.MainWindow = backorregister;
            //backorregister.ShowDialog();
            //TabItem_maintain.IsEnabled = backorregister.IsAuthorized;
        }

        private void ComboBoxItem_short15_Selected(object sender, RoutedEventArgs e)
        {
            this.frame_op20.Source = new Uri("manual_page/manual_op10_1.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op30.Source = new Uri("manual_page/manual_op30_1.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op40.Source = new Uri("manual_page/manual_op40_1.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op50.Source = new Uri("manual_page/manual_op50_1.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op60.Source = new Uri("manual_page/manual_op60_1.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op70.Source = new Uri("manual_page/manual_op70_1.xaml", UriKind.RelativeOrAbsolute);
        }

        private void ComboBoxItem_short25_Selected(object sender, RoutedEventArgs e)
        {
            this.frame_op20.Source = new Uri("manual_page/manual_op10_2.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op30.Source = new Uri("manual_page/manual_op30_2.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op40.Source = new Uri("manual_page/manual_op40_2.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op50.Source = new Uri("manual_page/manual_op50_2.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op60.Source = new Uri("manual_page/manual_op60_2.xaml", UriKind.RelativeOrAbsolute);
            this.frame_op70.Source = new Uri("manual_page/manual_op70_2.xaml", UriKind.RelativeOrAbsolute);
        }

        private void ComboBoxItem_seek_Selected(object sender, RoutedEventArgs e)
        {
            Border_seek.Visibility = Visibility.Visible;
            Border_detect.Visibility = Visibility.Collapsed;
        }

        private void ComboBoxItem_detect_Selected(object sender, RoutedEventArgs e)
        {
            Border_seek.Visibility = Visibility.Collapsed;
            Border_detect.Visibility = Visibility.Visible;
        }

        private void btn_camera_Click(object sender, RoutedEventArgs e)
        {
            switch (comboBox_camera.SelectedIndex)
            {
                case (0):
                    camera_test.Content = "选中相机1";
                    break;
                case (1):
                    camera_test.Content = "选中相机2";
                    break;
                case (2):
                    camera_test.Content = "选中相机3";
                    break;
                case (3):
                    camera_test.Content = "选中相机4";
                    break;
            }
        }

        private void btn_light_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_lighthandle.SelectedIndex == 0)
            {
                switch (comboBox_light.SelectedIndex)
                {
                    case (0):
                        light_test.Content = "打开光源1";
                        break;
                    case (1):
                        light_test.Content = "打开光源2";
                        break;
                    case (2):
                        light_test.Content = "打开光源3";
                        break;
                    case (3):
                        light_test.Content = "打开光源4";
                        break;
                }
            }
            else if (comboBox_lighthandle.SelectedIndex == 1)
            {
                switch (comboBox_light.SelectedIndex)
                {
                    case (0):
                        light_test.Content = "关闭光源1";
                        break;
                    case (1):
                        light_test.Content = "关闭光源2";
                        break;
                    case (2):
                        light_test.Content = "关闭光源3";
                        break;
                    case (3):
                        light_test.Content = "关闭光源4";
                        break;
                }
            }
        }


        //    ObservableCollection<coffee> InfoList = new ObservableCollection<coffee>();

        //    internal ObservableCollection<coffee> MyInfoList
        //    {
        //        get { return InfoList; }
        //        set { InfoList = value; }
        //    }

        //    private int index = 0;
        //    static bool[] fakeCheckResult = { false, true, false, true, true, true, true, true, false };
        //    //static bool[] P = { true, true, false, true, true, true, true, true, false };

        //    public void Updatelist(bool[] checkResult)
        //    {
        //        coffee abcd = new coffee();
        //        abcd.ID = ++index;
        //        abcd.OP10 = checkResult[1];
        //        abcd.Result = checkResult[0];
        //        InfoList.Add(abcd);
        //        int setIndex = 0;
        //        for (int i = InfoList.Count - 1; i >= 0 && setIndex < 8; i--)
        //        {
        //            InfoList[i].setOp(setIndex, checkResult[setIndex + 1]);              

        //            setIndex++;
        //        }

        //        while ((InfoList.Count > 15))
        //        {
        //            InfoList.RemoveAt(0);
        //        }

        //        listview_detect.ItemsSource = InfoList;
        //    }

        //private void button_add_Click(object sender, RoutedEventArgs e)
        //    {
        //    Updatelist(fakeCheckResult);
        //}



        //    public int _1_1_detect_max
        //    {
        //        get;
        //        set;
        //    }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //vm.AddNewCoffee();
        }
    }
}
