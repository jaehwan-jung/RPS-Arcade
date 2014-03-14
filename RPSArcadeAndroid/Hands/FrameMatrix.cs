#region File Description
//-----------------------------------------------------------------------------//
// Animation.cs																   //
//																			   //
// Class																	   //
// Represents a matrix for animation frames (or textures)					   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

namespace RPSArcadeAndroid
{
	public sealed class FrameMatrix
	{
		public int Columns { get; set; }

		public int Rows { get; set; }

		public int TotalFrames{ get { return Columns * Rows; } }
	}
}

