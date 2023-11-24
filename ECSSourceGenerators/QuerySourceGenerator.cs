using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ECSSourceGenerator
{
    [Generator]
    public class QuerySourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var classes = context.Compilation.SyntaxTrees.SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes())
                    .Where(x => x is ClassDeclarationSyntax)
                    .Cast<ClassDeclarationSyntax>()
                    .Where(c =>
                    {
                        if (c.Modifiers.ToString().Contains("abstract"))
                        {
                            return false;
                        }

                        var bl = c.BaseList?.ToString();
                        if (bl is null)
                        {
                            return false;
                        }

                        if (bl.Split().Contains("Query"))
                        {
                            return true;
                        }

                        return false;
                    }).ToList()
                ;


            foreach (var classDeclaration in classes)
            {
                var constructor = classDeclaration.Members
                    .Where(x => x is ConstructorDeclarationSyntax)
                    .Cast<ConstructorDeclarationSyntax>()
                    .FirstOrDefault(x => x.Identifier.Text == classDeclaration.Identifier.Text);
                var builder = new IndentBuilder();
                if (constructor is null)
                {
                    builder.AppendLine($"// {classDeclaration.Identifier.Text} ERROR missing constructor");
                    continue;
                }

                CreateQuery(builder, classDeclaration.Identifier.Text, constructor.ParameterList.Parameters, true);
                context.AddSource(classDeclaration.Identifier.Text.Trim() + "Generated", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
        }

        public static void CreateQuery(IndentBuilder builder, string queryIdentifier, SeparatedSyntaxList<ParameterSyntax> parameters, bool isPartial)
        {
            builder.AppendLine("using System;");


            builder.AppendLine($"//{queryIdentifier}");

            var partial = isPartial ? "partial" : "";
            var baseClass  = isPartial ? "" : ": Query";
            builder.AppendLine($"public sealed {partial} class {queryIdentifier} {baseClass}");
            builder.Braces(() =>
            {
                builder.AppendLine($"public {queryIdentifier}()");
                builder.Braces(() => { });
                
                builder.AppendLine("public override Type[] GetWithTypes()");
                builder.Braces(() =>
                {
                    builder.AppendLine(@"return new Type[]");
                    builder.Braces(() =>
                    {
                        foreach (var parameter in parameters)
                        {
                            builder.AppendLine($"typeof({parameter.Type}),");
                        }
                    });
                    builder.AppendLine(@";");
                });
                builder.AppendLine("protected override void LookupArchComponents()");
                builder.Braces(() =>
                {
                    foreach (var parameter in parameters)
                    {
                        var component = $"{parameter.Identifier.Text}Component";
                        builder.AppendLine($"{component} = currentArch.GetComponent<{parameter.Type.ToString()}>();");
                    }
                });

                foreach (var parameter in parameters)
                {
                    var component = $"{parameter.Identifier.Text}Component";
                    builder.AppendLine(
                        $"private Component<{parameter.Type.ToString()}> {component};");
                    builder.AppendLine(
                        $"public ref {parameter.Type.ToString()} {parameter.Identifier.Text} => ref {component}.Get(_index);");
                }
            });
        }
    }
}