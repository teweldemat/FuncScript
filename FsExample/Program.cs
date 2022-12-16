﻿// See https://aka.ms/new-console-template for more information
using funcscript;
using funcscript.core;
using System.Data;
using static funcscript.core.FuncScriptParser;

do
{
    Console.Write("Enter expression:");
    var exp = Console.ReadLine();
    try
    {
        var res=FuncScript.Evaluate(exp);
        Console.WriteLine("result");
        Console.WriteLine(res.ToString());
    }
    catch(funcscript.error.SyntaxError syntaxError)
    {
        if (syntaxError.Message != null)
            Console.WriteLine(syntaxError.Message);
        foreach(var d in syntaxError.data)
        {
            Console.WriteLine($"Error at {d.Loc+1}\n{d.Message}");
        }
    }
} while (true);
