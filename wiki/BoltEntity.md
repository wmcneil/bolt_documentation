# The BoltEntity

The BoltEntity is a Unity gameObject that will be represented on the network by Bolt.

*BoltEntity is similar to a Unity/uLink NetworkView or Photon PhotonView. It is the representation of a network-aware object, and is the base for having Bolt control and replicate an actual GameObject in Unity.*

A BoltEntity is not a *Server or Client*, but can be:

1. *Owned*, where`entity.isOwner == true`
2. *Controlled*, where `entity.hasControl == true`
3. *Proxy*, where `entity.isOwner == false && entity.hasControl == false`

For more information about Owner, Controller and Proxies see [Who is Who?](WhoIsWho.md)

#### Synching the BoltEntity to others
To sync state from the Owner to the other Clients you'll need to create a **State**.
This can be done through Bolt's *Editor*. 
*Make sure to recompile Bolt Assets after creating/changing a State*

## Controlling a BoltEntity

To set up how a BoltEntity is controlled you'll need to override the entity's `ExecuteCommand()` and `SimulateController()` methods.
This is (usually) done for **client-side prediction**. Note that the Owner will still have full control over the actual State of the BoltEntity.

**Note that to be able to use authoritative and client predicted movement, you have to use either the transform component directly or a Character Controller component. You can not use mecanim root motion or rigidbodies to control your characters.**

#### Method: `BoltEntity.SimulateController()`
Only runs on the person which has been assigned control of an entity. This can either be the Owner that has given itself control by calling `TakeControl()` on an Entity, or it can be someone which has a remote proxy of an object and has been given control from the owner, which the owner does by calling `AssignControl(BoltConnection connection);` 
You are allowed to call QueueCommand inside of SimulateController, which is used for queuing up a command for execution.

#### Method: `BoltEntity.ExecuteCommand(BoltCommand cmd, bool resetState)`
This function runs on both the owner and controller, but has different behaviors depending on if it runs on the owner or a remote controller.

On the owner, no matter if the Owner is the controller or not, it **only runs once** for each command. No matter if the command was created locally, by the owner itself or if it came from a remote connection which has been given control. The second parameter called **resetState will never be true on the owner**.

On a controller, **if it's not the owner**, ExecuteCommand is a little bit more complex. It needs to be able to handle both local prediction of the movement but also corrections of the state from the owner. The first thing that happens every frame is that Bolt will pass in the last command which has had it's state verified from the owner, when this command get passed in the **resetState parameter will be true**.

Once the latest verified command has executed with resetState, Bolt will then run all other commands which have not been verified by the Owner yet. What this means in practice is that ExecuteCommand will be called several times for one command on a remote controller during multiple frames in a row until it has been verified by the owner.

#### What does 'resetState' entail?

This is cause for a lot of confusion, when the command which has resetState set to true executes, Bolt wants you to set the local state of the character motor you are using to the state that is represented by the command. You should not use the "input" of the command here, only the "state". This is what allows Bolt to correct the local movement of a remote controller (often a client) with the correct state from the owner (often the server).

Note that the state you need to reset is all of the state which effects movement of your entity in any way, usually this is position, rotation and some state variables like if you are grounded or crouching, etc. If you have a complex controller other things you want to reset could be velocity, acceleration, external forces, etc.

#### What is BoltCommand.isFirstExecution used for?

Code which perform direct actions from your commands, for example firing your weapon or setting the animation state - should be wrapped in a block that looks like this:

```C#
if (cmd.isFirstExecution) {
  // code ...
}
```

Since on a remote controller ExecuteCommand will be called several times for the same command, if you don't check so that things like animations and other direct actions only happen on the first execution - your character will act very weird on the remote controllers.

#### What about normal proxies, which are not the controller?

On normal proxies, which are not in direct control of the entity and are not the owner of the entity, the only callback which executes is [*SimulateProxy()*](SimulateProxy.md), neither *SimulateController()* or *ExecuteCommand()* runs on them. 

The position, rotation, animation and state sync is done through the Bolt state object for these entities. This means that any type of state, action, event, etc. you want visible to everyone connected has to be sent with either the Bolt State mechanism or over a Bolt Event,  as Bolt Commands never execute on anyone but the Controller and Owner.