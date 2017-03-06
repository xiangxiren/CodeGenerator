using System.IO;
using System.Linq;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
    public class WebModelGenerateCode : GenerateCodeBase, IGenerateCode
    {
        public void Generate(TableInfo tableInfo, string classNamespace, string fileSavePath)
        {
            var formatTableName = tableInfo.GetFormatTableName();
            using (var fs = new FileStream(Path.Combine(fileSavePath, formatTableName) + "WebModel.cs", FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    #region using

                    sw.WriteLine("using System;");
                    sw.WriteLine("using System.Collections.Generic;");
                    sw.WriteLine("using System.Linq;");
                    sw.WriteLine("using System.ComponentModel.DataAnnotations;");
                    sw.WriteLine("using Scm.Component.Common;");
                    sw.WriteLine("using JG.Core;");
                    sw.WriteLine("using Newtonsoft.Json;\r\n");

                    #endregion

                    sw.WriteLine("namespace {0}", classNamespace);
                    sw.WriteLine("{");

                    #region WebModelClass

                    sw.WriteLine("    [QueryTable(\"{0}\")]", tableInfo.Code);
                    sw.WriteLine("    public class {0}WebModel : BaseWebModel", formatTableName);
                    sw.WriteLine("    {");

                    #region 属性

                    sw.WriteLine("        #region 属性");
                    foreach (var columnInfo in tableInfo.ColumnInfos.Where(c => !IgnoreColumns.Contains(c.Code) && c.Code != tableInfo.GetPrimaryKeyColumnName()))
                    {
                        sw.WriteLine();
                        sw.WriteLine("        /// <summary>");
                        sw.WriteLine("        /// {0}", columnInfo.Comment);
                        sw.WriteLine("        /// </summary>");
                        if (columnInfo.GetColumnType() == "string")
                            sw.WriteLine("        [StringLength({0}), ErrorMessage = \"{1}长度不能超过{2}\")]", columnInfo.Length,
                                string.IsNullOrEmpty(columnInfo.Comment) ||
                                string.IsNullOrEmpty(columnInfo.Comment.Trim())
                                    ? columnInfo.Code
                                    : columnInfo.Comment, columnInfo.Length);
                        sw.WriteLine("        public {0} {1} {2} get; set; {3}", columnInfo.GetColumnType(),
                            columnInfo.Code, "{", "}");
                    }
                    sw.WriteLine();
                    sw.WriteLine("        #endregion");
                    sw.WriteLine();

                    #endregion

                    #region 构造函数

                    sw.WriteLine("        #region 构造函数");
                    sw.WriteLine();
                    sw.WriteLine("        public static {0}WebModel New()", tableInfo.GetFormatTableName());
                    sw.WriteLine("        {");
                    sw.WriteLine("            return {0}Entity.New().AsWebModel();", tableInfo.GetFormatTableName());
                    sw.WriteLine("        }");
                    sw.WriteLine();
                    sw.WriteLine("        #endregion");

                    #endregion

                    sw.WriteLine("    }");
                    sw.WriteLine();

                    #endregion

                    sw.WriteLine("    public static class {0}WebModelExtensions", formatTableName);
                    sw.WriteLine("    {");
                    sw.WriteLine("        #region WebModel转换为Entity");
                    sw.WriteLine();
                    sw.WriteLine("        public static {0}Entity AsEntity(this {0}WebModel model)", formatTableName);
                    sw.WriteLine("        {");
                    sw.WriteLine("            var entity = new {0}BL().Get(model.{1}, true);", formatTableName, tableInfo.GetPrimaryKeyColumnName());
                    sw.WriteLine("            DataProcess.InitModel(model);");
                    sw.WriteLine("            if (entity == null)");
                    sw.WriteLine("            {");
                    sw.WriteLine("                entity = {0}Entity.New();", formatTableName);
                    sw.WriteLine("            }");

                    foreach (var columnInfo in tableInfo.ColumnInfos.Where(
                        c => !IgnoreColumns.Contains(c.Code) && c.Code != tableInfo.GetPrimaryKeyColumnName()))
                    {
                        var columnType = columnInfo.GetColumnType();
                        sw.WriteLine("            entity.{0} = model.{0}{1};", columnInfo.Code, columnType == "decimal" || columnType == "DateTime" ? ".Value" : "");
                    }

                    sw.WriteLine();
                    sw.WriteLine("            return entity;");
                    sw.WriteLine("        }");
                    sw.WriteLine();
                    sw.WriteLine("        public static IEnumerable<{0}Entity> AsEntity(this IEnumerable<{0}WebModel> modelList)", formatTableName);
                    sw.WriteLine("        {");
                    sw.WriteLine("            return modelList == null ? null : modelList.Select(AsEntity).Where(c => c != null);");
                    sw.WriteLine("        }");
                    sw.WriteLine();
                    sw.WriteLine("        #endregion");
                    sw.WriteLine();
                    sw.WriteLine("        #region Entity转换为WebModel");
                    sw.WriteLine();
                    sw.WriteLine("        public static {0}WebModel AsWebModel(this {0}Entity entity, bool isEdit = true)", formatTableName);
                    sw.WriteLine("        {");
                    sw.WriteLine("            if (entity == null) return null;");
                    sw.WriteLine("            var model = new {0}WebModel();", formatTableName);
                    sw.WriteLine("            model.Init(entity);");

                    foreach (var columnInfo in tableInfo.ColumnInfos.Where(
                        c => !IgnoreColumns.Contains(c.Code) && c.Code != tableInfo.GetPrimaryKeyColumnName()))
                    {
                        sw.WriteLine("            model.{0} = entity.{0};", columnInfo.Code);
                    }

                    sw.WriteLine();
                    sw.WriteLine("            return model;");
                    sw.WriteLine("        }");
                    sw.WriteLine();
                    sw.WriteLine("        public static IEnumerable<{0}WebModel> AsWebModel(this IEnumerable<{0}Entity> entities, bool isEdit = false)", formatTableName);
                    sw.WriteLine("        {");
                    sw.WriteLine("            return entities == null ? null : entities.Select(t => t.AsWebModel(isEdit)).Where(c => c != null);");
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