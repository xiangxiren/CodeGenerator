using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CodeGenerator.AutoComplete;

namespace CodeGenerator.Form
{
    /// <summary>
    /// SearchTableWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchTableWindow
    {
        public string SelectedNodeId { get; set; }

        public SearchTableWindow(List<AutoCompleteEntry> autoCompleteEntries)
        {
            InitializeComponent();

            if (autoCompleteEntries != null)
                autoCompleteEntries.ForEach(o => TxtSearch.AddItem(o));

            TxtSearch.SelecteEvent += TxtSearch_SelecteEvent;
        }

        private void TxtSearch_SelecteEvent(string id)
        {
            SelectedNodeId = id;

            DialogResult = true;
            Close();
        }

        private void SearchTableWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
                Close();
        }

        private void SearchTableWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtSearch.TextBoxFocus();
        }
    }
}
