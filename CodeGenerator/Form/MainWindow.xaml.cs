using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CodeGenerator.Operate;
using CodeGenerator.Pdm;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace CodeGenerator.Form
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
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
            ColumnDataGrid.DataContext = XmlNodeOperate.Context.GetColumnDataGridDataSource(tableId);
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

            TreeModelOperate.Context.Init(pdmReader.Models);
            XmlNodeOperate.Init(pdmReader.Tables);

            TwLeaf.ItemsSource = TreeModelOperate.Context.TreeModels;
            var tableInfos = XmlNodeOperate.Context.TableInfos;
            if (tableInfos != null && tableInfos.Count > 0)
            {
                var firstOrDefault = tableInfos.FirstOrDefault();
                if (firstOrDefault != null) BindColumnDataGrid(firstOrDefault.Id);
            }
        }

        #endregion

        #region 事件交互

        private void MenuExpandAll_Click(object sender, RoutedEventArgs e)
        {
            TreeModelOperate.Context.SetAllNodeIsExpanded(true);
        }

        private void MenuUnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            TreeModelOperate.Context.SetAllNodeIsExpanded(false);
        }

        private void TwLeaf_OnSelected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TreeViewItem item)
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

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if (Keyboard.IsKeyDown(Key.O))
                    OpenFile();
                else if (Keyboard.IsKeyDown(Key.S))
                    GenerateSingleTable();
                else if (Keyboard.IsKeyDown(Key.B))
                    GenerateBatchGenerate();
                else if (Keyboard.IsKeyDown(Key.F))
                    FindTreeNode();
            }
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

            var tables = new GenerateOperate(treeModel, XmlNodeOperate.Context.TableInfos).GetGenerateTables();

            var progressWindow = new ProgressWindow(tables, window.GenerateArgument) { Owner = this };
            progressWindow.ShowDialog();
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

        /// <summary>
        /// 查找表
        /// </summary>
        private void FindTreeNode()
        {
            var window = new SearchTableWindow(TreeModelOperate.Context.AutoCompleteEntries)
            {
                Owner = this
            };
            var result = window.ShowDialog();
            if (result.HasValue && result.Value)
            {
                TreeModelOperate.Context.SetAllNodeIsExpanded(false);
                TreeModelOperate.Context.ExpandTreeNode(window.SelectedNodeId);
            }
        }

        #endregion
    }
}