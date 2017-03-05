using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CodeGenerator.Generate;
using CodeGenerator.Pdm;

namespace CodeGenerator
{
    /// <summary>
    /// ProgressWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressWindow
    {
        private readonly IList<TableInfo> _tableInfos;
        private readonly IList<GenerateArgument> _generateArguments;

        private Thread _generateThread;

        public ProgressWindow(IList<TableInfo> tableInfos, IList<GenerateArgument> generateArguments)
        {
            _tableInfos = tableInfos;
            _generateArguments = generateArguments;
            InitializeComponent();
        }

        private void ExecuteGenerate(object obj)
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
                    }
                    catch (Exception e)
                    {
                        LogHelper.Error(this, string.Format("表{0}生成{1}错误.{2}", info.Code, argument.GenerateType, e.Message));
                    }
                }

                TxtProgress.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
                    new Action<long, long>(UpdateCopyProgress), _tableInfos.Count, ++num);
            }
        }

        private void UpdateCopyProgress(long fileLength, long currentLength)
        {
            //刷新进度条            
            GenerateProgress.Value = currentLength;
        }

        private void ProgressBarWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            GenerateProgress.Maximum = _tableInfos.Count;

            _generateThread = new Thread(ExecuteGenerate);
            _generateThread.Start();
        }

        private void ProgressBarWindow_OnClosed(object sender, EventArgs e)
        {
            try
            {
                if (_generateThread != null)
                    _generateThread.Abort();
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex);
            }
        }
    }
}
