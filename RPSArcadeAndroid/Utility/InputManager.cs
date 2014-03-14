#region File Description
//-----------------------------------------------------------------------------//
// InputManager.cs															   //
//																			   //
// Statc Class																   //
// Represents a utility for checking inputs								       //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

#endregion
namespace RPSArcadeAndroid
{
	public static class InputManager
	{
		public static Vector2 LastTappedPosition { get; set; }

		public static bool IsTapped ()
		{
			while (TouchPanel.IsGestureAvailable) {
				var gesture = TouchPanel.ReadGesture ();
				switch (gesture.GestureType) {
				case GestureType.Tap:
					LastTappedPosition = Resolution.GetVirtualFromActual (gesture.Position);
					return true;
				}
			}
			return false;
		}
	}
}

