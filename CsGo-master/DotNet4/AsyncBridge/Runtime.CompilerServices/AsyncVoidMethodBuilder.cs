using System.Diagnostics;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    public struct AsyncVoidMethodBuilder : IAsyncMethodBuilder
    {
        private readonly SynchronizationContext synchronizationContext;
        private AsyncMethodBuilderCore coreState;
        private object objectIdForDebugger;
        private static int preventUnobservedTaskExceptionsInvoked;

        private object ObjectIdForDebugger
        {
            get
            {
                return objectIdForDebugger ?? (objectIdForDebugger = new object());
            }
        }

        static AsyncVoidMethodBuilder()
        {
            try
            {
                PreventUnobservedTaskExceptions();
            }
            catch
            {
            }
        }

        private AsyncVoidMethodBuilder(SynchronizationContext synchronizationContext)
        {
            this.synchronizationContext = synchronizationContext;
            if (synchronizationContext != null)
                synchronizationContext.OperationStarted();
            coreState = new AsyncMethodBuilderCore();
            objectIdForDebugger = null;
        }

        internal static void PreventUnobservedTaskExceptions()
        {
            if (Interlocked.CompareExchange(ref preventUnobservedTaskExceptionsInvoked, 1, 0) != 0)
                return;
            TaskScheduler.UnobservedTaskException += (EventHandler<UnobservedTaskExceptionEventArgs>)((s, e) => e.SetObserved());
        }

        public static AsyncVoidMethodBuilder Create()
        {
            return new AsyncVoidMethodBuilder(SynchronizationContext.Current);
        }

        [DebuggerStepThrough]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            coreState.Start(ref stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            coreState.SetStateMachine(stateMachine);
        }

        void IAsyncMethodBuilder.PreBoxInitialization()
        {
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
where TAwaiter : INotifyCompletion
where TStateMachine : IAsyncStateMachine
        {
            try
            {
                var completionAction = coreState.GetCompletionAction(ref this, ref stateMachine);
                awaiter.OnCompleted(completionAction);
            }
            catch (Exception ex)
            {
                AsyncMethodBuilderCore.ThrowAsync(ex, null);
            }
        }

        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
where TAwaiter : ICriticalNotifyCompletion
where TStateMachine : IAsyncStateMachine
        {
            try
            {
                var completionAction = coreState.GetCompletionAction(ref this, ref stateMachine);
                awaiter.UnsafeOnCompleted(completionAction);
            }
            catch (Exception ex)
            {
                AsyncMethodBuilderCore.ThrowAsync(ex, null);
            }
        }

        public void SetResult()
        {
            if (synchronizationContext == null)
                return;
            NotifySynchronizationContextOfCompletion();
        }

        public void SetException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");
            if (synchronizationContext != null)
            {
                try
                {
                    AsyncMethodBuilderCore.ThrowAsync(exception, synchronizationContext);
                }
                finally
                {
                    NotifySynchronizationContextOfCompletion();
                }
            }
            else
                AsyncMethodBuilderCore.ThrowAsync(exception, null);
        }

        private void NotifySynchronizationContextOfCompletion()
        {
            try
            {
                synchronizationContext.OperationCompleted();
            }
            catch (Exception ex)
            {
                AsyncMethodBuilderCore.ThrowAsync(ex, null);
            }
        }
    }
}
