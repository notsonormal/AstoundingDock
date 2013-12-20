using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.AstoundingDock.Ui
{
    /// <summary>
    /// Converts items in the Master list to Items in the target list, and back again.
    /// </summary>
    /// <remarks>
    /// http://blog.functionalfun.net/2009/02/how-to-databind-to-selecteditems.html
    /// </remarks>
    interface IListItemConverter
    {
        /// <summary>
        /// Converts the specified master list element.
        /// </summary>
        /// <param name="masterListItem">The master list element.</param>
        /// <returns>The result of the conversion.</returns>
        object Convert(object masterListItem);

        /// <summary>
        /// Converts the specified target list element.
        /// </summary>
        /// <param name="targetListItem">The target list element.</param>
        /// <returns>The result of the conversion.</returns>
        object ConvertBack(object targetListItem);
    }
}
