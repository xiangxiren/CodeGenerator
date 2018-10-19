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
		/// <summary>
		/// 实体
		/// </summary>
		Entity,

		/// <summary>
		/// 映射配置
		/// </summary>
		Config,

		/// <summary>
		/// 仓储
		/// </summary>
		Repository,

		/// <summary>
		/// 服务
		/// </summary>
		Service,

		/// <summary>
		/// Dto
		/// </summary>
		Dto,

		/// <summary>
		/// 控制器
		/// </summary>
		Controller,

		/// <summary>
		/// 视图
		/// </summary>
		View
	}

	public class GenerateArgument
	{
		/// <summary>
		/// 参数集合
		/// </summary>
		public List<ArgumentInfo> ArgumentInfos { get; set; }
	}
}