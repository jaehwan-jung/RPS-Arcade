#region File Description
//-----------------------------------------------------------------------------//
// BackgroundScreen.cs														   //
//																			   //
// Class																	   //
// Represents the background of the game screen						           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion
namespace RPSArcadeAndroid
{
	public sealed class BackgroundScreen: GameScreen
	{
		#region Constants

		public const string BACKGROUND_XNB = "Textures/background";

		#endregion

		#region Properties

		public Texture2D Background { get; set; }

		#endregion

		#region Initialization

		public BackgroundScreen (Game game, ScreenManager screenManager)
			: base (game, screenManager)
		{			
			LoadTextures ();   
		}

		private void LoadTextures ()
		{
			Background = LoadTexture2D (BACKGROUND_XNB);
		}

		#endregion

		#region Update and Draw

		public override void Draw (GameTime gameTime)
		{
			Resolution.DrawFullScreen (Background);
			base.Draw (gameTime);
		}

		#endregion
	}
}

