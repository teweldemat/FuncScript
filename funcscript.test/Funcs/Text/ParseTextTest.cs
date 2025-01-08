using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestParseTextFunction
    {
        [Test]
        public void TestParseHexValid()
        {
            var exp = "parse('FF', 'hex')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(255));
        }

        [Test]
        public void TestParseHexInvalid()
        {
            var exp = "parse('GG', 'hex')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestParseLongValid()
        {
            var exp = "parse('123456789', 'l')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(123456789L));
        }

        [Test]
        public void TestParseLongInvalid()
        {
            var exp = "parse('abc', 'l')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestParseFsScriptInvalid()
        {
            var exp = "parse('1+2', 'fs')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestParseWithNoParams()
        {
            var exp = "parse()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestParseWithNull()
        {
            var exp = "parse(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestParseWithEmptyFormat()
        {
            var exp = "parse('test', '')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("test"));
        }
    }
}
