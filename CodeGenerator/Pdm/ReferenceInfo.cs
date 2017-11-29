using System.Collections.Generic;

namespace CodeGenerator.Pdm
{
    public class ReferenceInfo : InfoBase
    {
        [ChildObject("c:ParentTable")]
        public ReferenceTable ParentTable { get; set; }

        [ChildObject("c:ChildTable")]
        public ReferenceTable ChildTable { get; set; }

        [ChildObject("c:Joins", typeof(ReferenceJoinInfo))]
        public List<ReferenceJoinInfo> ReferenceJoinInfos { get; set; }
    }

    public class ReferenceTable
    {
        [NodeAttribute]
        public string Ref { get; set; }
    }
    
    public class ReferenceJoinInfo : InfoBase
    {
        [ChildObject("c:Object1")]
        public ReferenceColumn FirstColumn { get; set; }

        [ChildObject("c:Object2")]
        public ReferenceColumn SecondColumn { get; set; }
    }

    public class ReferenceColumn
    {
        [NodeAttribute]
        public string Ref { get; set; }
    }
}