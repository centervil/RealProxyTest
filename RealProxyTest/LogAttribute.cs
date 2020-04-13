using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Contexts;
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
            var rp = new DynamicProxy(target, serverType);

            rp.BeforeExecute += (s, e) => Log("Start '{0}'", e.MethodName);
            rp.BeforeExecute += (s, e) => LogInArgs("InArgs '{0}'", e.MethodBase.GetParameters(), e.InArgs);

            rp.AfterExecute += (s, e) => Log("End '{0}'", e.MethodName);
            rp.AfterExecute += (s, e) => Log("ReturnValue '{0}'", e.ReturnValue);
            rp.AfterExecute += (s, e) => LogOutArgs("OutArgs '{0}'", e.MethodBase.GetParameters(), e.OutArgs);

            rp.ErrorExecuting += (s, e) => Log("Error executing '{0}'", e.MethodName);
            //rp.Filter = m => !m.Name.StartsWith("Get");

            return rp.GetTransparentProxy() as MarshalByRefObject;
        }

        public override RealProxy CreateProxy(ObjRef objRef, Type serverType, object serverObject, Context serverContext)
        {
            return base.CreateProxy(objRef, serverType, serverObject, serverContext);
        }
        private static void Log(string msg, object arg = null)
        {
            Console.WriteLine(msg, arg);
        }
        private static void Log(string msg, object[] args)
        {
            var str = string.Join(",", args);
            Console.WriteLine(msg, str);
        }
        private void LogOutArgs(string msg, ParameterInfo[] parameterInfo, object[] outArgs)
        {
            var outArgsName = "Name : " + string.Join(", ", parameterInfo.Where(p => p.IsOut).Select(p => p.Name));
            var outArgsValue = "Value : " + string.Join(", ", outArgs);
            Console.WriteLine(msg, outArgsName + " / " + outArgsValue);
        }
        private void LogInArgs(string msg, ParameterInfo[] parameterInfo, object[] inArgs)
        {
            var inArgName = "Name : " + string.Join(", ", parameterInfo.Where(p => !p.IsOut).Select(p => p.Name));
            var inArgValue = "Value : " + string.Join(", ", inArgs);
            Console.WriteLine(msg, inArgName + " / " + inArgValue);
        }

    }
}
