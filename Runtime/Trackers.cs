using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radkii.Trackers
{
	public class Tracker<T>
	{
		public T Value
		{
			get
			{
				timesGet++;
				onGetValue?.Invoke(val);
				return val;
			}
			set
			{
				timesSet++;
				onSetValue?.Invoke(val, value);
				val = value;
			}
		}
		private T val;

		public delegate void GetDelegate(T currentValue);
		public delegate void SetDelegate(T oldValue, T newValue);
		public GetDelegate onGetValue;
		public SetDelegate onSetValue;

		public int timesGet, timesSet = 0;

		public T Peek()
		{
			timesGet--;
			return Value;
		}

		public void Sneak(T value)
		{
			timesSet--;
			Value = value;
		}
	}
}
