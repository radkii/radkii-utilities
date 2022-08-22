using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Animations;
using TMPro;
using System;
using Radkii.ScriptMethods;

namespace Radkii.Dialogue
{
	//1. Add this script to a dedicated trie gameobject
	//2. Add an Animator component to the gameobject
	//3. Create an Animation Controller asset for the trie and assign it to the Animator
	//4. Build the trie in the Animator window 
	//(state names are choice names; dialogue in a state precedes choices)
	//5. Set maxChoices as the max number of transitions a state can have in your trie
	//6. Press "Setup Trie"
	//7. Add valid dialogue txts to every state for DialogueInterpreter to read
	//8. Add int parameters as global statemachine variables
	//9. Add conditions in the State scripts if necessary
	//*if Space is being used as input for dialogue, choices being incorrectly
	//triggered by pressing Space may be fixed by setting the Navigation of
	//every button that uses the Button() function below to None
	[RequireComponent(typeof(Animator))]
	public class TrieAnimator : MonoBehaviour
	{
	
		[SerializeField] private Animator anim; //Reference to the animator
		//public DialogueInterpreter interpreter; //DialogueInterpreter to read every state's dialogue
		public Button[] buttons; //Choice buttons

		public AnimatorStateMachine stateMachine;

		private int maxChoices;
		[HideInInspector]
		public State currentState, previousState, burn, pointBurn;
		private int dialogueProgress = 0;
		[HideInInspector]
		public int burnIndex, pointBurnIndex;

		//To be set in script
		[HideInInspector] public List<ScriptMethod<string>> customMethods;
		[HideInInspector] public List<Character> characters;
		private Character currentCharacter;

		private List<string> toDelete, toIgnore;

		public void Awake()
		{
			customMethods = new List<ScriptMethod<string>>();
			characters = new List<Character>();
		}

		public void Start()
		{
			toDelete = new List<string>();
			toIgnore = new List<string>();

			maxChoices = buttons.Length;

			foreach(Button b in buttons)
			{
				b.gameObject.SetActive(false);
			}

			DontDestroyOnLoad(gameObject);
		}

		public void Setup()
		{
			RuntimeAnimatorController rctrl = anim.runtimeAnimatorController;
			AnimatorController ctrl = (AnimatorController)anim.runtimeAnimatorController;

			ctrl.parameters = new AnimatorControllerParameter[] { };
			if(buttons.Length == 0)
			{
				Debug.LogError("No buttons were found. Maybe you forgot to assign them in your TrieAnimator object?");
				return;
			}
			for (int i = 0; i < buttons.Length; i++)
			{
				ctrl.AddParameter(i.ToString(), AnimatorControllerParameterType.Trigger);
			}	

			stateMachine = ((AnimatorController)rctrl).layers[0].stateMachine;
			var states = stateMachine.states;
			int maxTransitions = 0;
		
			foreach (var s in states)
			{
				if (s.state.behaviours.Length == 0)
				{
					ctrl.AddEffectiveStateMachineBehaviour<State>(s.state, 0);
				}
			}
			foreach(var s in states)
			{
				State st = s.state.behaviours[0] as State;
				List<NodeNameFormat> formats = new List<NodeNameFormat>();
				//List<State> children = new List<State>();
				for (int i = 0; i < s.state.transitions.Length; i++)
				{
					var t = s.state.transitions[i];

					//print(s.state.transitions[i].destinationState.behaviours[0]);

					t.name = t.destinationState.name;
					State dest = s.state.transitions[i].destinationState.behaviours[0] as State;
					formats.Add(new NodeNameFormat(dest.stateName, dest.displayName));
					//children.Add(t.destinationState.behaviours[0] as State);

					t.hasExitTime = false;
					t.duration = 0f;
					t.exitTime = 0f;

					if (i > maxTransitions) { maxTransitions = i; }
					if (t.conditions.Length == 0 && (st.type != StateTypes.Node || st.type != StateTypes.NodeSkip)) { t.AddCondition(AnimatorConditionMode.If, 0, i.ToString()); }
					if (st.type == StateTypes.Node || st.type == StateTypes.NodeSkip) { t.hasExitTime = true; }
				}

				st.stateName = s.state.name;
				if (st.displayName == "") { st.displayName = s.state.name; }
				st.ta = this;

				//NodeNameFormat[] newNodeNames = new NodeNameFormat[names.Count];
				//for(int i = 0; i < names.Count; i++)
				//{
				//	if(!Array.Exists<NodeNameFormat>(st.nodeNames, nnf => nnf.nodeName == names[i]))
				//	{
				//		newNodeNames[i] = new NodeNameFormat(names[i], names[i]);
				//	}
				//	else
				//	{
				//		newNodeNames[i] = new NodeNameFormat(names[i], st.nodeNames[i].displayName);
				//	}
				//}
				//st.nodeNames = names.ConvertAll<NodeNameFormat>(s => new NodeNameFormat(s, s)).ToArray();
				st.nodeNames = new NodeNameFormat[formats.Count];
				Array.Copy(formats.ToArray(), st.nodeNames, formats.Count);
				st.self = s.state;
			}
		}

