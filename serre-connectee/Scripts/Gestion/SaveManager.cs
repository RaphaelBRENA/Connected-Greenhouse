using Godot;
using System;

public partial class SaveManager : Node
{
	Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data;
    public Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> SendData()
    {
        return Data;
    }

    public void KeepData(Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> Data)
    {
        this.Data = Data;
    }

}
