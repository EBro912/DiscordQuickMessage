namespace DiscordQuickMessage
{
    public class Cooldown
    {
        // the message author who this cooldown applies to
        public ulong User { get; set; }

        // how many seconds are left
        public int SecondsLeft { get; set; }

        public Cooldown(ulong user, int seconds)
        {
            User = user;
            SecondsLeft = seconds;
        }
    }
}
