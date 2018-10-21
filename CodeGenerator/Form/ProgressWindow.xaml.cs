using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
				//				CodeGenerators = CodeGeneratorFactory.GetCodeGenerators("{\"MainTable\": \"User\", \"Author\": \"熊力伟\"}");
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

			foreach (var info in _tableInfos)
			{
				foreach (GenerateType type in Enum.GetValues(typeof(GenerateType)))
				{
					try
					{
						var generateCodes = GetGenerateCodes(type);

						generateCodes.ForEach(t => t.Generate(info, _generateArgument));

						//                        foreach (var generator in CodeGenerators)
						//                        {
						//                            var keyValuePair = generator.Value.Generate(info, argument.ClassNamespace);
						//                        }
					}
					catch (Exception e)
					{
						LogHelper.Error(this, $"表{info.Code}生成{type}错误.{e.Message}");
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

		private List<IGenerateCode> GetGenerateCodes(GenerateType generateType)
		{
			var types = GetType().Assembly.GetTypes()
				.Where(t => typeof(IGenerateCode).IsAssignableFrom(t) && !t.IsAbstract);

			var list =
				(from type in types
				 let attr = type.GetCustomAttribute<CodeGeneratorAttribute>()
				 where attr != null && attr.GenerateType == generateType
				 select type)
				.ToList();

			return list.Select(t => (IGenerateCode)Activator.CreateInstance(t)).ToList();
		}
	}
}
