<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>Microsoft.CodeAnalysis.Compilers</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Diagnostics</Namespace>
  <Namespace>System.Collections.Immutable</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

async Task Main()
{
	var syntaxTree = CSharpSyntaxTree.ParseText(@"
		class MyClass
		{
			void MyFunc(int x)
			{
			}
		}
	");
	
	var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
	var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
	DiagnosticAnalyzer analyzer = new DontStartClassNamesWithMyAnalyzer();
	var compilation = CSharpCompilation.Create("MyAssembly")
		.AddSyntaxTrees(syntaxTree)
		.AddReferences(mscorlib)
		.WithOptions(options)
		.WithAnalyzers(ImmutableArray.Create(analyzer));
	
	var analyzerDiagnostics = await compilation.GetAnalyzerDiagnosticsAsync();
	analyzerDiagnostics.Dump();
}

public class DontStartClassNamesWithMyAnalyzer : DiagnosticAnalyzer
{
	const string DiagnosticId = "DG0001";
	const string Title = "Don't start class names with 'My'";
	const string MessageFormat = "Class name started with 'My': {0}";
	const string Category = "Syntax";
	
	static DiagnosticDescriptor Descriptor =
      new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
	  DiagnosticSeverity.Warning, true);
    
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
	{ 
		get { return ImmutableArray.Create(Descriptor); } 
	}
	
	public override void Initialize(AnalysisContext context)
    {
		context.RegisterSyntaxNodeAction(CheckForMy, SyntaxKind.ClassDeclaration);
    }
	
	void CheckForMy(SyntaxNodeAnalysisContext context)
	{
		TypeDeclarationSyntax typeDeclaration = (TypeDeclarationSyntax)context.Node;
		if(typeDeclaration.Identifier.Text.StartsWith("My"))
		{
			context.ReportDiagnostic(
				Diagnostic.Create(Descriptor, typeDeclaration.Identifier.GetLocation(), typeDeclaration.Identifier.Text));
		}
	}
}