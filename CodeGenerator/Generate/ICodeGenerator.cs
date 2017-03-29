using System.Collections.Generic;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
    public interface ICodeGenerator
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="tableInfo">需要生成代码的表信息</param>
        /// <param name="classNamespace">命名空间</param>
        /// <returns>key表示文件名,value表示生成的代码文本</returns>
        KeyValuePair<string, string> Generate(TableInfo tableInfo, string classNamespace);
    }
}