#region File Description
//-----------------------------------------------------------------------------//
// MenuScreen.cs															   //
//																			   //
// Class																	   //
// Screen containing all menus										           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statementsusing System;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

#endregion
namespace RPSArcadeAndroid
{
	public class MenuScreen: GameScreen
	{
		#region Constants

		public const string MENU_STRING_FONT = "Fonts/motorWerkFont";
		public const string EXIT_CONFIRMATION__BACKGROUND = "Textures/hudBar";
		public const string BACKGROUND_XNB = "Textures/background";
		public const string START_STRING = "START";
		public const string RESTART_STRING = "RESTART";
		public const string RESUME_STRING = "RESUME";
		public const string LEADERBOARD_STRING = "LEADER BOARD";
		public const string SOUND_STRING_ON = "SOUND ON";
		public const string SOUND_STRING_OFF = "SOUND OFF";
		public const string ON_STRING = "ON";
		public const string OFF_STRING = "OFF";
		public const string EXIT_STRING = "EXIT";
		public const string EXIT_CONFIRMATION__STRING = "ARE YOU SURE?";
		public const float MENU_CENTER_X = 240f;
		public const float START_CENTER_Y = 200f;
		public const float RESTART_CENTER_Y = 300f;
		public const float LEADERBOARD_CENTER_Y = 400f;
		public const float SOUND_CENTER_Y = 500f;
		public const float EXIT_CENTER_Y = 600f;
		public const float EXIT_CONFIRMATION_STRING_X = 240f;
		public const float EXIT_CONFIRMATION_STRING_Y = 300f;
		public const float EXIT_YESNO_Y = 400f;
		public const float EXIT_YES_CENTER_X = 150f;
		public const float EXIT_NO_CENTER_X = 350f;

		#endregion

		#region Fields

		private Rectangle backgroundRectangle;
		private Rectangle exitConfirmationRectangle;
		private Color menuStringColor = Color.White;
		private Color inactiveMenuStringColor = Color.Gray;

		#endregion

		#region Properties

		public bool IsSoundOn { get { return SoundText.Equals (SOUND_STRING_ON); } }

		public Texture2D BackgroundTexture { get; set; }

		public SpriteFont Font { get; set; }

		public Vector2 StartCenter { get; set; }

		public Vector2 RestartCenter { get; set; }

		public Vector2 LeaderBoardCenter { get; set; }

		public Vector2 SoundCenter { get; set; }

		public Vector2 ExitCenter { get; set; }

		public Vector2 ExitConfirmationCenter { get; set; }

		public Texture2D ExitConfirmationTexture { get; set; }

		public Vector2 ExitYesCenter { get; set; }

		public Vector2 ExitNoCenter { get; set; }

		public Rectangle StartRectangle { get; set; }

		public Rectangle RestartRectangle { get; set; }

		public Rectangle LeaderBoardRectangle { get; set; }

		public Rectangle SoundRectangle { get; set; }

		public Rectangle ExitRectangle { get; set; }

		public Rectangle ExitYesRectangle { get; set; }

		public Rectangle ExitNoRectangle { get; set; }

		public string SoundText { get; set; }

		#endregion

		#region Initialization

		public MenuScreen (Game game, ScreenManager screenManager)
			: base (game, screenManager)
		{
			LoadContent ();
			InitializeMenus ();			
			SoundText = SOUND_STRING_ON;
		}

		private void LoadContent ()
		{
			Font = LoadSpriteFont (MENU_STRING_FONT);
//			BackgroundTexture = new Texture2D (ScreenManager.GraphicsDevice, 1, 1);
//			BackgroundTexture.SetData (new Color[] { Color.White });
			BackgroundTexture = LoadTexture2D (BACKGROUND_XNB);
			backgroundRectangle = new Rectangle (0, 0, 480, 800);
			ExitConfirmationTexture = LoadTexture2D (EXIT_CONFIRMATION__BACKGROUND);
		}

		private void InitializeMenus ()
		{
			InitializeStartMenu ();
			InitializeRestartMenu ();
			InitializeLeaderBoardMenu ();
			InitializeSoundMenu ();
			InitializeExitMenu ();						
		}

		private void InitializeStartMenu ()
		{
			StartCenter = new Vector2 (MENU_CENTER_X, START_CENTER_Y);
			StartRectangle = GetRectangleForText (START_STRING, MENU_CENTER_X, START_CENTER_Y);
		}

		private void InitializeRestartMenu ()
		{
			RestartCenter = new Vector2 (MENU_CENTER_X, RESTART_CENTER_Y);
			RestartRectangle = GetRectangleForText (RESTART_STRING, MENU_CENTER_X, RESTART_CENTER_Y);
		}

		private void InitializeLeaderBoardMenu ()
		{
			LeaderBoardCenter = new Vector2 (MENU_CENTER_X, LEADERBOARD_CENTER_Y);
			LeaderBoardRectangle = GetRectangleForText (LEADERBOARD_STRING, MENU_CENTER_X, LEADERBOARD_CENTER_Y);
		}

		private void InitializeSoundMenu ()
		{
			SoundCenter = new Vector2 (MENU_CENTER_X, SOUND_CENTER_Y);
			SoundRectangle = GetRectangleForText (SOUND_STRING_ON, MENU_CENTER_X, SOUND_CENTER_Y);
		}

