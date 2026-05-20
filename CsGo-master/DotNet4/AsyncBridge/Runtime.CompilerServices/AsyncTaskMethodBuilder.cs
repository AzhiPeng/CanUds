using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    public struct AsyncTaskMethodBuilder : IAsyncMethodBuilder
    {
        private static readonly TaskCompletionSource<VoidTaskResult> CachedCompleted = AsyncTaskMethodBuilder<VoidTaskResult>.DefaultResultTask;

#pragma warning disable 649
        private AsyncTaskMethodBuilder<VoidTaskResult> builder;
#pragma warning restore 649

        public Task Task
        {
            get
            {
                return builder.Task;
            }
        }

        private object ObjectIdForDebugger
        {
            get
            {
                return Task;
            }
        }

        static AsyncTaskMethodBuilder()
        {
        }

        public static AsyncTaskMethodBuilder Create()
        {
            return new AsyncTaskMethodBuilder();
        }

        [DebuggerStepThrough]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            builder.Start(ref stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            builder.SetStateMachine(stateMachine);
        }

        void IAsyncMethodBuilder.PreBoxInitialization()
        {
#pragma warning disable 168
            var task = Task;
#pragma warning restore 168
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
where TAwaiter : INotifyCompletion
where TStateMachine : IAsyncStateMachine
        {
            builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
where TAwaiter : ICriticalNotifyCompletion
where TStateMachine : IAsyncStateMachine
        {
            builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }

        public void SetResult()
        {
            builder.SetResult(CachedCompleted);
        }

        public void SetException(Exception exception)
        {
            builder.SetException(exception);
        }

        internal void SetNotificationForWaitCompletion(bool enabled)
        {
            builder.SetNotificationForWaitCompletion(enabled);
        }
    }

    public struct AsyncTaskMethodBuilder<TResult> : IAsyncMethodBuilder
    {
        internal static readonly TaskCompletionSource<TResult> DefaultResultTask = AsyncMethodTaskCache<TResult>.CreateCompleted(default(TResult));
#pragma warning disable 649
        private AsyncMethodBuilderCore coreState;
#pragma warning restore 649
        private TaskCompletionSource<TResult> task;

        internal TaskCompletionSource<TResult> CompletionSource
        {
            get
            {
                var completionSource = task;
                if (completionSource == null)
                    task = completionSource = new TaskCompletionSource<TResult>();
                return completionSource;
            }
        }

        public Task<TResult> Task
        {
            get
            {
                return CompletionSource.Task;
            }
        }

        private object ObjectIdForDebugger
        {
            get
            {
                return Task;
            }
        }

        static AsyncTaskMethodBuilder()
        {
            try
            {
                AsyncVoidMethodBuilder.PreventUnobservedTaskExceptions();
            }
            catch
            {
            }
        }

        public static AsyncTaskMethodBuilder<TResult> Create()
        {
            return new AsyncTaskMethodBuilder<TResult>();
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
#pragma warning disable 168
            var task = Task;
#pragma warning restore 168
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

        public void SetResult(TResult result)
        {
            var completionSource = task;
            if (completionSource == null)
                task = GetTaskForResult(result);
            else if (!completionSource.TrySetResult(result))
                throw new InvalidOperationException("The Task was already completed.");
        }

        internal void SetResult(TaskCompletionSource<TResult> completedTask)
        {
            if (task == null)
                task = completedTask;
            else
                SetResult(default(TResult));
        }

        public void SetException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");
            var completionSource = CompletionSource;
            var setException = (exception is OperationCanceledException ? completionSource.TrySetCanceled() : completionSource.TrySetException(exception));
            if (!setException)
                throw new InvalidOperationException("The Task was already completed.");
        }

        internal void SetNotificationForWaitCompletion(bool enabled)
        {
        }

        private TaskCompletionSource<TResult> GetTaskForResult(TResult result)
        {
            var asyncMethodTaskCache = AsyncMethodTaskCache<TResult>.Singleton;
            if (asyncMethodTaskCache == null)
                return AsyncMethodTaskCache<TResult>.CreateCompleted(result);
            return asyncMethodTaskCache.FromResult(result);
        }
    }
}
