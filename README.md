# AYAYABot
The AYAYABot is designed to add some cutsey anime action to your discord server.
## Features
- The bot will interact with chat messages by sending emotes on messages with AYAYA related words.
- The bot will send random TTS enabled messages to random text channels on the server.
- The bot will greet new people that join the discord server with a welcom message and AYAYA emotes.
- The bot will randomly join voice channels and "say" cute things.
## Known Issues
- No known issues at this time, feel free to create an issue if you find one.
## Running The Bot
- You will need to create a discord application/bot before you can run the bot : https://discord.com/developers/docs/getting-started
- Through the bot creation process you will create a token used for authentication, you will need to create the file "bottoken.txt" and place it in the main directory for the bot. The code will use that file in order to authenticate against Discord.
  - <b>NOTE:</b> Never share the token value with anyone as it will allow them to login as your bot.
## Customizing The Bot
- The settings file can be found <a href=https://github.com/Nethegre/AYAYABot/blob/8584a8c81bf158c2e30f604d56a5bc2e5942342b/AYAYABot/appsettings.json>here</a> and will be named appsettings.json in the release/install files.
  - The settings contain a lot of internal timers that control how often the bot has a chance to interact with your discord server, the timers will be pretty long by default so feel free to lengthen them if you want more action.
- Feel free to fork this project if you want to use it as a template for your own AYAYABot or something else.
## Special Thanks
- <b>DSharpPlus</b> for providing the discord integration library : https://dsharpplus.github.io/
- <b>Artosis</b> and his chat for giving me plenty of AYAYA inspiration : https://www.twitch.tv/artosis
- <b>Cika</b> for providing some good audio clips
## Feedback
- If you have feedback or suggestions feel free to create an issue or message me on discord : Nethegre#2803
