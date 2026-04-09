using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace utils
{
    public class Awaiter
    {
        private readonly TaskCompletionSource<bool> _tcs =
    new TaskCompletionSource<bool>();

        public Task Task => _tcs.Task;

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

        public bool CompleteWithSuccess()
        {
            return _tcs.TrySetResult(true);
        }

        public bool CompleteWithError(Exception error)
        {
            return _tcs.TrySetException(error);
        }

        public bool Cancel()
        {
            return _tcs.TrySetCanceled();
        }
        public TaskAwaiter <bool> GetAwaiter()
        {
            return _tcs.Task.GetAwaiter();
        }
    }
}