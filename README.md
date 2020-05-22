Very first basic version of the pack logger.

It works by scanning regularly (every 0.5s) whether a new pack has been opened, and then stores the contents of the pack in a file.

For now, nothing is configurable:
- The file is stored at `C:\ProgramData\logWatcher.txt`
- The scan interval is 0.5s
- The output is JSON-like (each line is a json object, but overall it does not form a json array, so if you want a full json object you need to do some small updates to the file)
- The file is appended every time a new pack is opened (it is never overwritten)
- If you launch the app while opening a pack, it will output the contents of the current pack to the file