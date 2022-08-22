using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor.Animations;
using System;
using System.Globalization;

namespace Radkii.Dialogue
{
    //This class is added to every state as a behaviour in the statemachine
    //by Setup() or the "Setup Trie" button in TrieAnimator
    public class State : StateMachineBehaviour
    {
        [Space(5f)]
        public DialogueField[] dialogue;
    
        [HideInInspector]
        public string stateName;
        [TextArea]
        public string displayName; //Name to display in choices; can be different from state name
        [Space(5f)]
        public string[] nodeNames; //nodeNames is filled automatically by Setup() in TrieAnimator

        [HideInInspector]
        public TrieAnimator ta; //TrieAnimator reference assigned by itself in Setup()

        [Space(5f)]
        [HideInInspector] public AnimatorState self;
    
	    [Space(5f)]
        [HideInInspector] public StateTypes type = StateTypes.Default;

        //[Header("Conditions:")]
        public Condition[] conditions = { new Condition() };
        //[Header("Actions:")]
        public Action[] actions = { new Action() };

        [Serializable]
        public class ScriptMethodFormat
	    {
            public string methodName;
            public string arguments;
	    }

        [Space(20f)]
        public ScriptMethodFormat[] enterEventNames;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        bool reading = false, sceneChange = false;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ta = animator.gameObject.GetComponent<TrieAnimator>();

            ta.previousState = ta.currentState;
            ta.currentState = this;

		    //Custom types
		    switch (type)
		    {
			    case StateTypes.Default:
				    break;
			    case StateTypes.Burner:
                    ta.burn = ta.previousState;
                    ta.burnIndex = Array.FindIndex<string>(ta.previousState.nodeNames, s => s == self.name);
                    break;
                case StateTypes.BurnerSkip:
                    ta.burn = ta.previousState;
                    ta.burnIndex = Array.FindIndex<string>(ta.previousState.nodeNames, s => s == self.name);
                    break;
			    case StateTypes.PointBurner:
                    ta.pointBurn = ta.previousState;
                    ta.pointBurnIndex = Array.FindIndex<string>(ta.previousState.nodeNames, s => s == self.name);
                    break;
                case StateTypes.PointBurnerSkip:
                    ta.pointBurn = ta.previousState;
                    ta.pointBurnIndex = Array.FindIndex<string>(ta.previousState.nodeNames, s => s == self.name);
                    break;
                case StateTypes.Bomb:
                    ta.Delete(this);
				    break;
			    default:
				    break;
		    }

            //Effects of custom types
            if(ta.burn != null && self.name == ta.burn.self.name)
		    {
                State b = self.transitions[ta.burnIndex].destinationState.behaviours[0] as State;
                ta.Delete(b);
                ta.burn = null;
            }
            if (ta.pointBurn != null && self.name == ta.pointBurn.self.name)
            {
                State b = self.transitions[ta.pointBurnIndex].destinationState.behaviours[0] as State;
                ta.Ignore(b);
                ta.pointBurn = null;
            }

            //Event
            foreach(ScriptMethodFormat s in enterEventNames)
		    {
                if(s.methodName == "load_scene") { sceneChange = true; }
            }

            //Action
            ta.ParameterFunction(this);
		 
            if (!sceneChange) 
            { 
                //ta.interpreter.Interpret(dialogue);
                //ta.interpreter.Read(true); 
            }

            reading = true;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
		    //if(type == StateTypes.Bomb) { show = false; }
        
            //Waiting until Read() returns true and the dialogue carried
            //by the interpreter is finished to show the choice buttons assigned
            //in the TrieAnimator
            if (reading && !sceneChange)
		    {
			    if(type == StateTypes.Node || type == StateTypes.NodeSkip)
			    {
                    ta.ShowButtons(type == StateTypes.Skip || 
                        type == StateTypes.BurnerSkip || 
                        type == StateTypes.PointBurnerSkip || 
                        type == StateTypes.NodeSkip);
                    reading = false;
                }
            
       //         if (ta.interpreter.Read(false))
			    //{
       //             ta.ShowButtons(type == StateTypes.Skip || 
       //                 type == StateTypes.BurnerSkip || 
       //                 type == StateTypes.PointBurnerSkip || 
       //                 type == StateTypes.NodeSkip);
       //             reading = false;
			    //}   
		    }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }

    //When a Burner state becomes the current state, it will stop showing after its parent state becomes the current state
    //When a PointBurner state becomes the current state, it will acting on parameters after its parent state becomes the current state
    //When a Bomb state becomes the current state, it will be flagged not to show again
    //Ex types transition automatically without button interface (make sure there would be only 1 button!)
    public enum StateTypes { Default, Skip, Burner, BurnerSkip, PointBurner, PointBurnerSkip, Bomb, Node, NodeSkip}

    [Serializable]
    public class Condition
    {
        public string par;
        [HideInInspector]
        //public int opInt;
        public Operator op;
        public string threshold;  
    }

    public enum Operator { Greater, Less, Equal, NotEqual }

    [Serializable]
    public class Action
    {
        public string par;
        [HideInInspector]
        //public int opInt;
        public Function fn;
        public int value;
    }

    public enum Function { Add, Subtract, Set }
}

