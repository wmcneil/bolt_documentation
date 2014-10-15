# What is an *Authorative Server*?

An authoritative server is a setup where the client sends instructions/information to the server. Server verifies validates this, and updates the proxies and/or the client accordingly. For instance, a client tries to move 1000 m/s; however, the server knows the maximum velocity for the client is only 10 m/s. While the client, for a moment, may appear to move very far, they will be forced back into the correct position by the server. Authority over clients. Even if a player uses a hacked client to not allow the server to move them back to the correct position, the server's location will not reflect this. The hacked client will be out of sync and fairly ineffectual to use in this manner. How authoritative your server is depends on how you set everything up. It does not occur naturally unless you make it as such.

### Pros

1. Client-side hacking is much harder (near impossible in most scenarios)
2. It's clear who has the authority and that there is one 'state' of the world that is used for all clients.

### Cons

1. To avoid input delay you would need client-side prediction. This might be hard to implement in some scenarios and with certain game mechanics.
2. The Server will need to do more calculation (it will need to process all data to get the output and confirm to the client); this could make Server hardware pricier.