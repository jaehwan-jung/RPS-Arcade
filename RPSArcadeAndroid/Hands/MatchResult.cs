#region File Description
//-----------------------------------------------------------------------------//
// MatchResult.cs															   //
//																			   //
// Enum																		   //
// Represents the result of a single match of the Rock-Paper-Scissors game.    //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

namespace RPSArcadeAndroid
{
	public sealed class MatchResult
	{
		public ResultType Result { get; set; }

		public Timing TapTiming { get; set; }
	}
}

