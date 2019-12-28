using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace Helpers
{
    class DispatcherTimerTick
    {
        //private System.Timers.Timer _timer = new System.Timers.Timer();
        private DispatcherTimer _timer = new DispatcherTimer();

        public delegate void OnTick();
        private OnTick _onTick;
        public DispatcherTimerTick()
        {
            //_timer.Interval = 1;
            //_timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            _timer.Interval = new TimeSpan(0, 0, 0, 1);
            _timer.Tick += dispatcherTimer_Tick;
        }

        public void Triger(OnTick onTick)
        {
            _onTick = onTick;
            _timer.Start();
        }

        //private async void Timer_Elapsed(object sender, EventArgs e)
        private void dispatcherTimer_Tick(object sender, object e)
        {
            _timer.Stop();
            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (_onTick != null)
                {
                    OnTick tick = _onTick;
                    _onTick = null;
                    tick();
                }
            }
            //);
        }
    }
}
