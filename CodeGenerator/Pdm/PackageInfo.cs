using System.Collections.Generic;

namespace CodeGenerator.Pdm
{
    public class PackageInfo : InfoBase
    {
        [ChildObject("c:Packages", typeof(PackageInfo))]
        public List<PackageInfo> PackageInfos { get; set; }

        [ChildObject("c:Tables", typeof(TableInfo))]
        public List<TableInfo> TableInfos { get; set; }
    }
}