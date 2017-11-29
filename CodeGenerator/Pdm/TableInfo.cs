using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.Pdm
{
    public class TableInfo : InfoBase
    {
        public string PhysicalOptions { get; set; }

        [ChildObject("c:Columns", typeof(ColumnInfo))]
        public List<ColumnInfo> ColumnInfos { get; set; }

        [ChildObject("c:Keys", typeof(KeyInfo))]
        public List<KeyInfo> KeyInfos { get; set; }

        [ChildObject("c:PrimaryKey", typeof(KeyInfo))]
        public List<KeyInfo> PrimaryKeys { get; set; }

        public List<ChildTableInfo> ChildTableInfos { get; set; }

        public List<ReferenceTableInfo> ReferenceTableInfos { get; set; }

        public string GetFormatTableName()
        {
            var index = Code.LastIndexOf('_');
            return Code.Substring(index + 1);
        }

        public string GetPrimaryKeyColumnName()
        {
            if (PrimaryKeys?.FirstOrDefault() == null ) return string.Empty;

            var primaryKey = PrimaryKeys.FirstOrDefault();

            if (primaryKey?.Columns == null ) return string.Empty;

            var column = primaryKey.Columns.FirstOrDefault();
            return column == null ? string.Empty : column.Code;
        }
    }

    public class ChildTableInfo
    {
        public ColumnInfo ForeignKey { get; set; }

        public TableInfo ChildTable { get; set; }
    }

    public class ReferenceTableInfo
    {
        public TableInfo ParentTable { get; set; }
        
        public ColumnInfo ReferenceKey { get; set; }
        
        public ColumnInfo ForeignKey { get; set; }
    }
}