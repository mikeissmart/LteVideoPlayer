using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;
using Reinforced.Typings;

namespace LteVideoPlayer.Spa.ViewModels
{
    public class NullablePropertyOverridingVisitor : TypeScriptExportVisitor
    {
        public NullablePropertyOverridingVisitor(TextWriter writer, ExportContext exportContext) : base(writer, exportContext)
        {
        }

        public override void Visit(RtField node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            AppendTabs();
            if (Context != WriterContext.Interface)
            {
                Decorators(node);
                Modifiers(node);
            }
            Visit(node.Identifier);
            Write(": ");
            Visit(node.Type);

            // added this to the original code
            if (node.Identifier.IsNullable) Write(" | null");

            if (!string.IsNullOrEmpty(node.InitializationExpression))
            {
                Write(" = ");
                Write(node.InitializationExpression);
            }

            Write(";");
            Br();
            if (!string.IsNullOrEmpty(node.LineAfter))
            {
                AppendTabs();
                Write(node.LineAfter);
                Br();
            }
        }

        public override void Visit(RtIdentifier node)
        {
            if (node == null) return;
            Write(node.IdentifierName);
            // this line from original was commented out
            // if (node.IsNullable) Write("?");
        }
    }
}
