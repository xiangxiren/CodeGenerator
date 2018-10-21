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
		/// 服务接口
		/// </summary>
		Interface,

		/// <summary>
		/// 服务实现
		/// </summary>
		Implement,

		/// <summary>
		/// Dto
		/// </summary>
		Dto,

		/// <summary>
		/// 查询参数
		/// </summary>
		QueryParam,

		/// <summary>
		/// 控制器
		/// </summary>
		Controller,

		/// <summary>
		/// 列表视图
		/// </summary>
		ViewIndex,

		/// <summary>
		/// 编辑视图
		/// </summary>
		ViewEdit
	}

	public class GenerateArgument
	{
		/// <summary>
		/// 项目名称，所有类命名空间根
		/// </summary>
		public string ProjectName { get; set; }

		/// <summary>
		/// 生成文件存放根路径
		/// </summary>
		public string FilePath { get; set; }

		/// <summary>
		/// 实体文件存放路径
		/// </summary>
		public static string EntityFilePath { get; } = "Data\\Entity";

		/// <summary>
		/// 映射文件存放路径
		/// </summary>
		public static string ConfigFilePath { get; } = "Data\\Configuration";

		/// <summary>
		/// 数据传输对象文件存储路径
		/// </summary>
		public static string DtoFilePath { get; set; } = "Service\\Dto";

		/// <summary>
		/// 查询参数存放路径
		/// </summary>
		public static string QueryParamFilePath { get; set; } = "Service\\QueryParam";

		/// <summary>
		/// 服务接口文件存放路径
		/// </summary>
		public static string InterfaceFilePath { get; set; } = "Service\\Interface";

		/// <summary>
		/// 服务实现文件存放路径
		/// </summary>
		public static string ImplementFilePath { get; set; } = "Service\\Implement";

		/// <summary>
		/// 控制器文件存放路径
		/// </summary>
		public static string ControllerFilePath { get; set; } = "Web\\Controllers";

		/// <summary>
		/// 视图文件存放路径
		/// </summary>
		public static string ViewFilePath { get; set; } = "Web\\Views";

		/// <summary>
		/// 实体命名空间模板
		/// </summary>
		public static string EntityNamespaceTemp { get; } = "Data.Entity";

		/// <summary>
		/// 映射命名空间模板
		/// </summary>
		public static string ConfigNamespaceTemp { get; } = "Data.Configuration";

		/// <summary>
		/// 数据传输对象命名空间模板
		/// </summary>
		public static string DtoNamespaceTemp { get; set; } = "Service.Dto";

		/// <summary>
		/// 查询参数命名空间模板
		/// </summary>
		public static string QueryParamNamespaceTemp { get; set; } = "Service.QueryParam";

		/// <summary>
		/// 服务接口命名空间模板
		/// </summary>
		public static string InterfaceNamespaceTemp { get; set; } = "Service.Interface";

		/// <summary>
		/// 服务实现命名空间模板
		/// </summary>
		public static string ImplementNamespaceTemp { get; set; } = "Service.Implement";

		/// <summary>
		/// 控制器命名空间模板
		/// </summary>
		public static string ControllerNamespaceTemp { get; set; } = "Web.Controllers";
	}
}