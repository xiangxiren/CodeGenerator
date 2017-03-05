using System.Collections.Generic;

namespace CodeGenerator.Pdm
{
    public class PackageInfo : InfoBase
    {
        public List<PackageInfo> PackageInfos { get; set; }

        public List<TableInfo> TableInfos { get; set; }
    }
}