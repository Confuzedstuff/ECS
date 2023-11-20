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

                var visibility = classDeclaration.Modifiers.First(x => new List<string> { "public", "private", "internal" }.Contains(x.Text.Trim()));
                builder.AppendLine($"{visibility} sealed partial class {classDeclaration.Identifier.Text}");
                builder.Braces(() =>
                {
                    builder.AppendLine("public override Type[] GetWithTypes()");
                    builder.Braces(() =>
                    {
                        builder.AppendLine(@"return new Type[]");
                        builder.Braces(() =>
                        {
                            for (var i = 0; i < updateEntityMethod.ParameterList.Parameters.Count; i++)
                            {
                                var parameter = updateEntityMethod.ParameterList.Parameters[i];
                                builder.AppendLine($"typeof({parameter.Type}),");
                            }
                        });
                        builder.AppendLine(@";");
                    });
                    builder.AppendLine("public override void Execute(in float delta)");
                    builder.Braces(() =>
                    {
                        builder.AppendLine("this.delta = delta;");
                        builder.AppendLine("for (var iarch = 0; iarch < arches.Count(); iarch++)");
                        builder.Braces(() =>
                        {
                            builder.AppendLine($"var arch = arches[iarch];");
                            for (var i = 0; i < updateEntityMethod.ParameterList.Parameters.Count; i++)
                            {
                                var parameter = updateEntityMethod.ParameterList.Parameters[i];
                                builder.AppendLine($"var component{i} = arch.GetComponent<{parameter.Type}>();");
                                builder.AppendLine($"var component{i}Elements = component{i}.Elements;");
                                if (parameter.Type.ToString().Contains("GlobalId"))
                                {
                                    builder.AppendLine($"ref var /*{parameter.Type}*/ {parameter.Identifier.Text} = ref component{i}Elements[0];");
                                }
                            }

                            if (updateEntityMethod.ParameterList.Parameters.Any())
                            {
                                builder.AppendLine($"var nAlive = component0.nextIndex;");
                            }

                            builder.AppendLine("for (var iitem = 0; iitem < nAlive; iitem++)");
                            builder.Braces(() =>
                            {
                                for (var i = 0; i < updateEntityMethod.ParameterList.Parameters.Count; i++)
                                {
                                    var parameter = updateEntityMethod.ParameterList.Parameters[i];
                                    if (!parameter.Type.ToString().Contains("GlobalId"))
                                    {
                                        builder.AppendLine(
                                            $"ref var /*{parameter.Type}*/ {parameter.Identifier.Text} = ref component{i}Elements[iitem];");
                                    }
                                }

                                builder.AppendLine("UpdateEntity(");
                                for (var i = 0; i < updateEntityMethod.ParameterList.Parameters.Count; i++)
                                {
                                    var comma = i == updateEntityMethod.ParameterList.Parameters.Count - 1 ? "" : ",";
                                    var parameter = updateEntityMethod.ParameterList.Parameters[i];
                                    builder.AppendLine($"{parameter.Modifiers.ToString()} {parameter.Identifier.Text}{comma}");
                                }

                                builder.AppendLine(");");
                            });
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