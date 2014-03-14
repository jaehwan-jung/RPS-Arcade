#region File Description
//-----------------------------------------------------------------------------//
// ScreenManager.cs															   //
//																			   //
// Class																	   //
// Represents the base class for the manager for screens.  				   	   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

#endregion
namespace RPSArcadeAndroid
{
	public abstract class ScreenManager: DrawableGameComponent
	{
		#region Constants

		public const int LEVEL_UP_INTERVAL_IN_SEC = 10;

		#endregion

		#region Properties

		public bool IsGameExiting { get; set; }

		public TimeSpan LastLevelUpTime { get; set; }

		public TimeSpan LevelUpInterval { get; set; }

		public IList<GameScreen> Screens { get; set; }

		#endregion

		#region Initialization

		protected ScreenManager (Game game)
            : base (game)
		{	
			Screens = new List<GameScreen> ();
			LevelUpInterval = TimeSpan.FromSeconds (LEVEL_UP_INTERVAL_IN_SEC);
		}

		protected override void UnloadContent ()
		{
			for (int i = 0, ScreensCount = Screens.Count; i < ScreensCount; i++)
				Screens [i].UnloadContent ();
			
			Screens.Clear ();
			
			base.UnloadContent ();
		}

		#endregion

		#region Update and Draw

		public override void Update (GameTime gameTime)
		{
			for (int i = 0, ScreensCount = Screens.Count; i < ScreensCount; i++)
				Screens [i].Update (gameTime);
						
			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			Resolution.BeginDraw ();
			
			for (int i = 0, ScreensCount = Screens.Count; i < ScreensCount; i++)
				Screens [i].Draw (gameTime);
			
			Resolution.EndDraw ();
			base.Draw (gameTime);
		}

		#endregion

		#region Methods

		public virtual void AddScreen (GameScreen screen)
		{
			Screens.Add (screen);
		}

		public virtual void PopScreen (GameScreen screen)
		{
			Screens.Remove (screen);
			screen.UnloadContent ();			
		}

		public virtual IEnumerable<GameScreen> GetScreens (Func<GameScreen, bool> filter)
		{
			return (from x in Screens
			        where filter (x)
			        select x);
		}

		public virtual void Reset (GameTime gameTime)
		{
			LastLevelUpTime = TimeSpan.Zero;
			MyLogger.CurrentMessage = string.Empty;
			GameControl.InformResetCompletion ();
		}

		#endregion
	}
}

