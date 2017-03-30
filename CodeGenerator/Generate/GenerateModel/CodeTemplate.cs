namespace CodeGenerator.Generate.GenerateModel
{
    /// <summary>
    /// 代码生成设置
    /// </summary>
    public class CodeTemplate
    {
        /// <summary>
        /// 代码语言
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 生成文件后缀
        /// </summary>
        public string FileExtension
        {
            get
            {
                switch (Language.ToUpper())
                {
                    case "C#":
                        return ".cs";
                    case "JAVASCRIPT":
                        return ".js";
                    default:
                        return ".txt";
                }
            }
        }
    }

    /// <summary>
    /// 需要引用的程序集
    /// </summary>
    public class Assembly
    {
        public string Name { get; set; }

        /// <summary>
        /// .dll或者.exe
        /// </summary>
        public string Extension { get; set; }
    }

    /// <summary>
    /// 需要引用的命名空间
    /// </summary>
    public class Import
    {
        public string Namespace { get; set; }
    }

    public class Property
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Default { get; set; }

        public string Optional { get; set; }
    }

}
