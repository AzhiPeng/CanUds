using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;


public static class AsyncCompatLibExtensions
{
    private static readonly ConditionalWeakTable<CancellationTokenSource, Timer> _cancelTimers = new ConditionalWeakTable<CancellationTokenSource, Timer>();
    private static readonly TimeSpan _timeoutInfinite = new TimeSpan(0, 0, 0, 0, -1);

    public static TaskAwaiter GetAwaiter(this Task task)
    {
        if (task == null)
            throw new ArgumentNullException("task");

        return new TaskAwaiter(task);
    }

    public static TaskAwaiter<TResult> GetAwaiter<TResult>(this Task<TResult> task)
    {
        if (task == null)
            throw new ArgumentNullException("task");

        return new TaskAwaiter<TResult>(task);
    }

    public static ConfiguredTaskAwaitable<TResult> ConfigureAwait<TResult>(this Task<TResult> task, bool continueOnCapturedContext)
    {
        if (task == null)
            throw new ArgumentNullException("task");

        return new ConfiguredTaskAwaitable<TResult>(task, continueOnCapturedContext);
    }

    public static ConfiguredTaskAwaitable ConfigureAwait(this Task task, bool continueOnCapturedContext)
    {
        if (task == null)
            throw new ArgumentNullException("task");

        return new ConfiguredTaskAwaitable(task, continueOnCapturedContext);
    }

