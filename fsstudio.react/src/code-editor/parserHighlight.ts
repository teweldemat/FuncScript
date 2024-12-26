// parserHighlight.ts

import { EditorView, Decoration, DecorationSet, ViewPlugin, ViewUpdate } from "@codemirror/view";
import { RangeSetBuilder } from "@codemirror/state";
import { tokenizeFuncScript } from "./parser/main";

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
        const tokens = tokenizeFuncScript(text);

        for (const token of tokens) {
          const deco = Decoration.mark({
            class: `cm-${token.type.toLowerCase()}`
          });
          builder.add(token.start, token.end, deco);
        }
        return builder.finish();
      }
    },
    {
      decorations: (v) => v.decorations
    }
  );
}

