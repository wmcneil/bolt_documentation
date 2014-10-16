# The BoltEntity

The BoltEntity is a Unity gameObject that will be represented on the network by Bolt. 

A BoltEntity is not a *Server or Client*, but can be:

1. *Owned*, where`entity.isOwner == true`
2. *Controlled*, where `entity.isController == true`
3. *Proxy*, where `entity.isOwner == false && entity.isController == false`

For more information about Owner, Controller and Proxies see [Who is Who?](WhoIsWho.md)

#### Synching the BoltEntity to others
To sync state from the Owner to the other Clients you'll need to create a **State**.
This can be done through Bolt's *Editor*. 
*Make sure to recompile Bolt Assets after creating/changing a State*

#### Controlling a BoltEntity

To set up how a BoltEntity is controlled you'll need to override the entity's `ExecuteCommand()` and `SimulateController()` methods.
This is (usually) done for **client-side prediction**. Note that the Owner will still have full control over the actual State of the BoltEntity.