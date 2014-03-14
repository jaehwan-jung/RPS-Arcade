#region File Description
//-----------------------------------------------------------------------------//
// Movement.cs											    	   			   //
//																			   //
// Class																	   //
// Represents Movement including directions and speeds 						   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

namespace RPSArcadeAndroid
{
	public sealed class Movement
	{
		public Direction HorizontalDirection { get; set; }

		public int HorizontalSpeed { get; set; }

		public Direction VerticalDirection { get; set; }

		public int VerticalSpeed { get; set; }
	}
}

