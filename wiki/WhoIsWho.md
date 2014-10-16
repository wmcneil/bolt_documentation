# Who is who?

Within Bolt we'll be using some jargon that might require some more information if you're unfamiliar with networking.
First of all, let's discuss the differences between *Server*, *Client*, *Connection*, *Owner*, *Controller* and *Proxy*.

#### Server

The Server is the connection that is hosting the game.

If it's an *Authorative Server* then the Server will also be the Owner of (most) BoltEntities.

#### Client

A client is normally anyone who is connected to the game who is not considered to be the Server.
In short every remote connection from the Server is a Client.

#### Connection

Within Bolt a *connection* is considered to be a link to another *Client/Server* over the network.
This means a *connection* can refer to either a *Client* or the *Server*.

If the *Server* checks his own connection it will be `null` since there's no remote link required to send/receive data.

#### Owner

When you're considered the Owner of an Entity than you're the one (by default) to have full **authority** over it.
In the case of an *Authorative Server* (most) BoltEntities would be owned by the *Server*.

Bolt considers the one that Instantiates the object (through `BoltNetwork.Instantiate()`) to be the Owner of the created BoltEntity.
Thus ownership can be set by instantiating on the required connection.

#### Controller

Both the Server and a Client can gain control of a BoltEntity. Though control can only be given by the Owner of the object.

The Owner can *take control* with: `entity.TakeControl()`.
Another Client can be *assigned control* with: `entity.AssignControl(connection)`

Note that in most scenarios the Server will be the Owner so one will often read the Server is *taking control* and other clients are *assigned control*.

#### Proxy

A BoltEntity is considered a Proxy in your connection/game if you're not the Owner or Controller.
Even though it may seem logical for some it's important to keep in mind that what might be a Proxy BoltEntity on a Client will not be on another connection (where instead it's the Owner/Controller).