// this took way more work (and frustration) than needed


using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using quickMessage;
namespace test
{
    // test mantra: AAA
    /*
     * 1. ARRANGE
     * 2. ACT
     * 3. ASSERT
     */
    public class Test
    {
        // parameters must be string, which can simply be cast when called
        public string constructErrorMessage(string expect, string actual)
        {
            string msg = "ERROR:\nExpected: " + expect + "\nRecieved: " + actual + "\n";
            return msg;
        }
    }


    // STORAGE TESTS
    [TestClass] // REQUIRED FOR CLASSES THAT CONDUCT TESTS
    public class StorageTest : Test
    {
        [TestMethod]
        public void stores_empty_prompt_correctly()
        {
            // 1. arrange
            QuickMessage q = new QuickMessage();

            // 2. Act
            string expect = String.Empty;
            string actual = q.prompt();
            string msg = constructErrorMessage(expect, actual);

            // 3. Assert
            Assert.AreEqual<string>(expect, actual, msg);
        }
        [TestMethod]
        public void stores_prompt_correctly()
        {
            QuickMessage q = new QuickMessage("Are you coming to the meeting at 6pm?");
            string expect = "Are you coming to the meeting at 6pm?";
            string actual = q.prompt();
            string msg = constructErrorMessage(expect, actual);
            Assert.AreEqual<string>(expect, actual, msg);
        }
        [TestMethod]
        public void stores_yes_prompt_correctly()
        {
            QuickMessage q = new QuickMessage();
            string expect = "Accept this: ";
            string actual = q.yesPrompt();
            string msg = constructErrorMessage(expect, actual);
            Assert.AreEqual<string>(expect, actual, msg);
        }
        [TestMethod]
        public void stores_no_prompt_correctly()
        {
            QuickMessage q = new QuickMessage();
            string expect = "Decline this: ";
            string actual = q.noPrompt();
            string msg = constructErrorMessage(expect, actual);
            Assert.AreEqual<string>(expect, actual, msg);
        }
        [TestMethod]
        public void stores_maybe_prompt_correctly()
        {
            QuickMessage q = new QuickMessage();
            string expect = "Give a maybe response: ";
            string actual = q.maybePrompt();
            string msg = constructErrorMessage(expect, actual);
            Assert.AreEqual<string>(expect, actual, msg);
        }
        [TestMethod]
        public void stores_yes_response_correctly()
        {
            QuickMessage q = new QuickMessage();
            string expect = "YES";
            string actual = q.yesResponse();
            string msg = constructErrorMessage(expect, actual);
            Assert.AreEqual<string>(expect, actual, msg);
        }
        [TestMethod]
        public void stores_no_response_correctly()
        {
            QuickMessage q = new QuickMessage();
            string expect = "NO";
            string actual = q.noResponse();
            string msg = constructErrorMessage(expect, actual);
            Assert.AreEqual<string>(expect, actual, msg);
        }
        [TestMethod]
        public void stores_maybe_response_correctly()
        {
            QuickMessage q = new QuickMessage();
            string expect = "MAYBE";
            string actual = q.maybeResponse();
            string msg = constructErrorMessage(expect, actual);
            Assert.AreEqual<string>(expect, actual, msg);
        }
        [TestMethod]
        public void stores_message_correctly()
        {
            QuickMessage q = new QuickMessage();
            q.genMessage();
            string expect = "1: \"YES\"\n2: \"NO\"\n3: \"MAYBE\"\nCLICK BELOW TO SEND RESPONSE\n:one: :two: :three:\n";
            string actual = q.message();
            string msg = constructErrorMessage(expect, actual);
            Assert.AreEqual<string>(expect, actual, msg);
        }
        [TestMethod]
        public void genQuickMessage_works_correctly()
        {
            string expect = "1: \"YES\"\n2: \"NO\"\n3: \"MAYBE\"\nCLICK BELOW TO SEND RESPONSE\n:one: :two: :three:\n";
            string actual = QuickMessage.genQuickMessage(String.Empty);
            string msg = constructErrorMessage(expect, actual);
            Assert.AreEqual<string>(expect, actual, msg);
        }
    }
}