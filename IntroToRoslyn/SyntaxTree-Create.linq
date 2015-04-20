<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference Prerelease="true">Microsoft.CodeAnalysis.Compilers</NuGetReference>
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
	
	var subTree = CSharpSyntaxTree.Create(syntaxTree
		.GetRoot()
		.DescendantNodes()
		.OfType<CSharpSyntaxNode>()
		.FirstOrDefault(x => x.Kind() == SyntaxKind.PropertyDeclaration));
		
	CodeAnalysisUtil.DumpSyntaxTree(subTree);
}