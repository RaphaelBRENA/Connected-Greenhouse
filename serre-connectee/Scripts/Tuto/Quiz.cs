using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

public partial class Quiz : Popup
{
	private Godot.Popup MyPopup;
	private Label Question;
	private Label Explanation;
	private Timer Timer;
	private List<Button> Choices;
	private GridContainer GridContainer;
	private ProgressBar ProgressBar;
	private List<string> Currents;
	private Button HintButton;
	private bool IsActive = false;

	/// <summary>
	/// Initializes the quiz popup components by setting references to the popup,
	/// question, explanation, timer, grid container, progress bar, and hint button.
	/// </summary>
	public override void _Ready()
	{
		MyPopup = GetNode<Godot.Popup>("Popup");
		Question = GetNode<Label>("Popup/Panel/Question");
		Explanation = GetNode<Label>("Popup/Panel/Explanation");
		Timer = GetNode<Timer>("Popup/Timer");
		GridContainer = GetNode<GridContainer>("Popup/Panel/GridContainer");
		ProgressBar = GetNode<ProgressBar>("Popup/Panel/ProgressBar");
		HintButton = GetNode<Button>("Popup/Panel/HBoxContainer/Hint");
		Choices = new List<Button>();
		for (int i = 1; i <= 4; i++)
		{
			Choices.Add(GetNode<Button>("Popup/Panel/GridContainer/Choice" + i));
		}
	}

	/// <summary>
	/// Called every frame. Updates the progress bar value to reflect the remaining time on the timer.
	/// </summary>
	/// <param name="delta">The time elapsed since the last frame.</param>
	public override void _Process(double delta)
	{
		ProgressBar.Value = Timer.TimeLeft;
	}

	/// <summary>
	/// Starts the quiz popup with the specified question, explanation, choices, correct choice, time limit, and hint.
	/// Pauses the game tree, plays the "blur" animation, and centers the popup.
	/// Sets the question text, explanation text, and choices text, and starts the timer.
	/// </summary>
	/// <param name="QuestionText">The text of the question.</param>
	/// <param name="ExplanationText">The text of the explanation.</param>
	/// <param name="ChoicesText">A list of strings representing the choices.</param>
	/// <param name="CorrectChoice">The index of the correct choice.</param>
	/// <param name="Time">The time in seconds that the player has to answer.</param>
	/// <param name="Hint">The text of the hint.</param>
	public void StartQuestion(string QuestionText, string ExplanationText, List<string> ChoicesText, int CorrectChoice, double Time, string Hint)
	{
		IsActive = true;
		GetTree().Paused = true;
		GetNode<AnimationPlayer>("AnimationPlayer").Play("BLUR");
		HintButton.Show();
		MyPopup.PopupCentered();
		Explanation.Visible = false;
		GridContainer.Visible = true;
		Question.Text = QuestionText;
		Explanation.Text = ExplanationText;
		ProgressBar.MaxValue = Time;
		HintButton.Text = "Voir Indice";
		Currents = new List<string>() { QuestionText, ExplanationText, ChoicesText[0], ChoicesText[1], ChoicesText[2], ChoicesText[3], CorrectChoice.ToString(), Time.ToString(), Hint };
		for (int i = 0; i < 4; i++)
		{
			Choices[i].Text = ChoicesText[i];
		}
		Timer.Start(Time);
	}

	/// <summary>
	/// Displays the explanation for the quiz question based on the provided answer.
	/// Updates the question text to indicate whether the answer was correct, incorrect, or if the time ran out.
	/// Marks the question as completed and toggles the visibility of the explanation and choices.
	/// </summary>
	/// <param name="Answer">The index of the chosen answer or 0 if the time ran out.</param>
	public void ShowExplanation(int Answer)
	{
		HintButton.Hide();
		PopupManager.SetBoolQuiz(Currents[0], true);
		if (Answer == int.Parse(Currents[6]))
		{
			Question.Text = "Bonne réponse !";
		}
		else if (Answer == 0)
		{
			Question.Text = "Temps écoulé !";
		}
		else
		{
			Question.Text = "Mauvaise réponse !";
		}
		Explanation.Visible = true;
		GridContainer.Visible = false;
	}

	/// <summary>
	/// Converts the provided text to speech using the first available French voice.
	/// Stops any ongoing text-to-speech processes before speaking the new text.
	/// </summary>
	/// <param name="Text">The text to be spoken aloud.</param>
	public void TTS(string Text)
	{
		string[] Voices = DisplayServer.TtsGetVoicesForLanguage("fr");
		string VoiceId = Voices[0];
		DisplayServer.TtsStop();
		DisplayServer.TtsSpeak(Text, VoiceId);
	}

	/// <summary>
	/// Called when the time limit runs out. Displays the explanation
	/// for the current question, marks it as completed, and toggles the
	/// visibility of the explanation and choices.
	/// </summary>
	public void Timeout()
	{
		ShowExplanation(0);
	}

	/// <summary>
	/// Called when a quiz choice is pressed. Stops the timer and 
	/// displays the explanation for the selected answer based on
	/// the button index provided.
	/// </summary>
	/// <param name="Button">The index of the button pressed, representing the chosen answer.</param>
	public void OnChoicePressed(int Button)
	{
		Timer.Stop();
		ShowExplanation(Button);
	}

	/// <summary>
	/// Called when the bottom buttons are pressed.
	/// If the "TTS" button is pressed and the question is not one of the
	/// "Bonne réponse !", "Temps écoulé !", and "Mauvaise réponse !" questions,
	/// it converts the current question and choices to speech using the first
	/// available French voice and stops any ongoing text-to-speech processes.
	/// If the "Hint" button is pressed, it toggles between the hint text and "Voir Indice".
	/// </summary>
	/// <param name="Button">The text of the button pressed, either "TTS" or "Hint".</param>
	public void OnBottomButtonsPressed(string Button)
	{
		if (Button == "TTS" && (Question.Text != "Bonne réponse !" && Question.Text != "Temps écoulé !" && Question.Text != "Mauvaise réponse !"))
		{
			TTS("La question est : " + Currents[0] + " . " + "Choix 1 :" + Currents[2] + ". " + "Choix 2 :" + Currents[3] + ". " + "Choix 3 :" + Currents[4] + ". " + "Choix 4 :" + Currents[5] + ".");
		}
		else if (Button == "Hint")
		{
			if (HintButton.Text == "Voir Indice")
			{
				HintButton.Text = Currents[8];
			}
			else
			{
				HintButton.Text = "Voir Indice";
			}
		}
	}

	/// <summary>
	/// Handles the input of the user in the Quiz scene.
	/// If the escape key is pressed while the quiz is active, the quiz is closed and the user is returned to the gameplay scene.
	/// This stops any ongoing text-to-speech processes, and sets the mouse mode to captured.
	/// </summary>
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey EventKey)
		{
			if (EventKey.Keycode == Key.Escape && IsActive)
			{
				GetTree().Paused = false;
				GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("BLUR");
				DisplayServer.TtsStop();
				IsActive = false;
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}
	}
}
