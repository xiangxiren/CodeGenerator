using System.Collections.Generic;

namespace CodeGenerator.Pdm
{
    public class ModelInfo : InfoBase
    {
        public List<PackageInfo> PackageInfos { get; set; }

        public List<TableInfo> TableInfos { get; set; }
    }
}