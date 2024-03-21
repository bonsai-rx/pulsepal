using System;
using System.Threading;
using System.Reactive.Disposables;

namespace Bonsai.PulsePal
{
    sealed class PulsePalDisposable : ICancelable, IDisposable
    {
        IDisposable resource;

        public PulsePalDisposable(PulsePalDevice pulsePal, IDisposable disposable)
        {
            PulsePal = pulsePal ?? throw new ArgumentNullException(nameof(pulsePal));
            resource = disposable ?? throw new ArgumentNullException(nameof(disposable));
        }

        public PulsePalDevice PulsePal { get; private set; }

        public bool IsDisposed => resource == null;

        public void Dispose()
        {
            var disposable = Interlocked.Exchange(ref resource, null);
            disposable?.Dispose();
        }
    }
}
