using DiscordQuickMessage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Discord;

namespace DiscordQuickMessageTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void Stores_Prompt_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url", 0);

            string expect = "Test prompt";

            Assert.AreEqual(expect, q.Prompt);
        }

        [TestMethod]
        public void Stores_Embed_Correctly()
        {
            EmbedBuilder eb = new EmbedBuilder()
            {
                Title = "Title",
                Description = "Description"
            };

            QuickMessage q = new QuickMessage("Test prompt", eb, "url", 0);

            string expectTitle = "Title";
            string expectDescription = "Description";

            Assert.AreEqual(expectTitle, q.Embed.Title);
            Assert.AreEqual(expectDescription, q.Embed.Description);
        }

        [TestMethod]
        public void Stores_JumpUrl_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url", 0);

            string expect = "url";

            Assert.AreEqual(expect, q.JumpUrl);
        }

        [TestMethod]
        public void Stores_Default_Positive_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url", 0);

            string expect = "Yes";

            Assert.AreEqual(q.PositiveResponse, expect);
        }

        [TestMethod]
        public void Stores_Default_Neutral_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url", 0);

            string expect = "Maybe";

            Assert.AreEqual(q.NeutralResponse, expect);
        }

        [TestMethod]
        public void Stores_Default_Negative_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url", 0);

            string expect = "No";

            Assert.AreEqual(q.NegativeResponse, expect);
        }

        [TestMethod]
        public void Stores_Message_Id_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url", 1234567);

            int expect = 1234567;

            Assert.AreEqual(q.MessageId, expect);
        }

        [TestMethod]
        public void Stores_Cooldown_User_Correctly()
        {
            Cooldown c = new Cooldown(1234567, 0);

            int expect = 1234567;

            Assert.AreEqual(c.User, expect);
        }

        [TestMethod]
        public void Stores_Cooldown_Time_Correctly()
        {
            Cooldown c = new Cooldown(0, 15);

            int expect = 15;

            Assert.AreEqual(c.SecondsLeft, expect);
        }

        [TestMethod]
        public void Is_DoNotDisturb_Non_Existing_User()
        {
            bool result = QuickMessageHandler.IsUserDoNotDisturb(0);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Is_DoNotDisturb_Existing_User()
        {
            QuickMessageHandler.ToggleDoNotDisturb(0);
            bool result = QuickMessageHandler.IsUserDoNotDisturb(0);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Is_DoNotDisturb_Existing_User_Toggle_Off()
        {
            QuickMessageHandler.ToggleDoNotDisturb(0);
            bool result = QuickMessageHandler.IsUserDoNotDisturb(0);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Cooldown_Non_Existing_User()
        {
            bool result = QuickMessageHandler.HasCooldown(0, 1);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Cooldown_Existing_User()
        {
            QuickMessageHandler.ApplyCooldown(0, 1, 5);
            bool result = QuickMessageHandler.HasCooldown(0, 1);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Empty_QuickMessage_List()
        {
            bool result = QuickMessageHandler.GetQuickMessageByMessageId(0, 1, out _);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Add_QuickMessage()
        {
            QuickMessageHandler.AddQuickMessage(0, new QuickMessage("test", new EmbedBuilder(), string.Empty, 1));
            bool result = QuickMessageHandler.GetQuickMessageByMessageId(0, 1, out _);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Stores_QuickMessage_Correctly()
        {
            QuickMessageHandler.GetQuickMessageByMessageId(0, 1, out QuickMessage q);

            string expected = "test";

            Assert.AreEqual(expected, q.Prompt);
        }

        [TestMethod]
        public void Add_Multiple_QuickMessages()
        {
            QuickMessageHandler.AddQuickMessage(0, new QuickMessage(string.Empty, new EmbedBuilder(), string.Empty, 2));
            bool result = QuickMessageHandler.GetQuickMessageByMessageId(0, 2, out QuickMessage q);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Remove_QuickMessage()
        {
            QuickMessageHandler.GetQuickMessageByMessageId(0, 1, out QuickMessage q);
            QuickMessageHandler.RemoveQuickMessage(0, q);
            bool result = QuickMessageHandler.GetQuickMessageByMessageId(0, 1, out _);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Remove_Second_QuickMessage()
        {
            QuickMessageHandler.GetQuickMessageByMessageId(0, 2, out QuickMessage q);
            QuickMessageHandler.RemoveQuickMessage(0, q);
            bool result = QuickMessageHandler.GetQuickMessageByMessageId(0, 2, out _);

            Assert.IsFalse(result);
        }

    }
}