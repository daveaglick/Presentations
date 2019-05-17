<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>Microsoft.CodeAnalysis.Compilers</NuGetReference>
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
	
	MyRewriter rewriter = new MyRewriter();
	var newSyntaxTree = CSharpSyntaxTree.Create((CSharpSyntaxNode)rewriter.Visit(syntaxTree.GetRoot()));
	
	newSyntaxTree.ToString().Dump();
}

class MyRewriter : CSharpSyntaxRewriter
{	
	public override SyntaxToken VisitToken(SyntaxToken token)
	{
		if(token.Kind() == SyntaxKind.IdentifierToken)
		{
			// Have to create a new token since they're immutable
			return SyntaxFactory.Identifier(token.Text + "New");
		}
		return token;
	}
}