using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radkii.Strings
{
	public static class StringExtensions
	{
		public static string Bold(this string text) => $"<b>{text}</b>";
		public static string Italic(this string text) => $"<i>{text}</i>";
		public static string Colored(this string text, Color color) => $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
		public static string Size(this string text, int size) => $"<size={size}>{text}</color>";

		//public static string Bold(string text) => $"<b>{text}</b>";
		//public static string Italic(string text) => $"<i>{text}</i>";
		//public static string Colored(string text, Color color) => $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
		//public static string Size(string text, int size) => $"<size={size}>{text}</color>";
	}
}
