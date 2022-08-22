using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Radkii.Strings;
using Radkii.Trackers;

public class TrackerTest : MonoBehaviour
{
    Tracker<string> trackedString = new Tracker<string>();

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			trackedString.Sneak("Sneaky!");
			print($"Snook value is: {trackedString.Peek()} (set {trackedString.timesSet} times)");
		}
	}

	public void ReadInputField(string text)
	{
        trackedString.Value = text;
        print($"Set value is: {trackedString.Peek()} (set {trackedString.timesSet} times)");
	}

    public void ReadValue()
	{
        print($"Read value is: {trackedString.Value} (read {trackedString.timesGet} times)");
	}

    public void PeekValue()
	{
        print($"Peeked value is: {trackedString.Peek()} (read {trackedString.timesGet} times)");
    }

	private void Start()
	{
		trackedString.onSetValue += (string old, string current) => print(old + " - " + current);
	}
}
