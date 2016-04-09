<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference Prerelease="true">Microsoft.CodeAnalysis.Compilers</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
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
	
	MyWalker walker = new MyWalker();
	walker.Visit(syntaxTree.GetRoot());
}

class MyWalker : CSharpSyntaxWalker
{	
	public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
	{
		node.ChildTokens()
			.FirstOrDefault(token => token.Kind() == SyntaxKind.IdentifierToken)
			.Text
			.Dump();
	}
}