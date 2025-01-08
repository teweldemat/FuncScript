// See https://aka.ms/new-console-template for more information
using FuncScript;
using FuncScript.Core;
using System.Data;
using static FuncScript.Core.FuncScriptParser;

do
{
    Console.Write("Enter expression:");
    var exp = Console.ReadLine();
    try
    {
        var res=FuncScript.Helpers.Evaluate(exp);
        Console.WriteLine("result");
        Console.WriteLine(res.ToString());
    }
    catch(FuncScript.Error.SyntaxError syntaxError)
    {
        if (syntaxError.Message != null)
            Console.WriteLine(syntaxError.Message);
        Console.WriteLine($"{syntaxError.Message}\n{syntaxError.Line}" );
    }
} while (true);
