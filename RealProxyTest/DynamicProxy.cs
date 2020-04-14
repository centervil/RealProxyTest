using System;
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
        private readonly MarshalByRefObject _decorated;
        private Predicate<MethodInfo> _filter;

        public event EventHandler<IMethodCallMessage> BeforeExecute;

        public event EventHandler<IMethodReturnMessage> AfterExecute;

        public event EventHandler<IMethodReturnMessage> ErrorExecuting;

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

        private void OnAfterExecute(IMethodReturnMessage methodReturn)
        {
            if (AfterExecute != null)
            {
                MethodInfo methodInfo = methodReturn.MethodBase as MethodInfo;
                if (_filter(methodInfo))
                {
                    AfterExecute(this, methodReturn);
                }
            }
        }

        private void OnErrorExecuting(IMethodReturnMessage methodReturn)
        {
            if (ErrorExecuting != null)
            {
                MethodInfo methodInfo = methodReturn.MethodBase as MethodInfo;
                if (_filter(methodInfo))
                {
                    ErrorExecuting(this, methodReturn);
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
            RemotingServices.GetRealProxy(_decorated).InitializeServerObject(constructionCall);
            MarshalByRefObject tp = GetTransparentProxy() as MarshalByRefObject;
            IConstructionReturnMessage res = EnterpriseServicesHelper.CreateConstructionReturnMessage(constructionCall, tp);
            return res;
        }

        private IMethodReturnMessage InvokeMethod(IMethodCallMessage methodCall)
        {
            OnBeforeExecute(methodCall);
            IMethodReturnMessage res = RemotingServices.ExecuteMessage(_decorated, methodCall);

            if (res.Exception == null)
            {
                OnAfterExecute(res);
                return res;
            }
            else
            {
                OnErrorExecuting(res);
                return new ReturnMessage(res.Exception, methodCall);
            }
        }
    }
}
