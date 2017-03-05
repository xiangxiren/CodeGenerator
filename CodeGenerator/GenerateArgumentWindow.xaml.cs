using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CodeGenerator.Generate;
using CodeGenerator.Pdm;

namespace CodeGenerator
{
    /// <summary>
    ///     EahouseListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GenerateArgumentWindow
    {
        #region 常量、属性

        private const string GenerateArgumentConfigPath = "GenerateArgument.ini";
        public List<GenerateArgument> GenerateArguments { get; set; }

        #endregion

        #region 构造函数

        public GenerateArgumentWindow()
        {
            InitializeComponent();

            GenerateArguments = new List<GenerateArgument>();
            ReadGenerateArgumentFile();
        }

        #endregion

        #region 事件交互

        private void BtnFilePathSelect_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)e.OriginalSource;
            if (button == null) return;

            switch (button.Name)
            {
                case "BtnEntityFilePath":
                    SelectFileSavePath(TxtEntityFilePath);
                    break;
                case "BtnWebModelFilePath":
                    SelectFileSavePath(TxtWebModelFilePath);
                    break;
                case "BtnRepositoryFilePath":
                    SelectFileSavePath(TxtRepositoryFilePath);
                    break;
                case "BtnBlFilePath":
                    SelectFileSavePath(TxtBlFilePath);
                    break;
            }
        }

        private void SelectFileSavePath(TextBox filePathTextBox)
        {
            if (filePathTextBox == null) return;

            var mDialog = new System.Windows.Forms.FolderBrowserDialog();
            mDialog.SelectedPath = filePathTextBox.Text;
            var result = mDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel) return;

            filePathTextBox.Text = mDialog.SelectedPath.Trim();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!GetGenerateArgument(CbEntity, TxtEntityNamespace, TxtEntityFilePath, BtnEntityFilePath, GenerateType.Entity)) return;

            if (!GetGenerateArgument(CbWebModel, TxtWebModelNamespace, TxtWebModelFilePath, BtnWebModelFilePath, GenerateType.WebModel)) return;

            if (!GetGenerateArgument(CbRepository, TxtRepositoryNamespace, TxtRepositoryFilePath, BtnRepositoryFilePath, GenerateType.Repository)) return;

            if (!GetGenerateArgument(CbBl, TxtBlNamespace, TxtBlFilePath, BtnBlFilePath, GenerateType.Bl)) return;

            WriteGenerateArgumentFile();
            DialogResult = true;
            Close();
        }

        private bool GetGenerateArgument(CheckBox checkBox, TextBox namespaceTextBox, TextBox filePathTextBox, Button filePathButton, GenerateType generateType)
        {
            if (!checkBox.IsChecked.HasValue || !checkBox.IsChecked.Value) return true;

            if (string.IsNullOrEmpty(namespaceTextBox.Text))
            {
                MessageBox.Show(this, string.Format("如果要生成{0}，请输入命名空间", generateType), "提示");
                namespaceTextBox.Focus();

                return false;
            }
            if (string.IsNullOrEmpty(filePathTextBox.Text))
            {
                MessageBox.Show(this, string.Format("如果要生成{0}，请选择文件保存目录", generateType), "提示");
                filePathButton.Focus();

                return false;
            }

            GenerateArguments.Add(new GenerateArgument
            {
                GenerateType = generateType,
                ClassNamespace = namespaceTextBox.Text,
                FileSavePath = filePathTextBox.Text
            });

            return true;
        }

        #endregion

        #region 文件读写

        private void WriteGenerateArgumentFile()
        {
            using (var fs = new FileStream(GenerateArgumentConfigPath, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    foreach (var argument in GenerateArguments)
                    {
                        sw.WriteLine("{0},{1},{2}", argument.GenerateType, argument.ClassNamespace, argument.FileSavePath);
                    }
                    sw.Flush();
                }
            }
        }

        private void ReadGenerateArgumentFile()
        {
            try
            {
                using (var sr = new StreamReader(GenerateArgumentConfigPath, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        if (arr.Length != 3) continue;

                        GenerateType type;
                        if (Enum.TryParse(arr[0], out type))
                        {
                            switch (type)
                            {
                                case GenerateType.Entity:
                                    SetValueFromConfig(CbEntity, TxtEntityNamespace, TxtEntityFilePath, arr);
                                    break;
                                case GenerateType.WebModel:
                                    SetValueFromConfig(CbWebModel, TxtWebModelNamespace, TxtWebModelFilePath, arr);
                                    break;
                                case GenerateType.Repository:
                                    SetValueFromConfig(CbRepository, TxtRepositoryNamespace, TxtRepositoryFilePath, arr);
                                    break;
                                case GenerateType.Bl:
                                    SetValueFromConfig(CbBl, TxtBlNamespace, TxtBlFilePath, arr);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(this, e);
            }
        }

        private void SetValueFromConfig(CheckBox checkBox, TextBox namespaceTextBox, TextBox filePathTextBox, string[] arguments)
        {
            checkBox.IsChecked = true;
            namespaceTextBox.Text = arguments[1];

            var filePath = arguments[2];
            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
            }
            catch (Exception e)
            {
                LogHelper.Error(this, e);
            }
            filePathTextBox.Text = filePath;
        }

        #endregion
    }
}