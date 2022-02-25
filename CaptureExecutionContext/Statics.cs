using System.Runtime.InteropServices;
using System.Threading;

internal class Statics
{
    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public unsafe static extern void* CreateThread(
        uint* lpThreadAttributes,
        uint dwStackSize,
        ThreadStart lpStartAddress,
        uint* lpParameter,
        uint dwCreationFlags,
        out uint lpThreadId);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public unsafe static extern uint WaitForSingleObject(
        void* hHandle,
        uint dwMilliseconds
        );

    public const uint INFINITE = unchecked((uint)-1);
}
