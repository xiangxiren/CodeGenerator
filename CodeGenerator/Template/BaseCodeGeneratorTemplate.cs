public class <%=TemplateName >GenerateCode : GenerateCodeBase, ICodeGenerator
{
    protected override string FileName
    {
        get { return "<%=FileName >"; }
    }

    public void Generate(TableInfo tableInfo, string classNamespace)
    {
        var formatTableName = table.GetFormatTableName();
        var codeBuilder = new StringBuilder();
        <%=ChildTemplate>
        return codeBuilder.ToString();
    }
}