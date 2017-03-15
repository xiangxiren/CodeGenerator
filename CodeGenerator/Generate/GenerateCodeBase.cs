using System;
using System.Collections.Generic;
using System.IO;

namespace CodeGenerator.Generate
{
    public abstract class GenerateCodeBase
    {
        public static readonly List<string> IgnoreColumns = new List<string> { "CREATEUSERID", "CREATEDATE", "UPDATEUSERID", "UPDATEDATE", "CLIENTID", "STATUS", "UpdateUserName", "ORGID", "STAFFID" };

        protected virtual string FileName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 获取文件生成的完整路径（包括文件名、扩展名）
        /// </summary>
        /// <param name="formatTableName"></param>
        /// <param name="fileSavePath"></param>
        /// <returns></returns>
        protected virtual string GetFullFilePath(string formatTableName, string fileSavePath)
        {
            SurePathExist(fileSavePath);
            return Path.Combine(fileSavePath, formatTableName + FileName);
        }

        /// <summary>
        /// 确保目录存在
        /// </summary>
        /// <param name="fileSavePath"></param>
        private void SurePathExist(string fileSavePath)
        {
            if (!Directory.Exists(fileSavePath))
            {
                Directory.CreateDirectory(fileSavePath);
            }
        }
    }
}