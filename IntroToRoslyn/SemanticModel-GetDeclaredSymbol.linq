<Query Kind="Statements">
  <Output>DataGrids</Output>
  <NuGetReference>Microsoft.CodeAnalysis.Compilers</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

var syntaxTree = CSharpSyntaxTree.ParseText(@"
	class MyClass
	{
		void MyFunc(int x)
		{
		}
		
		void Func2()
		{
			MyFunc(3);
		}
	}
");

var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
var compilation = CSharpCompilation.Create("MyAssembly").AddSyntaxTrees(syntaxTree).AddReferences(mscorlib).WithOptions(options);

var semanticModel = compilation.GetSemanticModel(syntaxTree);
var symbol = semanticModel.GetDeclaredSymbol(syntaxTree.GetRoot()
	.DescendantNodes()
	.OfType<MethodDeclarationSyntax>()
	.First(syntax => syntax.Identifier.Text == "MyFunc"));
symbol.Dump();