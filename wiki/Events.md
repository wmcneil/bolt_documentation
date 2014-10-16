#Events

Events are Bolt's way of setting up a RPC-like call over the network.



#### *Calling* a global event (global events are always reliable):

    using(var evnt = ChangeDoorState.Raise(Bolt.GlobalTargets.Everyone))  
    {  
            evnt.isOpen = b;  
    }  
    
(In-editor: [http://i.imgur.com/91ezcj6.png](http://i.imgur.com/91ezcj6.png))

#### *Calling* an entity event (entity events are unreliable):

	using(var casingEvnt = SpawnCasing.Raise(entity))
	{
		casingEvnt.cPosition = weaponStatsScript.caseEjectionPoint.position;
		casingEvnt.gunRef = correctIndex;
	}

(In-editor: [http://i.imgur.com/tbsryoO.png](http://i.imgur.com/tbsryoO.png)

#### *Receiving* an event:
Make sure your script inherits from either:  

	Bolt.GlobalEventListener  
or  

	Bolt.EntityEventListener / Bolt.EntityEventListener<StateNameGoesHere> 
    
*(Note: These inherit from Bolt.EntityBehaviour already)*

Then, to receive an event:

	public override void OnEvent (ChangeDoorState evnt)
	{
		if(evnt.isOpen == true)
			SendMessage("PlayLinkedSound");  
	}