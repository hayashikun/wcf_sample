using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wcf;

namespace wcf_sample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private WCF.IHost[] hosts;
        public MainWindow()
        {
            InitializeComponent();

            StartHost();
        }

        private void StartHost()
        {
            var p1 = Process.Start(@"..\..\..\host\bin\Debug\host.exe", "1");
            var p2 = Process.Start(@"..\..\..\host\bin\Debug\host.exe", "2");

            p1.WaitForInputIdle();
            p2.WaitForInputIdle();
            hosts = new WCF.IHost[] {
                WCF.Callback.ConnectHost("1", HandleData, Handle),
                WCF.Callback.ConnectHost("2", HandleData, Handle)
            };
    }

        private void HandleData(string id, Data data)
        {
            switch (id)
            {
                case "1":
                    H1TextTextBox.Text = data.text;
                    H1NumberTextBox.Text = data.number.ToString();
                    break;
                case "2":
                    H2TextTextBox.Text = data.text;
                    H2NumberTextBox.Text = data.number.ToString();
                    break;
            }
            Console.WriteLine("HandleData");
        }

        private void Handle(string id)
        {
            Console.WriteLine("Handle");
        }

        private void H1Load_Click(object sender, RoutedEventArgs e)
        {
            hosts[0].LoadData();
        }

        private void H2Load_Click(object sender, RoutedEventArgs e)
        {
            hosts[1].LoadData();
        }

        private void H1Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            hosts[0].UpdateSlider(H1Slider.Value);
        }

        private void H2Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            hosts[1].UpdateSlider(H2Slider.Value);

        }
    }
}
