using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CodeGenerator.Pdm
{
    public class PdmReader
    {
        public const string OModel = "o:Model";

        private readonly Dictionary<Type, Type> _listTypeDictionary = new Dictionary<Type, Type>();

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
            get => _pdmFile;
            set
            {
                _pdmFile = value;
                if (_xmlDoc != null) return;
                _xmlDoc = new XmlDocument();
                _xmlDoc.Load(_pdmFile);
                _xmlnsManager = new XmlNamespaceManager(_xmlDoc.NameTable);
                _xmlnsManager.AddNamespace("a", "attribute");
                _xmlnsManager.AddNamespace("c", "collection");
                _xmlnsManager.AddNamespace("o", "object");
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

            StructureReference();
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

            return model;
        }

        /// <summary>
        /// 遍历节点下的子节点并将赋值给实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="node"></param>
        private void SetNodeValueInfo<T>(T info, XmlNode node)
        {
            var properties = info.GetType().GetProperties();

            var childNodes = node.ChildNodes.Cast<XmlNode>().ToList();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsClass && property.PropertyType.GetConstructor(new Type[0]) != null)
                {
                    var childObject =
                        property.GetCustomAttributes(typeof(ChildObjectAttribute), false)
                            .FirstOrDefault() as ChildObjectAttribute;

                    if (childObject == null) continue;

                    if (property.PropertyType.GetInterface("IList", false) != null)
                    {
                        var list = GetListChild(node, childObject);

                        property.SetValue(info, list);
                        continue;
                    }

                    var referenceTable = Activator.CreateInstance(property.PropertyType);

                    var tableNode = node.ChildNodes.Cast<XmlNode>()
                        .FirstOrDefault(o => o.Name == childObject.ChildNodeName);
                    if (tableNode == null) continue;

                    foreach (XmlNode xnTable in tableNode)
                    {
                        SetNodeValueInfo(referenceTable, xnTable);
                    }

                    property.SetValue(info, referenceTable);
                }
                else
                {
                    if (property.GetCustomAttributes(typeof(NodeAttributeAttribute), false)
                        .FirstOrDefault() is NodeAttributeAttribute nodeAttribute)
                    {
                        var attributeName = string.IsNullOrEmpty(nodeAttribute.AttributeName)
                            ? property.Name
                            : nodeAttribute.AttributeName;

                        var element = (XmlElement)node;
                        property.SetValue(info, element.GetAttribute(attributeName));
                    }
                    else
                    {
                        var nodeChild =
                            property.GetCustomAttributes(typeof(NodeChildAttribute), false).FirstOrDefault() as NodeChildAttribute;

                        var nodeName = string.IsNullOrEmpty(nodeChild?.ChildNodeName)
                            ? property.Name
                            : nodeChild.ChildNodeName;

                        var childNode = childNodes.FirstOrDefault(o => o.Name == "a:" + nodeName);
                        if (childNode == null) continue;

                        var innerText = childNode.InnerText;
                        if (!string.IsNullOrEmpty(innerText)) innerText = innerText.Replace("\r\n", "");

                        switch (property.PropertyType.FullName)
                        {
                            case "System.Int32":
                                property.SetValue(info, Convert.ToInt32(innerText));
                                break;
                            case "System.Boolean":
                                if (!bool.TryParse(innerText, out var value))
                                    value = innerText == "1";

                                property.SetValue(info, value);
                                break;
                            default:
                                property.SetValue(info, innerText);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取集合子元素
        /// </summary>
        /// <param name="node"></param>
        /// <param name="childObjectAttribute"></param>
        /// <returns></returns>
        private object GetListChild(XmlNode node, ChildObjectAttribute childObjectAttribute)
        {
            if (childObjectAttribute.ElementType == null) return null;

            var listType = GetLisTypeFromDictionary(childObjectAttribute.ElementType);
            if (listType == null)
            {
                var typeName = $"System.Collections.Generic.List`1[[{childObjectAttribute.ElementType.AssemblyQualifiedName}]]";
                listType = Type.GetType(typeName);

                if (listType == null) return null;

                _listTypeDictionary.Add(childObjectAttribute.ElementType, listType);
            }

            var list = Activator.CreateInstance(listType);

            var xnChild = node.ChildNodes.Cast<XmlNode>().FirstOrDefault(o => o.Name == childObjectAttribute.ChildNodeName);
            if (xnChild == null) return list;

            foreach (XmlNode xnNode in xnChild)
            {
                var declaringObj = Activator.CreateInstance(childObjectAttribute.ElementType);
                SetNodeValueInfo(declaringObj, xnNode);

                AddElementToList(listType, list, declaringObj);
            }

            return list;
        }

        /// <summary>
        /// 向指定集合实例中添加元素
        /// </summary>
        /// <param name="listType"></param>
        /// <param name="instance"></param>
        /// <param name="element"></param>
        private void AddElementToList(Type listType, object instance, object element)
        {
            var method = listType.GetMethod("Add");
            if (method == null) return;

            method.Invoke(instance, new[] { element });

            if (!(element is TableInfo table)) return;
            table.ChildTableInfos = new List<ChildTableInfo>();
            table.ReferenceTableInfos = new List<ReferenceTableInfo>();

            Tables.Add((TableInfo)element);
        }

        /// <summary>
        /// 从字典中获取指定元素类型的集合类型
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        private Type GetLisTypeFromDictionary(Type elementType) => !_listTypeDictionary.ContainsKey(elementType) ? null : _listTypeDictionary[elementType];

        /// <summary>
        /// 构建表之间的关联关系
        /// </summary>
        private void StructureReference()
        {
            foreach (var model in Models)
            {
                foreach (var reference in model.ReferenceInfos)
                {
                    var joinInfo = reference.ReferenceJoinInfos.FirstOrDefault();
                    if (joinInfo?.FirstColumn == null || joinInfo.SecondColumn == null) continue;

                    var parentTable = Tables.FirstOrDefault(t => t.Id == reference.ParentTable.Ref);
                    var childTable = Tables.FirstOrDefault(t => t.Id == reference.ChildTable.Ref);
                    if (parentTable == null || childTable == null) continue;

                    var referenceKey =
                        parentTable.ColumnInfos.FirstOrDefault(t => t.Id == joinInfo.FirstColumn.Ref ||
                                                                    t.Id == joinInfo.SecondColumn.Ref);

                    var foreignKey =
                        childTable.ColumnInfos.FirstOrDefault(t => t.Id == joinInfo.FirstColumn.Ref ||
                                                                    t.Id == joinInfo.SecondColumn.Ref);

                    if (referenceKey == null || foreignKey == null) continue;
                    if (referenceKey == foreignKey) continue;

                    parentTable.ChildTableInfos.Add(new ChildTableInfo
                    {
                        ForeignKey = foreignKey,
                        ChildTable = childTable,
                        ChildPropertyName = GetChildPropertyName(parentTable, childTable, foreignKey)
                    });

                    childTable.ReferenceTableInfos.Add(new ReferenceTableInfo
                    {
                        ForeignKey = foreignKey,
                        ReferenceKey = referenceKey,
                        ParentTable = parentTable,
                        ParentPropertyName = GetParentPropertyName(parentTable, foreignKey)
                    });
                }
            }
        }

        private string GetChildPropertyName(TableInfo parentTable, TableInfo childTable, ColumnInfo foreignKey)
        {
            var propertyName = childTable.TableName;
            var foreignKeyCode = foreignKey.Code.Substring(0, foreignKey.Code.Length - 2);
            if (foreignKeyCode != parentTable.TableName)
                propertyName = foreignKeyCode + propertyName;

            return propertyName;
        }

        private string GetParentPropertyName(TableInfo parentTable, ColumnInfo foreignKey)
        {
            var propertyName = foreignKey.Code.Substring(0, foreignKey.Code.Length - 2);

            if (propertyName.EndsWith(parentTable.TableName) || propertyName.EndsWith(parentTable.TableName.Replace("Zt", "")))
                return propertyName;

            if (propertyName != parentTable.TableName)
                propertyName += parentTable.TableName;

            return propertyName;
        }
    }
}