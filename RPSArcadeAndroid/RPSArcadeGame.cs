#region File Description
//-----------------------------------------------------------------------------//
// RPSArcadeGame.cs															   //
//																			   //
// Class																   	   //
// Represents the control for all stages of the game (start, stop, etc)		   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion
namespace RPSArcadeAndroid
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public sealed class RPSArcadeGame : Game
	{
		#region Fields

		private GraphicsDeviceManager graphics;
		private GameFlowManager GameFlowScreenManager;
		private GamePlayManager GamePlayScreenManager;
		private SpriteBatch spriteBatch;

		#endregion

		#region Initialize

		public RPSArcadeGame ()
		{
			ConfigureGraphics ();				
			ConfigureScreen ();
			ConfigureScreenManagers ();		
			IsFixedTimeStep = false;
		}

		private void ConfigureGraphics ()
		{
			graphics = new GraphicsDeviceManager (this);		
			Content.RootDirectory = "Content";
		}

		private void ConfigureScreen ()
		{
			graphics.SupportedOrientations = DisplayOrientation.Portrait;		
			graphics.IsFullScreen = true;
			IsMouseVisible = true;
		}

		private void ConfigureScreenManagers ()
		{
			GameFlowScreenManager = GameFlowManager.GetInstance (this);
			Components.Add (GameFlowScreenManager);
			
			GamePlayScreenManager = GamePlayManager.GetInstance (this);
			Components.Add (GamePlayScreenManager);		
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			base.Initialize ();				
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			LoadGraphicDevice ();
			base.LoadContent ();
		}

		private void LoadGraphicDevice ()
		{
			spriteBatch = new SpriteBatch (graphics.GraphicsDevice);
			Resolution.Initialize (graphics, spriteBatch);
		}

		#endregion

		#region Update and Draw

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{			
			ExitIfBackButtonIsPressed ();					
			base.Update (gameTime);			
		}

		private void ExitIfBackButtonIsPressed ()
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				this.Exit ();
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{			
			base.Draw (gameTime);
		}

		#endregion
	}
}

