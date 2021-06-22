
[![SCAML License](https://img.shields.io/badge/license-SCAML-green)](https://github.com/siscodeorg/SCAML)
![](https://img.shields.io/github/contributors-anon/siscodeorg/sisbase.net)
![](https://img.shields.io/github/last-commit/siscodeorg/sisbase.net)
# sisbase.net

Collection of helpful abstractions for creating discord bots.


## Features

- "Frequently" updated
- Advanced module system
- Async by default
- No unwanted caching nonsense
- Cross-platform


  
## Demo

```csharp
using Sisbase;
using System.Threading.Tasks;

class Program {
    static async Task Main(){
        SisbaseBot bot = new ("path/to/config.json");
        bot.Loads(typeof(Program).Assembly);
        await bot.RunAsync();
    }
}
```

Creating a discord bot is that easy.

Then its just making commands as you normally would, and sprinkle some systems where you deem necessary.

The `Program.cs` file is done. Nothing else is needed there. Worry about making cool stuff, and leave the boring bits to us. 
  
## Roadmap

- Interactivity (With awaitables `eg. SocketMessage#WaitForReactionAsync`)
- Discord Buttons
- Encryption ([next#2](https://github.com/siscodeorg/next/issues/2))
  
