using System.IO;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
    public class BlGenerateCode : IGenerateCode
    {
        public void Generate(TableInfo tableInfo, string classNamespace, string fileSavePath)
        {
            var formatTableName = tableInfo.GetFormatTableName();
            using (var fs = new FileStream(Path.Combine(fileSavePath, formatTableName) + "BL.cs", FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    #region using

                    sw.WriteLine("using System;");
                    sw.WriteLine("using System.Collections.Generic;");
                    sw.WriteLine("using System.Linq;");
                    sw.WriteLine("using System.Text;");
                    sw.WriteLine("using JG.Core;");
                    sw.WriteLine("using Scm.Component.Common;");
                    sw.WriteLine("using Scm.Component.SecurityModel;\r\n");

                    #endregion

                    sw.WriteLine("namespace {0}", classNamespace);
                    sw.WriteLine("{");
                    sw.WriteLine("    /// <summary>");
                    sw.WriteLine("    /// {0}BL", tableInfo.Comment);
                    sw.WriteLine("    /// </summary>");
                    sw.WriteLine("    public class {0}BL", formatTableName);
                    sw.WriteLine("    {");
                    sw.WriteLine("        private I{0}Repository m{0}Repository = null;", formatTableName);
                    sw.WriteLine();
                    sw.WriteLine("        #region 构造函数");
                    sw.WriteLine();
                    sw.WriteLine("        public {0}BL()", formatTableName);
                    sw.WriteLine("            : this(new {0}Repository())", formatTableName);
                    sw.WriteLine("        {");
                    sw.WriteLine();
                    sw.WriteLine("        }");
                    sw.WriteLine();
                    sw.WriteLine("        public {0}BL(I{0}Repository repository)", formatTableName);
                    sw.WriteLine("        {");
                    sw.WriteLine("            this.m{0}Repository = repository ?? new {0}Repository();", formatTableName);
                    sw.WriteLine("        }");
                    sw.WriteLine();
                    sw.WriteLine("        #endregion");
                    sw.WriteLine();
                    sw.WriteLine("        #region 业务逻辑");
                    sw.WriteLine();
                    sw.WriteLine("        public {0}Entity Get(object ID, bool isCopy = false)", formatTableName);
                    sw.WriteLine("        {");
                    sw.WriteLine("            var entity = this.m{0}Repository.Get(ID);", formatTableName);
                    sw.WriteLine("            return entity != null && isCopy ? DataProcess.CloneObject(entity) : entity;");
                    sw.WriteLine("        }");
                    sw.WriteLine();
                    sw.WriteLine("        public PageDataSet<{0}Entity> GetList(QueryModel queryModel, int pageSize, int pageIndex)", formatTableName);
                    sw.WriteLine("        {");
                    sw.WriteLine("            return this.m{0}Repository.GetList(queryModel, pageSize, pageIndex);", formatTableName);
                    sw.WriteLine("        }");
                    sw.WriteLine();
                    sw.WriteLine("        public {0}Entity Update({0}Entity entity)", formatTableName);
                    sw.WriteLine("        {");
                    sw.WriteLine("            entity.UpdateUserID = UserContext.CurrentUser.UserID;");
                    sw.WriteLine("            entity.UpdateDate = DateTimeUtil.GetNowDateTime();");
                    sw.WriteLine("            this.m{0}Repository.Update(entity);", formatTableName);
                    sw.WriteLine("            return entity;");
                    sw.WriteLine("        }");
                    sw.WriteLine();
                    sw.WriteLine("        #endregion");
                    sw.WriteLine("    }");
                    sw.Write("}");
                    sw.Flush();
                }
            }
        }
    }
}