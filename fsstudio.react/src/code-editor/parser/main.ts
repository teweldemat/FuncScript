// parser.ts

export function tokenizeFuncScript(input:String) {
    const tokens = [];
    let pos = 0;
  
    while (pos < input.length) {
      const char = input[pos];
  
      if (/\s/.test(char)) {
        pos++;
        continue;
      }
  
      if (char === '"') {
        let start = pos;
        pos++;
        while (pos < input.length && input[pos] !== '"') {
          pos++;
        }
        pos++;
        tokens.push({
          type: "STRING",
          value: input.slice(start, pos),
          start,
          end: pos,
        });
        continue;
      }
  
      if (/\d/.test(char)) {
        let start = pos;
        while (pos < input.length && /\d/.test(input[pos])) {
          pos++;
        }
        tokens.push({
          type: "NUMBER",
          value: input.slice(start, pos),
          start,
          end: pos,
        });
        continue;
      }
  
      if (char === "+") {
        tokens.push({
          type: "PLUS",
          value: "+",
          start: pos,
          end: pos + 1,
        });
        pos++;
        continue;
      }
  
      throw new Error("Unexpected character: " + char);
    }
  
    return tokens;
  }