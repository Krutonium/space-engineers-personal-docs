List<IMyTerminalBlock> list = new List<IMyTerminalBlock>
void Main(string argument)
{
	Vector3D origin = new Vector3D(0,0,0);
	if(this.Storage == null || this.Storage == "") //First Run?
	{
		//Records where it was initially setup
		//Presumably so it can return here later for... repairs? bullets?
		
		origin = Me.GetPosition(); 
		this.Storage = origin.ToString(); 
	}
	else
	{
		Vector3D.TryParse(this.Storage, out origin);	//Sets origin from memory
	}
	
	GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(list);
	if(list.Count > 0)
	{
		var remote = list(0) as IMyRemoteControl; //Gets first Remote Controller
		remote.ClearWaypoints();
		Vector3D player = new Vector3D(0,0,0);
		bool success = remote.GetNearestPlayer(out player); //WHY OUT KEYWORD
		if(success)
		{
			bool gotoOrigin = false;
			GridTerminalSystem.GetBlocksOfType<IMyUserControllableGun>(list);
			if(list.Count == 0){
				gotoOrigin = true;
				//No Guns, Return to Base for Repairs
			}
			else
			{
				bool hasUsableGun = false;
				for(int i = 0; i < list.Count; ++i)
				{
					var weapon = list(i);
					if(!weapon.IsFunctional) continue;
					if(weapons.HasInventory() && !weapon.GetInventory(0).IsItemAt(0)) continue;
					hasUsableGun = true;
					//The gun is functional, and has bullets.
				}
				if(!hasUsableGun)
				{
					gotoOrigin = true;
					//no usable gun, return to base.
				}
				if(Vector3D.DistanceSquared(player, origin) > 20000 * 20000)
				{
					gotoOrigin = true;
					//Player is too far away, return to base.
				}
				
				if(gotoOrigin)
				{
					remote.AddWaypoint(origin, "Origin");
					//Send 'er Home, Captain
				}
				
				else
				{
					remote.AddWaypoint(player + remote.GetTotalGravity() * -2f, "Player");
					//Aim to be 20m above the player, and engage.
				}
				remote.SetAutoPilotEnabled(true);
			}
		}
	}
}				
