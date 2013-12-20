using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace AstoundingApplications.Win32Interface
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static class Win32Shortcut
    {
        const int MaxFeatureLength = 38;
        const int MaxGuidLength = 38;
        const int MaxPathLength = 1024;

        enum InstallState
        {
            NotUsed = -7,
            BadConfig = -6,
            Incomplete = -5,
            SourceAbsent = -4,
            MoreData = -3,
            InvalidArg = -2,
            Unknown = -1,
            Broken = 0,
            Advertised = 1,
            Removed = 1,
            Absent = 2,
            Local = 3,
            Source = 4,
            Default = 5
        }

        /// <summary>
        /// Try to get real file path from an MSI (windows installer) shortcut. 
        /// </summary>
        /// <remarks>http://www.geektieguy.com/2007/11/19/how-to-parse-special-lnk-files-aka-msi-shortcuts-aka-windows-installer-advertised-shortcuts-using-c/</remarks>
        public static string ParseMsiShortcut(string file)
        {
            StringBuilder product = new StringBuilder(MaxGuidLength + 1);
            StringBuilder feature = new StringBuilder(MaxFeatureLength + 1);
            StringBuilder component = new StringBuilder(MaxGuidLength + 1);

            NativeMethods.MsiGetShortcutTarget(file, product, feature, component);

            int pathLength = MaxPathLength;
            StringBuilder path = new StringBuilder(pathLength);

            InstallState installState = NativeMethods.MsiGetComponentPath(product.ToString(), component.ToString(), path, ref pathLength);
            if (installState == InstallState.Local)
            {
                return path.ToString();
            }
            else
            {
                return null;
            }
        }

        private static class NativeMethods
        {
            /*
            UINT MsiGetShortcutTarget(
                LPCTSTR szShortcutTarget,
                LPTSTR szProductCode,
                LPTSTR szFeatureId,
                LPTSTR szComponentCode
            );
            */
            [DllImport("msi.dll", CharSet = CharSet.Auto)]
            internal static extern uint MsiGetShortcutTarget(string targetFile, StringBuilder productCode, StringBuilder featureID, StringBuilder componentCode);

            /*
            INSTALLSTATE MsiGetComponentPath(
              LPCTSTR szProduct,
              LPCTSTR szComponent,
              LPTSTR lpPathBuf,
              DWORD* pcchBuf
            );
            */
            [DllImport("msi.dll", CharSet = CharSet.Auto)]
            internal static extern InstallState MsiGetComponentPath(string productCode, string componentCode, StringBuilder componentPath, ref int componentPathBufferSize);
        }
    }
}
