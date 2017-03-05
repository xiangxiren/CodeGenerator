using System.Collections.Generic;

namespace CodeGenerator.Pdm
{
    public class TreeModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<TreeModel> Children { get; set; }

        public string ImageSource { get; set; }

        public NodeType NodeType { get; set; }

        public bool IsExpanded { get; set; }
    }

    public enum NodeType
    {
        Model,
        Package,
        Table
    }
}