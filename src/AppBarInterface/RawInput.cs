using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AstoundingApplications.Win32Interface;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.Win32;
using System.Diagnostics;

namespace AstoundingApplications.AppBarInterface
{
    delegate void RawInputEventHandler(object sender, RawInputEventArgs e);
    enum RawInputAction { MouseMove }    

    class RawInputEventArgs : EventArgs
    {
        public RawInputAction Action { get; set; }

        public RawInputEventArgs(RawInputAction action)
        {
            Action = action;
        }
    }

    class RawInput : IDisposable
    {
        public event RawInputEventHandler RawInputEvent = delegate { };

        public class DeviceInfo
        {
            public string DeviceName { get; set; }
            public RIM DeviceType { get; set; }
            public IntPtr DeviceHandle { get; set; }
            public string Name { get; set; }
            public string Source { get; set; }
            public ushort Key { get; set; }
            public string VKey { get; set; }
            public Point Position { get; set; }
        }
        
        IntPtr handle;
        List<DeviceInfo> mice;
        List<DeviceInfo> keyboards;
        List<DeviceInfo> otherDevices;

        public RawInput(IntPtr handle)
        {
            this.handle = handle;
            mice = new List<DeviceInfo>();
            keyboards = new List<DeviceInfo>();
            otherDevices = new List<DeviceInfo>();

            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];
            rid[0].usUsagePage = Win32RawInput.HID_HWIND.HidMouse.UsagePage;
            rid[0].usUsage = Win32RawInput.HID_HWIND.HidMouse.UsageId;
            rid[0].dwFlags = (int)RIDEV.INPUTSINK;
            rid[0].hwndTarget = handle;

            if (!Win32RawInput.RegisterRawInputDevices(rid, rid.Length, Marshal.SizeOf(rid[0])))            
                throw new Win32Exception("Failed to register raw input device(s).");            

            int loadedDevices = LoadDevices();
            if (mice.Count <= 0)
                throw new Win32Exception("Unable to detect an attached mouse");
        }

        public int LoadDevices()
        {
            uint deviceCount = 0;
            int dwSize = (Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));

