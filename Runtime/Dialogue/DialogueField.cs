using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Radkii.Dialogue
{
    public enum DialogueFieldType { Talk, CustomMethod }

    [Serializable]
    public class DialogueField
    {
        public DialogueField()
		{
            waitForInput = true;
		}
        
        public DialogueFieldType fieldType;
        public bool waitForInput = true;

        public string character, line;
        public string customMethodName, customMethodParam;
        //public Action action;
    }
}