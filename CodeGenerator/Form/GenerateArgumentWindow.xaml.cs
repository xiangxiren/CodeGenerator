using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CodeGenerator.Generate;
using CodeGenerator.Pdm;
using Newtonsoft.Json;

namespace CodeGenerator.Form
{
    /// <summary>
    ///     EahouseListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GenerateArgumentWindow
    {
        #region 常量、属性

        private const string GenerateArgumentConfigPath = "GenerateArgument.ini";
        public GenerateArgument GenerateArgument { get; set; }

        #endregion

        #region 构造函数

        public GenerateArgumentWindow()
        {
            InitializeComponent();

            GenerateArgument = new GenerateArgument { ArgumentInfos = new List<ArgumentInfo>() };
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
                    SelectFileSavePath(TxtMapFilePath);
                    break;
                    //case "BtnRepositoryFilePath":
                    //    SelectFileSavePath(TxtRepositoryFilePath);
                    //    break;
                    //case "BtnBlFilePath":
                    //    SelectFileSavePath(TxtBlFilePath);
                    //    break;
            }
        }

        private void SelectFileSavePath(TextBox filePathTextBox)
        {
            if (filePathTextBox == null) return;

            var mDialog = new System.Windows.Forms.FolderBrowserDialog { SelectedPath = filePathTextBox.Text };
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
            if (CbContext.IsChecked.HasValue && CbContext.IsChecked.Value)
            {
                if (string.IsNullOrEmpty(TxtContextName.Text))
                {
                    MessageBox.Show(this, "如果要生成上下文，请输入上下文名称", "提示");
                    TxtContextName.Focus();

                    return;
                }
                GenerateArgument.ContextName = TxtContextName.Text;
            }

            if (GetGenerateArgument(CbEntity, TxtEntityNamespace, TxtEntityFilePath, BtnEntityFilePath, GenerateType.Entity))
            {
                var entityInfo = GenerateArgument.ArgumentInfos.First(t => t.GenerateType == GenerateType.Entity);
                GenerateArgument.ContextFileSavePath = entityInfo.FileSavePath;
                GenerateArgument.ContextNamespace = entityInfo.ClassNamespace;
            }
            else
            {
                if (CbContext.IsChecked.HasValue && CbContext.IsChecked.Value)
                {
                    if (string.IsNullOrEmpty(TxtContextName.Text))
                    {
                        MessageBox.Show(this, "如果要生成上下文，必须选择生成实体和Map", "提示");
                        TxtContextName.Focus();

                        return;
                    }
                }
            }


            if (GetGenerateArgument(CbWebModel, TxtMapNamespace, TxtMapFilePath, BtnMapFilePath, GenerateType.Map))
            {
                var mapInfo = GenerateArgument.ArgumentInfos.First(t => t.GenerateType == GenerateType.Map);
                GenerateArgument.MapNamespace = mapInfo.ClassNamespace;
            }
            else
            {
                if (CbContext.IsChecked.HasValue && CbContext.IsChecked.Value)
                {
                    if (string.IsNullOrEmpty(TxtContextName.Text))
                    {
                        MessageBox.Show(this, "如果要生成上下文，必须选择生成实体和Map", "提示");
                        TxtContextName.Focus();

                        return;
                    }
                }
            }

            //if (!GetGenerateArgument(CbRepository, TxtRepositoryNamespace, TxtRepositoryFilePath, BtnRepositoryFilePath, GenerateType.Repository)) return;

            //if (!GetGenerateArgument(CbBl, TxtBlNamespace, TxtBlFilePath, BtnBlFilePath, GenerateType.Bl)) return;

            WriteGenerateArgumentFile();
            DialogResult = true;
            Close();
        }

        private bool GetGenerateArgument(CheckBox checkBox, TextBox namespaceTextBox, TextBox filePathTextBox, Button filePathButton, GenerateType generateType)
        {
            if (!checkBox.IsChecked.HasValue || !checkBox.IsChecked.Value) return false;

            if (string.IsNullOrEmpty(namespaceTextBox.Text))
            {
                MessageBox.Show(this, $"如果要生成{generateType}，请输入命名空间", "提示");
                namespaceTextBox.Focus();

                return false;
            }
            if (string.IsNullOrEmpty(filePathTextBox.Text))
            {
                MessageBox.Show(this, $"如果要生成{generateType}，请选择文件保存目录", "提示");
                filePathButton.Focus();

                return false;
            }

            GenerateArgument.ArgumentInfos.Add(new ArgumentInfo
            {
                GenerateType = generateType,
                ClassNamespace = namespaceTextBox.Text,
                FileSavePath = filePathTextBox.Text
            });

            return true;
        }

        private void GenerateArgumentWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
                Close();

            if (Keyboard.IsKeyDown(Key.F5))
                BtnSave_OnClick(null, null);
        }

        #endregion

        #region 文件读写

        private void WriteGenerateArgumentFile()
        {
            using (var fs = new FileStream(GenerateArgumentConfigPath, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(JsonConvert.SerializeObject(GenerateArgument));
                sw.Flush();
            }
        }

        private void ReadGenerateArgumentFile()
        {
            try
            {
                string argument;

                using (var sr = new StreamReader(GenerateArgumentConfigPath, Encoding.Default))
                {
                    argument = sr.ReadToEnd();
                }

                if (string.IsNullOrEmpty(argument)) return;

                var generateArgument = JsonConvert.DeserializeObject<GenerateArgument>(argument);
                if (!string.IsNullOrEmpty(generateArgument.ContextName))
                {
                    CbContext.IsChecked = true;
                    TxtContextName.Text = generateArgument.ContextName;
                }
                foreach (var info in generateArgument.ArgumentInfos)
                {
                    switch (info.GenerateType)
                    {
                        case GenerateType.Entity:
                            SetValueFromConfig(CbEntity, TxtEntityNamespace, TxtEntityFilePath, info);
                            break;
                        case GenerateType.Map:
                            SetValueFromConfig(CbWebModel, TxtMapNamespace, TxtMapFilePath, info);
                            break;
                            //case GenerateType.Repository:
                            //    SetValueFromConfig(CbRepository, TxtRepositoryNamespace, TxtRepositoryFilePath, arr);
                            //    break;
                            //case GenerateType.Bl:
                            //    SetValueFromConfig(CbBl, TxtBlNamespace, TxtBlFilePath, arr);
                            //    break;
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(this, e);
            }
        }

        private void SetValueFromConfig(CheckBox checkBox, TextBox namespaceTextBox, TextBox filePathTextBox, ArgumentInfo argumentInfo)
        {
            checkBox.IsChecked = true;
            namespaceTextBox.Text = argumentInfo.ClassNamespace;

            var filePath = argumentInfo.FileSavePath;
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