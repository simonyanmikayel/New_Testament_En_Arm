using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThisApp.Helpers
{
    public static class Dbg
    {
        static int _logNN;
        static StringBuilder _dbgStringBuilder = new StringBuilder();
        public static void d(Object trace = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            _dbgStringBuilder.Clear();
            _dbgStringBuilder.Append(++_logNN);
            _dbgStringBuilder.Append(": ");
            _dbgStringBuilder.Append("-> ");
            _dbgStringBuilder.Append(memberName);
            _dbgStringBuilder.Append('[');
            _dbgStringBuilder.Append(sourceLineNumber);
            _dbgStringBuilder.Append(":] ");
            if (trace != null)
                _dbgStringBuilder.Append(trace.ToString());
            Debug.WriteLine(_dbgStringBuilder.ToString());
        } 
    }
}
