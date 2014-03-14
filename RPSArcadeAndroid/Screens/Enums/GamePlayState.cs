#region File Description
//-----------------------------------------------------------------------------//
// GamePlayState.cs														   	   //
//																			   //
// Enum																	   	   //
// Represents the state of the game	           						           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

namespace RPSArcadeAndroid
{
	/// <summary>
	/// NotStarted = Game has not started yet
	/// Starting = Game has just started fresh
	/// Playing = Game is playing
	/// Paused = Game is paused.
	/// Terminated = Game is terminated;
	/// </summary>
	public enum GamePlayState
	{
		NotStarted,
		Restarting,
		Playing,
		Paused,
		Ended,
		Ending
	}
}

