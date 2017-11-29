using System;

namespace CodeGenerator.Pdm
{
    public class ColumnInfo : InfoBase
    {
        public string DataType { get; set; }

        public int Length { get; set; }

        public int Precision { get; set; }

        public bool Identity { get; set; }

        [NodeChild("Column.Mandatory")]
        public bool Mandatory { get; set; }

        public string ExtendedAttributesText { get; set; }

        public string PhysicalOptions { get; set; }

        public bool PrimaryKey { get; set; }

        public string SystemType => GetColumnType();

        public string GetColumnType()
        {
            if (DataType.ToUpper().Contains("VARCHAR")) return "string";
            if (DataType.ToUpper().Contains("NUMBER")) return "decimal";
            if (DataType.ToUpper().Contains("NUMERIC")) return "decimal";
            switch (DataType.ToUpper())
            {
                case "INT":
                case "INTEGER":
                    return "int";
                case "BIGINT":
                    return "long";
                case "DATETIME":
                case "DATE":
                    return "DateTime";
                case "BIT":
                    return "bool";
                case "TEXT":
                    return "string";
                case "UNIQUEIDENTIFIER":
                    return "Guid";
            }

            throw new Exception($"数据库列{Code}类型{DataType}无法转换为System基础类型");
        }
    }
}