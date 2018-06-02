using System;
using System.Collections.Generic;
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

namespace host
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private string id = "";
        public MainWindow()
        {
            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2)
            {
                id = args[1];
                Title = "Host: " + id;
            }
            WCF.Host.StartHost(id, LoadData, UpdateSlider);
        }

        private Data LoadData()
        {
            var data = Dispatcher.Invoke(new Func<Data>(() =>
            {
                int number = -1;
                int.TryParse(NumberTextBox.Text, out number);
                return new Data() { text = TextTextBox.Text, number = number };
            }));
            return data;
        }

        private void UpdateSlider(double val)
        {
            Dispatcher.Invoke(new Action(() => Slider.Value = val));
        }
    }
}
