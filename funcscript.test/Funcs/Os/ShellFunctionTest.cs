using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;
using System.Collections.Generic;
using FuncScript.Funcs.OS;

namespace FuncScript.Test.Funcs.Os
{
    public class TestShellFunction
    {
        [Test]
        public void TestShellWithValidCommand()
        {
            var exp = "shell('echo Hello World')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is KeyValueCollection);

            var shellResult = ((KeyValueCollection)res).ConvertTo<ShellResult>();
            Assert.That(shellResult.ExitCode, Is.EqualTo(0));
            Assert.That(shellResult.Output.Count, Is.GreaterThan(0));
            Assert.That(shellResult.Output[0].Msg, Is.EqualTo("Hello World"));
            Assert.That(shellResult.Output[0].Error, Is.False);
        }

        [Test]
        public void TestShellWithCommandTimeout()
        {
            var exp = "shell('ping -n 10 127.0.0.1', 1000)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_EVALUATION));
        }

        [Test]
        public void TestShellWithInvalidCommandType()
        {
            var exp = "shell(123)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestShellWithTooManyParameters()
        {
            var exp = "shell('echo Hello', 'extra param')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestShellWithInvalidTimeoutType()
        {
            var exp = "shell('echo Hello', 'not an int')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
