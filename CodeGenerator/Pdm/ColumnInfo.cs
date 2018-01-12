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
            if (DataType.ToUpper().Contains("CHAR")) return "string";
            if (DataType.ToUpper().Contains("VARCHAR")) return "string";
            if (DataType.ToUpper().Contains("NUMBER")) return "decimal";
            if (DataType.ToUpper().Contains("NUMERIC")) return "decimal";

            switch (DataType.ToUpper())
            {
                case "INT":
                case "INTEGER":
                    return Mandatory ? "int" : "int?";
                case "BIGINT":
                    return "long";
                case "SMALLINT":
                    return "short";
                case "DATETIME":
                case "DATE":
                    return Mandatory ? "DateTime" : "DateTime?";
                case "BIT":
                    return Mandatory ? "bool" : "bool?";
                case "TEXT":
                    return "string";
                case "UNIQUEIDENTIFIER":
                    return Mandatory ? "Guid" : "Guid?";
                case "TINYINT":
                    return Mandatory ? "byte" : "byte?";
            }

            throw new Exception($"数据库列{Code}类型{DataType}无法转换为System基础类型");
        }
    }
}