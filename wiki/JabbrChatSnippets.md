
# Gems of snippets from the Jabbr Chat

Still unorganized snippets that haven't found their way into other parts of the Wiki! :D

###  "abuse" the executecommand stuff in bolt and use it a bit unorthodox 
  
  *Usually* you use executecommand/simulatecontroller to do the type of authoritative client-side predicted movement that say an FPS or TPS would but you can use it like that, basically ignoring the "resetState" and just have that as a target to "get to"
  
  '''C#
  public override void SimulateController() {
    IMyVechileCommandInput input = MyVechileCommand.Create();

    input.Throttle = ...
    input.Gear = ...
    input.Blah = ...;
    
    entity.QueueInput(input);
  }

  MyVechileCommand serverResult;

  public override void ExecuteCommand(Bolt.Command command, bool resetState) {
    MyVechileCommand cmd = (MyVechileCommand)command;

    // resetState means we got a state update from the server, this will only be true on the client
    if (resetState) {
      serverResult = cmd;
    }

    if (command.IsFirstExecution) {
      // and becaue resetState is only true on the client, this will
      // also only be true on the client
      if (serverResult != null) {
        // if we have a server result, this is the last "verified" 
        // result from the server, the data will be available in 
        // cmd.Result and is specified by you on the Command asset.
        // this is your lerp target
      }

      
      // perform movement logic for player, this code execute on both client (controlling) and server
    }
  }
  '''