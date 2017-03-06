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

        public string GetColumnType()
        {
            if (DataType.ToUpper().Contains("VARCHAR")) return "string";
            switch (DataType.ToUpper())
            {
                case "DATE":
                    return "DateTime";
                case "NUMBER":
                    return "decimal";
            }

            throw new Exception("未知类型");
        }
    }
}