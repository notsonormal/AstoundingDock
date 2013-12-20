using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.ComponentModel;

namespace AstoundingApplications.Win32Interface
{   
    /// <summary> Raw input pData command flags </summary>
    /// <remarks>http://msdn.microsoft.com/en-us/library/ms645596(v=vs.85).aspx</remarks>
    public enum RID : uint
    {
        /// <summary> Get header information. </summary>
        HEADER = 0x10000005,
        /// <summary> Get raw pData. </summary>
        INPUT = 0x10000003,
    }

    /// <summary> Raw input device info flags </summary>
    /// <remarks>http://msdn.microsoft.com/en-us/library/ms645597(VS.85).aspx</remarks>
    public enum RIDI : uint
    {
        /// <summary> pData contains the device name. </summary>
        DEVICENAME = 0x20000007,
        /// <summary> pData contains the device info structure. </summary>
        DEVICEINFO = 0x2000000b,
        /// <summary> pData contains the previous parsed pData. </summary>
        PREPARSEDDATA = 0x20000005
    }

    public enum RIM : int
    {
        /// <summary> The device is a mouse </summary>
        TYPEMOUSE = 0,
        /// <summary> The device is a keyboard </summary>
        TYPEKEYBOARD = 1,
        /// <summary> The device is not a mouse of a keyboard </summary>
        TYPEHID = 2
    }