		private void InitializeExitMenu ()
		{
			ExitCenter = new Vector2 (MENU_CENTER_X, EXIT_CENTER_Y);
			ExitRectangle = GetRectangleForText (EXIT_STRING, MENU_CENTER_X, EXIT_CENTER_Y);
			
			ExitConfirmationCenter = new Vector2 (EXIT_CONFIRMATION_STRING_X, EXIT_CONFIRMATION_STRING_Y);
			var fontSize = Font.MeasureString (EXIT_CONFIRMATION__STRING);
			var x = EXIT_CONFIRMATION_STRING_X - (fontSize.X / 2 + 50);
			var y = EXIT_CONFIRMATION_STRING_Y - (fontSize.Y / 2 + 10);
			var width = fontSize.X + 100;
			var height = EXIT_YESNO_Y - EXIT_CONFIRMATION_STRING_Y + 80;
			exitConfirmationRectangle = new Rectangle ((int)x, (int)y, (int)width, (int)height);			
			ExitYesCenter = new Vector2 (EXIT_YES_CENTER_X, EXIT_YESNO_Y);
			ExitNoCenter = new Vector2 (EXIT_NO_CENTER_X, EXIT_YESNO_Y);
			ExitYesRectangle = GetRectangleForText ("YES", EXIT_YES_CENTER_X, EXIT_YESNO_Y);
			ExitNoRectangle = GetRectangleForText ("No", EXIT_NO_CENTER_X, EXIT_YESNO_Y);			
		}

		private Rectangle GetRectangleForText (string txt, float centerX, float centerY)
		{
			var stringSize = Font.MeasureString (txt);
			var originX = centerX - stringSize.X / 2;
			var originY = centerY - stringSize.Y / 2;
			return new Rectangle ((int)originX, (int)originY, (int)stringSize.X, (int)stringSize.Y);
		}

		#endregion

		#region Update and Draw

		public override void Update (GameTime gameTime)
		{
			if (GameControl.GamePlayState == GamePlayState.Restarting)
				TouchPanel.EnabledGestures = GestureType.None;
			else
				TouchPanel.EnabledGestures = GestureType.Tap;	
			
			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{			
			DrawBackground ();			
			DrawStartMenu ();	
			DrawRestartMenu ();
			DrawLeaderBoardMenu ();			
			DrawSoundMenu ();
			DrawExitMenu ();
			if (ScreenManager.IsGameExiting)
				DrawExitConfirmation ();
			base.Draw (gameTime);		
		}

		private void DrawBackground ()
		{
			Resolution.DrawAtRectangle (BackgroundTexture, backgroundRectangle, 0.5f);
		}

		private void DrawStartMenu ()
		{
			var gamePlayState = GameControl.GamePlayState;
			switch (gamePlayState) {
			case GamePlayState.NotStarted:
			case GamePlayState.Ended:
				Resolution.DrawStringAtCenter (Font, START_STRING, StartCenter, menuStringColor);
				break;
			case GamePlayState.Playing:
			case GamePlayState.Paused:
			case GamePlayState.Restarting:
			case GamePlayState.Ending:
				Resolution.DrawStringAtCenter (Font, RESUME_STRING, StartCenter, menuStringColor);
				break;
			default:
				throw new Exception (string.Format ("Unrecognized GamePlayState: {0}", gamePlayState));
			}				
		}

		private void DrawRestartMenu ()
		{
			var gamePlayState = GameControl.GamePlayState;
			switch (gamePlayState) {
			case GamePlayState.Restarting:
			case GamePlayState.Paused:
				Resolution.DrawStringAtCenter (Font, RESTART_STRING, RestartCenter, menuStringColor);
				break;
			case GamePlayState.NotStarted:
			case GamePlayState.Playing:
			case GamePlayState.Ended:
			case GamePlayState.Ending:
				Resolution.DrawStringAtCenter (Font, RESTART_STRING, RestartCenter, inactiveMenuStringColor);
				break;
			default:
				throw new Exception (string.Format ("Unrecognized GamePlayState: {0}", gamePlayState));
			}				
		}

		private void DrawLeaderBoardMenu ()
		{
			Resolution.DrawStringAtCenter (Font, LEADERBOARD_STRING, LeaderBoardCenter, menuStringColor);
		}

		private void DrawSoundMenu ()
		{
			Resolution.DrawStringAtCenter (Font, SoundText, SoundCenter, menuStringColor);
		}

		private void DrawExitMenu ()
		{
			Resolution.DrawStringAtCenter (Font, EXIT_STRING, ExitCenter, menuStringColor);
		}

		private void DrawExitConfirmation ()
		{
			Resolution.DrawAtRectangle (BackgroundTexture, backgroundRectangle, 0.5f);
			Resolution.DrawAtRectangle (ExitConfirmationTexture, exitConfirmationRectangle, 1f, Color.DodgerBlue);
			Resolution.DrawStringAtCenter (Font, EXIT_CONFIRMATION__STRING, ExitConfirmationCenter, Color.White);
			Resolution.DrawStringAtCenter (Font, "Yes", ExitYesCenter, Color.White);
			Resolution.DrawStringAtCenter (Font, "No", ExitNoCenter, Color.White);
		}

		public void SoundOn ()
		{
			SoundText = SOUND_STRING_ON;
		}

		public void SoundOff ()
		{
			SoundText = SOUND_STRING_OFF;
		}

		#endregion
	}
}

