using System.Collections.Generic;

namespace CodeGenerator.Pdm
{
    public class ModelInfo : InfoBase
    {
        [ChildObject("c:Packages", typeof(PackageInfo))]
        public List<PackageInfo> PackageInfos { get; set; }

        [ChildObject("c:Tables", typeof(TableInfo))]
        public List<TableInfo> TableInfos { get; set; }

        [ChildObject("c:References", typeof(ReferenceInfo))]
        public List<ReferenceInfo> ReferenceInfos { get; set; }
    }
}