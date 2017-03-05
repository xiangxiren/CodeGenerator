using System.Collections.Generic;

namespace CodeGenerator.Generate
{
    public abstract class GenerateCodeBase
    {
        public static readonly List<string> IgnoreColumns = new List<string> { "CREATEUSERID", "CREATEDATE", "UPDATEUSERID", "UPDATEDATE", "CLIENTID", "STATUS", "UpdateUserName", "ORGID", "STAFFID" };
    }
}