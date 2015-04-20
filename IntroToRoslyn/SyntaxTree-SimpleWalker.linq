<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference Prerelease="true">Microsoft.CodeAnalysis.Compilers</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
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
	int _spaces = 0;
	
	public override void Visit(SyntaxNode node)
	{
		_spaces++;
		(new string(' ', _spaces) + node.Kind()).Dump();
		base.Visit(node);
		_spaces--;
	}
}