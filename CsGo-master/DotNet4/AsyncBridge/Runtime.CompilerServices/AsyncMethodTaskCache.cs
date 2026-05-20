using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    internal class AsyncMethodTaskCache<TResult>
    {
        internal static readonly AsyncMethodTaskCache<TResult> Singleton = CreateCache();

        static AsyncMethodTaskCache()
        {
        }

        internal static TaskCompletionSource<TResult> CreateCompleted(TResult result)
        {
            var completionSource = new TaskCompletionSource<TResult>();
            completionSource.TrySetResult(result);
            return completionSource;
        }

        private static AsyncMethodTaskCache<TResult> CreateCache()
        {
            var type = typeof(TResult);
            if (type == typeof(bool))
                return (AsyncMethodTaskCache<TResult>)(object)new AsyncMethodBooleanTaskCache();
            if (type == typeof(int))
                return (AsyncMethodTaskCache<TResult>)(object)new AsyncMethodInt32TaskCache();

            return null;
        }

        internal virtual TaskCompletionSource<TResult> FromResult(TResult result)
        {
            return CreateCompleted(result);
        }

        private sealed class AsyncMethodBooleanTaskCache : AsyncMethodTaskCache<bool>
        {
            private static readonly TaskCompletionSource<bool> True = CreateCompleted(true);
            private static readonly TaskCompletionSource<bool> False = CreateCompleted(false);

            internal override TaskCompletionSource<bool> FromResult(bool result)
            {
                return result ? True : False;
            }
        }

        private sealed class AsyncMethodInt32TaskCache : AsyncMethodTaskCache<int>
        {
            private static readonly TaskCompletionSource<int>[] Int32Tasks = CreateInt32Tasks();
            private const int InclusiveInt32Min = -1;
            private const int ExclusiveInt32Max = 9;

            static AsyncMethodInt32TaskCache()
            {
            }

            private static TaskCompletionSource<int>[] CreateInt32Tasks()
            {
                var completionSourceArray = new TaskCompletionSource<int>[10];
                for (var index = 0; index < completionSourceArray.Length; ++index)
                    completionSourceArray[index] = CreateCompleted(index - 1);
                return completionSourceArray;
            }

            internal override TaskCompletionSource<int> FromResult(int result)
            {
                if (result < InclusiveInt32Min || result >= ExclusiveInt32Max)
                    return CreateCompleted(result);

                return Int32Tasks[result - -1];
            }
        }
    }
}
