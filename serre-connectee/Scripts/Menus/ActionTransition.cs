using Godot;
using System.Threading.Tasks;

public partial class ActionTransition : Control
{
	[Export] private float Time = 0.8f;
	private ColorRect ColorBlack;

    public override void _Ready()
    {
        ColorBlack = GetNode<ColorRect>("Black");
		ColorBlack.Modulate = new Color(0,0,0,0);
    }
    public async void ChangeToScene(string SceneName){
		await _TransitionIn();
		GetTree().ChangeSceneToFile($"res://Scenes/{SceneName}");
		_TransitionOut();
	}

	private async Task _TransitionIn() {
		Tween CurrentTween = GetTree().CreateTween();
		CurrentTween.TweenProperty(ColorBlack, "modulate:a", 1.0f, Time / 2f);
		await ToSignal(CurrentTween, Tween.SignalName.Finished);

	}

	private void _TransitionOut() {
		Tween CurrentTween = GetTree().CreateTween();
		CurrentTween.TweenProperty(ColorBlack, "modulate:a", 0.0f, Time / 2f);
		
	}
}
