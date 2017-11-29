using System;

namespace CodeGenerator.Pdm
{
    /// <summary>
    /// 指定属性值从节点属性中获取，如果不指定此属性，将从与属性同名的子节点中获取
    /// </summary>
    public class NodeAttributeAttribute : Attribute
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string AttributeName { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>与节点属性名相同</remarks>
        public NodeAttributeAttribute()
            :this(string.Empty)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="attributeName">属性名</param>
        public NodeAttributeAttribute(string attributeName)
        {
            AttributeName = attributeName;
        }
    }

    /// <summary>
    /// 指定属性值从某一子节点中获取，如果不指定此属性，将从与属性同名的子节点中获取
    /// </summary>
    public class NodeChildAttribute : Attribute
    {
        /// <summary>
        /// 子节点名称
        /// </summary>
        public string ChildNodeName { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="childNodeName">子节点名称</param>
        public NodeChildAttribute(string childNodeName)
        {
            ChildNodeName = childNodeName;
        }
    }

    /// <summary>
    /// 非基元类型属性，指定赋值的子节点
    /// </summary>
    public class ChildObjectAttribute : Attribute
    {
        /// <summary>
        /// 子节点名称
        /// </summary>
        public string ChildNodeName { get; }

        /// <summary>
        /// 元素类型，属性为集合类型需要指定元素类型
        /// </summary>
        public Type ElementType { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>非集合类型属性使用此构造函数</remarks>
        /// <param name="childNodeName">子节点名称</param>
        public ChildObjectAttribute(string childNodeName)
            : this(childNodeName, null)
        {
            ChildNodeName = childNodeName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>非集合类型属性使用此构造函数</remarks>
        /// <param name="childNodeName">子节点名称</param>
        /// <param name="elementType">元素类型</param>
        public ChildObjectAttribute(string childNodeName, Type elementType)
        {
            ChildNodeName = childNodeName;
            ElementType = elementType;
        }
    }
}