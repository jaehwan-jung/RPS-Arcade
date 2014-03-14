#region File Description
//-----------------------------------------------------------------------------//
// ScreenManager.cs															   //
//																			   //
// Class																	   //
// Represents the manager for all screens.	   								   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

#endregion
namespace RPSArcadeAndroid
{
	public class ScreenManager: DrawableGameComponent
	{
		#region Properties

		public ActionBarScreen ActionBarScreen { get; set; }

		public BackgroundScreen BackgroundScreen { get; set; }

		public HandScreen HandScreen { get; set; }

		public HUDScreen HudScreen { get; set; }

		public IList<GameScreen> Screens { get; set; }

		#endregion

		#region Singleton

		private static volatile ScreenManager instance;

		public static ScreenManager GetInstance (Game game)
		{
			if (instance == null)
				instance = new ScreenManager (game);
				
			return instance;
		}

		#endregion

		#region Initialization

		private ScreenManager (Game game)
            : base (game)
		{	
			Screens = new List<GameScreen> ();
		}

		public override void Initialize ()
		{
			TouchPanel.EnabledGestures = GestureType.Tap;			
			base.Initialize ();
		}

		protected override void LoadContent ()
		{			
			BackgroundScreen = new BackgroundScreen (Game, this);
			AddScreen (BackgroundScreen);
			HandScreen = new HandScreen (Game, this);
			AddScreen (HandScreen);
			HudScreen = new HUDScreen (Game, this);
			AddScreen (HudScreen);
			ActionBarScreen = new ActionBarScreen (Game, this);
			AddScreen (ActionBarScreen);			
			base.LoadContent ();
		}

		protected override void UnloadContent ()
		{
			for (int i = 0, ScreensCount = Screens.Count; i < ScreensCount; i++) {
				GameScreen screen = Screens [i];
				screen.UnloadContent ();
			}
			
			base.UnloadContent ();
		}

		#endregion

		#region Update and Draw

		public override void Update (GameTime gameTime)
		{
			if (!IsGamePlaying ())
				return;
			
			for (int i = 0, ScreensCount = Screens.Count; i < ScreensCount; i++) {
				GameScreen screen = Screens [i];
				screen.Update (gameTime);
			}
			
			while (TouchPanel.IsGestureAvailable) {
				var gesture = TouchPanel.ReadGesture ();
				switch (gesture.GestureType) {
				case GestureType.Tap:
					var userHand = GetTappedUserHand (gesture.Position);
					if (IsUserHandTapped (userHand)) {
						var result = GetMatchResult (userHand);
						HandleMatchResult (result); 
					}
					break;
				}
			}
			base.Update (gameTime);
		}

		private Hand GetTappedUserHand (Vector2 actualTappedPosition)
		{
			var virtualTappedPosition = Resolution.GetVirtualFromActual (actualTappedPosition);
			return ActionBarScreen.GetTappedUserHand (virtualTappedPosition);
		}

		private bool IsUserHandTapped (Hand userHand)
		{
			return !Hand.IsDummy (userHand);
		}

		private MatchResult GetMatchResult (Hand tappedUserHand)
		{
			return HandScreen.GetMatchResult (tappedUserHand);
		}

		private void HandleMatchResult (MatchResult result)
		{
			switch (result) {
			case MatchResult.Victory:
				HandScreen.ChangeCollisionAreaColor (Color.Green);
				break;
			case MatchResult.Draw:
				HandScreen.ChangeCollisionAreaColor (Color.Yellow);
				break;
			case MatchResult.Defeat:
				HandScreen.ChangeCollisionAreaColor (Color.Red);
				break;
			case MatchResult.WrongTiming:
				HandScreen.ChangeCollisionAreaColor (Color.Blue);
				break;
			default:
				throw new ArgumentException ("Unrecognized MatchResult type");
				break;
			}
		}

		public override void Draw (GameTime gameTime)
		{
			if (!IsGamePlaying ())
				return;
			
			for (int i = 0, ScreensCount = Screens.Count; i < ScreensCount; i++) {
				GameScreen screen = Screens [i];
				screen.Draw (gameTime);
			}
			
			base.Draw (gameTime);
		}

		#endregion

		#region Methods

		public void AddScreen (GameScreen screen)
		{
			Screens.Add (screen);
		}

		public void PopScreen (GameScreen screen)
		{
			Screens.Remove (screen);
			screen.UnloadContent ();			
		}

		public IEnumerable<GameScreen> GetScreens (Func<GameScreen, bool> filter)
		{
			return (from x in Screens
			        where filter (x)
			        select x);
		}

		protected bool IsGamePlaying ()
		{
			return GameControl.IsPlaying;
		}

		#endregion
	}
}