    public static void CancelAfter(this CancellationTokenSource cancelSource, int millisecondsDelay)
    {
        if (millisecondsDelay < Timeout.Infinite)
            throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));

        cancelSource.CancelAfter(new TimeSpan(millisecondsDelay * TimeSpan.TicksPerMillisecond));
    }

    public static void CancelAfter(this CancellationTokenSource cancelSource, TimeSpan delay)
    {
        if (cancelSource == null)
            throw new ArgumentNullException(nameof(cancelSource));

        if (delay < _timeoutInfinite)
            throw new ArgumentOutOfRangeException(nameof(delay));

        if (cancelSource.IsCancellationRequested)
            return;

        Timer myTimer = null;

        while (!_cancelTimers.TryGetTimer(cancelSource, out myTimer))
        {
            myTimer = new Timer(OnCancelAfterTimer, cancelSource, Timeout.Infinite, Timeout.Infinite);

            if (_cancelTimers.TryAddTimer(cancelSource, myTimer))
                break;

            myTimer.Dispose();
        }

        try
        {
            myTimer.Change(delay, _timeoutInfinite);
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private static void OnCancelAfterTimer(object state)
    {
        var cancelSource = (CancellationTokenSource)state;

        if (!_cancelTimers.TryRemoveTimer(cancelSource, out var oldTimer))
            return;

        oldTimer.Dispose();

        try
        {
            cancelSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private static bool TryAddTimer(this ConditionalWeakTable<CancellationTokenSource, Timer> table, CancellationTokenSource cancelSource, Timer timer)
    {
        try
        {
            table.Add(cancelSource, timer);

            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static bool TryGetTimer(this ConditionalWeakTable<CancellationTokenSource, Timer> table, CancellationTokenSource cancelSource, out Timer timer)
    {
        return table.TryGetValue(cancelSource, out timer);
    }

    private static bool TryRemoveTimer(this ConditionalWeakTable<CancellationTokenSource, Timer> table, CancellationTokenSource cancelSource, out Timer timer)
    {
        if (!table.TryGetValue(cancelSource, out timer))
            return false;

        table.Remove(cancelSource);
        return true;
    }

    public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state)
    {
        return task.ContinueWith(new ContinueWithState(continuationAction, state).ContinueWith);
    }

    public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken)
    {
        return task.ContinueWith(new ContinueWithState(continuationAction, state).ContinueWith, cancellationToken);
    }

    public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
    {
        return task.ContinueWith(new ContinueWithState(continuationAction, state).ContinueWith, CancellationToken.None, continuationOptions, TaskScheduler.Current);
    }

    public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskScheduler scheduler)
    {
        return task.ContinueWith(new ContinueWithState(continuationAction, state).ContinueWith, CancellationToken.None, TaskContinuationOptions.None, scheduler);
    }

    public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
    {
        return task.ContinueWith(new ContinueWithState(continuationAction, state).ContinueWith, cancellationToken, continuationOptions, scheduler);
    }

    public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state)
    {
        return task.ContinueWith(new ContinueWithInState<TResult>(continuationAction, state).ContinueWith);
    }

    public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken)
    {
        return task.ContinueWith(new ContinueWithInState<TResult>(continuationAction, state).ContinueWith, cancellationToken);
    }

    public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
    {
        return task.ContinueWith(new ContinueWithInState<TResult>(continuationAction, state).ContinueWith, CancellationToken.None, continuationOptions, TaskScheduler.Current);
    }

    public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskScheduler scheduler)
    {
        return task.ContinueWith(new ContinueWithInState<TResult>(continuationAction, state).ContinueWith, CancellationToken.None, TaskContinuationOptions.None, scheduler);
    }

    public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
    {
        return task.ContinueWith(new ContinueWithInState<TResult>(continuationAction, state).ContinueWith, cancellationToken, continuationOptions, scheduler);
    }

    public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state)
    {
        return task.ContinueWith(new ContinueWithOutState<TResult>(continuationFunction, state).ContinueWith);
    }

    public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
    {
        return task.ContinueWith(new ContinueWithOutState<TResult>(continuationFunction, state).ContinueWith, cancellationToken);
    }

    public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
    {
        return task.ContinueWith(new ContinueWithOutState<TResult>(continuationFunction, state).ContinueWith, CancellationToken.None, continuationOptions, TaskScheduler.Current);
    }

    public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
    {
        return task.ContinueWith(new ContinueWithOutState<TResult>(continuationFunction, state).ContinueWith, CancellationToken.None, TaskContinuationOptions.None, scheduler);
    }

    public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
    {
        return task.ContinueWith(new ContinueWithOutState<TResult>(continuationFunction, state).ContinueWith, cancellationToken, continuationOptions, scheduler);
    }

    public static Task<TNewResult> ContinueWith<TResult, TNewResult>(this Task<TResult> task, Func<Task<TResult>, object, TNewResult> continuationFunction, object state)
    {
        return task.ContinueWith(new ContinueWithInOutState<TResult, TNewResult>(continuationFunction, state).ContinueWith);
    }

    public static Task<TNewResult> ContinueWith<TResult, TNewResult>(this Task<TResult> task, Func<Task<TResult>, object, TNewResult> continuationFunction, object state, CancellationToken cancellationToken)
    {
        return task.ContinueWith(new ContinueWithInOutState<TResult, TNewResult>(continuationFunction, state).ContinueWith, cancellationToken);
    }

    public static Task<TNewResult> ContinueWith<TResult, TNewResult>(this Task<TResult> task, Func<Task<TResult>, object, TNewResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
    {
        return task.ContinueWith(new ContinueWithInOutState<TResult, TNewResult>(continuationFunction, state).ContinueWith, CancellationToken.None, continuationOptions, TaskScheduler.Current);
    }

    public static Task<TNewResult> ContinueWith<TResult, TNewResult>(this Task<TResult> task, Func<Task<TResult>, object, TNewResult> continuationFunction, object state, TaskScheduler scheduler)
    {
        return task.ContinueWith(new ContinueWithInOutState<TResult, TNewResult>(continuationFunction, state).ContinueWith, CancellationToken.None, TaskContinuationOptions.None, scheduler);
    }

    public static Task<TNewResult> ContinueWith<TResult, TNewResult>(this Task<TResult> task, Func<Task<TResult>, object, TNewResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
    {
        return task.ContinueWith(new ContinueWithInOutState<TResult, TNewResult>(continuationFunction, state).ContinueWith, cancellationToken, continuationOptions, scheduler);
    }

    private sealed class ContinueWithState
    {
        private readonly Action<Task, object> _continuationAction;
        private readonly object _state;

        internal ContinueWithState(Action<Task, object> continuationAction, object state)
        {
            if (continuationAction == null)
                throw new ArgumentNullException(nameof(continuationAction));

            _continuationAction = continuationAction;
            _state = state;
        }

        internal void ContinueWith(Task task)
        {
            _continuationAction(task, _state);
        }
    }

    private sealed class ContinueWithInState<TIn>
    {
        private readonly Action<Task<TIn>, object> _continuationAction;
        private readonly object _state;

        internal ContinueWithInState(Action<Task<TIn>, object> continuationAction, object state)
        {
            if (continuationAction == null)
                throw new ArgumentNullException(nameof(continuationAction));

            _continuationAction = continuationAction;
            _state = state;
        }

        internal void ContinueWith(Task<TIn> task)
        {
            _continuationAction(task, _state);
        }
    }

    private sealed class ContinueWithOutState<TOut>
    {
        private readonly Func<Task, object, TOut> _continuationFunction;
        private readonly object _state;

        internal ContinueWithOutState(Func<Task, object, TOut> continuationFunction, object state)
        {
            if (continuationFunction == null)
                throw new ArgumentNullException(nameof(continuationFunction));

            _continuationFunction = continuationFunction;
            _state = state;
        }

        internal TOut ContinueWith(Task task)
        {
            return _continuationFunction(task, _state);
        }
    }

    private sealed class ContinueWithInOutState<TIn, TOut>
    {
        private readonly Func<Task<TIn>, object, TOut> _continuationFunction;
        private readonly object _state;

        internal ContinueWithInOutState(Func<Task<TIn>, object, TOut> continuationFunction, object state)
        {
            if (continuationFunction == null)
                throw new ArgumentNullException(nameof(continuationFunction));

            _continuationFunction = continuationFunction;
            _state = state;
        }

        internal TOut ContinueWith(Task<TIn> task)
        {
            return _continuationFunction(task, _state);
        }
    }
}
