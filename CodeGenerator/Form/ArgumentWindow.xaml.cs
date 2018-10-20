using System;
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

			GenerateArgument = new GenerateArgument { ProjectName = string.Empty, FilePath = string.Empty };
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
			if (string.IsNullOrEmpty(TxtProjectName.Text))
			{
				MessageBox.Show(this, "请输入项目名称", "提示");
				TxtProjectName.Focus();

				return;
			}

			if (string.IsNullOrEmpty(TxtFilePath.Text))
			{
				MessageBox.Show(this, "请选择文件存放根路径", "提示");
				BtnFilePath.Focus();

				return;
			}

			GenerateArgument.ProjectName = TxtProjectName.Text;
			GenerateArgument.FilePath = TxtFilePath.Text;

			WriteGenerateArgumentFile();
			DialogResult = true;
			Close();
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

		private void TxtProjectName_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = (TextBox)sender;
			if (sender == null) return;

			TxtEntityNamespace.Text = $"{textBox.Text}.{GenerateArgument.EntityNamespaceTemp}";
			TxtConfigNamespace.Text = $"{textBox.Text}.{GenerateArgument.ConfigNamespaceTemp}";
			TxtDtoNamespace.Text = $"{textBox.Text}.{GenerateArgument.DtoNamespaceTemp}";
			TxtQueryParamNamespace.Text = $"{textBox.Text}.{GenerateArgument.QueryParamNamespaceTemp}";
			TxtInterfaceNamespace.Text = $"{textBox.Text}.{GenerateArgument.InterfaceNamespaceTemp}";
			TxtImplementNamespace.Text = $"{textBox.Text}.{GenerateArgument.ImplementNamespaceTemp}";
			TxtControllerNamespace.Text = $"{textBox.Text}.{GenerateArgument.ControllerNamespaceTemp}";
		}

		private void TxtFilePath_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = (TextBox)sender;
			if (sender == null) return;

			TxtEntityFilePath.Text = Path.Combine(textBox.Text, GenerateArgument.EntityFilePath);
			TxtConfigFilePath.Text = Path.Combine(textBox.Text, GenerateArgument.ConfigFilePath);
			TxtDtoFilePath.Text = Path.Combine(textBox.Text, GenerateArgument.DtoFilePath);
			TxtQueryParamFilePath.Text = Path.Combine(textBox.Text, GenerateArgument.QueryParamFilePath);
			TxtInterfaceFilePath.Text = Path.Combine(textBox.Text, GenerateArgument.InterfaceFilePath);
			TxtImplementFilePath.Text = Path.Combine(textBox.Text, GenerateArgument.ImplementFilePath);
			TxtControllerFilePath.Text = Path.Combine(textBox.Text, GenerateArgument.ControllerFilePath);
			TxtViewFilePath.Text = Path.Combine(textBox.Text, GenerateArgument.ViewFilePath);
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

				TxtProjectName.Text = generateArgument.ProjectName;
				TxtFilePath.Text = generateArgument.FilePath;
			}
			catch (Exception e)
			{
				LogHelper.Error(this, e);
			}
		}

		#endregion
	}
}