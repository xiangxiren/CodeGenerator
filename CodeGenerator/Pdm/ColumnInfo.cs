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
			var dataType = DataType.ToUpper();

			if (dataType.Contains("CHAR")) return "string";
			if (dataType.Contains("VARCHAR")) return "string";
			if (dataType.Contains("NUMBER")) dataType = "DECIMAL";
			else if (dataType.Contains("NUMERIC")) dataType = "DECIMAL";
			else if (dataType.Contains("DECIMAL")) dataType = "DECIMAL";

			switch (dataType)
			{
				case "INT":
				case "INTEGER":
					return Mandatory ? "int" : "int?";
				case "TINYINT":
					return Mandatory ? "int" : "int?";
				case "BIGINT":
					return Mandatory ? "long" : "long?";
				case "SMALLINT":
					return "short";
				case "DATETIME":
				case "DATE":
					return Mandatory ? "DateTime" : "DateTime?";
				case "BOOL":
				case "BIT":
					return Mandatory ? "bool" : "bool?";
				case "TEXT":
					return "string";
				case "UNIQUEIDENTIFIER":
					return Mandatory ? "Guid" : "Guid?";
				case "DECIMAL":
					return Mandatory ? "decimal" : "decimal?";
				case "DOUBLE":
					return Mandatory ? "double" : "double?";
			}

			throw new Exception($"数据库列{Code}类型{DataType}无法转换为System基础类型");
		}
	}
}