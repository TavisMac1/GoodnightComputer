using System;
using System.Runtime.InteropServices;

namespace GoodnightComputer.Utils;

[Flags]
public enum ThreadAccess : int
{
    Terminate = (0x0001),
    SuspendResume = (0x0002),
    GetContext = (0x0008),
    SetContext = (0x0010),
    SetInformation = (0x0020),
    QueryInformation = (0x0040),
    SetThreadToken = (0x0080),
    Impersonate = (0x0100),
    DirectImpersonation = (0x0200)
}

public static class NativeMethods
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

    [DllImport("kernel32.dll")]
    public static extern uint SuspendThread(IntPtr hThread);

    [DllImport("kernel32.dll")]
    public static extern int ResumeThread(IntPtr hThread);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);
}