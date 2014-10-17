# Bolt: SimulateProxy()

You could use a `SimulateProxy()` method to perform a *simulation* on the Client where the Entity is a Proxy.

In versions before 0.4 Bolt contained a **SimulateProxy()** method on *BoltEntity*.
This has been removed in 0.4+ as it was deemed unneccesary in most scenarios and is rather trivial to implement yourself when you need it.

*SimulateProxy()* should be called on the Client that is not the Owner nor the Controller.
For more information about *Proxy* have a look at [*Who is Who?*](WhoIsWho.md)

So to implement it yourself you would implement FixedUpdate() on your BoltEntity:

    public class PlayerEntity :  Bolt.EntityBehaviour<IPlayerState> {
        void FixedUpdate()
        {
            if (!entity.isOwner && !entity.hasControl)
                SimulateProxy()
        }
        
        void SimulateProxy()
        {
            // Do your simulate proxy stuff here
        }
    }
 
## Use cases

**Mimicking Mecanim speed-up and slow-downs on the Proxy side.**

You might not have a seperate animation for running and jogging in your game, but instead you want to speed or respectively slow down the animation based on the speed of your character.
Bolt doesn't sync animation speed with mecanim, but you can mimick such behaviour nicely within SimulateProxy().
So in order to make the legs move faster with the increased speed you could read out the force that is applied to the character on its controller.

**Providing your own smoothing/prediction for State properties**

You could predict/adjust local values based on the State values and your own algorithm to modify the proxies' value that better represents how the value would change between updates within your game mechanics. This could allow you to perform a *sense of client-side prediction* that could work better than interpolation or extrapolation.

## Examples

### Implement a custom interpolation for State property on Proxy side

You could Lerp a locally stored value towards the one in the State.

    public class PlayerEntity :  Bolt.EntityBehaviour<IPlayerState> {
    
        int _health;
    
        void FixedUpdate()
        {
            if (!entity.isOwner && !entity.hasControl)
                SimulateProxy()
        }
        
        void SimulateProxy()
        {
            // Lerp our local value towards the new one in the State to smooth it in our own way
            _health = Mathf.Lerp(_health, State.health, 0.5f);
        }
    }
       
