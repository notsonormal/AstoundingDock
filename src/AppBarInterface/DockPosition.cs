using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.Win32Interface;

namespace AstoundingApplications.AppBarInterface
{
    public enum DockEdge { Right, Left, Top, Bottom }

    public class DockPosition : IEquatable<DockPosition>
    {
        public DockEdge Selected { get; set; }

        public DockPosition(DockEdge edge)
        {
            Selected = edge;
        }

        public override string ToString()
        {
            return Selected.ToString();
        }

        public static DockPosition Left()
        {
            return new DockPosition(DockEdge.Left);
        }

        public static DockPosition Top()
        {
            return new DockPosition(DockEdge.Top);
        }

        public static DockPosition Right()
        {
            return new DockPosition(DockEdge.Right);
        }

        public static DockPosition Bottom()
        {
            return new DockPosition(DockEdge.Bottom);
        }

        internal static DockPosition FromNative(ABE abEdge)
        {
            switch (abEdge)
            {
                case ABE.LEFT:
                    return new DockPosition(DockEdge.Left);
                case ABE.TOP:
                    return new DockPosition(DockEdge.Top);
                case ABE.RIGHT:
                    return new DockPosition(DockEdge.Right);
                case ABE.BOTTOM:
                    return new DockPosition(DockEdge.Bottom);
                default:
                    throw new NotImplementedException(abEdge.ToString());
            }
        }

        internal ABE ToNative()
        {
            switch (Selected)
            {
                case DockEdge.Left:
                    return ABE.LEFT;
                case DockEdge.Top:
                    return ABE.TOP;
                case DockEdge.Right:
                    return ABE.RIGHT;
                case DockEdge.Bottom:
                    return ABE.BOTTOM;
                default:
                    throw new NotImplementedException(Selected.ToString());
            }
        }

        public bool Equals(DockPosition other)
        {
            if (other == null)
                return false;
            return other.Selected == Selected;
        }
    }
}
