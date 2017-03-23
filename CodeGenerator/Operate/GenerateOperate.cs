using System.Collections.Generic;
using System.Linq;
using CodeGenerator.Pdm;

namespace CodeGenerator.Operate
{
    public class GenerateOperate
    {
        private readonly TreeModel _treeModel;

        private readonly List<TableInfo> _tableInfos;

        public GenerateOperate(TreeModel treeModel, List<TableInfo> tableInfos)
        {
            _treeModel = treeModel;
            _tableInfos = tableInfos;
        }

        public List<TableInfo> GetGenerateTables()
        {
            var tables = new List<TableInfo>();
            if (_treeModel == null) return tables;

            if (_treeModel.NodeType == NodeType.Table)
            {
                var table = _tableInfos.FirstOrDefault(t => t.Id == _treeModel.Id && t.ColumnInfos != null && t.ColumnInfos.Count > 0);

                if (table != null)
                    tables.Add(table);
            }
            else if (_treeModel.NodeType == NodeType.Package)
            {
                tables.AddRange(GetTableInfosFromPackage(_treeModel));
            }
            else
            {
                if (_treeModel.Children == null) return tables;

                tables.AddRange(
                    _treeModel.Children.Where(o => o.NodeType == NodeType.Table)
                        .Select(
                            tableNode =>
                                _tableInfos.FirstOrDefault(
                                    t => t.Id == tableNode.Id && t.ColumnInfos != null && t.ColumnInfos.Count > 0))
                        .Where(table => table != null));

                foreach (var packageNode in _treeModel.Children.Where(o => o.NodeType == NodeType.Package))
                {
                    tables.AddRange(GetTableInfosFromPackage(packageNode));
                }
            }

            return tables;
        }

        private IEnumerable<TableInfo> GetTableInfosFromPackage(TreeModel treeModel)
        {
            var tables = new List<TableInfo>();

            if (treeModel == null || treeModel.Children == null) return tables;
            foreach (var child in treeModel.Children)
            {
                if (child.NodeType == NodeType.Package)
                    tables.AddRange(GetTableInfosFromPackage(child));
                else
                {
                    var table = _tableInfos.FirstOrDefault(t => t.Id == child.Id && t.ColumnInfos != null && t.ColumnInfos.Count > 0);

                    if (table != null)
                        tables.Add(table);
                }
            }

            return tables;
        }

    }
}