    /// <summary> Raw input device flags </summary>
    /// <remarks>http://msdn.microsoft.com/en-us/library/ms645565(VS.85).aspx</remarks>
    [Flags()]
    public enum RIDEV
    {
        APPKEYS = 0x00000400,
        CAPTUREMOUSE = 0x00000200,
        DEVNOTIFY = 0x00002000,
        EXCLUDE = 0x00000010,
        EXINPUTSINK = 0x00001000,
        INPUTSINK = 0x00000100,
        NOHOTKEYS = 0x00000200,
        NOLEGACY = 0x00000030,
        PAGEONLY = 0x00000020,
        REMOVE = 0x00000001
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible", 
            Justification="This is how the structure is defined in Win32")]
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTDEVICELIST
    {
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible",
            Justification = "This is how the structure is defined in Win32")]
        public IntPtr hDevice;
        [MarshalAs(UnmanagedType.U4)]
        public int dwType;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RAWINPUT
    {
        [FieldOffset(0)]
        public RAWINPUTHEADER header;
        [FieldOffset(16)]
        public RAWMOUSE mouse;
        [FieldOffset(16)]
        public RAWKEYBOARD keyboard;
        [FieldOffset(16)]
        public RAWHID hid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTHEADER
    {
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible",
            Justification = "This is how the structure is defined in Win32")]
        [MarshalAs(UnmanagedType.U4)]
        public int dwType;
        [MarshalAs(UnmanagedType.U4)]
        public int dwSize;
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible",
            Justification = "This is how the structure is defined in Win32")]
        public IntPtr hDevice;
        [MarshalAs(UnmanagedType.U4)]
        public int wParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RAWHID
    {
        [MarshalAs(UnmanagedType.U4)]
        public int dwSizHid;
        [MarshalAs(UnmanagedType.U4)]
        public int dwCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BUTTONSSTR
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort usButtonFlags;
        [MarshalAs(UnmanagedType.U2)]
        public ushort usButtonData;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RAWMOUSE
    {
        [MarshalAs(UnmanagedType.U2)]
        [FieldOffset(0)]
        public ushort usFlags;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(4)]
        public uint ulButtons;
        [FieldOffset(4)]
        public BUTTONSSTR buttonsStr;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(8)]
        public uint ulRawButtons;
        [FieldOffset(12)]
        public int lLastX;
        [FieldOffset(16)]
        public int lLastY;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(20)]
        public uint ulExtraInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RAWKEYBOARD
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort MakeCode;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Flags;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Reserved;
        [MarshalAs(UnmanagedType.U2)]
        public ushort VKey;
        [MarshalAs(UnmanagedType.U4)]
        public uint Message;
        [MarshalAs(UnmanagedType.U4)]
        public uint ExtraInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTDEVICE
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort usUsagePage;
        [MarshalAs(UnmanagedType.U2)]
        public ushort usUsage;
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible",
            Justification = "This is how the structure is defined in Win32")]
        [MarshalAs(UnmanagedType.U4)]
        public int dwFlags;
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible",
            Justification = "This is how the structure is defined in Win32")]
        public IntPtr hwndTarget;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RID_DEVICE_INFO
    {
        [FieldOffset(0)]
        public int Size;
        [FieldOffset(4)]
        public int Type;

        [FieldOffset(8)]
        public RID_DEVICE_INFO_MOUSE MouseInfo;
        [FieldOffset(8)]
        public RID_DEVICE_INFO_KEYBOARD KeyboardInfo;
        [FieldOffset(8)]
        public RID_DEVICE_INFO_HID HIDInfo;
    }

    public struct RID_DEVICE_INFO_MOUSE
    {
        public uint ID;
        public uint NumberOfButtons;
        public uint SampleRate;
    }

    public struct RID_DEVICE_INFO_KEYBOARD
    {
        public uint Type;
        public uint SubType;
        public uint KeyboardMode;
        public uint NumberOfFunctionKeys;
        public uint NumberOfIndicators;
        public uint NumberOfKeysTotal;
    }

    public struct RID_DEVICE_INFO_HID
    {
        public uint VendorID;
        public uint ProductID;
        public uint VersionNumber;
        public ushort UsagePage;
        public ushort Usage;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static class Win32RawInput
    {
        public static RID_DEVICE_INFO GetDeviceInfo(IntPtr deviceHandle)            
        {
            uint size = 0;
            GetRawInputDeviceInfo(deviceHandle, RIDI.DEVICEINFO, IntPtr.Zero, ref size);

            if (size > 0)
            {
                IntPtr data = Marshal.AllocHGlobal((int)size);
                try
                {
                    GetRawInputDeviceInfo(deviceHandle, RIDI.DEVICEINFO, data, ref size);
                    return (RID_DEVICE_INFO) Marshal.PtrToStructure(data, typeof(RID_DEVICE_INFO));
                }
                finally
                {
                    Marshal.FreeHGlobal(data);
                }
            }

            return new RID_DEVICE_INFO();
        }

        public static string GetDeviceName(IntPtr deviceHandle)
        {
            uint size = 0;
            GetRawInputDeviceInfo(deviceHandle, RIDI.DEVICENAME, IntPtr.Zero, ref size);

            if (size > 0)
            {
                IntPtr data = Marshal.AllocHGlobal((int)size);
                try
                {
                    GetRawInputDeviceInfo(deviceHandle, RIDI.DEVICENAME, data, ref size);
                    return (string) Marshal.PtrToStringAnsi(data);
                }
                finally
                {
                    Marshal.FreeHGlobal(data);
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves the raw input from the device.
        /// </summary>
        public static RAWINPUT GetInputData(IntPtr rawInputHandle)
        {
            uint size = 0;
            GetRawInputData(rawInputHandle, RID.INPUT, IntPtr.Zero, ref size, Marshal.SizeOf(typeof(RAWINPUTHEADER)));

            if (size > 0)
            {
                IntPtr data = Marshal.AllocHGlobal((int)size);
                try
                {
                    GetRawInputData(rawInputHandle, RID.INPUT, data, ref size, Marshal.SizeOf(typeof(RAWINPUTHEADER)));
                    return (RAWINPUT) Marshal.PtrToStructure(data, typeof(RAWINPUT));
                }
                finally
                {
                    Marshal.FreeHGlobal(data);
                }
            }

            return new RAWINPUT();
        }

        public static RAWINPUTDEVICELIST[] GetInputDeviceList()
        {
            uint deviceCount = 0;
            int size = Marshal.SizeOf(typeof(RAWINPUTDEVICELIST));

            if (GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, size) != 0)
                throw new Win32Exception("Failed to get raw input devices"); 

            RAWINPUTDEVICELIST[] rawInputDevices = new RAWINPUTDEVICELIST[deviceCount + 1]; // TODO: This is right? deviceCount + 1?
            if (deviceCount > 0)
            {
                IntPtr data = Marshal.AllocHGlobal((int)(size * deviceCount));                
                try
                {
                    GetRawInputDeviceList(data, ref deviceCount, size);
                    for (int i = 0; i < deviceCount; i++)
                    {
                        RAWINPUTDEVICELIST rid = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(
                            new IntPtr((data.ToInt32() + (size * i))), typeof(RAWINPUTDEVICELIST));
                        rawInputDevices[i] = rid;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(data);
                }
            }
            return rawInputDevices;
        }

        public static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, int iNumDevices, int cbSize)
        {
            return NativeMethods.RegisterRawInputDevices(pRawInputDevice, (uint)iNumDevices, (uint)cbSize);
        }

        public static uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, int cbSize)
        {
            return NativeMethods.GetRawInputDeviceList(pRawInputDeviceList, ref uiNumDevices, (uint)cbSize);
        }

        public static uint GetRawInputDeviceInfo(IntPtr hDevice, RIDI command, IntPtr data, ref uint pcbSize)
        {
            return NativeMethods.GetRawInputDeviceInfo(hDevice, (uint)command, data, ref pcbSize);
        }

        public static uint GetRawInputData(IntPtr rawInput, RID command, IntPtr pData, ref uint pcbSize, int cbSizeHeader)
        {
            return NativeMethods.GetRawInputData(rawInput, (uint)command, pData, ref pcbSize, (uint)cbSizeHeader);
        }

        /// <summary>
        /// http://www.microsoft.com/whdc/archive/HID_HWID.mspx
        /// </summary>
        public class HID_HWIND
        {
            #region Fields
            static readonly HID_HWIND Pointer = new HID_HWIND(0x01, 0x01, "HID_DEVICE_SYSTEM_MOUSE");
            static readonly HID_HWIND Mouse = new HID_HWIND(0x01, 0x02, "HID_DEVICE_SYSTEM_MOUSE");
            static readonly HID_HWIND Joystick = new HID_HWIND(0x01, 0x04, "HID_DEVICE_SYSTEM_GAME");
            static readonly HID_HWIND Gamepad = new HID_HWIND(0x01, 0x05, "HID_DEVICE_SYSTEM_GAME");
            static readonly HID_HWIND Keyboard = new HID_HWIND(0x01, 0x06, "HID_DEVICE_SYSTEM_KEYBOARD");
            static readonly HID_HWIND Keypad = new HID_HWIND(0x01, 0x07, "HID_DEVICE_SYSTEM_KEYBOARD");
            static readonly HID_HWIND SystemControl = new HID_HWIND(0x01, 0x80, "HID_DEVICE_SYSTEM_CONTROL");
            static readonly HID_HWIND ConsumerAudioControl = new HID_HWIND(0x0C, 0x01, "HID_DEVICE_SYSTEM_CONSUMER");
            #endregion

            public ushort UsagePage { get; private set; }
            public ushort UsageId { get; private set; }
            public string AdditionalHardwareId { get; private set; }

            public HID_HWIND(ushort usagePage, ushort usageId, string additionalHardwareId)
            {
                UsagePage = usagePage;
                UsageId = usageId;
                AdditionalHardwareId = additionalHardwareId;
            }            

            public static HID_HWIND HidPointer { get { return Pointer; } }
            public static HID_HWIND HidMouse { get { return Mouse; } }
            public static HID_HWIND HidJoystick { get { return Joystick; } }
            public static HID_HWIND HidGamepad { get { return Gamepad; } }
            public static HID_HWIND HidKeyboard { get { return Keyboard; } }
            public static HID_HWIND HidKeypad { get { return Keypad; } }
            public static HID_HWIND HidSystemControl { get { return SystemControl; } }
            public static HID_HWIND HidConsumerAudioControl { get { return ConsumerAudioControl; } }
        }       
                
        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public extern static uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

            [DllImport("user32.dll", SetLastError = true)]
            public extern static uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

            [DllImport("user32.dll", SetLastError = true)]
            public extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

            [DllImport("user32.dll", SetLastError = true)]
            public extern static uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);
        }
    }
}
