using System.Collections.Generic;

namespace CodeGenerator.Pdm
{
    public class KeyInfo : InfoBase
    {
        [ChildObject("c:Key.Columns", typeof(RefInfo))]
        public List<RefInfo> Columns { get;  set; }
    }
}