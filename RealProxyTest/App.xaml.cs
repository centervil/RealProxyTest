using System;
using System.Windows;

namespace RealProxyTest
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Console.WriteLine("***\r\n Begin program - logging with dynamic proxy\r\n");
            IRepository customerRepository = new Repository();
            Customer customer = new Customer
            {
                Id = 1,
                Name = "test Name",
                Address = "test Address"
            };
            try
            {
                customerRepository.Add(customer, out var test);
                customerRepository.Update(customer);
                customerRepository.Delete(customer);
                customerRepository.ThrowException();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Console.WriteLine("\r\nEnd program - logging with dynamic proxy\r\n***");
            Console.ReadLine();
        }
    }
}
