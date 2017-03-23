using System.Collections.Generic;
using System.Linq;
using CodeGenerator.AutoComplete;
using CodeGenerator.Pdm;

namespace CodeGenerator.Operate
{
    public sealed class TreeModelOperate
    {
        #region 属性、常量、字段

        private const string ImageTable = "/Image/table.png";

        private const string ImagePackage = "/Image/package.png";

        private static TreeModelOperate _treeModelOperate;

        private static readonly object SynObject = new object();

        private List<AutoCompleteEntry> _autoCompleteEntries;

        public List<TreeModel> TreeModels { get; private set; }

        public static TreeModelOperate Context
        {
            get
            {
                if (_treeModelOperate == null)
                {
                    lock (SynObject)
                    {
                        if (_treeModelOperate == null)
                            _treeModelOperate = new TreeModelOperate();
                    }
                }

                return _treeModelOperate;
            }
        }

        public List<AutoCompleteEntry> AutoCompleteEntries
        {
            get
            {
                if (_autoCompleteEntries == null && TreeModels != null)
                {
                    lock (SynObject)
                    {
                        if (_autoCompleteEntries == null && TreeModels != null)
                        {
                            _autoCompleteEntries = new List<AutoCompleteEntry>();
                            if (TreeModels == null) return _autoCompleteEntries;

                            foreach (var treeModel in TreeModels)
                            {
                                var list = GetAutoCompleteEntrys(treeModel);
                                if (list != null && list.Count > 0)
                                    _autoCompleteEntries.AddRange(list);
                            }
                        }
                    }
                }

                return _autoCompleteEntries;
            }
        }

        #endregion

        #region 私有构造函数

        private TreeModelOperate()
        {

        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="modelInfos"></param>
        public void Init(List<ModelInfo> modelInfos)
        {
            TreeModels = new List<TreeModel>();
            if (modelInfos == null) return;

            foreach (var model in modelInfos)
            {
                var node = new TreeModel { Id = model.Id, Name = model.Name, NodeType = NodeType.Model, Icon = ImagePackage, IsExpanded = true, IsSelected = true };

                if (model.PackageInfos != null)
                    node.Children = GetNodeFromPackageInfo(node, model.PackageInfos);
                if (model.TableInfos != null)
                    node.Children.AddRange(
                            model.TableInfos.Select(
                                table =>
                                    new TreeModel
                                    {
                                        Id = table.Id,
                                        Name = string.Format("{0}({1})", table.Code, table.Name),
                                        NodeType = NodeType.Table,
                                        Icon = ImageTable,
                                        Parent = node
                                    }));

                TreeModels.Add(node);
            }
        }

        /// <summary>
        /// 设置节点是否展开
        /// </summary>
        /// <param name="isExpanded"></param>
        public void SetAllNodeIsExpanded(bool isExpanded)
        {
            if (TreeModels == null) return;
            foreach (var tree in TreeModels)
            {
                tree.IsExpanded = isExpanded;
                tree.SetChildrenExpanded(isExpanded);
            }
        }

        /// <summary>
        /// 展开指定id的节点
        /// </summary>
        /// <param name="id"></param>
        public void ExpandTreeNode(string id)
        {
            if (TreeModels == null) return;

            var model = GetSelectedTreeModel(TreeModels, id);

            if (model != null)
            {
                model.IsSelected = true;
                SetParentIsExpanded(model, true);
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 通过表Id获取表节点
        /// </summary>
        /// <param name="treeModels"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private TreeModel GetSelectedTreeModel(IEnumerable<TreeModel> treeModels, string id)
        {
            if (TreeModels == null) return null;

            foreach (var treeModel in treeModels)
            {
                if (treeModel.Id == id) return treeModel;
                if (treeModel.Children == null || treeModel.Children.Count <= 0) continue;
                var model = GetSelectedTreeModel(treeModel.Children, id);
                if (model != null) return model;
            }

            return null;
        }

        /// <summary>
        /// 设置父节点的展开状态
        /// </summary>
        /// <param name="treeModel"></param>
        /// <param name="isExpanded"></param>
        private void SetParentIsExpanded(TreeModel treeModel, bool isExpanded)
        {
            if (treeModel == null) return;
            treeModel.IsExpanded = isExpanded;
            SetParentIsExpanded(treeModel.Parent, isExpanded);
        }

        private List<TreeModel> GetNodeFromPackageInfo(TreeModel parent, List<PackageInfo> packageInfos)
        {
            var nodes = new List<TreeModel>();
            if (packageInfos == null) return nodes;

            foreach (var package in packageInfos)
            {
                var node = new TreeModel { Id = package.Id, Name = package.Name, NodeType = NodeType.Package, Icon = ImagePackage, Parent = parent };
                if (package.PackageInfos != null)
                    node.Children.AddRange(GetNodeFromPackageInfo(node, package.PackageInfos));

                if (package.TableInfos != null)
                    node.Children.AddRange(
                        package.TableInfos.Select(
                            table =>
                                new TreeModel
                                {
                                    Id = table.Id,
                                    Name = string.Format("{0}({1})", table.Code, table.Name),
                                    NodeType = NodeType.Table,
                                    Icon = ImageTable,
                                    Parent = node
                                }));

                nodes.Add(node);
            }

            return nodes;
        }

        private List<AutoCompleteEntry> GetAutoCompleteEntrys(TreeModel treeModel)
        {
            var list = new List<AutoCompleteEntry>();
            if (treeModel == null) return list;
            list.Add(new AutoCompleteEntry(treeModel.Id, treeModel.Name, null));

            if (treeModel.Children == null || treeModel.Children.Count <= 0) return list;

            foreach (var child in treeModel.Children)
            {
                var childs = GetAutoCompleteEntrys(child);
                if (childs != null && childs.Count > 0)
                    list.AddRange(childs);
            }

            return list;
        }

        #endregion
    }
}
