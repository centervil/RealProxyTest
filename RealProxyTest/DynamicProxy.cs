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
            //ConstructorInfo constructorInfo = constructionCall.MethodBase as ConstructorInfo;
            //var result = constructorInfo.Invoke(_decorated, constructionCall.InArgs);
            //return new ReturnMessage(
            //      result, null, 0, constructionCall.LogicalCallContext, constructionCall);
            //IConstructionReturnMessage constructionReturn = InitializeServerObject(constructionCall);
            //_decorated = GetUnwrappedServer();
            //SetStubData(this, _decorated);
            //return constructionReturn;

            RealProxy rp = RemotingServices.GetRealProxy(this._decorated);
            var res = rp.InitializeServerObject(constructionCall);
            MarshalByRefObject tp = this.GetTransparentProxy() as MarshalByRefObject;
            return EnterpriseServicesHelper.CreateConstructionReturnMessage(constructionCall, tp);
        }

        private IMethodReturnMessage InvokeMethod(IMethodCallMessage methodCall)
        {
            //Func<IMethodReturnMessage> baseInvoke = () => RemotingServices.ExecuteMessage(Target, methodCall);

            //var newInvoke = methodCall.MethodBase.GetCustomAttributes<AspectAttribute>(true)
            //    .Reverse()
            //    .Aggregate(baseInvoke, (f, a) => () => a.Invoke(f, Target, methodCall));

            //return newInvoke();
            MethodInfo methodInfo = methodCall.MethodBase as MethodInfo;
            try
            {
                OnBeforeExecute(methodCall);
                var res = RemotingServices.ExecuteMessage(this._decorated, methodCall);

                //object result = methodInfo.Invoke(_decorated, methodCall.InArgs);
                OnAfterExecute(methodCall);
                //return new ReturnMessage(
                //  result, null, 0, methodCall.LogicalCallContext, methodCall);
                return res;
            }
            catch (Exception e)
            {
                OnErrorExecuting(methodCall);
                return new ReturnMessage(e, methodCall);
            }
        }
    }
    //public override IMessage Invoke(IMessage msg)
    //{
    //    IMethodCallMessage methodCall = msg as IMethodCallMessage;
    //    MethodInfo methodInfo = methodCall.MethodBase as MethodInfo;
    //    if (methodInfo is null)
    //    {
    //        OnBeforeExecute(methodCall);
    //        ConstructorInfo constructorInfo = methodCall.MethodBase as ConstructorInfo;
    //        constructorInfo.Invoke(_decorated, methodCall.InArgs);
    //        OnAfterExecute(methodCall);
    //        //return new ReturnMessage(e, methodCall); //retrunの仕方がわからない
    //        }
    //    OnBeforeExecute(methodCall);
    //    try
    //    {
    //        object result = methodInfo.Invoke(_decorated, methodCall.InArgs);
    //        OnAfterExecute(methodCall);
    //        return new ReturnMessage(
    //          result, null, 0, methodCall.LogicalCallContext, methodCall);
    //    }
    //    catch (Exception e)
    //    {
    //        OnErrorExecuting(methodCall);
    //        return new ReturnMessage(e, methodCall);
    //    }
    //}
}
