﻿using funcscript.core;
using funcscript.error;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace funcscript.test
{
    public class TestErrorReporting
    {
        public TestErrorReporting()
        {
        }

        void AnalyzeError(Exception ex, String exp, int expectedPos, int expecctedLen)
        {
            Assert.AreEqual(typeof(error.EvaluationException), ex.GetType());
            var evalError = (EvaluationException)ex;
            Console.WriteLine(evalError.Message);
            if (evalError.InnerException != null)
                Console.WriteLine(evalError.InnerException.Message);
            Assert.AreEqual(expectedPos, evalError.Pos);
            Assert.AreEqual(expecctedLen, evalError.Len);
        }
        void AnalyzeSyntaxError(Exception ex, String exp)
        {
            Assert.AreEqual(typeof(error.SyntaxError), ex.GetType());
            var sError = (SyntaxError)ex;
            Console.WriteLine(sError.Message);
            if (sError.InnerException != null)
                Console.WriteLine(sError.InnerException.Message);
        }
        void AnalyzeMainSyntaxErrorLine(Exception ex,string line)
        {
            Assert.AreEqual(typeof(error.SyntaxError), ex.GetType());
            var sError = (SyntaxError)ex;
            var unique = sError.GetData().Distinct().ToList();
            var uniqueError = new SyntaxError(sError.TargetExpression , unique);
            Assert.That(uniqueError.Line,Is.EqualTo(line));
        }

        [Test]
        public void ErrorMissingSemicolon()
        {
            var exp = @"
{a:5;
b:4
c:4
}
";
            try
            {
                FuncScript.Evaluate(exp);
            }
            catch (Exception ex)
            {
                AnalyzeMainSyntaxErrorLine(ex, "c:4");
            }
        }
        [Test]
        public void TestFunctionError()
        {
            var exp = $"length(a)";
            try
            {
                FuncScript.Evaluate(exp);
            }
            catch (Exception ex)
            {
                AnalyzeError(ex, exp, 0, exp.Length);
            }
        }


        [Test]
        public void TestFunctionError2()
        {
            var error_exp = "length(a)";
            var exp = $"10+{error_exp}";
            try
            {
                FuncScript.Evaluate(exp);
            }
            catch (Exception ex)
            {
                AnalyzeError(ex, exp, exp.IndexOf(error_exp), error_exp.Length);
            }
        }
        [Test]
        public void TestTypeMismatchError()
        {
            var error_exp = "len(5)";
            var exp = $"10+{error_exp}";
            try
            {
                FuncScript.Evaluate(exp);
                throw new Exception("No error");
            }
            catch (Exception ex)
            {
                AnalyzeError(ex, exp, exp.IndexOf(error_exp), error_exp.Length);
                Assert.AreEqual(typeof(error.TypeMismatchError), ex.InnerException.GetType());
            }
        }
        [Test]
        public void TestNullMemberAccessError()
        {
            var error_exp = "x.l";
            var exp = $"10+{error_exp}";
            try
            {
                FuncScript.Evaluate(exp);
                throw new Exception("No error");
            }
            catch (Exception ex)
            {
                AnalyzeError(ex, exp, exp.IndexOf(error_exp), error_exp.Length);
                Assert.AreEqual(typeof(error.TypeMismatchError), ex.InnerException.GetType());
            }
        }
        [Test]
        public void TestListMemberAccessError()
        {
            var error_exp = "[5,6].l";
            var exp = $"10+{error_exp}";
            try
            {
                FuncScript.Evaluate(exp);
                throw new Exception("No error");
            }
            catch (Exception ex)
            {
                AnalyzeError(ex, exp, exp.IndexOf(error_exp), error_exp.Length);
                Assert.AreEqual(typeof(error.TypeMismatchError), ex.InnerException.GetType());
            }

        }

        [Test]
        public void TestSyntaxErrorMissingOperand()
        {
            var error_exp = "3+";
            var exp = $"{error_exp}";
            var msg = Guid.NewGuid().ToString();
            try
            {
                //FuncScript.Evaluate(exp, new { f = new Func<int, int>((x) => { throw new Exception("internal"); }) });
                FuncScript.EvaluateWithVars(exp, new
                {
                    f = new Func<int, int>((x) =>
                    {
                        throw new Exception(msg);
                    })
                });
            }
            catch (Exception ex)
            {
                AnalyzeSyntaxError(ex, exp);
            }
        }
        [Test]
        public void TestSyntaxErrorIncompletKvc1()
        {
            var error_exp = "{a:3,c:";
            var exp = $"{error_exp}";
            var msg = Guid.NewGuid().ToString();
            try
            {
                //FuncScript.Evaluate(exp, new { f = new Func<int, int>((x) => { throw new Exception("internal"); }) });
                FuncScript.EvaluateWithVars(exp, new
                {
                    f = new Func<int, int>((x) =>
                    {
                        throw new Exception(msg);
                    })
                });
                throw new Exception("No error");
            }
            catch (Exception ex)
            {
                AnalyzeSyntaxError(ex, exp);
            }
        }
        [Test]
        public void TestLambdaErrorMemberAccessError()
        {
            var error_exp = "f(3)";
            var exp = $"10+{error_exp}";
            var msg = Guid.NewGuid().ToString();
            try
            {
                //FuncScript.Evaluate(exp, new { f = new Func<int, int>((x) => { throw new Exception("internal"); }) });
                FuncScript.EvaluateWithVars(exp, new { f = new Func<int, int>((x) =>
                {
                    throw new Exception(msg);
                })});
                throw new Exception("No error");
            }
            catch (Exception ex)
            {
                AnalyzeError(ex, exp, exp.IndexOf(error_exp), error_exp.Length);
                Assert.AreEqual(msg, ex.InnerException.InnerException.Message);
            }

        }
    }
}
