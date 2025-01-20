using System.Collections.Generic;
using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;
using FuncScript.Funcs.OS;

namespace FuncScript.Test.Funcs.Os
{
    public class TestShellFunction
    {
        [Test]
        public void TestShellWithValidCommand()
        {
            var exp = "shell('echo Hello World')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is KeyValueCollection);
            var shellResult = ((KeyValueCollection)res).ConvertTo<ShellResult>();
            Assert.That(shellResult.ExitCode, Is.EqualTo(0));
            var output=shellResult.Output as List<ShellMessage>;
            Assert.NotNull(output);
            Assert.That(output.Count, Is.GreaterThan(0));
            Assert.That(output[0].Msg.Trim(), Is.EqualTo("Hello World"));
            Assert.That(output[0].Error, Is.False);
        }

        [Test]
        public void TestShellWithCommandTimeout()
        {
            var exp = "shell('ping -n 10 127.0.0.1', 1000)";
            var res = Helpers.Evaluate(exp);
            if (res is KeyValueCollection)
            {
                var shellResult = ((KeyValueCollection)res).ConvertTo<ShellResult>();
                Assert.That(shellResult.ExitCode, Is.Not.EqualTo(0));
            }
            else if (res is FsError error)
            {
                Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_EVALUATION));
                Assert.That(error.ErrorMessage, Does.Contain("Command timed out"));
            }
            else
            {
                Assert.Fail("Expected a ShellResult or FsError due to timeout.");
            }
        }

        [Test]
        public void TestShellWithInvalidCommandType()
        {
            var exp = "shell(123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError, "Expected an error due to invalid command type.");

            var shellResult = res as FsError;
            Assert.That(shellResult, Is.Not.Null);
            Assert.That(shellResult.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
            Assert.That(shellResult.ErrorMessage, Does.Contain("Type mismatch. First parameter (command) must be string."));
        }

        [Test]
        public void TestShellWithTooManyParameters()
        {
            var exp = "shell('echo Hello', 'extra param')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError, "Expected an error due to invalid time out parameter type.");

            var shellResult = res as FsError;
            Assert.That(shellResult, Is.Not.Null);
            Assert.That(shellResult.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestShellWithInvalidTimeoutType()
        {
            var exp = "shell('echo Hello', 'not an int')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError, "Expected an error due to invalid timeout type.");

            var shellResult = res as FsError;
            Assert.That(shellResult, Is.Not.Null);
            Assert.That(shellResult.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
            Assert.That(shellResult.ErrorMessage, Does.Contain("Second parameter (timeout) must be int."));
        }

        [Test]
        public void TestShellWithReducer()
        {
            // Test using a reducer that concatenates messages with a counter
            var exp = "shell('echo Line1 && echo Line2', (msg, acc) => if(acc = null,msg.Msg, acc + ' | ' + msg.Msg))";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is KeyValueCollection);
            
            var shellResult = ((KeyValueCollection)res).ConvertTo<ShellResultAccumulator>();
            Assert.That(shellResult.ExitCode, Is.EqualTo(0));
            Assert.That(shellResult.Output, Is.TypeOf<string>());
            Assert.That(shellResult.Output.ToString(), Is.EqualTo("Line1 | Line2"));
        }

        [Test]
        public void TestShellWithReducerAndBreak()
        {
            // Test using a reducer that accumulates lines until it sees "Line2" and then breaks
            var exp = "shell('echo Line1 && echo Line2 && echo Line3', (msg, acc, idx) => " +
                     "if(msg.Msg = 'Line2', break(acc + ' | ' + msg.Msg), " +
                     "if(acc = null, msg.Msg, acc + ' | ' + msg.Msg)))";
            
            var res = Helpers.Evaluate(exp);
            Assert.That(res is KeyValueCollection);
            
            var shellResult = ((KeyValueCollection)res).ConvertTo<ShellResultAccumulator>();
            Assert.That(shellResult.Output, Is.TypeOf<string>());
            Assert.That(shellResult.Output.ToString(), Is.EqualTo("Line1 | Line2"));
        }
    }
}
