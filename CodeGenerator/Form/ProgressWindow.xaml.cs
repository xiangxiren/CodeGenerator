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
        private readonly IList<GenerateArgument> _generateArguments;

        private static readonly IList<ICodeGenerator> ICodeGenerators = new CodeGeneratorFactory().GetTemplateGenerateCodes();

        public ProgressWindow(IList<TableInfo> tableInfos, IList<GenerateArgument> generateArguments)
        {
            _tableInfos = tableInfos;
            _generateArguments = generateArguments;
            InitializeComponent();
        }

        private void ExecuteGenerate()
        {
            int num = 0;
            foreach (var info in _tableInfos)
            {
                foreach (var argument in _generateArguments)
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

                        foreach (var generator in ICodeGenerators)
                        {
                            var keyValuePair = generator.Generate(info, argument.ClassNamespace);
                        }


                    }
                    catch (Exception e)
                    {
                        LogHelper.Error(this, string.Format("表{0}生成{1}错误.{2}", info.Code, argument.GenerateType, e.Message));
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
