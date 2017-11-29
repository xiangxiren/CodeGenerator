using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
    public interface IGenerateCode
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="table">需要生成代码的表信息</param>
        /// <param name="classNamespace">命名空间</param>
        /// <param name="fileSavePath">文件保存路径</param>
        void Generate(TableInfo table, string classNamespace, string fileSavePath);
    }
}
