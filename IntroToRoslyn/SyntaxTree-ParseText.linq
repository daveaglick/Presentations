<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>Microsoft.CodeAnalysis.Compilers</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

void Main()
{
	var syntaxTree = CSharpSyntaxTree.ParseText(@"
		class MyClass
		{
			int X { get; set; }
		}
	");
	
	CodeAnalysisUtil.DumpSyntaxTree(syntaxTree);
}