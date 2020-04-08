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
              //RepositoryFactory.Create<Customer>();
            Customer customer = new Customer
            {
                Id = 1,
                Name = "Customer 1",
                Address = "Address 1"
            };
            customerRepository.Add(customer);
            customerRepository.Update(customer);
            customerRepository.Delete(customer);
            Console.WriteLine("\r\nEnd program - logging with dynamic proxy\r\n***");
            Console.ReadLine();
        }
    }
}
