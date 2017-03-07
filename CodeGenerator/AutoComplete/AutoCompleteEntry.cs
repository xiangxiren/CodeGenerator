namespace CodeGenerator.AutoComplete
{
    public class AutoCompleteEntry
    {
        private string[] _keywordStrings;

        public string[] KeywordStrings
        {
            get { return _keywordStrings ?? (_keywordStrings = new[] { DisplayName }); }
            private set { _keywordStrings = value; }
        }

        public string DisplayName { get; set; }
        public string Id { get; set; }

        public AutoCompleteEntry(string id, string name, params string[] keywords)
        {
            Id = id;
            DisplayName = name;
            KeywordStrings = keywords;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
