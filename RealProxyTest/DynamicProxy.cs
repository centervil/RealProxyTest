using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;

namespace RealProxyTest
{
    internal class DynamicProxy : RealProxy
    {
        private MarshalByRefObject _decorated;
        private Predicate<MethodInfo> _filter;

        public event EventHandler<IMethodCallMessage> BeforeExecute;

        public event EventHandler<IMethodCallMessage> AfterExecute;

        public event EventHandler<IMethodCallMessage> ErrorExecuting;

        public DynamicProxy(MarshalByRefObject decorated, Type serverType)
          : base(serverType)
        {
            _decorated = decorated;
            Filter = m => true;
        }

        public Predicate<MethodInfo> Filter
        {
            get => _filter;
            set
            {
                if (value == null)
                {
                    _filter = m => true;
                }
                else
                {
                    _filter = value;
                }
            }
        }

        private void OnBeforeExecute(IMethodCallMessage methodCall)
        {
            if (BeforeExecute != null)
            {
                MethodInfo methodInfo = methodCall.MethodBase as MethodInfo;
                if (_filter(methodInfo))
                {
                    BeforeExecute(this, methodCall);
                }
            }
        }

        private void OnAfterExecute(IMethodCallMessage methodCall)
        {
            if (AfterExecute != null)
            {
                MethodInfo methodInfo = methodCall.MethodBase as MethodInfo;
                if (_filter(methodInfo))
                {
                    AfterExecute(this, methodCall);
                }
            }
        }

        private void OnErrorExecuting(IMethodCallMessage methodCall)
        {
            if (ErrorExecuting != null)
            {
                MethodInfo methodInfo = methodCall.MethodBase as MethodInfo;
                if (_filter(methodInfo))
                {
                    ErrorExecuting(this, methodCall);
                }
            }
        }
        public override IMessage Invoke(IMessage msg)
        {
            if (msg is IConstructionCallMessage constructionCall)
            {
                return InvokeConstructor(constructionCall);
            }

            if (msg is IMethodCallMessage methodCall)
            {
                return InvokeMethod(methodCall);
            }

            throw new InvalidOperationException();
        }

        private IConstructionReturnMessage InvokeConstructor(IConstructionCallMessage constructionCall)
        {
            RemotingServices.GetRealProxy(this._decorated).InitializeServerObject(constructionCall);
            var tp = this.GetTransparentProxy() as MarshalByRefObject;
            var res = EnterpriseServicesHelper.CreateConstructionReturnMessage(constructionCall, tp);
            return res;
        }

        private IMethodReturnMessage InvokeMethod(IMethodCallMessage methodCall)
        {
            try
            {
                OnBeforeExecute(methodCall);
                var res = RemotingServices.ExecuteMessage(this._decorated, methodCall);
                OnAfterExecute(methodCall);
                return res;
            }
            catch (Exception e)
            {
                OnErrorExecuting(methodCall);
                return new ReturnMessage(e, methodCall);
            }
        }
    }
}
