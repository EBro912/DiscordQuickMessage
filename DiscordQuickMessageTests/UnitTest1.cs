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
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url");

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

            QuickMessage q = new QuickMessage("Test prompt", eb, "url");

            string expectTitle = "Title";
            string expectDescription = "Description";

            Assert.AreEqual(expectTitle, q.Embed.Title);
            Assert.AreEqual(expectDescription, q.Embed.Description);
        }

        [TestMethod]
        public void Stores_JumpUrl_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url");

            string expect = "url";

            Assert.AreEqual(expect, q.JumpUrl);
        }

        [TestMethod]
        public void Stores_Default_Positive_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url");

            string expect = "Yes";

            Assert.AreEqual(q.PositiveResponse, expect);
        }

        [TestMethod]
        public void Stores_Default_Neutral_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url");

            string expect = "Maybe";

            Assert.AreEqual(q.NeutralResponse, expect);
        }

        [TestMethod]
        public void Stores_Default_Negative_Correctly()
        {
            QuickMessage q = new QuickMessage("Test prompt", new EmbedBuilder(), "url");

            string expect = "No";

            Assert.AreEqual(q.NegativeResponse, expect);
        }
    }
}