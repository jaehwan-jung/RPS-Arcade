#region File Description
//-----------------------------------------------------------------------------//
// IGameScreen.cs															   //
//																			   //
// Interface																   //
// Represents the interface for all screens        	   						   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using Microsoft.Xna.Framework;

#endregion
namespace RPSArcadeAndroid
{
	public interface IGameScreen
	{
		void UnloadContent ();

		void Update (GameTime gameTime);

		void Draw (GameTime gameTime);
	}
}

