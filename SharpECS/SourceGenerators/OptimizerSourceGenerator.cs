using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ECSSourceGenerator
{
    [Generator]
    public class OptimizerSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var classes = context.Compilation.SyntaxTrees.SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes())
                .Where(x => x is StructDeclarationSyntax)
                .Cast<StructDeclarationSyntax>()
                .Where(x=>x.BaseList?.ToString().Contains("IPvo") ?? false)
                .ToList();
            var builder = new IndentBuilder();

            foreach (var structDeclarationSyntax in classes)
            {
                var baseType = structDeclarationSyntax.BaseList.Types.First(x => x.ToString().Contains("IPvo"));
                var regex = new Regex(@"\w+<(\w+)>");
                var match =regex.Match(baseType.ToString());
                
                builder.AppendLine($"// {structDeclarationSyntax.Identifier.Text} {match.Groups[1]}");
            }

            context.AddSource("OptGenerated", SourceText.From(builder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}