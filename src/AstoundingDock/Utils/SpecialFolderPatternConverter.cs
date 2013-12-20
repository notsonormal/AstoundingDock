using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.AstoundingDock.Utils
{
    public class SpecialFolderPatternConverter : log4net.Util.PatternConverter
    {
        override protected void Convert(System.IO.TextWriter writer, object state)
        {
            Environment.SpecialFolder specialFolder = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), base.Option, true);
            string path = Environment.GetFolderPath(specialFolder);
            writer.Write(path);
        }
    }
}
