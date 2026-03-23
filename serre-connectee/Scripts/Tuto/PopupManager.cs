using Godot;
using System;
using System.Collections.Generic;

public partial class PopupManager : Node
{
	private static Popup PopupScene;
	private static Quiz Quiz;
	private static List<KeyValuePair<string, bool>> TutorialActionsArray;
	private static List<KeyValuePair<string, bool>> QuizArray;


	/// <summary>
	/// Called when entering the PopupManager scene. Loads the popup and quiz scenes, and loads the tutorial actions and quizes arrays.
	/// </summary>
	public override void _Ready()
	{
		PopupScene = GD.Load<PackedScene>("res://Scenes/Tuto/Popup.tscn").Instantiate<Popup>();
		AddChild(PopupScene);
		Quiz = GD.Load<PackedScene>("res://Scenes/Tuto/Quiz.tscn").Instantiate<Quiz>();
		AddChild(Quiz);
		TutorialActionsArray = new List<KeyValuePair<string, bool>>{
			new KeyValuePair<string, bool>("Dans l'ordinateur, vous pouvez programmer différentes actions qui s'executeront tous les jours. Essayez de programmer l'arrosage d'un pot !", false),
			new KeyValuePair<string, bool>("Voici le coffre. Vous pouvez y stocker des plantes et des objets. Ne vous inquiétez pas, vous pouvez les récupérez à tout moment !", false),
			new KeyValuePair<string, bool>("Vous êtes entré dans l'interface d'un pot. C'est ici que vous plantez vos graines, placez vos capteurs et veillez à la bonne croissance de vos plantes !", false),
			new KeyValuePair<string, bool>("Bienvenue dans la boutique. C'est ici que vous pouvez commercer avec le marchand. Mais attention, n'essayez pas de l'entourlouper !", false),
			new KeyValuePair<string, bool>("Voici votre guide. Vous y retrouverez un tas d'informations sur les plantes que vous pouvez faire pousser, sur les objets, et bien plus encore !", false),
			new KeyValuePair<string, bool>("Vous pouvez régler la température de la serre grâce au thermostat. Essayez de le programmer !", false),
			new KeyValuePair<string, bool>("Quand vous avez fini vos actions de la journée, venez ici pour passer au jour suivant. Vous pouvez aussi y constater la météo du jour !", false),
			new KeyValuePair<string, bool>("Commencez par déposer une graine dans une case. Ensuite vous pourrez l'arroser pour la faire pousser !", false),
			new KeyValuePair<string, bool>("C'est ici que vous pouvez constater l'état de vos plantes. Mais attention, il faudra peut-être avoir placé un capteur au préalable !", false),
			new KeyValuePair<string, bool>("C'est dans votre inventaire que sont stockés les plantes et capteurs que vous possédez. Vous pouvez l'organiser comme vous le souhaitez !", false),
			new KeyValuePair<string, bool>("Essayez de modifier les lignes de code pour automatiser votre serre !", false),
			new KeyValuePair<string, bool>("Bienvenue dans votre serre ! Vous allez pouvoir faire pousser un tas de plantes ! Commencez par explorer les différentes parties de la serre !", false)
		};
		QuizArray = new List<KeyValuePair<string, bool>>{
			new KeyValuePair<string, bool>("A votre avis, dans quel département de l'IUT utilise-t'on des ordinateurs pour faire de la programmation ?", false),
			new KeyValuePair<string, bool>("A votre avis, dans quel département de l'IUT étudie-t'on la croissance des plantes ?", false),
			new KeyValuePair<string, bool>("A votre avis, dans quel département de l'IUT étudie-t'on l'économie énergie ?", false),
			new KeyValuePair<string, bool>("A votre avis, dans quel département de l'IUT étudie-t'on l'économie et la gestion de marché ?", false),
			new KeyValuePair<string, bool>("A votre avis, dans quel département de l'IUT étudie-t'on la gestion des stocks ?", false),
			new KeyValuePair<string, bool>("A votre avis, dans quel département de l'IUT étudie-t'on comment diminuer le gaspillage d'énergie dans les entreprises ?", false)
		};

	}

