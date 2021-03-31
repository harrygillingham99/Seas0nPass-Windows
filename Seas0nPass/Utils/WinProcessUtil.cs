using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

namespace Seas0nPass.Utils
{
    public static class WinProcessUtil
    {
        private static List<Process> _startedProcesses = new List<Process>();

        public static Process StartNewProcess()
        {
            var p = new Process();
            _startedProcesses.Add(p);
            return p;
        }

        public static Process StartNewProcess(ProcessStartInfo processStartInfo)
        {
            var p = Process.Start(processStartInfo);
            _startedProcesses.Add(p);
            return p;
        }

        public static void KillAllProcesses()
        {
            foreach (var p in _startedProcesses.Where(x => !x.HasExited))
            {
                try
                {
                    p.Kill();
                    p.WaitForExit(1000); // wait for exit no longer than 1 second
                }
                catch (Win32Exception)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}
