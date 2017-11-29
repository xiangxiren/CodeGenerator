using System;
using System.Collections.Generic;
using System.IO;

namespace CodeGenerator.Generate
{
    public abstract class GenerateCodeBase
    {
        protected virtual string FileName => throw new NotImplementedException();

        /// <summary>
        /// 获取文件生成的完整路径（包括文件名、扩展名）
        /// </summary>
        /// <param name="formatTableName"></param>
        /// <param name="fileSavePath"></param>
        /// <returns></returns>
        protected virtual string GetFullFilePath(string formatTableName, string fileSavePath)
        {
            EnsurePathExist(fileSavePath);
            return Path.Combine(fileSavePath, formatTableName + FileName);
        }

        /// <summary>
        /// 确保目录存在
        /// </summary>
        /// <param name="fileSavePath"></param>
        private void EnsurePathExist(string fileSavePath)
        {
            if (!Directory.Exists(fileSavePath))
            {
                Directory.CreateDirectory(fileSavePath);
            }
        }

        protected string GetListPropertyName(string name)
        {
            var endChar = name.Substring(name.Length - 1, 1);

            string replace = endChar;
            switch (endChar)
            {
                case "y":
                    replace = "ies";
                    break;
                case "o":
                case "s":
                    replace = endChar + "es";
                    break;
            }

            return name.Substring(0, name.Length - 1) + replace;
        }
    }
}