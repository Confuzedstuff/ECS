using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ECSSourceGenerator
{
    [Generator]
    public class SystemSourceGenerator : ISourceGenerator // TODO reuse Query code to generate this
    {
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

                        if (bl.Contains("ECSSystem"))
                        {
                            return true;
                        }

                        return false;
                    }).ToList()
                ;


            foreach (var classDeclaration in classes)
            {
                var builder = new IndentBuilder();
                var root = classDeclaration.Ancestors().First(x => x is CompilationUnitSyntax) as CompilationUnitSyntax;
                builder.AppendLine("using System;");
                builder.AppendLine("using System.Linq;");

                foreach (var usingDirectiveSyntax in root.Usings)
                {
                    builder.AppendLine(usingDirectiveSyntax.ToString());
                }

                builder.AppendLine($"//{classDeclaration.Identifier.Text}");


                var updateEntityMethod = classDeclaration.Members
                    .Where(x => x is MethodDeclarationSyntax)
                    .Cast<MethodDeclarationSyntax>()
                    .FirstOrDefault(x => x.Identifier.Text == "UpdateEntity");

                if (updateEntityMethod is null)
                {
                    builder.AppendLine("// Update method not found");

                    continue;
                }

                var queryIdentifier = classDeclaration.Identifier.Text + "Query";
                var parameters = updateEntityMethod.ParameterList.Parameters;
                QuerySourceGenerator.CreateQuery(builder, queryIdentifier, parameters, false);
                var visibility = classDeclaration.Modifiers.First(x => new List<string> { "public", "private", "internal" }.Contains(x.Text.Trim()));
                builder.AppendLine($"{visibility} sealed partial class {classDeclaration.Identifier.Text}");
                builder.Braces(() =>
                {
                    builder.AppendLine($"private {queryIdentifier} query;");
                    builder.AppendLine("public override Type[] GetWithTypes()=> query.GetWithTypes();");
                    builder.AppendLine("public override void Execute(in float delta)");
                    builder.Braces(() =>
                    {
                        builder.AppendLine("this.delta = delta;");
                        builder.AppendLine("query.Reset();");
                        builder.AppendLine("while(query.Next())");
                        builder.Braces(() =>
                        {
                            builder.AppendLine("UpdateEntity(");

                            for (var index = 0; index < parameters.Count; index++)
                            {
                                var parameter = parameters[index];
                                var comma = index == updateEntityMethod.ParameterList.Parameters.Count - 1 ? "" : ",";
                                builder.AppendLine($"{parameter.Modifiers.ToString()} query.{parameter.Identifier.Text}{comma}");
                            }

                            builder.AppendLine(");");
                        });
                    });
                });
                context.AddSource(classDeclaration.Identifier.Text.Trim() + "Generated", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}