# Bolt API Documentation
[Bolt.ArrayIndices](#Bolt.ArrayIndices)
[BoltConnection](#BoltConnection)
[BoltEntity](#BoltEntity)
[BoltHitbox](#BoltHitbox)
[BoltHitboxBody](#BoltHitboxBody)
[BoltNetwork](#BoltNetwork)
[BoltPhysicsHit](#BoltPhysicsHit)
[BoltPhysicsHits](#BoltPhysicsHits)
[Bolt.Command](#Bolt.Command)
[EntityArray](#EntityArray)
[EntityArrayModifier](#EntityArrayModifier)
[Bolt.EntityBehaviour](#Bolt.EntityBehaviour)
[Bolt.EntityBehaviour`1](#Bolt.EntityBehaviour`1)
[Bolt.EntityTargets](#Bolt.EntityTargets)
[Bolt.Event](#Bolt.Event)
[FloatArray](#FloatArray)
[FloatArrayModifier](#FloatArrayModifier)
[BoltInternal.GlobalEventListenerBase](#BoltInternal.GlobalEventListenerBase)
[Bolt.GlobalTargets](#Bolt.GlobalTargets)
[Bolt.IPriorityCalculator](#Bolt.IPriorityCalculator)
[Bolt.IState](#Bolt.IState)
[IntegerArray](#IntegerArray)
[IntegerArrayModifier](#IntegerArrayModifier)
[TransformArray](#TransformArray)
## Bolt.ArrayIndices
 Contains the indices for all arrays that contain a specific property, in order 
#### *int* Length
 The amount of indices 
## BoltConnection
 Represents a connection from a remote host 
#### *bool* isLoadingMap
 Returns true if the remote computer on the other end of this connection is loading a map currently 
#### *UdpKit.UdpConnection* udpConnection
 The underlying UdpKit connection for this connection 
#### *int* remoteFrame
 This is the estimated remote frame of the other end of this connection 
#### *float* ping
 Ping (in seconds) of this connection 
#### *float* pingAliased
 Aliased ping (in seconds) of this connection 
#### *int* bitsPerSecondIn
 How many bits per second we are receiving in 
#### *int* bitsPerSecondOut
 How many bits per second we are sending out 
#### *UdpKit.UdpEndPoint* remoteEndPoint
 Remote end point of this connection 
#### *object* userToken
 User assignable token which lets you pair arbitrary data with the connection 
#### void Disconnect()

 Disconnect this connection 
#### int GetSkippedUpdates(BoltEntity en)

 How many updates have been skipped for the entity to this connection 
## BoltEntity
 Enables a game object to be tracked by Bolt over the network 
#### *Bolt.UniqueId* uniqueId
 The unique id of this object, can be assigned by calling BoltEntity.SetUniqueId 
#### *BoltConnection* source
 If this entity was created on another computer, contains the connection we received this entity from, otherwise null 
#### *BoltConnection* controller
 If this entity is controlled by a remote connection it contains that connection, otherwise null 
#### *bool* isAttached
 If this entity is attached to Bolt or not 
#### *bool* isSceneObject
 This is a scene object placed in the scene in the Unity editor 
#### *bool* isOwner
 Did the local computer create this entity or not? 
#### *bool* hasControl
 Do we have control of this entity? 
#### *bool* hasControlWithPrediction
 Do we have control of this entity and are we using client side prediction 
#### *bool* persistsOnSceneLoad
 Should this entity persist between scene loads 
#### BoltEntitySettingsModifier ModifySettings()

 Creates an object which lets you modify the internal settings of an entity before it is attached to Bolt. 
#### void SetScopeAll(bool inScope)
- **inScope** If this entity should be in scope or not

 Sets the scope of all currently active connections for this entity. Only usable if Scope Mode has been set to Manual. 
#### void SetScope(BoltConnection connection, bool inScope)
- **inScope** If this entity should be in scope or not

 Sets the scope for the connection passed in for this entity. Only usable if Scope Mode has been set to Manual. 
#### void SetParent(BoltEntity parent)
- **parent** The parent of this entity

 Sets the parent of this entity 
#### void TakeControl()

 Takes local control of this entity 
#### void ReleaseControl()

 Releases local control of this entity 
#### void SetUniqueId(Bolt.UniqueId id)
- **id**

#### void AssignControl(BoltConnection connection)
- **connection** The connection to assign control to

 Assigns control of this entity to a connection 
#### void RevokeControl()

 Revokes control of this entity from a connection 
#### bool IsController(BoltConnection connection)
- **connection** The connection to check

 Checks if this entity is being controlled by the connection 
#### bool QueueInput(Bolt.ICommandInput command)
- **command** The command to queue

 Queue a command not his entity for execution. This is called on a client which is controlling a proxied entity the command will also be sent to the server. 
#### void Idle(BoltConnection connection)
- **connection** The connection to idle the entity on

 Set this entity as idle on the supplied connection, this means that the connection will not receive update state for this entity as long as it's idle. 
#### void Wakeup(BoltConnection connection)
- **connection** The connection to wake the entity up on

 Wakes this entity up from being idle on the supplied connection, this means that the connection will start receiving updated state for this entity 
#### void AddEventListener(MonoBehaviour behaviour)
- **behaviour** The behaviour to invoke event callbacks on

 Add an event listener to this entity. 
#### void RemoveEventListener(MonoBehaviour behaviour)
- **behaviour** The behaviour to remove

 Remove an event listern from this entity 
#### TState GetState()

 Get the state if this entity 
#### bool StateIs()

 Checks which type of state this entity has 
## BoltHitbox
 Defines one hitbox on a BoltHitboxBody 
#### *BoltHitboxShape* hitboxShape
 Shape of the hitbox (box or sphere) 
#### *BoltHitboxType* hitboxType
 Type of the hitbox 
#### *Vector3* hitboxCenter
 Center of the hitbox in local coordinates 
#### *Vector3* hitboxBoxSize
 Size of the hitbox if this shape is a box 
#### *float* hitboxSphereRadius
 Radius of the hitbox if this shape is a sphere 
## BoltHitboxBody
 Defines a body of hitboxes to be tracked 
## BoltNetwork
 Holds global methods and properties for starting and stopping bolt, instantiating prefabs and other utils 
#### *int* frame
 The current local simulation frame number 
#### *System.Collections.Generic.IEnumerable&lt;BoltEntity&gt;* entities
 The current server simulation frame number 
#### *int* serverFrame
 On the server this returns the local frame, on a client this returns the currently estimated frame of all server objects we have received 
#### *float* serverTime
 The current server simulation time 
#### *float* time
 The local time, same as Time.time 
#### *float* frameDeltaTime
 The fixed frame delta, same as Time.fixedDeltaTime 
#### *float* frameBeginTime
 The time the last fixed update begain, same as Time.fixedTime 
#### *float* frameAlpha
 Normalized value of how much time have passed since the last FixedUpdate 
#### *System.Collections.Generic.IEnumerable&lt;BoltConnection&gt;* connections
 All the connections connected to this host 
#### *System.Collections.Generic.IEnumerable&lt;BoltConnection&gt;* clients
 All clients connected to this host 
#### *BoltConnection* server
 The server connection 
#### *int* framesPerSecond
 How many FixedUpdate frames per second bolt is configured to run 
#### *bool* isServer
 Returns true if this host is a server 
#### *bool* isClient
 Returns true if this host is a client 
#### *bool* isRunning
 If bolt is running 
#### *bool* isDebugMode
 Returns true if Bolt was compiled in debug mode 
#### *Bolt.ScopeMode* scopeMode
 The scoping mode active 
#### *GameObject* globalObject
 The global object that all global behaviours will be attached to 
#### BoltEntity FindEntity(Bolt.UniqueId id)
- **id** The id to look up

 Find an entity based on unique id 
#### BoltEntity Instantiate(GameObject prefab)
- **prefab** The prefab to use

 Instantiates and attaches an instance of this prefab to Bolt 
#### BoltEntity Instantiate(Bolt.PrefabId prefabId)
- **prefabId** The prefab id to create an instance of

 Instantiates and attaches an instance of this prefab to Bolt 
#### GameObject Attach(GameObject gameObject)
- **gameObject** The game object that contains the Bolt Entity component

 Attaches a manually configured entity to bolt 
#### void Detach(GameObject gameObject)
- **gameObject** The gameobject holding the entity

 Detaches an entity from bolt 
#### BoltPhysicsHits OverlapSphereAll(Vector3 origin, float radius)
- **origin** The origin of the sphere
- **radius** The radius of the sphere

 Perform a sphere overlap against Bolt hiboxes 
#### void Accept(UdpKit.UdpEndPoint ep)
- **ep** The endpoint to access the connection from

 Accept a connection from a specific endpoint, only usable if Accept Mode has been set to Manual 
#### void Refuse(UdpKit.UdpEndPoint ep)
- **ep** The endpoint to refuse the connection from

 Refuse a connection from a specific endpoint, only usable if Accept Mode has been set to Manual 
#### void AddGlobalEventListener(MonoBehaviour mb)
- **mb** The monobehaviour to invoke events on

 Manually add a global event listener 
#### void RemoveGlobalEventListener(MonoBehaviour mb)
- **mb** The monobehaviour to be removed

 Manually remove a global event listener 
#### void Destroy(GameObject gameobject)
- **gameobject** The game object which contains the entity

 Destroy a bolt entity 
#### void LoadScene(int scene)
- **scene** The scene to load

 Load a scene based on index, only possible on the Server 
#### void LoadScene(string scene)
- **scene** The scene to load

 Load a scene based on name, only possible on the Server 
#### void Connect(UdpKit.UdpEndPoint endpoint)
- **endpoint** Server end point to connect to

 Connect to a server 
#### void SetSessionData(string serverName, string userData)
- **serverName** Name of the server
- **userData** User definable data

 Set session data for LAN Broadcast/Master Server listing 
#### void DisableLanBroadcast()

 Disable LAN broadcasting 
#### void EnableLanBroadcast()

 Enable LAN broadcasting 
#### UdpKit.UdpSession[] GetSessions()

 Sessions currently vailable from the LAN Broadcasting/Master Server listing 
## BoltPhysicsHit
 Describes a hit to a BoltHitbox on a BoltHitboxBody 
#### *float* distance
 The distance away from the origin of the ray 
#### *BoltHitbox* hitbox
 Which hitbox was hit 
#### *BoltHitboxBody* body
 The body which was hit 
## BoltPhysicsHits
 Container for a group of BoltPhysicsHits 
#### *int* count
 How many hits we have in the collection 
#### BoltPhysicsHit GetHit(int index)

 Get the hit at a specific index 
## Bolt.Command
 Base class that all commands inherit from 
#### *int* ServerFrame
 The value of the BoltNetwork.serverFrame property of the computer this command was created on 
#### *bool* IsFirstExecution
 Returns true if it's the first time this command executed 
#### *object* UserToken
 User assignable token that lets you pair arbitrary data with the command, this is not replicated over the network to any remote computers. 
## EntityArray
 Represents an array of entities on a state 
#### *int* Length
 The size of the array 
#### EntityArrayModifier Modify()

 Creates aa modifier object for this array 
## EntityArrayModifier
 Object which allows you to modify an entity array 
#### *int* Length
 How many entities are available in this array 
## Bolt.EntityBehaviour
 Base class for unity behaviours that want to access Bolt methods 
#### *BoltEntity* entity
 The entity for this behaviour 
#### void Initialized()

 Invoked when the entity has been initialized, before Attached 
#### void Attached()

 Invoked when Bolt is aware of this entity and all internal state has been setup 
#### void Detached()

 Invoked when this entity is removed from Bolts awareness 
#### void SimulateOwner()

 Invoked each simulation step on the owner 
#### void SimulateController()

 Invoked each simulation step on the controller 
#### void ControlGained()

 Invoked when you gain control of this entity 
#### void ControlLost()

 Invoked when you lost control of this entity 
#### void MissingCommand(Bolt.Command previous)
- **previous**

 Invoked on the owner when a remote connection is controlling this entity but we have not received any command for the current simulation frame. 
#### void ExecuteCommand(Bolt.Command command, bool resetState)
- **command** The command to execute
- **resetState** Indicates if we should reset the state of the local motor or not

 Invoked on both the owner and controller to execute a command 
## Bolt.EntityBehaviour`1
 Base class for unity behaviours that want to access Bolt methods with the state available also 
#### *TState* state
 The state for this behaviours entity 
## Bolt.EntityTargets
 Enumeration of target options for events sent to entitiessx 
## Bolt.Event
 Base class that all events inherit from 
#### *bool* IsFromLocalComputer
 Returns true if this event was sent from the local computer 
#### *BoltConnection* RaisedBy
 Returns the connection this event was received from, will be null if this event was raised on the local computer 
#### void Send()

 Send this event 
## FloatArray
 Represents an array of floats on a state 
#### *int* Length
 The size of the array 
#### FloatArrayModifier Modify()

 Creates aa modifier object for this array 
## FloatArrayModifier
 Object which allows you to modify a float array 
#### *int* Length
 The size of the array 
## BoltInternal.GlobalEventListenerBase
 Base class for all BoltCallbacks objects 
#### bool PersistBetweenStartupAndShutdown()

 Override this method and return true if you want the event listener to keep being attached to Bolt even when bBolt shuts down and starts again. 
## Bolt.GlobalTargets
 Enumeration of target options for global events 
## Bolt.IPriorityCalculator
 Interface which can be implemented on a behaviour attached to an entity which lets you provide custom priority calculations for state and events. 
#### float CalculateStatePriority(BoltConnection connection, Bolt.BitArray mask, int skipped)
- **connection** The connection we are calculating priority for
- **mask** The mask of properties with updated values we want to replicate
- **skipped** How many packets since we sent an update for this entity

 Called for calculating the priority of this entity for the connection passed in 
#### float CalculateEventPriority(BoltConnection connection, Bolt.Event evnt)
- **connection** The connection we are calculating priority for
- **evnt** The event we are calculating priority for

 Called for calculating the priority of an event sent to this entity for the connection passed in 
## Bolt.IState
 Base interface for all states 
#### void SetAnimator(Animator animator)
- **animator** The animator object to use

 Set the animator object this state should use for reading/writing mecanim parameters 
#### void AddCallback(string path, Bolt.PropertyCallback callback)
- **path** The path of the property
- **callback** The callback delegate

 Allows you to hook up a callback to a specific property 
#### void AddCallback(string path, Bolt.PropertyCallbackSimple callback)
- **path** The path of the property
- **callback** The callback delegate

 Allows you to hook up a callback to a specific property 
#### void RemoveCallback(string path, Bolt.PropertyCallback callback)
- **path** The path of the property
- **callback** The callback delegate to remove

 Removes a callback from a property 
#### void RemoveCallback(string path, Bolt.PropertyCallbackSimple callback)
- **path** The path of the property
- **callback** The callback delegate to remove

 Removes a callback from a property 
## IntegerArray
 Represents an array of integers on a state 
#### *int* Length
 The size of the array 
#### IntegerArrayModifier Modify()

 Creates aa modifier object for this array 
## IntegerArrayModifier
 Object which allows you to modify an integer array 
#### *int* Length
 The size of the array 
## TransformArray
 Represents an array of transforms on a state 
#### *int* Length
 The size of the array 
