<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference Prerelease="true">Microsoft.CodeAnalysis.Compilers</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

void Main()
{
	var syntaxTree = CSharpSyntaxTree.ParseText(@"
		class MyClass
		{
			int X { get; set; }
			int Y { get; set; }
		}
	");
	
	CodeAnalysisUtil.DumpSyntaxTree(syntaxTree);
	
	syntaxTree.GetRoot()
		.DescendantNodes()
		.OfType<PropertyDeclarationSyntax>()
		.Select(node => node.ChildTokens()
			.FirstOrDefault(token => token.Kind() == SyntaxKind.IdentifierToken)
			.Text)
		.Dump();
}