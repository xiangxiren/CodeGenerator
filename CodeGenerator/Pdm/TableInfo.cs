using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.Pdm
{
    public class TableInfo : InfoBase
    {
        public string PhysicalOptions { get; set; }

        public List<ColumnInfo> ColumnInfos { get; set; }

        public List<KeyInfo> KeyInfos { get; set; }

        public List<KeyInfo> PrimaryKeys { get; set; }

        public string GetFormatTableName()
        {
            var index = Code.LastIndexOf('_');
            return Code.Substring(index + 1);
        }

        public string GetPrimaryKeyColumnName()
        {
            if (PrimaryKeys == null || PrimaryKeys.FirstOrDefault() == null ) return string.Empty;

            var primaryKey = PrimaryKeys.FirstOrDefault();
            if (primaryKey == null) return string.Empty;

            if (primaryKey.Columns == null ) return string.Empty;

            var column = primaryKey.Columns.FirstOrDefault();
            return column == null ? string.Empty : column.Code;
        }
    }
}