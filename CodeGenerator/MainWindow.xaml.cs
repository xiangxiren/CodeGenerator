using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CodeGenerator.Generate;
using CodeGenerator.Pdm;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace CodeGenerator
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private const string ImageTable = "Image/table.png";
        private const string ImagePackage = "Image/package.png";

        private List<TableInfo> _tableInfos;

        #region 构造函数

        public MainWindow()
        {
            InitializeComponent();

            ColumnDataGrid.LoadingRow += DataGridLoadingRow;
        }

        #endregion

        #region 绑定表格

        /// <summary>
        ///     绑定表格
        /// </summary>
        public void BindColumnDataGrid(string tableId)
        {
            var list = new List<ColumnInfo>();

            if (!string.IsNullOrEmpty(tableId) && _tableInfos != null)
            {
                var table = _tableInfos.FirstOrDefault(t => t.Id == tableId);
                if (table != null)
                {
                    list = table.ColumnInfos;

                    table.PrimaryKeys.ForEach(k =>
                    {
                        k.Columns.ForEach(c =>
                        {
                            c.PrimaryKey = true;
                        });
                    });
                }
            }

            ColumnDataGrid.DataContext = list;
        }

        #endregion

        #region 读取文件

        private void OpenFile()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "PDM文件|*.pdm",
                    FilterIndex = 1,
                    DefaultExt = "pdm",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() != true) return;
                var filepaths = openFileDialog.FileNames;
                if (filepaths.Length <= 0) return;

                ReadPdmFile(filepaths[0]);
            }
            catch (Exception ex)
            {
                LogHelper.Debug(this, ex);
                MessageBox.Show(this, ex.Message);
            }
        }

        private void ReadPdmFile(string filePath)
        {
            var pdmReader = new PdmReader(filePath);
            pdmReader.InitData();

            var roots = new List<TreeModel>();

            foreach (var model in pdmReader.Models)
            {
                var node = new TreeModel { Id = model.Id, Name = model.Name, NodeType = NodeType.Model, ImageSource = ImagePackage, IsExpanded = true };

                if (model.PackageInfos != null)
                    node.Children = GetNodeFromPackageInfo(model.PackageInfos);

                roots.Add(node);
            }

            TwLeaf.ItemsSource = roots;
            _tableInfos = pdmReader.Tables;
            if (_tableInfos != null && _tableInfos.Count > 0)
            {
                var firstOrDefault = _tableInfos.FirstOrDefault();
                if (firstOrDefault != null) BindColumnDataGrid(firstOrDefault.Id);
            }
        }

        private List<TreeModel> GetNodeFromPackageInfo(List<PackageInfo> packageInfos)
        {
            var nodes = new List<TreeModel>();
            if (packageInfos == null) return nodes;

            foreach (var package in packageInfos)
            {
                var node = new TreeModel { Id = package.Id, Name = package.Name, NodeType = NodeType.Package, ImageSource = ImagePackage, Children = new List<TreeModel>() };
                if (package.PackageInfos != null)
                    node.Children.AddRange(GetNodeFromPackageInfo(package.PackageInfos));

                if (package.TableInfos != null)
                    node.Children.AddRange(
                        package.TableInfos.Select(
                            table =>
                                new TreeModel
                                {
                                    Id = table.Id,
                                    Name = string.Format("{0}({1})", table.Code, table.Name),
                                    NodeType = NodeType.Table,
                                    ImageSource = ImageTable
                                }));

                nodes.Add(node);
            }

            return nodes;
        }

        #endregion

        #region 事件交互

        private void TwLeaf_OnSelected(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            if (item != null)
            {
                item.IsExpanded = true;

                var treeModel = (TreeModel)item.DataContext;
                if (treeModel != null && treeModel.NodeType == NodeType.Table)
                {
                    BindColumnDataGrid(treeModel.Id);
                }
            }
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)e.OriginalSource;
            switch (menuItem.Name)
            {
                case "MenuOpenFile":
                    OpenFile();
                    break;
                case "MenuSingleTable":
                    GenerateSingleTable();
                    break;
                case "MenuBatchGenerate":
                    GenerateBatchGenerate();
                    break;
            }
        }

        private void GenerateSingleTable()
        {
            var selectedItem = (TreeModel)TwLeaf.SelectedItem;
            if (selectedItem == null || selectedItem.NodeType != NodeType.Table)
            {
                MessageBox.Show(this, "请选择需要生成的表", "提示");
                return;
            }

            GenerateCode(selectedItem);
        }

        private void GenerateBatchGenerate()
        {
            var selectedItem = (TreeModel)TwLeaf.SelectedItem;
            if (selectedItem == null || selectedItem.NodeType == NodeType.Table)
            {
                MessageBox.Show(this, "请选择需要生成的包", "提示");
                return;
            }

            GenerateCode(selectedItem);
        }

        #endregion

        #region 其他方法

        /// <summary>
        ///     行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void GenerateCode(TreeModel treeModel)
        {
            var window = new GenerateArgumentWindow
            {
                Owner = this
            };

            var result = window.ShowDialog();
            if (!result.HasValue || !result.Value) return;

            var tables = new List<TableInfo>();
            if (treeModel == null) return;

            if (treeModel.NodeType == NodeType.Table)
            {
                var table = _tableInfos.FirstOrDefault(t => t.Id == treeModel.Id && t.ColumnInfos != null && t.ColumnInfos.Count > 0);

                if (table != null)
                    tables.Add(table);
            }
            else if (treeModel.NodeType == NodeType.Package)
            {
                tables.AddRange(GetTableInfosFromPackage(treeModel));
            }
            else
            {
                if (treeModel.Children == null) return;

                foreach (var child in treeModel.Children)
                {
                    tables.AddRange(GetTableInfosFromPackage(child));
                }
            }

            foreach (var info in tables)
            {
                foreach (var argument in window.GenerateArguments)
                {
                    try
                    {
                        switch (argument.GenerateType)
                        {
                            case GenerateType.Entity:
                                new EntityGenerateCode().Generate(info, argument.ClassNamespace, argument.FileSavePath);
                                break;
                            case GenerateType.WebModel:
                                new WebModelGenerateCode().Generate(info, argument.ClassNamespace, argument.FileSavePath);
                                break;
                            case GenerateType.Repository:
                                new RepositoryGenerateCode().Generate(info, argument.ClassNamespace, argument.FileSavePath);
                                break;
                            case GenerateType.Bl:
                                new BlGenerateCode().Generate(info, argument.ClassNamespace, argument.FileSavePath);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        LogHelper.Error(this, string.Format("表{0}生成{1}错误.{2}", info.Code, argument.GenerateType, e.Message));
                    }
                }
            }

            MessageBox.Show(this, "生成完成", "提示");

            //            var progressWindow = new ProgressWindow(tables, window.GenerateArguments) { Owner = this };
            //            progressWindow.ShowDialog();
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

        #endregion
    }
}