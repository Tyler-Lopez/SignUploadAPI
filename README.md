## About

Have you ever used Rust's in-game art tool to make something that you have so much pride over that you wish you could hang onto it wipe-to-wipe? SignUploadAPI is for you: save your art and your memories.

This plug-in is a simple tool to allow players with the appropriate permission level the ability to upload the art of an in-game sign they are looking at to Imgur. The plug-in may optionally be integrated with DiscordCore, to DM the player the Imgur link or post to a channel, and/or ServerRewards, to deduct a configurable amount of RP from the player's balance. To prevent players from spamming the functionality, a configurable cooldown is included.

This plug-in is NOT a copy of [Sign Artist](https://umod.org/plugins/sign-artist), which allows players to download pictures from a web address to the server's file storage. SignUploadAPI delivers the opposite functionality: giving players a way to upload pictures from the server's file storage to a web address on Imgur.

## Configuration

```json
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

* `signuploader.use` Allows the player or group to use the uploadsign plug-in.
* `signuploader.free` Allows the player or group to use the uploadsign plug-in free of charge.

## Chat Commands

While the command is configurable, by default it is set to `/uploadsign`.
> /uploadsign "Title of Image(Optional)"
 
## Developer API

On any sign successfully being uploaded to Imgur, the following hook is called.
> OnSignUploaded(string url, string title, ulong playerID)

## Known Issues

* Photographs do not work. A future feature may be to allow for uploading of photographs and mugshots.