# AYAYABot
The AYAYABot is designed to add some cutsey anime action to your discord server.
## Features
- The bot will interact with chat messages by sending emotes on messages with AYAYA related words (defined as ayayaEmoteNames in the settings).
- The bot will send random TTS enabled messages to the default text channel on the server (messages can be customized with the randomTTSMessages section in the settings).
- The bot will greet new people that join the discord server with a welcom message and AYAYA emotes.
- The bot will randomly join voice channels and "say" cute things.
## Known Issues
- Permissions are not working correctly with the DSharpPlus libraries so the bot can't determine if it has permissions to join voice channels or send messages in text channels.
  - I plan on getting permissions working if I can figure out the DSharpPlus library
## Customizing The Bot
- The settings file can be found <a href=https://github.com/Nethegre/AYAYABot/blob/8584a8c81bf158c2e30f604d56a5bc2e5942342b/AYAYABot/appsettings.json>here</a> and will be named appsettings.json in the release/install files.
- Feel free to fork this project if you want to use it as a template for your own AYAYABot or something else.
