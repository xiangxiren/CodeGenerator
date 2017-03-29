using System;

namespace CodeGenerator.Pdm
{
    public class ColumnInfo : InfoBase
    {
        public string DataType { get; set; }

        public string Length { get; set; }

        public bool Identity { get; set; }

        public bool Mandatory { get; set; }

        public string ExtendedAttributesText { get; set; }

        public string PhysicalOptions { get; set; }

        public bool PrimaryKey { get; set; }

        public string SystemType
        {
            get { return GetColumnType(); }
        }

        public string GetColumnType()
        {
            if (DataType.ToUpper().Contains("VARCHAR")) return "string";
            if (DataType.ToUpper().Contains("NUMBER")) return "decimal";
            switch (DataType.ToUpper())
            {
                case "DATE":
                    return "DateTime";
                case "INTEGER":
                    return "decimal";
            }

            throw new Exception(string.Format("数据库列{0}类型{1}无法转换为System基础类型", Code, DataType));
        }
    }
}