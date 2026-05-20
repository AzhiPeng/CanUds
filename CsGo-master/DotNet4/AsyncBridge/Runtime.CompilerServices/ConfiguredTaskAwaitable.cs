using System.Security;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    public struct ConfiguredTaskAwaitable<TResult>
    {
        private readonly ConfiguredTaskAwaiter configuredTaskAwaiter;

        internal ConfiguredTaskAwaitable(Task<TResult> task, bool continueOnCapturedContext)
        {
            configuredTaskAwaiter = new ConfiguredTaskAwaiter(task, continueOnCapturedContext);
        }

        public ConfiguredTaskAwaiter GetAwaiter()
        {
            return configuredTaskAwaiter;
        }

        public struct ConfiguredTaskAwaiter : ICriticalNotifyCompletion
        {
            private readonly Task<TResult> task;
            private readonly bool continueOnCapturedContext;

            public bool IsCompleted
            {
                get
                {
                    return task.IsCompleted;
                }
            }

            internal ConfiguredTaskAwaiter(Task<TResult> task, bool continueOnCapturedContext)
            {
                this.task = task;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public void OnCompleted(Action continuation)
            {
                TaskAwaiter.OnCompletedInternal(task, continuation, continueOnCapturedContext);
            }

            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation)
            {
                TaskAwaiter.OnCompletedInternal(task, continuation, continueOnCapturedContext);
            }

            public TResult GetResult()
            {
                TaskAwaiter.ValidateEnd(task);
                return task.Result;
            }
        }
    }

    public struct ConfiguredTaskAwaitable
    {
        private readonly ConfiguredTaskAwaiter configuredTaskAwaiter;

        internal ConfiguredTaskAwaitable(Task task, bool continueOnCapturedContext)
        {
            configuredTaskAwaiter = new ConfiguredTaskAwaiter(task, continueOnCapturedContext);
        }

        public ConfiguredTaskAwaiter GetAwaiter()
        {
            return configuredTaskAwaiter;
        }

        public struct ConfiguredTaskAwaiter : ICriticalNotifyCompletion
        {
            private readonly Task task;
            private readonly bool continueOnCapturedContext;

            public bool IsCompleted
            {
                get
                {
                    return task.IsCompleted;
                }
            }

            internal ConfiguredTaskAwaiter(Task task, bool continueOnCapturedContext)
            {
                this.task = task;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public void OnCompleted(Action continuation)
            {
                TaskAwaiter.OnCompletedInternal(task, continuation, continueOnCapturedContext);
            }

            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation)
            {
                TaskAwaiter.OnCompletedInternal(task, continuation, continueOnCapturedContext);
            }

            public void GetResult()
            {
                TaskAwaiter.ValidateEnd(task);
            }
        }
    }
}
