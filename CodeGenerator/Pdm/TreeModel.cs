using System.Collections.Generic;
using System.ComponentModel;

namespace CodeGenerator.Pdm
{
    public class TreeModel : INotifyPropertyChanged
    {
        #region 属性、字段

        private bool _isExpanded;

        private bool _isSelected;

        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public NodeType NodeType { get; set; }

        public TreeModel Parent { get; set; }

        public List<TreeModel> Children { get; set; }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value == _isExpanded) return;
                _isExpanded = value;
                NotifyPropertyChanged("IsExpanded");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        #endregion

        public TreeModel()
        {
            Children = new List<TreeModel>();
            IsExpanded = false;
            Icon = "/Image/package.png";
        }

        public void SetChildrenExpanded(bool isExpanded)
        {
            foreach (TreeModel child in Children)
            {
                if (child.NodeType == NodeType.Table) continue;
                child.IsExpanded = isExpanded;
                child.SetChildrenExpanded(isExpanded);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public enum NodeType
    {
        Model,
        Package,
        Table
    }
}