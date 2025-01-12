using FuncScript.Core;
using FuncScript.Error;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FuncScript.Block;
using FuncScript.Model;

namespace FuncScript.Test.Curated
{
    public class TestErrorReporting
    {
        public TestErrorReporting()
        {
        }

        void AnalyzeError(Exception ex, String exp, int expectedPos, int expecctedLen)
        {
            Assert.AreEqual(typeof(Error.EvaluationException), ex.GetType());
            var evalError = (EvaluationException)ex;
            Console.WriteLine(evalError.Message);
            if (evalError.InnerException != null)
                Console.WriteLine(evalError.InnerException.Message);
            Assert.AreEqual(expectedPos, evalError.Pos);
            Assert.AreEqual(expecctedLen, evalError.Len);
        }
        void AnalyzeSyntaxError(Exception ex, String exp)
        {
            Assert.AreEqual(typeof(Error.SyntaxError), ex.GetType());
            var sError = (SyntaxError)ex;
            Console.WriteLine(sError.Message);
            if (sError.InnerException != null)
                Console.WriteLine(sError.InnerException.Message);
        }
        void AnalyzeMainSyntaxErrorLine(Exception ex, string line)
        {
            Assert.AreEqual(typeof(Error.SyntaxError), ex.GetType());
            var sError = (SyntaxError)ex;
            var unique = sError.GetData().Distinct().ToList();
            var uniqueError = new SyntaxError(sError.TargetExpression, unique);
            Assert.That(uniqueError.Line, Is.EqualTo(line));
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
            Assert.Throws<SyntaxError>(()=>
            {
                Helpers.Evaluate(exp);
            });
        }
        [Test]
        public void TestFunctionError()
        {
            var exp = $"length(a)";
            try
            {
                Helpers.Evaluate(exp);
            }
            catch (Exception ex)
            {
                AnalyzeError(ex, exp, 0, exp.Length);
            }
        }

        [Test]
        public void TestNoFunction2()
        {
            var exp = $@"r:(f)=>f(0);
r(1);";
            var e = FuncScriptParser.Parse(exp);
            Assert.That(e.Block is KeyValueCollection);
            var kvc = (KvcExpression)e.Block;
            Assert.That(kvc.KeyValues.Count,Is.EqualTo(1));
            Assert.That(kvc.KeyValues[0].ValueExpression is LiteralBlock);
            var lb = (LiteralBlock)kvc.KeyValues[0].ValueExpression;
            Assert.That(lb.Value is ExpressionFunction);
            Assert.That(((ExpressionFunction)lb.Value).Expression is FunctionCallExpression);
            var fc = (FunctionCallExpression)((ExpressionFunction)lb.Value).Expression;
            Assert.That(fc.CodePos,Is.EqualTo("r:(f)=>".Length));
            Assert.That(fc.CodeLength,Is.EqualTo("f(0)".Length));

            var res = e.Block.Evaluate();
            
            Assert.That(res,Is.TypeOf<FsError>());
            var err = (FsError)res;
            Assert.That(err.ErrorData is CodeLocation);
            var loc = (CodeLocation)err.ErrorData;
            Assert.That(loc.Loc,Is.EqualTo("r:(f)=>".Length));
            Assert.That(loc.Length,Is.EqualTo("f(0)".Length));
        }

        [Test]
        public void TestFunctionError2()
        {
            var error_exp = "length(a)";
            var exp = $"10+{error_exp}";
            try
            {
                Helpers.Evaluate(exp);
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
            var res=Helpers.Evaluate(exp);
            Assert.That(res,Is.TypeOf<FsError>());        
        }
        [Test]
        public void TestNullMemberAccessError()
        {
            var error_exp = "x.l";
            var exp = $"10+{error_exp}";
            var res=Helpers.Evaluate(exp);
            Assert.That(res,Is.TypeOf<FsError>());
        }
        [Test]
        public void TestListMemberAccessError()
        {
            var error_exp = "[5,6].l";
            var exp = $"10+{error_exp}";
            var res=Helpers.Evaluate(exp);
            Assert.That(res,Is.TypeOf<FsError>());
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
                Helpers.EvaluateWithVars(exp, new
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
                Helpers.EvaluateWithVars(exp, new
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
                Helpers.EvaluateWithVars(exp, new { f = new Func<int, int>((x) =>
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
