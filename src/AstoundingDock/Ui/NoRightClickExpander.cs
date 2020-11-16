using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace AstoundingApplications.AstoundingDock.Ui
{
    /// <summary>
    /// Disables expanding on right click
    /// </summary>
    public class NoRightClickExpander : Expander
    {
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
