using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebViewRuntimeComponent
{
    public interface IWebViewEventHandler
    {
        void OnHtmlLoaded(String soundPosString);
        String GetSettings();
        void OnParagraphClick(int par, String dbgInfo);
    }
}
