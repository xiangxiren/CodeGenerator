using System.Collections.Generic;
using System.Linq;
using CodeGenerator.Pdm;

namespace CodeGenerator.Operate
{
    public sealed class XmlNodeOperate
    {
        private static readonly object SynObject = new object();

        private static XmlNodeOperate _xmlNodeOperate;

        public List<TableInfo> TableInfos { get; private set; }

        public static XmlNodeOperate Context
        {
            get
            {
                if (_xmlNodeOperate == null)
                {
                    lock (SynObject)
                    {
                        if (_xmlNodeOperate == null)
                            _xmlNodeOperate = new XmlNodeOperate();
                    }
                }

                return _xmlNodeOperate;
            }
        }

        public static void Init(List<TableInfo> tableInfos)
        {
            Context.TableInfos = tableInfos;
        }

        public List<ColumnInfo> GetColumnDataGridDataSource(string tableId)
        {
            var list = new List<ColumnInfo>();

            if (!string.IsNullOrEmpty(tableId) && TableInfos != null)
            {
                var table = TableInfos.FirstOrDefault(t => t.Id == tableId);
                if (table != null)
                {
                    list = table.ColumnInfos;
                }
            }

            return list;
        }
    }
}