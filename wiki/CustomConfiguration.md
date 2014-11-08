# Custom Configuration

If you want to customize the bolt configuration through code before starting your server or client, you can do so by calling BoltRuntimeSettings.GetConfigCopy, modifying the returned object and then passing it as the last argument to StartClient or StartServer.

```csharp

BoltConfig config = BoltRuntimeSettings.GetConfigCopy();

// change any settings you want on the config object here,
// be aware that bolt will not verify/limit any settings when you do
// it directly in code, so you can break things completely by supplying
// incorrect/invalid config values.

BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, 27000), config);

``` 