using System;
using System.Windows;
using TestClientWpf.SoapWebService;

namespace TestClientWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void cmdCallSoapServiceMethod_Click(object sender, RoutedEventArgs e)
        {
            bool webserverConnectionTestResult;

            //Создание объекта веб-службы.
            using (Service webServiceInstance = new Service())
            {
                //Вызов метода проверки соединения с БД.
                webserverConnectionTestResult = webServiceInstance.WebserverConnectionTest();
            }

            var messageText = webserverConnectionTestResult ? 
                "Connections successful." : 
                "Connection failed";
            MessageBox.Show(messageText);
        }

        private void cmdCallWebApiServiceMethod_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
