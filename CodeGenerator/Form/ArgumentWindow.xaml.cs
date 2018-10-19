using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
	public partial class ArgumentWindow
	{
		#region 常量、属性

		private const string GenerateArgumentConfigPath = "GenerateArgument.ini";
		public GenerateArgument GenerateArgument { get; set; }

		#endregion

		#region 构造函数

		public ArgumentWindow()
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

			var textBox = GeTextBox(button);

			SelectFileSavePath(textBox);
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
			var fieldInfos = GetFieldInfos();

			bool flag = false;

			foreach (GenerateType type in Enum.GetValues(typeof(GenerateType)))
			{
				var checkboxValue = GetCheckBox(fieldInfos, type);
				if (checkboxValue == null) continue;

				var txtNamespaceValue = GetNamespaceTextBox(fieldInfos, type);
				if (txtNamespaceValue == null) continue;

				var txtFilePathValue = GetFilePathTextBox(fieldInfos, type);
				if (txtFilePathValue == null) continue;

				var btnFilePathValue = GetFilePathButton(fieldInfos, type);
				if (btnFilePathValue == null) continue;

				if (GetGenerateArgument(checkboxValue, txtNamespaceValue, txtFilePathValue, BtnEntityFilePath,
					type)) continue;

				flag = true;
				break;
			}

			if (flag) return;

			WriteGenerateArgumentFile();
			DialogResult = true;
			Close();
		}

		private FieldInfo[] GetFieldInfos()
		{
			return GetType().GetFields(BindingFlags.Instance |
															BindingFlags.Static |
															BindingFlags.Public |
															BindingFlags.NonPublic |
															BindingFlags.DeclaredOnly);
		}

		private bool GetGenerateArgument(CheckBox checkBox, TextBox namespaceTextBox, TextBox filePathTextBox, Button filePathButton, GenerateType generateType)
		{
			if (!checkBox.IsChecked.HasValue || !checkBox.IsChecked.Value) return true;

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

		private TextBox GeTextBox(Button button)
		{
			var name = button.Name.Replace("Btn", "");

			var fieldInfo = GetType().GetFields(BindingFlags.Instance |
											  BindingFlags.Static |
											  BindingFlags.Public |
											  BindingFlags.NonPublic |
											  BindingFlags.DeclaredOnly)
				.FirstOrDefault(t => t.Name == $"Txt{name}");

			if (fieldInfo == null) return null;

			return (TextBox)fieldInfo.GetValue(this);
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

				var fieldInfos = GetFieldInfos();

				foreach (var info in generateArgument.ArgumentInfos)
				{
					var checkBox = GetCheckBox(fieldInfos, info.GenerateType);
					if (checkBox == null) continue;

					var txtNamespace = GetNamespaceTextBox(fieldInfos, info.GenerateType);
					if (txtNamespace == null) continue;

					var txtFilePath = GetFilePathTextBox(fieldInfos, info.GenerateType);
					if (txtFilePath == null) continue;

					SetValueFromConfig(checkBox, txtNamespace, txtFilePath, info);
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

		#region 获取私有字段

		private CheckBox GetCheckBox(FieldInfo[] fieldInfos, GenerateType generateType)
		{
			var checkbox = fieldInfos.FirstOrDefault(t => t.Name == $"Cb{generateType.ToString()}");
			if (checkbox == null)
				return null;

			return (CheckBox)checkbox.GetValue(this);
		}

		private TextBox GetNamespaceTextBox(FieldInfo[] fieldInfos, GenerateType generateType)
		{
			var txtNamespace = fieldInfos.FirstOrDefault(t => t.Name == $"Txt{generateType.ToString()}Namespace");
			if (txtNamespace == null) return null;

			return (TextBox)txtNamespace.GetValue(this);
		}

		private TextBox GetFilePathTextBox(FieldInfo[] fieldInfos, GenerateType generateType)
		{
			var txtFilePath = fieldInfos.FirstOrDefault(t => t.Name == $"Txt{generateType.ToString()}FilePath");
			if (txtFilePath == null) return null;

			return (TextBox)txtFilePath.GetValue(this);
		}

		private Button GetFilePathButton(FieldInfo[] fieldInfos, GenerateType generateType)
		{
			var btnFilePath = fieldInfos.FirstOrDefault(t => t.Name == $"Btn{generateType.ToString()}FilePath");
			if (btnFilePath == null) return null;

			return (Button)btnFilePath.GetValue(this);
		}

		#endregion
	}
}