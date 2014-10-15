###Deciding between interpolated snapshots or dead reckoning

They serve the same basic purpose: Smooth movement of remote player characters on screen, but they do it in vastly different ways.

###Interpolated Snapshots

Interpolated Snapshots, or IS for short, works by taking two old but known positions and moving the character between them. It's usually accomplished by having an array of received positions and rotations, along with the time they represent. We usually take our current local time minus some pre-defined amount say 100 milliseconds, and then go into our array of received positions and rotations and find the two indexes which are just before and just after this time.

IS only allows extrapolation (guessing) of the position if we don't have a received position and rotation for the time we're looking for, and extrapolation is also limited to go on for about ~200 millisecond and then the character will stop.

The gains of IS is that it in most cases provides a very accurate representation of the world to each player, as in general only already known positions of the remote objects are rendered and only in rare cases will it try to extrapolate (guess) where an entity is.

The cost is that IS requires us to "delay" the rendering by some pre-set amount, in our example above we always render 100 ms behind current time, so that new packets have time to arrive with data. Since IS works best if you have new data for every object in every packet, it is usually a better fit for games with <= 32 players.

###Dead Reckoning

Dead Reckoning, or DR for short, works by taking the last known position, rotation and velocity and then look at them and try to guess where the entity is going to be in the future. If we send a packet every third frame, that packet contains the current position, rotation and velocity of our object. The DR algorithm in Bolt will look at this and say that from this we can extrapolate (guess) where the object is going be for the next three frames until a new packet arrives.

The key is that if a packet has not arrived when we have extrapolated to our third frame, we can keep guessing with the same algorithm - sure the further into the future we guess the more likely it is that we're going to be wrong, but the DR algorithm uses something called projective velocity blending to do corrections when our real data arrives.

The benefit of DR is that we don't need any artificial delay on our packets, or well - we need very little at least, this lets the game render things faster to the players and does not introduce the same type of artificial lag as IS. DR is also a lot better when dealing with games with large player counts, as it is a lot better at handling lost packets or skipped packets (where there was not space for a specific entities position, rotation and velocity).

The cost of DR is that it's not as precise as IS, and it can be hard to use if you have something like an FPS game where you want to do authoritative and lag compensated shooting, because of the nature of extrapolation (guessing) things might look a bit more different on each players screen then if you were using IS, which means that you could miss a shot even if you were aiming straight on the head if the player you were shooting was moving perpendicular to you.