﻿using System;

namespace Alex.Controls.Shared.Extentions
{
	internal static class AccessExtensions
	{
		internal static object Call(this object o, string methodName, params object[] args)
		{
			var mi = o.GetType ().GetMethod (methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance );
			if (mi != null) {
				return mi.Invoke (o, args);
			}
			return null;
		}
	}
}

