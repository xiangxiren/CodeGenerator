namespace CodeGenerator.Generate
{
    public class GenerateArgument
    {
        /// <summary>
        /// 类命名空间
        /// </summary>
        public GenerateType GenerateType { get; set; }

        /// <summary>
        /// 类命名空间
        /// </summary>
        public string ClassNamespace { get; set; }

        /// <summary>
        /// 文件保存地址
        /// </summary>
        public string FileSavePath { get; set; }
    }

    public enum GenerateType
    {
        Entity,
        WebModel,
        Repository,
        Bl
    }
}