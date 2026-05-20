using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    public struct TaskAwaiter : ICriticalNotifyCompletion
    {
        private static readonly MethodInfo PrepForRemoting = GetPrepForRemotingMethodInfo();

        private static readonly object[] EmptyParams = new object[0];

        internal const bool ContinueOnCapturedContextDefault = true;

        private const string InvalidOperationExceptionTaskNotCompleted = "The task has not yet completed.";

        private readonly Task task;

        public bool IsCompleted
        {
            get { return task.IsCompleted; }
        }

        private static bool IsValidLocationForInlining
        {
            get
            {
                var current = SynchronizationContext.Current;
                if (current != null && current.GetType() != typeof(SynchronizationContext))
                    return false;
                return TaskScheduler.Current == TaskScheduler.Default;
            }
        }

        static TaskAwaiter()
        {
        }

        internal TaskAwaiter(Task task)
        {
            this.task = task;
        }

        public void OnCompleted(Action continuation)
        {
            OnCompletedInternal(task, continuation, true);
        }

        [SecurityCritical]
        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompletedInternal(task, continuation, true);
        }

        public void GetResult()
        {
            ValidateEnd(task);
        }

        internal static void ValidateEnd(Task task)
        {
            if (task.Status == TaskStatus.RanToCompletion)
                return;
            HandleNonSuccess(task);
        }

        private static void HandleNonSuccess(Task task)
        {
            if (!task.IsCompleted)
            {
                try
                {
                    task.Wait();
                }
                catch
                {
                }
            }
            if (task.Status == TaskStatus.RanToCompletion)
                return;
            ThrowForNonSuccess(task);
        }

        private static void ThrowForNonSuccess(Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.Canceled:
                    throw new TaskCanceledException(task);
                case TaskStatus.Faulted:
                    throw PrepareExceptionForRethrow(task.Exception.InnerException);
                default:
                    throw new InvalidOperationException("The task has not yet completed.");
            }
        }

        internal static void OnCompletedInternal(Task task, Action continuation, bool continueOnCapturedContext)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");
            var scheduler = continueOnCapturedContext ? TaskScheduler.Current : TaskScheduler.Default;
            task.ContinueWith(result =>
            {
                if (scheduler == TaskScheduler.Default)
                {
                    RunNoException(continuation);
                }
                else
                {
                    Task.Factory.StartNew(state => RunNoException((Action)state), continuation,
                                          CancellationToken.None, TaskCreationOptions.None,
                                          TaskScheduler.Default);
                }
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private static void RunNoException(Action continuation)
        {
            try
            {
                continuation();
            }
            catch (Exception ex)
            {
                AsyncMethodBuilderCore.ThrowAsync(ex, null);
            }
        }

        internal static Exception PrepareExceptionForRethrow(Exception exc)
        {
            if (PrepForRemoting != null)
            {
                try
                {
                    PrepForRemoting.Invoke(exc, EmptyParams);
                }
                catch
                {
                }
            }
            return exc;
        }

        private static MethodInfo GetPrepForRemotingMethodInfo()
        {
            try
            {
                return typeof(Exception).GetMethod("PrepForRemoting", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            catch
            {
                return null;
            }
        }
    }

    public struct TaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly Task<TResult> task;

        public bool IsCompleted
        {
            get { return task.IsCompleted; }
        }

        internal TaskAwaiter(Task<TResult> task)
        {
            this.task = task;
        }

        public void OnCompleted(Action continuation)
        {
            TaskAwaiter.OnCompletedInternal(task, continuation, true);
        }

        [SecurityCritical]
        public void UnsafeOnCompleted(Action continuation)
        {
            TaskAwaiter.OnCompletedInternal(task, continuation, true);
        }

        public TResult GetResult()
        {
            TaskAwaiter.ValidateEnd(task);
            return task.Result;
        }
    }
}
