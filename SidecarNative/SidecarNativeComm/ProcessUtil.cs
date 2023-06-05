using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SidecarNativeApp
{
    public static class ProcessUtil
    {
        public static int GetParentProcessIdOfPid(int pid)
        {
            try
            {
                var myId = Process.GetCurrentProcess().Id;
                var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId);
#pragma warning disable CA1416 // Validate platform compatibility
                var search = new ManagementObjectSearcher("root\\CIMV2", query);
                var results = search.Get().GetEnumerator();
                results.MoveNext();
                var queryObj = results.Current;
                var parentId = (int)queryObj["ParentProcessId"];
#pragma warning restore CA1416 // Validate platform compatibility
                return parentId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // process extension method that returns parent process
        public static Process? GetParent(this Process process)
        {
            try
            {
                return Process.GetProcessById(GetParentProcessIdOfPid(process.Id));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
