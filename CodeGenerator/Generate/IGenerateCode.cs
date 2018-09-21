using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
	public interface IGenerateCode
	{
		/// <summary>
		/// 生成代码
		/// </summary>
		/// <param name="table">需要生成代码的表信息</param>
		/// <param name="argumentInfo">生成参数对象</param>
		void Generate(TableInfo table, ArgumentInfo argumentInfo);
	}
}
