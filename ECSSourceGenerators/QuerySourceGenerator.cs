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
                var builder = new IndentBuilder();
                builder.AppendLine("using System;");

                var constructor = classDeclaration.Members
                    .Where(x => x is ConstructorDeclarationSyntax)
                    .Cast<ConstructorDeclarationSyntax>()
                    .FirstOrDefault(x => x.Identifier.Text == classDeclaration.Identifier.Text);
                builder.AppendLine($"//{classDeclaration.Identifier.Text}");

                builder.AppendLine($"public sealed partial class {classDeclaration.Identifier.Text}");
                builder.Braces(() =>
                {
                    builder.AppendLine($"public {classDeclaration.Identifier.Text}()");
                    builder.Braces(() => { });

                    if (constructor is null)
                    {
                        builder.AppendLine($"// ERROR missing constructor");
                        return;
                    }

                    var pars = constructor.ParameterList.Parameters;
                    builder.AppendLine("protected override Type[] GetWithTypes()");
                    builder.Braces(() =>
                    {
                        builder.AppendLine(@"return new Type[]");
                        builder.Braces(() =>
                        {
                            foreach (var parameter in pars)
                            {
                                builder.AppendLine($"typeof({parameter.Type}),");
                            }
                        });
                        builder.AppendLine(@";");
                    });
                    builder.AppendLine("protected override void LookupArchComponents()");
                    builder.Braces(() =>
                    {
                        foreach (var parameter in pars)
                        {
                            var component = $"{parameter.Identifier.Text}Component";
                            builder.AppendLine($"{component} = currentArch.{parameter.Identifier.ToString()};");
                        }
                        
                    });
                    
                    foreach (var parameter in pars)
                    {
                        var component = $"{parameter.Identifier.Text}Component";
                        builder.AppendLine(
                            $"private Component<{parameter.Type.ToString()}> {component};");
                        builder.AppendLine(
                            $"public ref {parameter.Type.ToString()} {parameter.Identifier.Text} => ref {component}.Get(index);");
                    }
                });
                context.AddSource(classDeclaration.Identifier.Text.Trim() + "Generated", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
        }
    }
}