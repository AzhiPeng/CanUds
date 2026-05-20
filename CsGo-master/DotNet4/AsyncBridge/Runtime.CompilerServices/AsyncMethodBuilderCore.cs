using System.Diagnostics;
using System.Security;
using System.Threading;

namespace System.Runtime.CompilerServices
{
    internal struct AsyncMethodBuilderCore
    {
        private IAsyncStateMachine stateMachine;

        [SecuritySafeCritical]
        [DebuggerStepThrough]
        internal void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            if ((object)stateMachine == null)
                throw new ArgumentNullException("stateMachine");
            stateMachine.MoveNext();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            if (stateMachine == null)
                throw new ArgumentNullException("stateMachine");
            if (this.stateMachine != null)
                throw new InvalidOperationException("The builder was not properly initialized.");
            this.stateMachine = stateMachine;
        }

        [SecuritySafeCritical]
        internal Action GetCompletionAction<TMethodBuilder, TStateMachine>(ref TMethodBuilder builder, ref TStateMachine stateMachine)
where TMethodBuilder : IAsyncMethodBuilder
where TStateMachine : IAsyncStateMachine
        {
            var moveNextRunner = new MoveNextRunner(ExecutionContext.Capture());
            Action action = moveNextRunner.Run;
            if (this.stateMachine == null)
            {
                builder.PreBoxInitialization();
                this.stateMachine = stateMachine;
                this.stateMachine.SetStateMachine(this.stateMachine);
            }
            moveNextRunner.StateMachine = this.stateMachine;
            return action;
        }

        internal static void ThrowAsync(Exception exception, SynchronizationContext targetContext)
        {
            if (targetContext != null)
            {
                try
                {
                    targetContext.Post(state =>
                    {
                        throw TaskAwaiter.PrepareExceptionForRethrow((Exception)state);
                    }, exception);
                    return;
                }
                catch (Exception ex)
                {
                    exception = new AggregateException(new[] { exception, ex });
                }
            }
            ThreadPool.QueueUserWorkItem(state =>
            {
                throw TaskAwaiter.PrepareExceptionForRethrow((Exception)state);
            }, exception);
        }

        private sealed class MoveNextRunner
        {
            private readonly ExecutionContext context;
            internal IAsyncStateMachine StateMachine;
            [SecurityCritical]
            private static ContextCallback invokeMoveNext;

            [SecurityCritical]
            internal MoveNextRunner(ExecutionContext context)
            {
                this.context = context;
            }

            [SecuritySafeCritical]
            internal void Run()
            {
                if (context == null)
                {
                    StateMachine.MoveNext();
                    return;
                }

                try
                {
                    var callback = invokeMoveNext;
                    if (callback == null)
                        invokeMoveNext = callback = InvokeMoveNext;
                    ExecutionContext.Run(context, callback, StateMachine);
                }
                finally
                {
                    context.Dispose();
                }
            }

            [SecurityCritical]
            private static void InvokeMoveNext(object stateMachine)
            {
                ((IAsyncStateMachine)stateMachine).MoveNext();
            }
        }
    }
}
