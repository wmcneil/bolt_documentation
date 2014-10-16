# State Callbacks

Since Bolt 0.4+ one can define callbacks on arbitrary types.
This has made the callback method a bit more complex, but allows for much more freedom on how you set up your callbacks.

`state.AddCallback(".myStateProperty", myCallbackFunction)`


### Triggers

Note that triggers are a C# Delegate and don't require the `state.AddCallback()` method to be used. Instead you use the default *adding to a delegate*.
Like so:

`state.myTrigger += myCallbackFunction`