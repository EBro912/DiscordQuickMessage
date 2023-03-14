# DiscordQuickMessage
Semester long group project for Software Engineering II at UML

The Discord Quick Message is a Discord bot that allows the user to easily reply to Discord mentions, without having to spend time thinking and typing a response back to the user, especially if they are busy and cannot respond effectively. When a user is mentioned in a server by another user, it will utilize OpenAI to generate a positive, neutral, and negative response, and display the answers to the mentioned user, allowing them to pick a response. Once a response is confirmed, it will automatically reply to the original user with the chosen response.

# Requirements
*All libraries can be found on NuGet*

Discord.Net >= 3.9.0

Microsoft.Extensions.DependencyInjection >= 7.0.0

OpenAI >= 1.6.0

Unit Test Libraries:

Microsoft.NET.Test.Sdk >= 16.11.0

MSTest.TestAdapter >= 2.2.7

MSTest.TestFramework >= 2.2.7

coverlet.collector >= 3.1.0

# How to Run
1. Clone the respository and open the .sln file in Visual Studio 2022.
2. Under Test > Test Explorer, click the dropdown on the settings cog and check off "Run Tests After Build" to auto run unit tests.
3. Press F5 or Alt+F5 to run the bot.
4. A console window will appear. After the message "Ready" is logged, the bot is ready to be interacted with.

# How to Use
1. When you are mentioned by another user in a channel, the bot will send you a Direct Message on Discord.
2. Click the corresponding number to the response you would like to send. You can also click "Ignore" to ignore the response or "Original Message" to jump to the original message.
3. After choosing a response, the bot will have you confirm your selection. Click "Yes" to send the message or "No" to start over.
4. If "Yes" is selected, the bot will send the corresponding response in the channel you were mentioned in.
