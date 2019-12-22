using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace WebViewRuntimeComponent
{
    [AllowForWeb]
    public sealed class WebViewEventHandler : IWebViewEventHandler
    {
        IWebViewEventHandler _myHandler;
        public WebViewEventHandler(IWebViewEventHandler handler)
        {
            _myHandler = handler;
        }
        public string GetSettings()
        {
            return _myHandler.GetSettings();
        }
        public void OnHtmlLoaded(string soundPosString)
        {
            _myHandler.OnHtmlLoaded(soundPosString);
        }
        public void OnParagraphClick(int par, String dbgInfo)
        {
            _myHandler.OnParagraphClick(par, dbgInfo);
        }
    }
}
