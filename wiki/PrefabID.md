# The Prefab ID

Each prefab which is compiled gets a unique id which is also refered as the "Prefab id"

## Uses of the Prefab ID

The prefab id can be used for instantiating bolt prefabs with their ID.
If you would like to make a drop system for objects in your game
and you want to transmit a prefab in an event then you just add a new property to your event.
Let's say "ItemToDrop" and define him as "Prefab id" and then do something like:

	void DropItem()
	{
		using(var evnt = DropItemEvent.Raise(Bolt.GlobalTargets.Server))
		{
			evnt.ItemToDrop = Item.DropPrefab.GetCompoment<BoltEntity>().ModifySettings().prefabid;
			
			//ModifySettings - this isnt for editing the entity, its for getting the modifysettings of it.
		}
	}

	override OnEvent(DropItemEvent evnt)
	{
	
	BoltNetwork.Instantiate(evnt.ItemToDrop); //Instantiate a prefab with the prefab id that we transmited in the event.
	
	}

