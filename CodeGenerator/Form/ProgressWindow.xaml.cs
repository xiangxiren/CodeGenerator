using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CodeGenerator.Generate;
using CodeGenerator.Pdm;

namespace CodeGenerator.Form
{
	/// <summary>
	/// ProgressWindow.xaml 的交互逻辑
	/// </summary>
	public partial class ProgressWindow
	{
		private readonly IList<TableInfo> _tableInfos;
		private readonly GenerateArgument _generateArgument;
		private static readonly Dictionary<string, ICodeGenerator> CodeGenerators;

		static ProgressWindow()
		{
			try
			{
				CodeGenerators = CodeGeneratorFactory.GetCodeGenerators("{\"MainTable\": \"User\", \"Author\": \"熊力伟\"}");
			}
			catch (Exception e)
			{
				LogHelper.Error(null, "模板读取错误", e);
			}
		}

		public ProgressWindow(IList<TableInfo> tableInfos, GenerateArgument generateArgument)
		{
			_tableInfos = tableInfos;
			_generateArgument = generateArgument;
			InitializeComponent();
		}

		private void ExecuteGenerate()
		{
			var num = 0;

			if (!string.IsNullOrEmpty(_generateArgument.ContextName))
				new ContextGenerateCode().Generate(_tableInfos, _generateArgument);
			foreach (var info in _tableInfos)
			{
				foreach (var argument in _generateArgument.ArgumentInfos)
				{
					try
					{
						argument.GenerateArgument = _generateArgument;

						switch (argument.GenerateType)
						{
							case GenerateType.Entity:
								new EntityGenerateCode().Generate(info, argument);
								break;
							case GenerateType.Map:
								new MapGenerateCode().Generate(info, argument);
								break;
							case GenerateType.Repository:
								new RepositoryGenerateCode().Generate(info, argument);
								break;
							case GenerateType.Bl:
								new BlGenerateCode().Generate(info, argument);
								break;
						}

						//                        foreach (var generator in CodeGenerators)
						//                        {
						//                            var keyValuePair = generator.Value.Generate(info, argument.ClassNamespace);
						//                        }
					}
					catch (Exception e)
					{
						LogHelper.Error(this, $"表{info.Code}生成{argument.GenerateType}错误.{e.Message}");
					}
				}

				GenerateProgress.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
					new Action<long, long>(UpdateCopyProgress), _tableInfos.Count, ++num);
			}

			Thread.Sleep(500);
			GenerateProgress.Dispatcher.Invoke(Close);
		}

		private void UpdateCopyProgress(long fileLength, long currentLength)
		{
			//刷新进度条            
			GenerateProgress.Value = currentLength;
		}

		private void ProgressBarWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			GenerateProgress.Maximum = _tableInfos.Count;

			Task.Factory.StartNew(ExecuteGenerate);
		}
	}
}