	/// <summary>
	/// Returns a list of the tutorial actions, which are a key-value pair of a string 
	/// description of the action and a boolean for whether the action is done.
	/// </summary>
	/// <returns>A List of KeyValuePair containing the tutorial actions</returns>
	public static List<KeyValuePair<string, bool>> GetTutorialActionsArray()
	{
		return TutorialActionsArray;
	}

	/// <summary>
	/// Sets the value of a tutorial action to the given boolean B.
	/// </summary>
	/// <param name="Action">The action to set the value of</param>
	/// <param name="B">The boolean value to set the action to</param>
	public static void SetBoolAction(string Action, bool B)
	{
		for (int i = 0; i < TutorialActionsArray.Count; i++)
		{
			if (TutorialActionsArray[i].Key == Action)
			{
				TutorialActionsArray[i] = new KeyValuePair<string, bool>(Action, B);
				return;
			}
		}
	}


	/// <summary>
	/// Displays a popup with the specified text if the action is not already marked as done.
	/// Marks the action as done once the popup is shown.
	/// </summary>
	/// <param name="text">The text to display in the popup.</param>
	public static void ShowPopup(string text)
	{
		if (!IsDone(text))
		{
			PopupScene.ShowPopup(text,"Tutoriel");
			SetBoolAction(text, true);
		}
	}

	/// <summary>
	/// Displays a popup with the specified text and the title "Annonce"
	/// </summary>
	/// <param name="text">The text to display in the popup.</param>
	public static void ShowInfo(string text)
	{
		PopupScene.ShowPopup(text,"Annonce");
	}

	/// <summary>
	/// Checks if a specific tutorial action has been completed.
	/// </summary>
	/// <param name="text">The description text of the tutorial action to check.</param>
	/// <returns>True if the action is marked as done; otherwise, false.</returns>
	public static bool IsDone(string text)
	{
		for (int i = 0; i < TutorialActionsArray.Count; i++)
		{
			if (TutorialActionsArray[i].Key == text && TutorialActionsArray[i].Value == true)
			{
				return true;
			}
		}
		return false;
	}


	/// <summary>
	/// Determines if all tutorial actions have been completed.
	/// </summary>
	/// <returns>True if all actions in the tutorial are marked as done; otherwise, false.</returns>
	public static bool TutoIsDone()
	{
		for (int i = 0; i < TutorialActionsArray.Count; i++)
		{
			if (TutorialActionsArray[i].Value == false)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Displays a quiz popup with the specified parameters if the question has not already been answered.
	/// </summary>
	/// <param name="QuestionText">The text of the question.</param>
	/// <param name="ExplanationText">The text of the explanation.</param>
	/// <param name="ChoicesText">A list of strings representing the choices.</param>
	/// <param name="CorrectChoice">The index of the correct choice.</param>
	/// <param name="Time">The time in seconds that the player has to answer.</param>
	/// <param name="Hint">The text of the hint.</param>
	public static void ShowQuizPopup(string QuestionText, string ExplanationText, List<string> ChoicesText, int CorrectChoice, double Time, string Hint)
	{
		if (!QuestionIsDone(QuestionText))
		{
			Quiz.StartQuestion(QuestionText, ExplanationText, ChoicesText, CorrectChoice, Time, Hint);
		}
	}

	/// <summary>
	/// Sets the completion status of a quiz question to the specified boolean value.
	/// </summary>
	/// <param name="Question">The text of the quiz question to update.</param>
	/// <param name="B">The boolean value to set, indicating whether the question has been completed.</param>
	public static void SetBoolQuiz(string Question, bool B)
	{
		for (int i = 0; i < QuizArray.Count; i++)
		{
			if (QuizArray[i].Key == Question)
			{
				QuizArray[i] = new KeyValuePair<string, bool>(Question, B);
				return;
			}
		}
	}

	/// <summary>
	/// Checks if a specific quiz question has been completed.
	/// </summary>
	/// <param name="Question">The text of the quiz question to check.</param>
	/// <returns>True if the question is marked as done; otherwise, false.</returns>
	public static bool QuestionIsDone(string Question)
	{
		for (int i = 0; i < QuizArray.Count; i++)
		{
			if (QuizArray[i].Key == Question && QuizArray[i].Value == true)
			{
				return true;
			}
		}
		return false;
	}
}
