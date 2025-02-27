using Godot;
using Godot.Collections;
using DialogueManagerRuntime;
using System.Threading.Tasks;

public partial class TestScene : Node2D
{
  [Export]
  PackedScene Balloon;

  [Export]
  PackedScene SmallBalloon;

  [Export]
  string Title = "start";

  [Export]
  Resource DialogueResource;

  /* Make sure to add an [Export] decorator so that the Dialogue Manager can see the property */
  [Export]
  string PlayerName = "Player";


  public async override void _Ready()
  {
    var dialogueManager = await DialogueManager.GetSingleton();

    dialogueManager.Connect("dialogue_ended", new Callable(this, "OnDialogueEnded"));

    await ToSignal(GetTree().CreateTimer(0.4), "timeout");

    // Show the dialogue
    bool isSmallWindow = (int)ProjectSettings.GetSetting("display/window/size/viewport_width") < 400;
    Balloon balloon = (Balloon)(isSmallWindow ? SmallBalloon : Balloon).Instantiate();
    AddChild(balloon);
    balloon.Start(DialogueResource, Title, new Array<Variant> { this });
  }


  public async Task AskForName()
  {
    var nameInputDialogue = GD.Load<PackedScene>("res://examples/name_input_dialog/name_input_dialog.tscn").Instantiate() as AcceptDialog;
    GetTree().Root.AddChild(nameInputDialogue);
    nameInputDialogue.PopupCentered();

    await ToSignal(nameInputDialogue, "confirmed");
    PlayerName = nameInputDialogue.GetNode<LineEdit>("NameEdit").Text;
    nameInputDialogue.QueueFree();
  }


  private async void OnDialogueEnded(Resource resource)
  {
    await ToSignal(GetTree().CreateTimer(0.4), "timeout");
    GetTree().Quit();
  }
}
