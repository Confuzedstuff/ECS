using System;
using System.Text;

namespace ECSSourceGenerator
{
    public class IndentBuilder
    {
        private int indent = 0;
        private readonly StringBuilder builder = new StringBuilder();
        public void PushIndent() => indent++;
        public void PopIndent() => indent--;

        public void AppendLine(string text)
        {
            var tabs = "".PadRight(indent, '\t');
            builder.Append(tabs);
            builder.AppendLine(text);
        }

        public void Braces(Action body)
        {
            AppendLine("{");
            PushIndent();
            body();
            PopIndent();
            AppendLine("}");
        }

        public override string ToString() => builder.ToString();
    }
}