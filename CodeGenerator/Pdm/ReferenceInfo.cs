using System.Collections.Generic;

namespace CodeGenerator.Pdm
{
    public class ReferenceInfo : InfoBase
    {
        [ChildObject("c:ParentTable")]
        public RefInfo ParentTable { get; set; }

        [ChildObject("c:ChildTable")]
        public RefInfo ChildTable { get; set; }

        [ChildObject("c:Joins", typeof(ReferenceJoinInfo))]
        public List<ReferenceJoinInfo> ReferenceJoinInfos { get; set; }
    }

    public class RefInfo
    {
        [NodeAttribute]
        public string Ref { get; set; }
    }
    
    public class ReferenceJoinInfo : InfoBase
    {
        [ChildObject("c:Object1")]
        public RefInfo FirstColumn { get; set; }

        [ChildObject("c:Object2")]
        public RefInfo SecondColumn { get; set; }
    }
}