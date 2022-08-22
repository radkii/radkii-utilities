using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//Add a GameObject with a Textbox component to dialogueBox or nameBox to automatically get
//the dialogueBox or nameBox from the Textbox component respectively
namespace Radkii.Dialogue
{
	public class Character
	{
		public string name;
		public Action<string> dialogueAction;
		public Action<Character> selectAction, deselectAction;
	
		public Character(string _name, Action<string> _dialogueAction, Action<Character> _selectAction, Action<Character> _deselectAction)
		{
			name = _name;
			dialogueAction = _dialogueAction;
			selectAction = _selectAction;
			deselectAction = _deselectAction;
		}

		public void Speak(string str) => dialogueAction.Invoke(str);
		public void Select(Character oldCharacter) => selectAction.Invoke(oldCharacter);
		public void Deselect(Character newCharacter) => deselectAction.Invoke(newCharacter);
	}
}