		public void Delete(State state)
		{
			if (!toDelete.Exists(s => s == state.self.name)) { toDelete.Add(state.self.name); }
		}

		public void Ignore(State state)
		{
			if (!toIgnore.Exists(s => s == state.self.name)) { toIgnore.Add(state.self.name); }
		}

		public void ParameterFunction(State state)
		{
			//print(toIgnore.Exists(s => s == state.self.name));
			if(toIgnore.Exists(s => s == state.self.name)) { return; }

			foreach(Action a in state.actions)
			{
				if(a.par == null)
				{
					continue;
				}
				switch (a.fn)
				{
					case Function.Add:
						anim.SetInteger(a.par, anim.GetInteger(a.par) + a.value);
						break;
					case Function.Subtract:
						anim.SetInteger(a.par, anim.GetInteger(a.par) - a.value);
						break;
					case Function.Set:
						//var pm = Array.Find<AnimatorControllerParameter>(anim.parameters, p => p.name == a.par);
						anim.SetInteger(a.par, a.value);
						break;
					default:
						break;
				}
			}
		}

		public void NextInDialogue()
		{
			if(dialogueProgress >= currentState.dialogue.Length && dialogueProgress >= 0)
			{
				if(currentState.nodeNames.Length > 0)
				{
					dialogueProgress = -1;
					ShowButtons(false);
				}
			}
			else
			{
				DialogueField df = currentState.dialogue[dialogueProgress];
			
				if(df.fieldType == DialogueFieldType.Talk)
				{
					Character thisCharacter = characters.Find(ch => ch.name == df.character);
					if(currentCharacter == null)
					{
						thisCharacter.Select(currentCharacter);
						currentCharacter = thisCharacter;
					}
					else if(currentCharacter != thisCharacter)
					{
						currentCharacter.Deselect(thisCharacter);
						thisCharacter.Select(currentCharacter);
						currentCharacter = thisCharacter;
					}

					//print(currentState.dialogue[dialogueProgress].line);
					currentCharacter.Speak(df.line);
				}
				else if(df.fieldType == DialogueFieldType.CustomMethod)
				{
					customMethods.Find(cm => cm.name == df.customMethodName).Invoke(df.customMethodParam);
				}
				dialogueProgress++;

				if (!df.waitForInput) NextInDialogue();
			}
		}

		public void ShowButtons(bool skip)
		{
			StartCoroutine(ShowButtonsCoroutine(skip));
		}

		public IEnumerator ShowButtonsCoroutine(bool skip)
		{
			//interpreter.ch.dialogueBox.SetActive(false);

			//AnimatorCondition[] cnds = currentState.self.state.transitions;
			AnimatorStateTransition[] cnds = currentState.self.transitions;
			for(int i = 0; i < currentState.nodeNames.Length; i++)
			{
				State target = cnds[i].destinationState.behaviours[0] as State;
				//print(target.stateName + " " + target.type.ToString());
				Condition[] cs = target.conditions;

				bool valid = true;
				foreach(Condition c in cs)
				{
					if (c.par != "")
					{
						int p1 = 0;
						if (c.par.Contains("+"))
						{
							string[] subs = c.par.Split('+');
							foreach(string s in subs)
							{
								p1 += anim.GetInteger(s);
							}
						}
						else
						{
							p1 = anim.GetInteger(c.par);
						}

						int p2, delta;
						if(int.TryParse(c.threshold, out p2))
						{
							delta = p1 - p2;
						}
						else
						{
							delta = p1 - anim.GetInteger(c.threshold);
						}
						if ((c.op == Operator.Greater && delta <= 0) ||
							(c.op == Operator.Less && delta >= 0) ||
							(c.op == Operator.Equal && delta != 0) ||
							(c.op == Operator.NotEqual && delta == 0))
						{
							valid = false;
						}
					}
				}

				if(toDelete.Exists(s => s == target.self.name)) { valid = false; }

				if (valid)
				{
					if (skip)
					{
						Button(i.ToString());
					}
					else
					{
						buttons[i].gameObject.SetActive(true);
						buttons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = currentState.nodeNames[i].displayName;
					
						//buttons[i].gameObject.GetComponent<Animator>().SetTrigger("Show");
						yield return new WaitForSecondsRealtime(0.5f);
					}	
				}
			}

			foreach (Button b in buttons)
			{
				b.interactable = true;
			}
		}

		public void Button(string index)
		{
			StartCoroutine(ButtonCoroutine(index));
		}

		public IEnumerator ButtonCoroutine(string index)
		{
			foreach(Button b in buttons)
			{
				//b.gameObject.GetComponent<Animator>().SetTrigger("Pressed");
				b.interactable = false;
			}

			yield return new WaitForSecondsRealtime(1f);

			foreach (Button b in buttons)
			{
				b.gameObject.SetActive(false);
			}

			if(currentState.nodeNames.Length > 0)
			{
				anim.SetTrigger(index);
				dialogueProgress = 0;

				State stateCheck = currentState;
				yield return new WaitUntil(() => currentState != stateCheck);
				NextInDialogue();
			}
		}
	}
}

