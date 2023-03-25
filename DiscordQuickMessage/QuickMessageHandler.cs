using System.Diagnostics.CodeAnalysis;

namespace DiscordQuickMessage
{
    public class QuickMessageHandler
    {
        // a dictionary to store current active quick message responses
        // the key is the user id and the value is the quick responses the user can interact with
        private static Dictionary<ulong, List<QuickMessage>> activeQuickMessages = new Dictionary<ulong, List<QuickMessage>>();

        // users who are currently in do not disturb mode
        private static HashSet<ulong> doNotDisturbUsers = new HashSet<ulong>();

        // helper function to get a quickmessage from a user's list of quickmesssages
        public static bool GetQuickMessageByMessageId(ulong userId, ulong messageId, [MaybeNullWhen(false)] out QuickMessage quickMessage)
        {
            if (activeQuickMessages.ContainsKey(userId))
            {
                quickMessage = activeQuickMessages[userId].FirstOrDefault(x => x.MessageId == messageId);
                if (quickMessage != null)
                {
                    return true;
                }
            }
            quickMessage = default;
            return false;
        }

        // helper function to add quickmessages 
        public static void AddQuickMessage(ulong userId, QuickMessage quickMessage)
        {
            if (activeQuickMessages.ContainsKey(userId))
            {
                activeQuickMessages[userId].Add(quickMessage);
            }
            else
            {
                activeQuickMessages.Add(userId, new List<QuickMessage> { quickMessage });
            }
        }

        // helper function to remove a quickmessage once it is no longer needed
        public static void RemoveQuickMessage(ulong userId, QuickMessage quickMessage)
        {
            if (activeQuickMessages.ContainsKey(userId))
            {
                activeQuickMessages[userId].Remove(quickMessage);
                if (activeQuickMessages[userId].Count == 0)
                {
                    activeQuickMessages.Remove(userId);
                }
            }
        }

        // toggles do not disturb mode for a user
        // returns if the user turned it on or off
        public static bool ToggleDoNotDisturb(ulong user)
        {
            if (doNotDisturbUsers.Contains(user))
            {
                doNotDisturbUsers.Remove(user);
                return false;
            }
            doNotDisturbUsers.Add(user);
            return true;
        }

        // checks if a user has do not disturb mode on
        public static bool IsUserDoNotDisturb(ulong user)
        {
            return doNotDisturbUsers.Contains(user);
        }


    }
}
