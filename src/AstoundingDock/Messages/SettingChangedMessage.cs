using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.AstoundingDock.Messages
{
    class SettingChangedMessage
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public SettingChangedMessage(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
