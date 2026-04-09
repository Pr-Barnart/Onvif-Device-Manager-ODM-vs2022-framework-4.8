using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace utils
{
    public class Awaiter<T>
    {
        private readonly TaskCompletionSource<T> _tcs =
            new TaskCompletionSource<T>();

        public Task<T> Task => _tcs.Task;

        public Awaiter()
        {
        }

        public Awaiter(CancellationToken ct, object owner = null)
        {
            if (ct.IsCancellationRequested)
            {
                Release(owner);
            }
            else
            {
                ct.Register(() => Release(owner));
            }
        }

        private void Release(object owner)
        {
            if (owner is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Cancel();
        }

        public bool CompleteWithSuccess(T result)
        {
            return _tcs.TrySetResult(result);
        }

        public bool CompleteWithError(Exception error)
        {
            return _tcs.TrySetException(error);
        }

        public bool Cancel()
        {
            return _tcs.TrySetCanceled();
        }
        public TaskAwaiter<T> GetAwaiter()
        {
            return _tcs.Task.GetAwaiter();
        }
    }
}