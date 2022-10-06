using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{
    public class PausableTimer : IDisposable
    {
        private System.Timers.Timer _timer;
        private Stopwatch _stopWatch;
        private bool _paused;
        private int _originalInterval;
        private double _remainingTimeBeforePause;
        public event System.Timers.ElapsedEventHandler Elapsed;

        public PausableTimer()
        {
            _stopWatch = new Stopwatch();
            _timer = new System.Timers.Timer();
            _timer.AutoReset = false;
            _timer.Elapsed += (sender, arguments) =>
            {
                Elapsed?.Invoke(sender, arguments);

                if (_timer != null && _timer.AutoReset)
                {
                    _stopWatch.Restart();
                }
            };

        }

        public PausableTimer(int milliSeconds) : this()
        {
            Interval = milliSeconds;
            _originalInterval = milliSeconds;
        }

        public bool AutoReset
        {
            get
            {
                return _timer.AutoReset;
            }
            set
            {
                _timer.AutoReset = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _timer?.Enabled ?? false;
            }
            set
            {
                _timer.Enabled = value;
            }
        }

        public double Interval
        {
            get
            {
                return _timer.Interval;
            }
            set
            {
                _timer.Interval = value;
            }
        }

        public bool Paused
        {
            get
            {
                return _paused;
            }
        }

        public void Start()
        {
            _timer.Start();
            _stopWatch.Restart();
        }

        public TimeSpan TimeRemaining()
        {
            if (_originalInterval > 0 && _stopWatch?.ElapsedMilliseconds != null)
            {
                return TimeSpan.FromMilliseconds(_originalInterval - _stopWatch.ElapsedMilliseconds);
            }
            return new TimeSpan();
        }

        public void Stop()
        {
            _timer.Stop();
            _stopWatch.Stop();
        }

        public void Pause()
        {
            if (!_paused && (_timer?.Enabled ?? true))
            {
                _paused = true;
                _stopWatch.Stop();
                _timer?.Stop();
                _remainingTimeBeforePause = Math.Max(0, Interval - _stopWatch.ElapsedMilliseconds);
            }
        }

        public void Resume()
        {
            if (_paused)
            {
                _paused = false;
                if (_remainingTimeBeforePause > 0)
                {
                    _timer.Interval = _remainingTimeBeforePause;
                    _timer.Start();
                    _stopWatch.Start();
                }
            }
        }

        bool _disposed = false;

        public void Dispose()
        {
            if (_timer != null && !_disposed)
            {
                // Not thread safe...
                _disposed = true;
                _timer.Dispose();
            }
        }

        ~PausableTimer()
        {
            Dispose();
        }
    }
}
