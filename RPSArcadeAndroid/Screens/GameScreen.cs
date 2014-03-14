#region File Description
//-----------------------------------------------------------------------------//
// GameScreen.cs															   //
//																			   //
// Abstract Class															   //
// Represents the base class for all game screens					           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

#endregion
namespace RPSArcadeAndroid
{
	public abstract class GameScreen: IGameScreen
	{
		#region Properties

		public ContentManager ContentManager { get; protected set; }

		public Game Game { get; protected set; }

		public ScreenManager ScreenManager { get; protected set; }

		#endregion

		#region Initialization

		public GameScreen (Game game, ScreenManager screenManager)
		{
			Game = game;
			ScreenManager = screenManager;
			GetNewContentManager (game);	
			ContentManager.RootDirectory = "Content";
		}

		private void GetNewContentManager (Game game)
		{
			var content = game.Content;
			ContentManager = new ContentManager (content.ServiceProvider, content.RootDirectory);
		}

		public virtual void UnloadContent ()
		{
			ContentManager.Unload ();
		}

		#endregion

		#region Update and Draw

		public virtual void Update (GameTime gameTime)
		{
		}

		public virtual void Draw (GameTime gameTime)
		{
		}

		#endregion

		#region Helper Methods

		public virtual void Reset (GameTime gameTime)
		{
		}

		protected Texture2D LoadTexture2D (string assetName)
		{
			return ContentManager.Load<Texture2D> (assetName);
		}

		protected SpriteFont LoadSpriteFont (string assetName)
		{
			return ContentManager.Load<SpriteFont> (assetName);
		}

		#endregion
	}
}

