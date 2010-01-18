using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ProductivityPackage
{
    /// <summary>
    /// Class for NativedMethods
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        /// 
        /// </summary>
        public const uint SHGFI_ICON = 0x100;
        /// <summary>
        /// 
        /// </summary>
        public const uint SHGFI_LARGEICON = 0x0;
        /// <summary>
        /// 
        /// </summary>
        public const uint SHGFI_SMALLICON = 0x1;

        /// <summary>
        /// SHFILEINFO Struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            /// <summary>
            /// 
            /// </summary>
            public IntPtr hIcon;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr iIcon;
            /// <summary>
            /// 
            /// </summary>
            public uint dwAttributes;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        /// <summary>
        /// Get file info.
        /// </summary>
        /// <param name="pszPath">The PSZ path.</param>
        /// <param name="dwFileAttributes">The dw file attributes.</param>
        /// <param name="psfi">The psfi.</param>
        /// <param name="cbSizeFileInfo">The cb size file info.</param>
        /// <param name="uFlags">The u flags.</param>
        /// <returns></returns>
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        /// <summary>
        /// Get image count.
        /// </summary>
        /// <param name="HIMAGELIST">The HIMAGELIST.</param>
        /// <returns></returns>
        [DllImport("COMCTL32")]
        public static extern int ImageList_GetImageCount(int HIMAGELIST);

        /// <summary>
        /// Get icon.
        /// </summary>
        /// <param name="HIMAGELIST">The HIMAGELIST.</param>
        /// <param name="ImgIndex">Index of the img.</param>
        /// <param name="hbmMask">The HBM mask.</param>
        /// <returns></returns>
        [DllImport("COMCTL32")]
        public static extern int ImageList_GetIcon(int HIMAGELIST, int ImgIndex, int hbmMask);

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <param name="pszPath">The PSZ path.</param>
        /// <returns></returns>
        public static Icon GetIcon(string pszPath)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            NativeMethods.SHGetFileInfo(pszPath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), NativeMethods.SHGFI_ICON | NativeMethods.SHGFI_SMALLICON);

            if((int)shinfo.hIcon == 0)
                return null;
            
            try
            {
                Icon myIcon = Icon.FromHandle(shinfo.hIcon);
                return myIcon;
            }
            catch(ArgumentException)
            {
                return null;
            }
        }
    }
}