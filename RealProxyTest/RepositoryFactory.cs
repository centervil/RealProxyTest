using System;

namespace RealProxyTest
{
    public class RepositoryFactory
    {
        private static void Log(string msg, object arg = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg, arg);
            Console.ResetColor();
        }
        public static IRepository<T> Create<T>()
        {
            IRepository<T> repository = new Repository<T>();
            var dynamicProxy = new DynamicProxy(repository, typeof(IRepository<T>));
            dynamicProxy.BeforeExecute += (s, e) => Log(
              "Before executing '{0}'", e.MethodName);
            dynamicProxy.AfterExecute += (s, e) => Log(
              "After executing '{0}'", e.MethodName);
            dynamicProxy.ErrorExecuting += (s, e) => Log(
              "Error executing '{0}'", e.MethodName);
            dynamicProxy.Filter = m => !m.Name.StartsWith("Get");
            return dynamicProxy.GetTransparentProxy() as IRepository<T>;
        }
    }
}