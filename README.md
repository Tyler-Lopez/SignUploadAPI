# SignUploadAPI
 
## TO-DO
TODO: Oxide permissions, create a hook on successful upload. Complete README

## About 
This plug-in is a simple tool to allow players with the appropriate permission level the ability to upload the art of an in-game sign they are looking at to Imgur. The plug-in may optionally be integrated with DiscordCore, to DM the player the Imgur link or post to a channel, and/or ServerRewards, to deduct a configurable amount of RP from the player's balance. To prevent players from spamming the functionality, a configurable cooldown is included.

This plug-in is NOT a copy of [Sign Artist](https://umod.org/plugins/sign-artist), which allows players to download pictures from a web address to the server's file storage. This plug-in delivers the opposite functionality: giving players a way to upload pictures from the server's file storage to a web address on Imgur.

## Examples
<img width=500 src=https://user-images.githubusercontent.com/77797048/125180937-df8c6f00-e1cd-11eb-88a9-fd506f6c8c51.png>
<img width=500 src=https://user-images.githubusercontent.com/77797048/125180922-ae13a380-e1cd-11eb-84c6-816a6c581bdc.png>
<img width=500 src=https://user-images.githubusercontent.com/77797048/125181111-8e7d7a80-e1cf-11eb-9cf8-a622207f25bb.png>


## Configuration
```
{
  "Chat Command": "uploadsign",
  "Use Discord Core": false,
  "Discord Channel to Upload to": "",
  "Use Server Rewards": false,
  "Cost to Upload": 20,
  "Cooldown Between Uploads (seconds)": 30.0
}
```

* `Use Discord Core`: Upon enabling this, if DiscordCore is successfully loaded players who have linked their Discord accounts with their Steam accounts on the server will receive a DM including the picture. This does not post to a channel on it's own.
* `Discord Channel to Upload to`: DiscordCore will post the file to the specified channel on your Discord server.
* `Cost to Upload`: This is the amount of RP which is deducted upon an upload attempt to Imgur if `Use Server Rewards` is set to true.  

## Permissions
```This plugin uses Oxide's permission system. To assign a permission, use oxide.grant <user or group> <name or steam id> <permission>. To remove a permission, use oxide.revoke <user or group> <name or steam id> <permission>.```

* `signuploader.use` Allows the player or group to use the uploadsign plug-in.
* `signuploader.free` Allows the player or group to use the uploadsign plug-in free of charge.

## Commands

### Upload a Sign
While the command is configurable, by default it is set to `/uploadsign`.
> /uploadsign "Title of Image(Optional)"
> 
## Developer API
On any sign successfully being uploaded to Imgur, the following hook is called.
> OnSignUploaded(string url, string title, ulong playerID)


