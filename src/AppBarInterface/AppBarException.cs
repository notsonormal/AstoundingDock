using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace AstoundingApplications.AppBarInterface
{
    [Serializable]
    public class AppBarException : Exception
    {
        public AppBarException() : base() { }
        public AppBarException(string errorMessage) : base(errorMessage) { }
        public AppBarException(string errorMessage, Exception innerException) : base(errorMessage, innerException) { }

        protected AppBarException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
    }
}
