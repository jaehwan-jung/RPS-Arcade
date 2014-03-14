#region File Description
//-----------------------------------------------------------------------------//
// HandState.cs																   //
//																			   //
// Enum																		   //
// Represents the state of a single hand of the Rock-Paper-Scissors game.      //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

namespace RPSArcadeAndroid
{
	/// <summary>	
	/// Represents the state of a hand.
	/// Active = Hand is valid but has not reached the collision area.
	/// Reactive = Hand is in the collision area.
	/// Destroyed = Hand has been destroyed in the collision area.
	/// Expired = Hand passed the collision area without being destroyed.
	/// </summary>
	public enum HandState
	{
		Active,
		Reactive,
		Destroyed,
		Expired
	}
}

