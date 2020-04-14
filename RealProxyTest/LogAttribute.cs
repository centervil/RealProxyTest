using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
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

            rp.BeforeExecute += (s, e) => TraceMethodStartLog(e);
            rp.AfterExecute += (s, e) => TraceMethodEndLog(e);
            rp.ErrorExecuting += (s, e) => TraceErrorLog(e);
            //rp.Filter = m => !m.Name.StartsWith("Get");

            return rp.GetTransparentProxy() as MarshalByRefObject;
        }

        public override RealProxy CreateProxy(ObjRef objRef, Type serverType, object serverObject, Context serverContext)
        {
            return base.CreateProxy(objRef, serverType, serverObject, serverContext);
        }

        private static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
        private void TraceMethodStartLog(IMethodCallMessage methodCall)
        {
            var methodName = methodCall?.MethodName;
            var inArgValue = string.Join(", ", methodCall?.InArgs);
            var msg = $"'{methodName}' is called with Args : {inArgValue}";
            Log(msg);
        }
        private void TraceMethodEndLog(IMethodReturnMessage methodReturn)
        {
            var methodName = methodReturn?.MethodName;
            var outArgValue = string.Join(", ", methodReturn?.OutArgs);
            var returnValue = methodReturn?.ReturnValue;
            var msg = $"'{methodName}' is End with returnValue : {returnValue} outArgs : {outArgValue}";
            Log(msg);
        }
        private void TraceErrorLog(IMethodReturnMessage methodReturn)
        {
            var methodName = methodReturn?.MethodName;
            var errorName = methodReturn?.Exception.GetType().Name;
            var msg = $"'{methodName}' throws Exception : {errorName}";
            Log(msg);
        }
    }
}
