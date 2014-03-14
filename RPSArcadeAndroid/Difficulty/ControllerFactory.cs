#region File Description
//-----------------------------------------------------------------------------//
// LevelFactory.cs											    		       //
//																			   //
// Statc Class																   //
// Represents a factory for (difficulty) levels for the game                   //
// Graphics based on 480x800 resolution.							           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;

#endregion
namespace RPSArcadeAndroid
{
	public static class ControllerFactory
	{
		public static DifficultyController Create ()
		{
			return Create (LevelTypes.Speed);
		}

		public static DifficultyController Create (LevelTypes levelType)
		{
			switch (levelType) {
			case LevelTypes.Speed:
				return new SpeedController ();
			case LevelTypes.HandType:
				return new HandTypeController ();
			default:
				return new SpeedController ();
			}
		}
	}
}

