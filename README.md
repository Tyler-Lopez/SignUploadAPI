## About

Have you ever used Rust's in-game art tool to make something that you have so much pride over that you wish you could hang onto it wipe-to-wipe? SignUploadAPI is for you: save your art and your memories.

This plug-in is a simple tool to allow players with the appropriate permission level the ability to upload the art of an in-game sign they are looking at to Imgur. The plug-in may optionally be integrated with DiscordCore, to DM the player the Imgur link or post to a channel, and/or ServerRewards, to deduct a configurable amount of RP from the player's balance. To prevent players from spamming the functionality, a configurable cooldown is included.

This plug-in is NOT a copy of [Sign Artist](https://umod.org/plugins/sign-artist), which allows players to download pictures from a web address to the server's file storage. SignUploadAPI delivers the opposite functionality: giving players a way to upload pictures from the server's file storage to a web address on Imgur.

If a Photo Frame is selected and contains a Photo, the photo file itself will be exported.

## Examples

![image](https://user-images.githubusercontent.com/77797048/125203743-cf18da80-e247-11eb-9f0d-1cf63aa2f976.png)

![image](https://user-images.githubusercontent.com/77797048/125203698-8d882f80-e247-11eb-8050-2ee932a5042b.png)

![image](https://user-images.githubusercontent.com/77797048/125203746-d213cb00-e247-11eb-829c-531bfb5ef517.png)

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

* `signuploadapi.use` Allows the player or group to use the plugin.
* `signuploadapi.free` Allows the player or group to use the plugin free of charge.

## Chat Commands

While the command is configurable, by default it is set to `/uploadsign`.
* `/uploadsign "Title of Image(Optional)"`
 
## Developer API

On any sign successfully being uploaded to Imgur, the following hook is called.
```csharp
void OnSignUploaded(string url, string title, ulong playerID)
```
