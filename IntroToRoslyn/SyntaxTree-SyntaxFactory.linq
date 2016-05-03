<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>Microsoft.CodeAnalysis.Compilers</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
</Query>

void Main()
{	
	var syntaxNode = SyntaxFactory.ClassDeclaration("MyClass");
	
	var syntaxTree = CSharpSyntaxTree.Create(syntaxNode);
		
	CodeAnalysisUtil.DumpSyntaxTree(syntaxTree);
}