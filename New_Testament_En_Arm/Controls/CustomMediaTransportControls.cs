using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NewTestamentEnArm.Controls
{
    public class CustomMediaTransportControls : MediaTransportControls
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Grid controlPanelGrid = GetTemplateChild("ControlPanelGrid") as Grid;
            if (controlPanelGrid != null)
            {
                controlPanelGrid.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }
        }
    }
}
