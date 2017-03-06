using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CodeGenerator.AutoComplete;
using System.Timers;

namespace CodeGenerator.Form
{
    /// <summary>
    /// 自定义自动匹配文本框
    /// </summary>
    public partial class AutoCompleteTextBox
    {
        #region 成员变量

        private readonly VisualCollection _controls;
        private readonly TextBox _textBox;
        private readonly ComboBox _comboBox;
        private readonly ObservableCollection<AutoCompleteEntry> _autoCompletionList;
        private readonly Timer _keypressTimer;
        private delegate void TextChangedCallback();
        private bool _insertText;
        private int _delayTime;
        private int _searchThreshold;

        public event Action<string> SelecteEvent;

        #endregion 成员变量

        #region 构造函数

        public AutoCompleteTextBox()
        {
            _controls = new VisualCollection(this);
            //            InitializeComponent();

            _autoCompletionList = new ObservableCollection<AutoCompleteEntry>();
            _searchThreshold = 0;        // default threshold to 2 char
            _delayTime = 100;

            // set up the key press timer
            _keypressTimer = new Timer();
            _keypressTimer.Elapsed += OnTimedEvent;

            // set up the text box and the combo box
            _comboBox = new ComboBox
            {
                IsSynchronizedWithCurrentItem = true,
                IsTabStop = false
            };
            SetZIndex(_comboBox, -1);
            _comboBox.SelectionChanged += comboBox_SelectionChanged;

            _textBox = new TextBox();
            _textBox.TextChanged += textBox_TextChanged;
            _textBox.GotFocus += textBox_GotFocus;
            _textBox.KeyUp += textBox_KeyUp;
            _textBox.KeyDown += textBox_KeyDown;
            _textBox.VerticalContentAlignment = VerticalAlignment.Center;

            _controls.Add(_comboBox);
            _controls.Add(_textBox);
        }

        #endregion 构造函数

        #region 成员方法

        public string Text
        {
            get { return _textBox.Text; }
            set
            {
                _insertText = true;
                _textBox.Text = value;
            }
        }

        public int DelayTime
        {
            get { return _delayTime; }
            set { _delayTime = value; }
        }

        public int Threshold
        {
            get { return _searchThreshold; }
            set { _searchThreshold = value; }
        }

        /// <summary>
        /// 添加Item
        /// </summary>
        /// <param name="entry"></param>
        public void AddItem(AutoCompleteEntry entry)
        {
            _autoCompletionList.Add(entry);
        }

        /// <summary>
        /// 清空Item
        /// </summary>
        public void ClearItem()
        {
            _autoCompletionList.Clear();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_comboBox.SelectedItem != null)
            {
                //                _insertText = true;
                ComboBoxItemEx cbItem = (ComboBoxItemEx)_comboBox.SelectedItem;
                //                _textBox.Text = cbItem.Content.ToString();
                OnSelecteEvent(cbItem.Entry.Id);
            }
        }

        private void TextChanged()
        {
            try
            {
                _comboBox.Items.Clear();
                if (!string.IsNullOrEmpty(_textBox.Text) && _textBox.Text.Length >= _searchThreshold)
                {
                    foreach (AutoCompleteEntry entry in _autoCompletionList)
                    {
                        if (!entry.KeywordStrings.Any(word => word.Contains(_textBox.Text))) continue;

                        ComboBoxItemEx cbItem = new ComboBoxItemEx { Content = entry.ToString(), Entry = entry };
                        _comboBox.Items.Add(cbItem);
                    }
                    _comboBox.IsDropDownOpen = _comboBox.HasItems;
                }
                else
                {
                    _comboBox.IsDropDownOpen = false;
                }
            }
            catch
            {
                // ignored
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _keypressTimer.Stop();
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new TextChangedCallback(this.TextChanged));
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_insertText) _insertText = false;
            else
            {
                if (_delayTime > 0)
                {
                    _keypressTimer.Interval = _delayTime;
                    _keypressTimer.Start();
                }
                else TextChanged();
            }
        }

        //获得焦点时
        public void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_insertText) _insertText = false;
            else
            {
                if (_delayTime > 0)
                {
                    _keypressTimer.Interval = _delayTime;
                    _keypressTimer.Start();
                }
                else TextChanged();
            }
        }

        public void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (_textBox.IsInputMethodEnabled)
            {
                _comboBox.IsDropDownOpen = false;
            }
        }

        /// <summary>
        /// 按向下按键时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && _comboBox.IsDropDownOpen)
            {
                _comboBox.Focus();
            }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            _textBox.Arrange(new Rect(arrangeSize));
            _comboBox.Arrange(new Rect(arrangeSize));
            return base.ArrangeOverride(arrangeSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _controls[index];
        }

        protected override int VisualChildrenCount
        {
            get { return _controls.Count; }
        }
        protected virtual void OnSelecteEvent(string id)
        {
            var handler = SelecteEvent;
            if (handler != null) handler(id);
        }

        #endregion 成员方法
    }
}
