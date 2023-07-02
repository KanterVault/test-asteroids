using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsteroidGame
{
    public struct DeltaTimeEventArgs
    {
        public double DeltaTime;
    }

    public class Simulation : IDisposable
    {
        public EventHandler<DeltaTimeEventArgs> OnUpdate;
        public EventHandler OnStarted;
        public EventHandler OnStoped;

        private bool _live = false;

        public void Init(uint delayMs) => new Action(async () => await Task.Run(() =>
        {
            _live = true;
            var delay = (int)(delayMs > 1000 ? 1000 : delayMs);
            var time = DateTime.UtcNow;
            OnStarted?.Invoke(this, null);
            while (_live)
            {
                OnUpdate?.Invoke(this, new DeltaTimeEventArgs() { DeltaTime = (DateTime.UtcNow - time).TotalMilliseconds });
                time = DateTime.UtcNow;
                Thread.Sleep(delay);
            }
        })).Invoke();

        public void Dispose()
        {
            OnStoped?.Invoke(this, null);
            _live = false;
        }
    }
}
