using System.Collections.Generic;

namespace CodeGenerator.Pdm
{
    public class KeyInfo : InfoBase
    {
        [ChildObject("c:Key.Columns", typeof(ColumnInfo))]
        public List<ColumnInfo> Columns { get;  set; }
    }
}