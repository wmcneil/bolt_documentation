# State Callbacks

Since Bolt 0.4+ one can define callbacks on arbitrary types.
This has made the callback method a bit more complex, but allows for much more freedom on how you set up your callbacks.

`state.AddCallback("myStateProperty", myCallbackFunction)`


### Triggers

Note that triggers are a C# Delegate and don't require the `state.AddCallback()` method to be used. Instead you use the default *adding to a delegate*.
Like so:

`state.myTrigger += myCallbackFunction`


### Callbacks on arrays

You can set up a callback on an array in an array like so:

There is an overload to `state.AddCallback` which takes a delegate that takes three parameters in the format of `(Bolt.IState, string path, Bolt.ArrayIndices indices)`.

1. **Bolt.IState** can be cast to your state type within the method (note that Bolt.IState is the class all States inherit from).
    - This is your actual State though it's just not strongly typed (to have a generic AddCallback method); that's the reason you need to cast it to your own type.
2. **string path** is the *full* path to the property.
3. **Bolt.ArrayIndices indices** contains the indices for any of the arrays that you attached in the callback.

**Example 1a: Array callback with method**

```C#
state.AddCallback("stats[]", OnStatsChanged); 
```

```C#
public void OnStatsChanged(IState state, string path, Bolt.ArrayIndices indices)
    {
        int index = indices[0];
        IActorState actorState = (IActorState)state;
        // The changed property:
        // actorState.stats[index]
    }
```

**Example 1b: Array callback with lambda**

```C#
state.AddCallback("stats[]", (x, y, z) => { OnStatsChanged((IActorState)x, (string)y, (Bolt.ArrayIndices)z); }); 
```

```C#
public void OnStatsChanged(IActorState state, string path, Bolt.ArrayIndices indices)
    {
        int index = indices[0];
    }
```

**Example 2a: Nested array with method**

```C#
state.AddCallback("vaults[].contents[]",  OnVaultContentsChanged); 
```

```C#
public void OnVaultContentsChanged(IState state, string path, Bolt.ArrayIndices indices)
    {
        IActorState actorState = (IActorState)state;
        int vaultIndex = indices[0];
        int index = indices[1];
        int itemID = actorState.vaults[vaultIndex].contents[index];
    }
```

**Example 2b: Nested array callback with lambda**

```C#
state.AddCallback("vaults[].contents[]", (x, y, z) => { OnVaultContentsChanged((IActorState)x, (string)y, (Bolt.ArrayIndices)z); }); 
```

```C#
public void OnVaultContentsChanged(IActorState state, string path, Bolt.ArrayIndices indices)
    {
        int vaultIndex = indices[0];
        int index = indices[1];
        int itemID = state.vaults[vaultIndex].contents[index];
    }
```

**Example 3: Using a look-up table for methods in your struct callback**

If you have defined a struct in your State and you want a single global callback instead of setting a callback for each of its fields and you still want a separate method to be called per specific property then it's recommended to use a look-up table as opposed to a switch statement.   
*This is more of an optimization than a necessity, also this will only be of any benefit if the callback is triggered a lot and there are many fields in your struct.*

Defining your look up table of Actions:

```C#
Dictionary<string, Action> lookupTable = new Dictionary<string, Action>()
{
    { "equippedItems.head", UpdateArmor(state.equippedItems.head, 0) },
    { "equippedItems.body", UpdateArmor(state.equippedItems.body, 1) },
    { "equippedItems.arms", UpdateArmor(state.equippedItems.arms, 2) }
};


Action UpdateArmor(Item item, int index) {
  return () => UpdateUMAArmor(item, index);
}
```

**Note that you can't acces the State's parameters yet until it has been attached. Therefore you should fill up the look-up table within Attached() OR do a look-up table that is initialized with lazy access. Otherwise you might end up with the following error:*

> Error *X* An object reference is required for the non-static field, method, or property 'Bolt.EntityBehaviour<IPlayerState>.state.get' 

And this would be used to add a callback:

```C#
state.AddCallback("equippedItems", UpdateNewArmor); 
```

And this is the method:

```C#
public void UpdateNewArmor(Bolt.IState state, string path, Bolt.ArrayIndices indices)
  {
    lookupTable[path]();
  }
```



