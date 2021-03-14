using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;

namespace Helpers
{
    /*
    class ThreadPoolTimerTick
    {

        public delegate void OnTick();
        private OnTick _onTick;

        public void Triger(OnTick onTick)
        {
            _onTick = onTick;
            ThreadPoolTimer.CreateTimer(TimerElapsedHandler, TimeSpan.FromMilliseconds(1));
        }
        private async void TimerElapsedHandler(ThreadPoolTimer timer)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (_onTick != null)
                {
                    OnTick tick = _onTick;
                    _onTick = null;
                    tick();
                }
            }
            );
        }
    }
    */
}
