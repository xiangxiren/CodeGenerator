using System;

namespace CodeGenerator.Generate
{
	public class CodeGeneratorAttribute : Attribute
	{
		public GenerateType GenerateType { get; }

		public CodeGeneratorAttribute(GenerateType generateType)
		{
			GenerateType = generateType;
		}
	}
}