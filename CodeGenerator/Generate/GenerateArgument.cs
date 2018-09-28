using System.Collections.Generic;

namespace CodeGenerator.Generate
{
	public class ArgumentInfo
	{
		/// <summary>
		/// 生成类型
		/// </summary>
		public GenerateType GenerateType { get; set; }

		/// <summary>
		/// 类命名空间
		/// </summary>
		public string ClassNamespace { get; set; }

		/// <summary>
		/// 文件保存地址
		/// </summary>
		public string FileSavePath { get; set; }

		public GenerateArgument GenerateArgument { get; set; }
	}

	public enum GenerateType
	{
		Entity,
		Config,
		Repository,
		Bl
	}

	public class GenerateArgument
	{
		/// <summary>
		/// 上下文名称
		/// </summary>
		public string ContextName { get; set; }

		/// <summary>
		/// 上下文命名空间
		/// </summary>
		public string ContextNamespace { get; set; }

		/// <summary>
		/// 实体命名空间
		/// </summary>
		public string EntityNamespace { get; set; }

		/// <summary>
		/// 映射命名空间
		/// </summary>
		public string ConfigNamespace { get; set; }

		/// <summary>
		/// 上下文存放路径
		/// </summary>
		public string ContextFileSavePath { get; set; }

		/// <summary>
		/// 参数集合
		/// </summary>
		public List<ArgumentInfo> ArgumentInfos { get; set; }
	}
}