using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CodeGenerator.Pdm
{
    public class PdmReader
    {
        public const string OModel = "o:Model";
        public const string CPackages = "c:Packages";
        public const string CTables = "c:Tables";
        public const string CColumns = "c:Columns";
        public const string CKeys = "c:Keys";
        public const string CKeyColumns = "c:Key.Columns";
        public const string CPrimaryKey = "c:PrimaryKey";

        private XmlDocument _xmlDoc;
        private XmlNamespaceManager _xmlnsManager;

        /// <summary>构造函数 </summary>
        public PdmReader()
        {
            _xmlDoc = new XmlDocument();
        }

        /// <summary>构造函数 </summary>
        public PdmReader(string pdmFile)
        {
            PdmFile = pdmFile;
        }

        private string _pdmFile;

        public string PdmFile
        {
            get { return _pdmFile; }
            set
            {
                _pdmFile = value;
                if (_xmlDoc == null)
                {
                    _xmlDoc = new XmlDocument();
                    _xmlDoc.Load(_pdmFile);
                    _xmlnsManager = new XmlNamespaceManager(_xmlDoc.NameTable);
                    _xmlnsManager.AddNamespace("a", "attribute");
                    _xmlnsManager.AddNamespace("c", "collection");
                    _xmlnsManager.AddNamespace("o", "object");
                }
            }
        }

        public List<ModelInfo> Models { get; set; }

        public List<TableInfo> Tables { get; set; }

        public void InitData()
        {
            if (Models == null)
                Models = new List<ModelInfo>();

            if (Tables == null)
                Tables = new List<TableInfo>();

            var xnModels = _xmlDoc.SelectNodes("//" + OModel, _xmlnsManager);

            if (xnModels == null) return;
            foreach (XmlNode xnModel in xnModels)
            {
                Models.Add(GetModel(xnModel));
            }
        }

        /// <summary>
        /// 初始化"o:Model"的节点
        /// </summary>
        /// <param name="xnModel"></param>
        /// <returns></returns>
        private ModelInfo GetModel(XmlNode xnModel)
        {
            var model = new ModelInfo();
            if (xnModel != null)
                SetNodeValueInfo(model, xnModel);

            model.PackageInfos = GetPackageInfos(xnModel);
            model.TableInfos = GetTableInfos(xnModel);

            return model;
        }

        /// <summary>
        /// 获取Model或Package下的Packages
        /// </summary>
        /// <param name="xnModel"></param>
        /// <returns></returns>
        private List<PackageInfo> GetPackageInfos(XmlNode xnModel)
        {
            var packages = new List<PackageInfo>();
            if (xnModel == null) return packages;

            var xnPackages = xnModel.ChildNodes.Cast<XmlNode>().FirstOrDefault(o => o.Name == CPackages);
            if (xnPackages == null) return packages;

            foreach (XmlNode xnPackage in xnPackages)
            {
                var package = new PackageInfo();
                SetNodeValueInfo(package, xnPackage);

                package.PackageInfos = GetPackageInfos(xnPackage);
                package.TableInfos = GetTableInfos(xnPackage);

                packages.Add(package);
            }

            return packages.OrderBy(p => p.Code).ToList();
        }

        /// <summary>
        /// 初始化"o:Model"的节点
        /// </summary>
        /// <param name="xnPackage"></param>
        /// <returns></returns>
        private List<TableInfo> GetTableInfos(XmlNode xnPackage)
        {
            var tables = new List<TableInfo>();
            if (xnPackage == null) return tables;

            var xnTables = xnPackage.ChildNodes.Cast<XmlNode>().FirstOrDefault(o => o.Name == CTables);
            if (xnTables == null) return tables;

            foreach (XmlNode xnTable in xnTables)
            {
                var table = new TableInfo();
                SetNodeValueInfo(table, xnTable);

                table.ColumnInfos = GetColumnInfos(xnTable);
                if (table.ColumnInfos == null || table.ColumnInfos.Count <= 0) continue;

                table.KeyInfos = GetKeyInfos(table, xnTable);
                table.PrimaryKeys = GetPrimaryKeys(table, xnTable);

                tables.Add(table);
            }

            Tables.AddRange(tables);
            return tables;
        }

        /// <summary>
        /// 获取Table下的Columns
        /// </summary>
        /// <param name="xnTable"></param>
        /// <returns></returns>
        private List<ColumnInfo> GetColumnInfos(XmlNode xnTable)
        {
            var columns = new List<ColumnInfo>();
            if (xnTable == null) return columns;

            var xnColumns = xnTable.ChildNodes.Cast<XmlNode>().FirstOrDefault(o => o.Name == CColumns);
            if (xnColumns == null) return columns;

            foreach (XmlNode xnColumn in xnColumns)
            {
                var column = new ColumnInfo();
                SetNodeValueInfo(column, xnColumn);

                columns.Add(column);
            }

            return columns;
        }

        /// <summary>
        /// 获取Table下的KeyInfos
        /// </summary>
        /// <param name="table"></param>
        /// <param name="xnTable"></param>
        /// <returns></returns>
        private List<KeyInfo> GetKeyInfos(TableInfo table, XmlNode xnTable)
        {
            var keyInfos = new List<KeyInfo>();
            if (xnTable == null) return keyInfos;

            var xnKeyInfos = xnTable.ChildNodes.Cast<XmlNode>().FirstOrDefault(o => o.Name == CKeys);
            if (xnKeyInfos == null) return keyInfos;

            foreach (XmlNode xnKey in xnKeyInfos)
            {
                var column = new KeyInfo();
                SetNodeValueInfo(column, xnKey);
                column.Columns = GetKeyColumns(table, xnKey);

                keyInfos.Add(column);
            }

            return keyInfos;
        }

        /// <summary>
        /// 获取Key下的Columns
        /// </summary>
        /// <param name="table"></param>
        /// <param name="xnKey"></param>
        /// <returns></returns>
        private List<ColumnInfo> GetKeyColumns(TableInfo table, XmlNode xnKey)
        {
            var columns = new List<ColumnInfo>();
            if (xnKey == null) return columns;

            var xnKeyColumns = xnKey.ChildNodes.Cast<XmlNode>().FirstOrDefault(o => o.Name == CKeyColumns);
            if (xnKeyColumns == null) return columns;

            columns.AddRange(
                xnKeyColumns.Cast<XmlElement>()
                    .Select(
                        xeKeyColumn => table.ColumnInfos.FirstOrDefault(o => o.Id == xeKeyColumn.GetAttribute("Ref")))
                    .Where(column => column != null));

            return columns;
        }

        /// <summary>
        /// 获取table的PrimaryKey
        /// </summary>
        /// <param name="xnTable"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<KeyInfo> GetPrimaryKeys(TableInfo table, XmlNode xnTable)
        {
            var primaryKeys = new List<KeyInfo>();
            if (xnTable == null) return primaryKeys;

            var xnPrimaryKey = xnTable.ChildNodes.Cast<XmlNode>().FirstOrDefault(o => o.Name == CPrimaryKey);
            if (xnPrimaryKey == null) return primaryKeys;

            primaryKeys.AddRange(
                xnPrimaryKey.Cast<XmlElement>()
                    .Select(element => table.KeyInfos.FirstOrDefault(o => o.Id == element.GetAttribute("Ref")))
                    .Where(key => key != null));

            return primaryKeys;
        }

        /// <summary>
        /// 遍历节点下的子节点并将赋值给实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="node"></param>
        private void SetNodeValueInfo<T>(T info, XmlNode node)
        {
            var properties = typeof(T).GetProperties();
            var childNodes = node.ChildNodes.Cast<XmlNode>().ToList();

            foreach (var property in properties)
            {
                if (property.Name == "Id")
                {
                    var element = (XmlElement)node;
                    property.SetValue(info, element.GetAttribute("Id"));
                }
                else if (property.Name == "Mandatory")
                {
                    var mandatoryNode = childNodes.FirstOrDefault(o => o.Name == "a:Column.Mandatory");
                    if (mandatoryNode == null) continue;

                    property.SetValue(info, Convert.ToBoolean(Convert.ToInt32(mandatoryNode.InnerText)));
                }
                else
                {
                    var childNode = childNodes.FirstOrDefault(o => o.Name == "a:" + property.Name);
                    if (childNode == null) continue;

                    switch (property.PropertyType.FullName)
                    {
                        case "System.Int32":
                            property.SetValue(info, Convert.ToInt32(childNode.InnerText));
                            break;
                        case "System.Boolean":
                            bool value;

                            if (!bool.TryParse(childNode.InnerText, out value))
                                value = childNode.InnerText == "1";

                            property.SetValue(info, value);
                            break;
                        default:
                            property.SetValue(info, childNode.InnerText);
                            break;
                    }
                }
            }
        }
    }
}