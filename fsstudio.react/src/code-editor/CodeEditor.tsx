import { useCodeMirror, EditorView } from "@uiw/react-codemirror";
import { useRef } from "react";
import { ExpressionType } from "../components/EvalNodeProvider";
import { parserHighlight } from "./parserHighlight";

const CodeEditor: React.FC<{
  expression: string | null;
  setExpression: (exp: string) => void;
  expressionType: ExpressionType; // Add this prop
}> = ({ expression, setExpression, expressionType }) => {
  const editorRef = useRef<HTMLDivElement | null>(null);

  useCodeMirror({
    container: editorRef.current,
    value: expression ?? '',
    extensions:
      expressionType === ExpressionType.ClearText
        ? [EditorView.lineWrapping]
        : [parserHighlight(), EditorView.lineWrapping],
    onChange: (value) => {
      setExpression(value);
    },
  });

  return (
    <div
      ref={editorRef}
      style={{ height: '100%', overflow: 'scroll', border: '1px solid #ccc' }}
    />
  );
};

export default CodeEditor