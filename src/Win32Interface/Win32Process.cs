using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;

namespace AstoundingApplications.Win32Interface
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class Win32Process
    {        
        public static Process GetParentProcess(int processId)
        {
            return GetParentProcess(Process.GetProcessById(processId));
        }

        public static Process GetParentProcess(Process process)
        {
            return GetParentProcess(process.Handle);   
        }

        public static Process GetParentProcess(IntPtr handle)
        {
            NativeMethods.PROCESS_BASIC_INFORMATION pbi = new NativeMethods.PROCESS_BASIC_INFORMATION();

            int pSize;
            int status = NativeMethods.NtQueryInformationProcess(
                handle, NativeMethods.PROCESSINFOCLASS.ProcessBasicInformation, 
                ref pbi, Marshal.SizeOf(pbi), out pSize);

            if (status != 0)
                throw new Win32Exception(status);

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException)
            {
                return null; // Not found
            }
        }

        private static class NativeMethods
        {
            public enum PROCESSINFOCLASS : int
            {
                ProcessBasicInformation = 0,
                ProcessQuotaLimits,
                ProcessIoCounters,
                ProcessVmCounters,
                ProcessTimes,
                ProcessBasePriority,
                ProcessRaisePriority,
                ProcessDebugPort,
                ProcessExceptionPort,
                ProcessAccessToken,
                ProcessLdtInformation,
                ProcessLdtSize,
                ProcessDefaultHardErrorMode,
                ProcessIoPortHandlers, // Note: this is kernel mode only
                ProcessPooledUsageAndLimits,
                ProcessWorkingSetWatch,
                ProcessUserModeIOPL,
                ProcessEnableAlignmentFaultFixup,
                ProcessPriorityClass,
                ProcessWx86Information,
                ProcessHandleCount,
                ProcessAffinityMask,
                ProcessPriorityBoost,
                MaxProcessInfoClass,
                ProcessWow64Information = 26
            };

            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESS_BASIC_INFORMATION
            {
                internal IntPtr Reserved1;
                internal IntPtr PebBaseAddress;
                internal IntPtr Reserved2_0;
                internal IntPtr Reserved2_1;
                internal IntPtr UniqueProcessId;
                internal IntPtr InheritedFromUniqueProcessId;
            }

            [DllImport("NTDLL.DLL", SetLastError = true)]
            public static extern int NtQueryInformationProcess(IntPtr hProcess, PROCESSINFOCLASS pic, 
                ref PROCESS_BASIC_INFORMATION pbi, int cb, out int pSize);
        }
    }
}
