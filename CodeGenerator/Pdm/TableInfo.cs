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

        [ChildObject("c:PrimaryKey", typeof(RefInfo))]
        public List<RefInfo> PrimaryKeys { get; set; }

        public List<ChildTableInfo> ChildTableInfos { get; set; }

        public List<ReferenceTableInfo> ReferenceTableInfos { get; set; }

        private string _tablename;
        private bool _isTableNameInit;

        public string TableName
        {
            get
            {
                if (_isTableNameInit) return _tablename;

                var index = Code.LastIndexOf('_');
                _tablename = Code.Substring(index + 1);

                _isTableNameInit = true;
                return _tablename;
            }
        }

        private string _primaryKeyCode;
        private bool _isPrimaryKeyCodeInit;

        public string PrimaryKeyCode
        {
            get
            {
                if (_isPrimaryKeyCodeInit) return _primaryKeyCode;

                var primaryKey = PrimaryKeys.FirstOrDefault();
                if (primaryKey == null) return string.Empty;

                var keyInfo = KeyInfos.FirstOrDefault(t => t.Id == primaryKey.Ref);

                var key = keyInfo?.Columns.FirstOrDefault();
                if (key == null) return string.Empty;

                var column = ColumnInfos.FirstOrDefault(t => t.Id == key.Ref);
                if (column == null) return string.Empty;

                _primaryKeyCode = column.Code;
                _isPrimaryKeyCodeInit = true;

                return _primaryKeyCode;
            }
        }
    }

    public class ChildTableInfo
    {
        public ColumnInfo ForeignKey { get; set; }

        public TableInfo ChildTable { get; set; }

        public string ChildPropertyName { get; set; }
    }

    public class ReferenceTableInfo
    {
        public TableInfo ParentTable { get; set; }

        public ColumnInfo ReferenceKey { get; set; }

        public ColumnInfo ForeignKey { get; set; }

        public string ParentPropertyName { get; set; }
    }
}