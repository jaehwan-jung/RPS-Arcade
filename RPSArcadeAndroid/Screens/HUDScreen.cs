#region File Description
//-----------------------------------------------------------------------------//
// HUDScreen.cs															       //
//																			   //
// Class																	   //
// Represents the screen for the head-up display (score, time, etc)        	   //
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
	public sealed class HUDScreen: GameScreen
	{
		#region Constants

		public const string NEXT_LEVEL_BAR_BACKGROUND = "Textures/progressBar";
		public const string NEXT_LEVEL_BAR = "Textures/whiteProgressBar";
		public const string LEVEL_XNB = "Textures/levelBackground";
		public const string HUDBAR_XNB = "Textures/hudBar";
		public const string FONT_XNB = "Fonts/motorWerkFont";
		public const string SMALL_FONT_XNB = "Fonts/motorWerkFontSmall";
		public const string MENU_ICON_XNB = "Textures/settingsIcon";
		public const float NEXTLEVEL_BAR_ORIGIN_X = 150f;
		public readonly float NEXTLEVEL_BAR_ORIGIN_Y = 130f + Resolution.GetVirtualFromActual (new Vector2 (0f, GameActivity.AdBannerHeight)).Y;
		public const float NEXTLEVEL_BAR_WIDTH = 320f;
		public const float NEXTLEVEL_BAR_HEIGHT = 10f;
		public const float LEVEL_CENTER_X = 75f;
		public readonly float LEVEL_CENTER_Y = 135f + Resolution.GetVirtualFromActual (new Vector2 (0f, GameActivity.AdBannerHeight)).Y;
		public const float LEVEL_WIDTH = 130f;
		public const float LEVEL_HEIGHT = 25f;
		public const float SCOREBAR_WIDTH = 460f;
		public const float SCOREBAR_HEIGHT = 50f;
		public const float SCOREBAR_CENTER_X = 240f;
		public readonly float SCOREBAR_CENTER_Y = 30f + Resolution.GetVirtualFromActual (new Vector2 (0f, GameActivity.AdBannerHeight)).Y;
		public const float SCORE_CENTER_X = 240f;
		public readonly float SCORE_CENTER_Y = 30f + Resolution.GetVirtualFromActual (new Vector2 (0f, GameActivity.AdBannerHeight)).Y;
		public const float TIMEBAR_WIDTH = 380f;
		public const float TIMEBAR_HEIGHT = 50f;
		public const float TIMEBAR_CENTER_X = 200f;
		public readonly float TIMEBAR_CENTER_Y = 90f + Resolution.GetVirtualFromActual (new Vector2 (0f, GameActivity.AdBannerHeight)).Y;
		public const float ELAPSED_TIME_ORIGIN_X = 20f;
		public readonly float ELAPSED_TIME_ORIGIN_Y = 90f + Resolution.GetVirtualFromActual (new Vector2 (0f, GameActivity.AdBannerHeight)).Y;
		public const float MENU_ICON_ORIGIN_X = 400f;
		public readonly float MENU_ICON_ORIGIN_Y = 65f + Resolution.GetVirtualFromActual (new Vector2 (0f, GameActivity.AdBannerHeight)).Y;
		public const float MENU_ICON_WIDHT = 70f;
		public const float MENU_ICON_HEIGHT = 50f;
		public const float TIMING_CENTER_X = 240f;
		public readonly float TIMING_CENTER_Y = 170f + Resolution.GetVirtualFromActual (new Vector2 (0f, GameActivity.AdBannerHeight)).Y;
		public const int TIMING_DURATION_IN_MILLISEC = 1000;

		#endregion

		#region Fields

		private int extraScore;
		private bool isNewlyTapped;
		private Vector2 levelBackgroundSize;
		private Vector2 levelBackgroundOrigin;
		private Vector2 levelStringCenter;
		private Vector2 nextLevelBarBackgroundSize;
		private Vector2 nextLevelBarBackgroundOrigin;
		private Vector2 nextLevelBarOrigin;
		private Vector2 scoreboardBackgroundSize;
		private Vector2 scoreboardBackgroundOrigin;
		private Vector2 scoreboardCenter;
		private Vector2 menuIconOrigin;
		private Vector2 menuIconSize;
		private float tapTimingTextAlpha;
		private Vector2 timeboardBackgroundSize;
		private Vector2 timeboardBackgroundOrigin;
		private Vector2 timeboardCenter;
		private Vector2 timingTextCenter;
		private Vector2 nextLevelInnerBarSize;
		private Color nextLevelInnerBarColor;

		#endregion

		#region Properties

		public TimeSpan ElapsedTime { get; set; }

		public Texture2D HudBarTexture { get; set; }

		public SpriteFont Font { get; set; }

		public TimeSpan LastUpdatedTime { get; set; }

		public int Level  { get; set; }

		public Texture2D LevelBackground { get; set; }

		public Texture2D NextLevelBarBackground { get; set; }

		public Texture2D NextLevelBar { get; set; }

		public int Score { get; private set; }

		public Rectangle MenuIconRectangle { get; set; }

		public Texture2D MenuIconTexture { get; set; }

		public SpriteFont SmallFont { get; set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan TappedTime { get; private set; }

		public Timing TapTiming { get; private  set; }

		#endregion

		#region Initialization

		public HUDScreen (Game game, ScreenManager screenManager)
			: base (game, screenManager)
		{	
			Level = 1;
			LoadContent ();
			InitializeVariables ();			
		}

		private void LoadContent ()
		{
			HudBarTexture = LoadTexture2D (HUDBAR_XNB);
			MenuIconTexture = LoadTexture2D (MENU_ICON_XNB);
			Font = LoadSpriteFont (FONT_XNB);
			SmallFont = LoadSpriteFont (SMALL_FONT_XNB);
			LevelBackground = LoadTexture2D (LEVEL_XNB);
			NextLevelBarBackground = LoadTexture2D (NEXT_LEVEL_BAR_BACKGROUND);
			NextLevelBar = LoadTexture2D (NEXT_LEVEL_BAR);
		}

		private void InitializeVariables ()
		{
			InitializeScoreboard ();
			InitializeTimeboard ();
			InitializeMenuIcon ();
			InitializeNextLevelBar ();
			InitializeLevel ();
			InitializeTiming ();
		}

		private void InitializeScoreboard ()
		{
			scoreboardBackgroundSize = new Vector2 (SCOREBAR_WIDTH, SCOREBAR_HEIGHT);
			scoreboardBackgroundOrigin = new Vector2 (SCOREBAR_CENTER_X, SCOREBAR_CENTER_Y);
			scoreboardCenter = new Vector2 (SCORE_CENTER_X, SCORE_CENTER_Y);
		}

		private void InitializeTimeboard ()
		{
			timeboardBackgroundSize = new Vector2 (TIMEBAR_WIDTH, TIMEBAR_HEIGHT);
			timeboardBackgroundOrigin = new Vector2 (TIMEBAR_CENTER_X, TIMEBAR_CENTER_Y);
			timeboardCenter = new Vector2 (TIMEBAR_CENTER_X, TIMEBAR_CENTER_Y);
		}

		private void InitializeMenuIcon ()
		{
			menuIconOrigin = new Vector2 (MENU_ICON_ORIGIN_X, MENU_ICON_ORIGIN_Y);
			menuIconSize = new Vector2 (MENU_ICON_WIDHT, MENU_ICON_HEIGHT);
			MenuIconRectangle = new Rectangle ((int)menuIconOrigin.X, (int)menuIconOrigin.Y, (int)menuIconSize.X, (int)menuIconSize.Y);
		}

		private void InitializeNextLevelBar ()
		{
			nextLevelBarBackgroundSize = new Vector2 (NEXTLEVEL_BAR_WIDTH, NEXTLEVEL_BAR_HEIGHT);
			nextLevelBarBackgroundOrigin = new Vector2 (NEXTLEVEL_BAR_ORIGIN_X, NEXTLEVEL_BAR_ORIGIN_Y);
			nextLevelBarOrigin = new Vector2 (NEXTLEVEL_BAR_ORIGIN_X, NEXTLEVEL_BAR_ORIGIN_Y);
		}

		private void InitializeLevel ()
		{
			levelBackgroundSize = new Vector2 (LEVEL_WIDTH, LEVEL_HEIGHT);
			levelBackgroundOrigin = new Vector2 (LEVEL_CENTER_X, LEVEL_CENTER_Y);
			levelStringCenter = new Vector2 (LEVEL_CENTER_X, LEVEL_CENTER_Y);
		}

		private void InitializeTiming ()
		{
			timingTextCenter = new Vector2 (TIMING_CENTER_X, TIMING_CENTER_Y);
		}

		#endregion

		#region Update and Draw

		public override void Update (GameTime gameTime)
		{			
			if (!GameControl.IsPlaying)
				return;
			
			UpdateGameTime (gameTime);
			UpdateScore (gameTime);			
			LastUpdatedTime = gameTime.TotalGameTime;
			UpdateNextLevelBar (gameTime);
			base.Update (gameTime);
		}

		private void UpdateGameTime (GameTime gameTime)
		{
			if (StartTime == null || StartTime.Equals (TimeSpan.Zero))
				StartTime = gameTime.TotalGameTime;
			
			ElapsedTime += gameTime.TotalGameTime.Subtract (LastUpdatedTime);
		}

		private void UpdateScore (GameTime gameTime)
		{
			if (ElapsedTime == null || ElapsedTime.Equals (TimeSpan.Zero))
				Score = 0;
			else
				Score = extraScore + (int)ElapsedTime.TotalSeconds;
			
			UserAccount.Score = Score;
		}

		private void UpdateNextLevelBar (GameTime gameTime)
		{
			var width = (gameTime.TotalGameTime.Subtract (ScreenManager.LastLevelUpTime).TotalSeconds / 10f) * NEXTLEVEL_BAR_WIDTH;
			nextLevelInnerBarSize = new Vector2 ((float)width, NEXTLEVEL_BAR_HEIGHT);
			int r = (int)Math.Min (255 * (width / (NEXTLEVEL_BAR_WIDTH / 2)), 255);
			int g = (int)Math.Min (510 - 510 * width / NEXTLEVEL_BAR_WIDTH, 255); 
			nextLevelInnerBarColor = new Color (r, g, 0);
		}

		public override void Draw (GameTime gameTime)
		{
			DrawScoreboard ();
			DrawTimeBoard ();
			DrawMenuIcon ();
			DrawNextLevelBar (gameTime);
			DrawLevel ();
			DrawTiming (gameTime);
			base.Draw (gameTime);
		}

		private void DrawScoreboard ()
		{
			DrawScoreboardBackground ();
			DrawScoreText ();			
		}

		private void DrawScoreboardBackground ()
		{			
			Resolution.DrawAtCenter (HudBarTexture, scoreboardBackgroundOrigin, scoreboardBackgroundSize);
		}

		private void DrawScoreText ()
		{			
			Resolution.DrawStringAtCenter (Font, string.Format ("SCORE: {0}", Score.ToString ()), scoreboardCenter, Color.White);
		}

		private void DrawTimeBoard ()
		{
			DrawTimeBoardBackground ();
			DrawTime ();
		}

		private void DrawTimeBoardBackground ()
		{
			Resolution.DrawAtCenter (HudBarTexture, timeboardBackgroundOrigin, timeboardBackgroundSize);
		}

		private void DrawTime ()
		{
			Resolution.DrawStringAtCenter (Font, string.Format ("{0,-5} {1,-11}", "TIME:", ElapsedTime.ToString (@"hh\:mm\:ss\.FF")), timeboardCenter, Color.White);
		}

		private void DrawMenuIcon ()
		{
			Resolution.DrawAtOrigin (MenuIconTexture, menuIconOrigin, menuIconSize);
		}

		private void DrawNextLevelBar (GameTime gameTime)
		{
			DrawNextLevelBarBackground ();
			DrawNextLevelInnerBar (gameTime);
		}

		private void DrawNextLevelBarBackground ()
		{			
			Resolution.DrawAtOrigin (NextLevelBarBackground, nextLevelBarBackgroundOrigin, nextLevelBarBackgroundSize);
		}

		private void DrawNextLevelInnerBar (GameTime gameTime)
		{
			
			Resolution.DrawAtOrigin (NextLevelBar, nextLevelBarOrigin, nextLevelInnerBarSize, nextLevelInnerBarColor);
		}

		private void DrawLevel ()
		{
			DrawLevelBackground ();
			DrawLevelString ();
		}

		private void DrawLevelBackground ()
		{
			Resolution.DrawAtCenter (LevelBackground, levelBackgroundOrigin, levelBackgroundSize);
		}

		private void DrawLevelString ()
		{
			Resolution.DrawStringAtCenter (SmallFont, "LvL: " + Level.ToString (), levelStringCenter, Color.White);
		}

		#endregion

		#region Method

		public void DisplayTiming (Timing timing)
		{
			TapTiming = timing;
			isNewlyTapped = true;
			
		}

		private void DrawTiming (GameTime gameTime)
		{
			ResetTimingTextIfNewlyTapped (gameTime);			
			
			if (tapTimingTextAlpha > 0) {
				isNewlyTapped = false;				
				tapTimingTextAlpha = GetNewAlphaValue (gameTime);
				Color color = GetNewColor (tapTimingTextAlpha);	
				
				if (TapTiming == Timing.None)
					Resolution.DrawStringAtCenter (Font, string.Empty, timingTextCenter, color);
				else
					Resolution.DrawStringAtCenter (Font, TapTiming.ToString (), timingTextCenter, color);				
			}
		}

		private void ResetTimingTextIfNewlyTapped (GameTime gameTime)
		{
			if (isNewlyTapped) {
				TappedTime = gameTime.TotalGameTime;
				tapTimingTextAlpha = 255;
			}
		}

		private float GetNewAlphaValue (GameTime gameTime)
		{
			var elapsed = gameTime.TotalGameTime.Subtract (TappedTime).TotalMilliseconds;
			var ratio = elapsed / TIMING_DURATION_IN_MILLISEC;
			return (float)(255 - 255 * ratio);			
		}

		private Color GetNewColor (float alpha)
		{
			Color color = new Color (255, 255, 255);		
			color *= tapTimingTextAlpha / (float)byte.MaxValue;
			return color;
		}

		public void AddToScore (int value)
		{
			extraScore += value;
		}

		public override void Reset (GameTime gameTime)
		{
			InitializeVariables ();
			StartTime = TimeSpan.Zero;
			ElapsedTime = TimeSpan.Zero;
			LastUpdatedTime = TimeSpan.Zero;
			base.Reset (gameTime);
		}

		#endregion
	}
}

