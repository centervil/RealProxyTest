using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace RealProxyTest
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LogAttribute : ProxyAttribute
    {
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            MarshalByRefObject target = base.CreateInstance(serverType);
            var rp = new DynamicProxy<MarshalByRefObject>(target);

            rp.BeforeExecute += (s, e) => Log(
              "Before executing '{0}'", e.MethodName);
            rp.AfterExecute += (s, e) => Log(
              "After executing '{0}'", e.MethodName);
            rp.ErrorExecuting += (s, e) => Log(
              "Error executing '{0}'", e.MethodName);
            rp.Filter = m => !m.Name.StartsWith("Get");

            return rp.GetTransparentProxy() as MarshalByRefObject;
        }
        private static void Log(string msg, object arg = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg, arg);
            Console.ResetColor();
        }
    }
}
