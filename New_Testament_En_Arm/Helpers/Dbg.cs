using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTestamentEnArm.Helpers
{
    public static class Dbg
    {
#if DEBUG
        static int _logNN;
        static StringBuilder _dbgStringBuilder = new StringBuilder();
#endif
        public static void d(Object trace = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
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
#endif
        } 
    }
}
