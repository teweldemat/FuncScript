using System;
using System.Linq;
using FuncScript.Core;
using NUnit.Framework;

namespace FuncScript.Test
{
    [TestFixture]
    public class TestGetLiteralMatch
    {
        [Test]
        public void TestExactMatch()
        {
            string exp = "Hello, world!";
            int index = FuncScriptParser.GetLiteralMatchInternal(exp, 0, new[] { "Hello" }).NextIndex;
            Assert.AreEqual(5, index);
        }

        [Test]
        public void TestNoMatch()
        {
            string exp = "Hello, world!";
            int index = FuncScriptParser.GetLiteralMatchInternal(exp, 0, new[] { "Goodbye" }).NextIndex;
            Assert.AreEqual(0, index);
        }

        [Test]
        public void TestCaseInsensitive()
        {
            string exp = "Hello, world!";
            int index = FuncScriptParser.GetLiteralMatchInternal(exp, 0, new[] { "HELLO" }).NextIndex;
            Assert.AreEqual(5, index);
        }

        [Test]
        public void TestMultipleKeywords()
        {
            string exp = "Hello, world!";
            int index = FuncScriptParser.GetLiteralMatchInternal(exp, 0, new[] { "Goodbye", "Hello", "Hi" }).NextIndex;
            Assert.AreEqual(5, index);
        }

        [Test]
        public void TestIndexOutOfBounds()
        {
            string exp = "Hello, world!";
            int index = FuncScriptParser.GetLiteralMatchInternal(exp, 20, new[] { "Hello" }).NextIndex;
            Assert.AreEqual(20, index);
        }

        [Test]
        public void TestEmptyString()
        {
            string exp = "";
            int index = FuncScriptParser.GetLiteralMatchInternal(exp, 0, new[] { "Hello" }).NextIndex;
            Assert.AreEqual(0, index);
        }

        [Test]
        public void TestStartIndexWithinSubstring()
        {
            string exp = "Hello, world!";
            int index = FuncScriptParser.GetLiteralMatchInternal(exp, 7, new[] { "world" }).NextIndex;
            Assert.AreEqual(12, index);
        }

        [Test]
        public void TestKeywordAtEndOfSubstring()
        {
            string exp = "Hello, world!";
            int index = FuncScriptParser.GetLiteralMatchInternal(exp, 12, new[] { "!" }).NextIndex;
            Assert.AreEqual(13, index);
        }

        [Test]
        public void TestNullString()
        {
            string exp = null;
            Assert.Throws<ArgumentNullException>(() => FuncScriptParser.GetLiteralMatchInternal(exp, 0, new[] { "Hello" }));
        }

        [Test]
        public void StressTest()
        {
            // Create a random generator
            Random rnd = new();

            // Create a large input string with random characters
            int prefixLength = 99_999_999;
            int suffixLength = 99_999_999;
            string exp = RandomString(prefixLength, rnd) + "Hello, world!" + RandomString(suffixLength, rnd);

            // Create an array of keywords, including the target keyword
            int keywordCount = 9_999_999;
            string[] keywords = Enumerable.Range(1, keywordCount).Select(x => "kw" + x).ToArray();

            // Randomly place the target keyword in the array
            int targetKeywordIndex = rnd.Next(keywordCount);
            keywords[targetKeywordIndex] = "Hello, world!";

            // Measure execution time
            var timer = System.Diagnostics.Stopwatch.StartNew();
            int index = FuncScriptParser.GetLiteralMatchInternal(exp, prefixLength, keywords).NextIndex;
            timer.Stop();

            // Check if the result is correct
            Assert.AreEqual(prefixLength + 13, index);

            // Check if the execution time is reasonable (e.g., less than 10 seconds)
            Assert.Less(timer.ElapsedMilliseconds, 10000);
        }

        // Generate a random string of the specified length using the provided random generator
        private string RandomString(int length, Random rnd)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
