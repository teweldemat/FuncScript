import { Parse as FuncScriptParse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";
import { EditorView, Decoration, DecorationSet, ViewPlugin, ViewUpdate } from "@codemirror/view";
import { RangeSetBuilder } from "@codemirror/state";
import { ParseContext, ParseNode, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main"

interface Token {
  start: number;
  end: number;
  type: string;
}

function getLeafNodes(node: ParseNode): Token[] {
  const tokens: Token[] = [];

  function traverse(node: ParseNode) {
    if (!node.Children || node.Children.length === 0) {
      tokens.push({
        start: node.Pos,
        end: node.Pos + node.Length,
        type: ParseNodeType[node.NodeType], // Use the name of the node type as the token type
      });
    } else {
      for (const child of node.Children) {
        traverse(child);
      }
    }
  }

  traverse(node);
  return tokens;
}

export function parserHighlight() {
  return ViewPlugin.fromClass(
    class {
      decorations: DecorationSet;

      constructor(view: EditorView) {
        this.decorations = this.buildDecorations(view);
      }

      update(update: ViewUpdate) {
        if (update.docChanged || update.viewportChanged) {
          this.decorations = this.buildDecorations(update.view);
        }
      }

      buildDecorations(view: EditorView) {
        const builder = new RangeSetBuilder<Decoration>();
        const text = view.state.doc.toString();

        const parseContext: ParseContext = {
          Expression: text,
          SyntaxErrors: [],
        };

        const parseResult = FuncScriptParse(parseContext);

        if (parseResult.ParseNode) {
          const tokens = getLeafNodes(parseResult.ParseNode);

          for (const token of tokens) {
            const deco = Decoration.mark({
              class: `cm-${token.type.toLowerCase()}`,
            });
            builder.add(token.start, token.end, deco);
          }
        }

        return builder.finish();
      }
    },
    {
      decorations: (v) => v.decorations,
    }
  );
}