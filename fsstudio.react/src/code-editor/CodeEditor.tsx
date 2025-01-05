import { useCodeMirror, EditorView } from "@uiw/react-codemirror";
import { useRef } from "react";
import { parserHighlight } from "./parserHighlight";
import { ExpressionType } from "../components/SessionContext";

const CodeEditor: React.FC<{
  expression: string | null;
  setExpression: (exp: string) => void;
  expressionType: ExpressionType;
  readOnly:boolean
  // Add this prop
}> = ({ expression, setExpression, expressionType,readOnly }) => {
  const editorRef = useRef<HTMLDivElement | null>(null);
  console.log(readOnly)
  useCodeMirror({
    container: editorRef.current,
    value: expression ?? '',
    readOnly:readOnly,
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