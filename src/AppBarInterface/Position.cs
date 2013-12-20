using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.Win32Interface;

namespace AstoundingApplications.AppBarInterface
{
    public class Position
    {
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public override string ToString()
        {
            return String.Format("Top: {0}, Bottom: {1}, Left: {2}, Right: {3}, Width: {4}, Height: {5}", 
                Top, Bottom, Left, Right, Width, Height);
        }

        internal static Position FromNative(ref RECT rect)
        {
            Position position = new Position()
            {
                Top = rect.top,
                Bottom = rect.bottom,
                Left = rect.left,
                Right = rect.right
            };
            position.Height = position.Bottom - position.Top;
            position.Width = position.Right - position.Left;

            return position;
        }

        internal void ToNative(ref RECT rect)
        {
            rect.top = (int)Top;
            rect.bottom = (int)Bottom;
            rect.left = (int)Left;
            rect.right = (int)Right;
        }
    }
}