            uint result = Win32RawInput.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, dwSize);
            if (result != 0)
            {
                throw new Win32Exception("Failed to get raw input devices");
            }

            IntPtr pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
            Win32RawInput.GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, dwSize);

            for (int i = 0; i < deviceCount; i++)
            {
                //DeviceInfo dInfo;
                //string deviceName;
                //uint pcbSize = 0;

                RAWINPUTDEVICELIST rid = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(
                    new IntPtr((pRawInputDeviceList.ToInt32() + (dwSize * i))), typeof(RAWINPUTDEVICELIST));

                string deviceName = Win32RawInput.GetDeviceName(rid.hDevice);

                // Drop the "root" keyboard and mouse devices used for Terminal Services and the Remote Desktop.
                if (!String.IsNullOrEmpty(deviceName) && !deviceName.ToUpper().Contains("ROOT"))
                {
                    switch ((RIM)rid.dwType)
                    {
                        case RIM.TYPEMOUSE:
                        case RIM.TYPEKEYBOARD:
                        case RIM.TYPEHID:
                            DeviceInfo dInfo = new DeviceInfo()
                            {
                                DeviceName = deviceName,
                                DeviceHandle = rid.hDevice,
                                DeviceType = (RIM)rid.dwType,
                            };

                            var additionalInfo = ReadRegisty(deviceName);

                            if (additionalInfo.ContainsKey("DeviceDesc"))                            
                                dInfo.Name = additionalInfo["DeviceDesc"];

                            if (additionalInfo.ContainsKey("Class"))
                            {
                                switch (additionalInfo["Class"].ToUpper())
                                {
                                    case "KEYBOARD":
                                        if (!keyboards.Contains(dInfo))
                                        {
                                            keyboards.Add(dInfo);
                                            deviceCount++;
                                        }
                                        break;
                                    case "MOUSE":
                                        if (!mice.Contains(dInfo))
                                        {
                                            mice.Add(dInfo);
                                            deviceCount++;
                                        }
                                        break;
                                    default:
                                        if (!otherDevices.Contains(dInfo))
                                        {
                                            otherDevices.Add(dInfo);
                                            deviceCount++;
                                        }
                                        break;
                                }
                            }
                            deviceCount++;
                            break;
                    }
                }

                /*
                if (!String.IsNullOrEmpty(deviceName) && !deviceName.ToUpper().Contains("ROOT"))
                {                    
                    switch ((RIM)rid.dwType)
                    {
                        case RIM.TYPEMOUSE:
                        case RIM.TYPEKEYBOARD:
                        case RIM.TYPEHID:
                            DeviceInfo dInfo = new DeviceInfo();

                            dInfo.DeviceName = deviceName;
                            dInfo.DeviceHandle = rid.hDevice;
                            dInfo.DeviceType = GetDeviceType(rid.dwType);

                            var additionalInfo = ReadRegisty(deviceName);
                            dInfo.Name = additionalInfo["DeviceDesc"];

                            switch (additionalInfo["Class"].ToUpper())
                            {
                                case "KEYBOARD":
                                    if (!keyboards.Contains(dInfo))
                                    {
                                        keyboards.Add(dInfo);
                                        deviceCount++;
                                    }
                                    break;
                                case "MOUSE":
                                    if (!mice.Contains(dInfo))
                                    {
                                        mice.Add(dInfo);
                                        deviceCount++;
                                    }
                                    break;
                                default:
                                    if (!otherDevices.Contains(dInfo))
                                    {
                                        otherDevices.Add(dInfo);
                                        deviceCount++;
                                    }
                                    break;
                            }
                            deviceCount++;
                            break;
                    }                    
                }
                 */

                /*
                  
                Win32RawInput.GetRawInputDeviceInfo(rid.hDevice, RIDI.DEVICENAME, IntPtr.Zero, ref pcbSize); 
                if (pcbSize > 0)
                {

                    Win32RawInput.GetRawInputDeviceInfo(rid.hDevice, RIDI.DEVICENAME, pData, ref pcbSize);
                    deviceName = (string)Marshal.PtrToStringAnsi(pData);

                    // Drop the "root" keyboard and mouse devices used for Terminal 
                    // Services and the Remote Desktop
                    if (deviceName.ToUpper().Contains("ROOT"))
                    {
                        continue;
                    }

                    // If the device is identified in the list as a keyboard or 
                    // HID device, create a DeviceInfo object to store information 
                    // about it
                    if (rid.dwType == (int)RIM.TYPEMOUSE || rid.dwType == (int)RIM.TYPEKEYBOARD || rid.dwType == (int)RIM.TYPEHID)
                    {
                        dInfo = new DeviceInfo();

                        dInfo.DeviceName = (string)Marshal.PtrToStringAnsi(pData);
                        dInfo.DeviceHandle = rid.hDevice;
                        dInfo.DeviceType = GetDeviceType(rid.dwType);

                        var additionalInfo = ReadRegisty(deviceName);
                        dInfo.Name = additionalInfo["DeviceDesc"];

                        switch (additionalInfo["Class"].ToUpper())
                        {
                            case "KEYBOARD":
                                if (!keyboards.Contains(dInfo))
                                {
                                    keyboards.Add(dInfo);
                                    deviceCount++;
                                }
                                break;
                            case "MOUSE":
                                if (!mice.Contains(dInfo))
                                {
                                    mice.Add(dInfo);
                                    deviceCount++;
                                }
                                break;
                            default:
                                if (!otherDevices.Contains(dInfo))
                                {
                                    otherDevices.Add(dInfo);
                                    deviceCount++;
                                }
                                break;
                        }
                        deviceCount++;
                    }                     
                    Marshal.FreeHGlobal(pData);
                }
                */
            }
            Marshal.FreeHGlobal(pRawInputDeviceList);
            return (int)deviceCount;
        }

        Dictionary<string, string> ReadRegisty(string deviceName)
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            // Example Device Identification string
            // @"\??\ACPI#PNP0303#3&13c0b0c5&0#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";
            
            if (deviceName.Length > 4)
            {
                // remove the \??\
                deviceName = deviceName.Substring(4);

                string[] split = deviceName.Split('#');

                string id_01 = split[0];    // ACPI (Class code)
                string id_02 = split[1];    // PNP0303 (SubClass code)
                string id_03 = split[2];    // 3&13c0b0c5&0 (Protocol code)
                // The final part is the class GUID and is not needed here

                // Open the appropriate key as read-only so no permissions are needed.
                RegistryKey OurKey = Registry.LocalMachine;
                string findme = String.Format(@"System\CurrentControlSet\Enum\{0}\{1}\{2}", id_01, id_02, id_03);
                OurKey = OurKey.OpenSubKey(findme, false);

                // Retrieve the desired information and set isKeyboard
                info["DeviceDesc"] = (string)OurKey.GetValue("DeviceDesc");
                info["Class"] = (string)OurKey.GetValue("Class");
            }

            return info;
        }

        [DebuggerStepThrough]
        public void ProcessInput(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (mice.Count <= 0)
                return;

            RAWINPUT raw = Win32RawInput.GetInputData(lParam);
            if (raw.header.dwType == (int)RIM.TYPEMOUSE)
            {
                RawInputEvent(this, new RawInputEventArgs(RawInputAction.MouseMove));
            }

            /*
            uint dwSize = 0;
            uint result = 0;

            // First call to GetRawInputData sets the value of dwSize, which can then be used to allocate the appropriate 
            // amount of memory, storing the pointer in "buffer".
            result = Win32RawInput.GetRawInputData(lParam, RID.INPUT, IntPtr.Zero, ref dwSize, Marshal.SizeOf(typeof(RAWINPUTHEADER)));
            if (result == 0)
            {
                IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
                try
                {
                    result = Win32RawInput.GetRawInputData(lParam, RID.INPUT, buffer, ref dwSize, Marshal.SizeOf(typeof(RAWINPUTHEADER)));
                    if (buffer != IntPtr.Zero && result == dwSize)
                    {
                        RAWINPUT raw = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));

                        // Only listening for mouse move events at the moment.
                        if (raw.header.dwType == (int)RIM.TYPEMOUSE)
                        {
                            DeviceInfo mouse = mice[0];
                            
                            //if ((raw.mouse.usFlags & (ushort)MouseEventFlags.MOVE) != 0 ||
                            //    (raw.mouse.usFlags & (ushort)MouseEventFlags.ABSOLUTE) != 0)
                            //{
                            //    Debug.WriteLine("MouseMove");
                            //    RawInputEvent(this, new RawInputEventArgs(RawInputAction.MouseMove));
                            //}                             
                            //RawInputEvent(this, new RawInputEventArgs(RawInputAction.MouseMove));
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }             
            }
             */
        }

        #region IDisposable Interface
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    // Dispose managed resources
                    // N/A
                }

                // Dispose unmanaged resources.
                RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];
                rid[0].usUsagePage = Win32RawInput.HID_HWIND.HidMouse.UsagePage;
                rid[0].usUsage = Win32RawInput.HID_HWIND.HidMouse.UsageId;
                rid[0].dwFlags = (int)RIDEV.REMOVE;
                rid[0].hwndTarget = this.handle;
                Win32RawInput.RegisterRawInputDevices(rid, rid.Length, Marshal.SizeOf(rid[0]));
            }
        }

        ~RawInput()
        {
            Dispose(false);
        }    
        #endregion
    }
}
