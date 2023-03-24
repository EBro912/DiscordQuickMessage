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
    }
}