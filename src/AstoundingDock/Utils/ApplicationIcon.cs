using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AstoundingApplications.AstoundingDock.Extensions;
using System.Drawing;
using System.Drawing.IconLib;
using System.Drawing.IconLib.Exceptions;
using System.Diagnostics;
using System.Diagnostics.Contracts;

using PathEx = AstoundingApplications.AstoundingDock.Extensions.PathEx;

namespace AstoundingApplications.AstoundingDock.Utils
{   
    public enum IconSize { Medium, Small, Large }

    /// <summary>
    /// Handles the extraction of images from a file.
    /// </summary>
    class ApplicationIcon
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const int Small = 16;
        public const int Medium = 32;
        public const int Large = 64;
        public const int Huge = 96;

        string _imagePath;

        public ApplicationIcon(string imagePath)
        {
            _imagePath = imagePath;
        }

        public Bitmap GetImage(IconSize imageSize)
        {
            if (!File.Exists(_imagePath))
                return null;

            if (PathEx.HasExtension(_imagePath, FileExtensions.Executable) ||
                PathEx.HasExtension(_imagePath, FileExtensions.Shortcut) ||
                PathEx.HasExtension(_imagePath, FileExtensions.Icon))
            {
                return GetIcon(imageSize);
            }

            try
            {
                return new Bitmap(_imagePath);
            }
            catch (ArgumentException ex)
            {
                // Unable to convert to an image, assume it's an exe/ico/lnk file even though the extension is wrong.
                if (Log.IsDebugEnabled)
                    Log.DebugFormat("Failed to convert {0} to a image, trying to convert to an icon instead: {1}", _imagePath, ex);

                return GetIcon(imageSize);
            }
            catch (IOException ex)
            {
                // Unable to convert to an image, assume it's an exe/ico/lnk file even though the extension is wrong.
                if (Log.IsDebugEnabled)
                    Log.DebugFormat("Failed to convert {0} to a image, trying to convert to an icon instead: {1}", _imagePath, ex);

                return GetIcon(imageSize);
            }
        }

        Bitmap GetIcon(IconSize imageSize)
        {
            // Attempt to extract the icon from the file
            if (imageSize == IconSize.Medium)
                return Icon.ExtractAssociatedIcon(_imagePath).ToBitmap();

            MultiIcon multicon = new MultiIcon();
            try
            {
                multicon.Load(_imagePath);
            }
            catch (InvalidFileException ex)
            {
                if (Log.IsDebugEnabled)
                    Log.DebugFormat("Failed to get icons from {0}, using default application icon, got exception\n{1}", _imagePath, ex);

                return Icon.ExtractAssociatedIcon(_imagePath).ToBitmap();
            }

            IconImage largeImage = null;
            IconImage smallImage = null;

            if (multicon.Count > 0)
            {
                SingleIcon icon = multicon[0];
                foreach (IconImage iconImage in icon)
                {
                    // Ignore low quality icons (they look ugly), really big icons (don't need them that big, saves memory) 
                    // or really small ones.
                    if (!IsLowQuality(iconImage) && iconImage.Size.Height <= Huge && iconImage.Size.Height >= Small)
                    {
                        if (largeImage == null)
                            largeImage = iconImage;
                        if (smallImage == null)
                            smallImage = iconImage;

                        if (iconImage.Size.Height > largeImage.Size.Height)
                            largeImage = iconImage;

                        if (iconImage.Size.Height < smallImage.Size.Height)
                            smallImage = iconImage;
                    }
                }
            }

            if (imageSize == IconSize.Small && smallImage != null)
                return smallImage.Transparent;

            if (imageSize == IconSize.Large && largeImage != null)
                return largeImage.Transparent;

            return Icon.ExtractAssociatedIcon(_imagePath).ToBitmap();
        }

        bool IsLowQuality(IconImage iconImage)
        {
            return iconImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed ||
                    iconImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed ||
                    iconImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
        }

        double ConvertIconSize(IconSize iconSize)
        {
            switch (iconSize)
            {
                case IconSize.Large:
                    return Large;
                case IconSize.Medium:
                    return Medium;
                case IconSize.Small:
                    return Small;
                default:
                    return Medium;
            }
        }
    }
}
