# Bolt: SimulateProxy()

In versions before 0.4 Bolt contained a **SimulateProxy()** method on *BoltEntity*.
This has been removed in 0.4+ as it was deemed unneccesary in most scenarios and is rather trivial to implement yourself when you need it.

*SimulateProxy()* should be called on the Client that is not the Owner nor the Controller.
For more information about *Proxy* have a look at [*Who is Who?*](WhoIsWho.md)

So to implement it yourself you would implement FixedUpdate() on your BoltEntity:

    public class PlayerEntity :  Bolt.EntityBehaviour<IPlayerState> {
        void FixedUpdate()
        {
            if (!entity.isOwner && !entity.isController)
                SimulateProxy()
        }
        
        void SimulateProxy()
        {
            // Do your simulate proxy stuff here
        }

        
### Use case

**Mimicking Mecanim speed-up and slow-downs on the Proxy side.**

You might not have a seperate animation for running and jogging in your game, but instead you want to speed or respectively slow down the animation based on the speed of your character.
Bolt doesn't sync animation speed with mecanim, but you can mimick such behaviour nicely within SimulateProxy().
So in order to make the legs move faster with the increased speed you could read out the force that is applied to the character on its controller.