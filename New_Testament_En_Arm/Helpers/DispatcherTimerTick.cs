using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using System.Diagnostics;

namespace Helpers
{
    class DispatcherTimerData
    {
        public DispatcherTimerTick.OnTick onTick;
        public int tickCount;
        public String description;
        public DispatcherTimerData(DispatcherTimerTick.OnTick onTick, int tickCount, String description)
        {
            this.tickCount = tickCount;
            this.description = description;
            this.onTick = onTick;
        }
    }

    class DispatcherTimerTick
    {
        //private System.Timers.Timer _timer = new System.Timers.Timer();
        private DispatcherTimer _timer = new DispatcherTimer();
        private System.Collections.Generic.List<DispatcherTimerData> dataList;
        public delegate void OnTick();
        public DispatcherTimerTick()
        {
            //_timer.Interval = 1;
            //_timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            dataList = new List<DispatcherTimerData>();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timer.Tick += dispatcherTimer_Tick;
        }

        public void Triger(OnTick onTick, int tickCount = 1, String description = "")
        {
            dataList.Add(new DispatcherTimerData(onTick, tickCount, description));
            if (dataList.Count == 1)
                _timer.Start();
        }

        //private async void Timer_Elapsed(object sender, EventArgs e)
        private void dispatcherTimer_Tick(object sender, object e)
        {
            DispatcherTimerData data = dataList[0];
            if (--(data.tickCount) > 0)
                return;

            NewTestamentEnArm.Helpers.Dbg.d(" " + data.description);
            dataList.RemoveAt(0);
            if (dataList.Count == 0)
                _timer.Stop();
            data.onTick();
            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //);
        }
    }
}
