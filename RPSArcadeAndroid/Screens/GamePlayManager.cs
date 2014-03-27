#region File Description
//-----------------------------------------------------------------------------//
// ScreenManager.cs															   //
//																			   //
// Class																	   //
// Represents the manager for gameplay logics and screens.	 				   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

#endregion
namespace RPSArcadeAndroid
{
	public sealed class GamePlayManager: ScreenManager
	{
		#region Constants

		public const string SMALL_FONT_XNB = "Fonts/motorWerkFontSmall";
		public const string LOADING_STRING = "RETRIEVING GLOBAL SCORES";
		public const string ROCK_SOUNDFX_XNB = "SoundFX/hit1";
		public const string PAPER_SOUNDFX_XNB = "SoundFX/hit2";
		public const string SCISSORS_SOUNDFX_XNB = "SoundFX/hit3";
		public const float LOADING_STRING_ORIGIN_X = 50f;
		public const float LOADING_STRING_ORIGIN_Y = 240f;
		public const int SCORE_FOR_GREAT = 100;
		public const int SCORE_FOR_GOOD = 25;
		public const int SCORE_FOR_SOSO = 10;

		#endregion

		#region Fields

		private bool loadingOn;

		#endregion

		#region Properties

		public ActionBarScreen ActionBarScreen { get; set; }

		public BackgroundScreen BackgroundScreen { get; set; }

		public HandScreen HandScreen { get; set; }

		public HUDScreen HudScreen { get; set; }

		public Vector2 LoadingStringOrigin { get; set; }

		public LeaderboardScreen LeaderboardScreen { get; set; }

		public bool IsGamePlaying { get { return GameControl.IsPlaying; } }

		public bool IsToBeReset { get { return GameControl.IsRestarting; } }

		public SpriteFont SmallFont { get; set; }

		public SoundEffectInstance RockSoundEffect { get; set; }

		public SoundEffectInstance PaperSoundEffect { get; set; }

		public SoundEffectInstance ScissorsSoundEffect { get; set; }

		#endregion

		#region Singleton

		private static volatile GamePlayManager instance;

		public static GamePlayManager GetInstance (Game game)
		{
			if (instance == null)
				instance = new GamePlayManager (game);
				
			return instance;
		}

		#endregion

		#region Initialization

		private GamePlayManager (Game game)
            : base (game)
		{	
			LoadingStringOrigin = new Vector2 (LOADING_STRING_ORIGIN_X, LOADING_STRING_ORIGIN_Y);
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
			LeaderboardScreen = new LeaderboardScreen (Game, this);
			AddScreen (LeaderboardScreen);
			SmallFont = LoadSpriteFont (SMALL_FONT_XNB);
			RockSoundEffect = LoadSoundFX (ROCK_SOUNDFX_XNB).CreateInstance ();
			RockSoundEffect.Volume = 1.3f;
			PaperSoundEffect = LoadSoundFX (PAPER_SOUNDFX_XNB).CreateInstance ();
			PaperSoundEffect.Volume = 3f;
			ScissorsSoundEffect = LoadSoundFX (SCISSORS_SOUNDFX_XNB).CreateInstance ();
			ScissorsSoundEffect.Volume = 0.3f;
			base.LoadContent ();
		}

		#endregion

		#region Update and Draw

		public override void Update (GameTime gameTime)
		{
			if (IsToBeReset)
				Reset (gameTime);
			
			if (GameControl.IsEnding) {
				if (!loadingOn && InputManager.IsTapped ()) {
					GameControl.Stop ();						
				}
			}
			
			if (!IsGamePlaying) {
				HudScreen.LastUpdatedTime = gameTime.TotalGameTime;
				HandScreen.LastReleaseTotalGameTime = gameTime.TotalGameTime;
				return;
			}
			
			if (HandScreen != null && HandScreen.Hands.Count != 0 && HandScreen.Hands [0].State == HandState.Expired)
				HandleMatchResult (new MatchResult () { Result = ResultType.Defeat });
			
			if (InputManager.IsTapped ()) {
				var tappedPosition = InputManager.LastTappedPosition;
				
				var userHand = GetTappedUserHand (tappedPosition);					
				if (IsUserHandTapped (userHand)) {
					userHand.ShowPressedEffect (gameTime);
					switch (userHand.Type) {
					case HandType.Rock:
					case HandType.RockPressed:
						RockSoundEffect.Play ();
						break;
					case HandType.Paper:
					case HandType.PaperPressed:
						PaperSoundEffect.Play ();
						break;
					case HandType.Scissors:
					case HandType.ScissorsPressed:
						ScissorsSoundEffect.Play ();
						break;
					default:
						break;
					}
					var result = GetMatchResult (userHand, gameTime);
					HandleMatchResult (result); 
				} else if (IsMenuTapped (tappedPosition)) {
					GameControl.Pause ();
				}
			}
				
			UpdateLevel (gameTime);
			base.Update (gameTime);
		}

		private Hand GetTappedUserHand (Vector2 virtualTappedPosition)
		{
			//	var virtualTappedPosition = Resolution.GetVirtualFromActual (actualTappedPosition);
			return ActionBarScreen.GetTappedUserHand (virtualTappedPosition);
		}

		private bool IsUserHandTapped (Hand userHand)
		{
			return !Hand.IsDummy (userHand);
		}

		private MatchResult GetMatchResult (Hand tappedUserHand, GameTime gameTime)
		{
			return HandScreen.GetMatchResult (tappedUserHand, gameTime);
		}

		private async void HandleMatchResult (MatchResult result)
		{
			switch (result.Result) {
			case ResultType.Victory:
				DisplayTiming (result.TapTiming);
				UpdateScore (result.TapTiming);
				break;
			case ResultType.Draw:
			case ResultType.Defeat:
			case ResultType.WrongTiming:					
				PauseWhileShowingScores ();
				loadingOn = true;				
				await PublishScore ();			
				await ShowLeaderboard ();
				ShowFullAd ();
				RefreshAd ();
				loadingOn = false;
				break;
			default:
				throw new ArgumentException (string.Format ("Unrecognized MatchResult type: {0}", result.Result));
			}
		}

		private void ShowFullAd ()
		{
			GameActivity.ShowFullAd ();
		}

		private void RefreshAd ()
		{
			GameActivity.RefreshFullAd ();
		}

		private void DisplayTiming (Timing timing)
		{
			HudScreen.DisplayTiming (timing);
		}

		private void UpdateScore (Timing timing)
		{
			switch (timing) {
			case Timing.Great:
				HudScreen.AddToScore (SCORE_FOR_GREAT);
				break;
			case Timing.Good:
				HudScreen.AddToScore (SCORE_FOR_GOOD);
				break;
			case Timing.SoSo:
				HudScreen.AddToScore (SCORE_FOR_SOSO);
				break;
			case Timing.None:
				break;
			default:
				throw new ArgumentException (string.Format ("Unrecognized MatchResult type: {0}", timing));
			}
		}

		private void PauseWhileShowingScores ()
		{
			GameControl.Pause ();
			GameControl.GamePlayState = GamePlayState.Ending;
		}

		private async Task PublishScore ()
		{
			await UserAccount.PublishScore (HudScreen.Score);	
		}

		private async Task ShowLeaderboard ()
		{			
			await LeaderboardScreen.ShowGlobalScores ();				
		}

		private bool IsMenuTapped (Vector2 tappedPosition)
		{
			return HudScreen.MenuIconRectangle.Contains (tappedPosition);
		}

		private void UpdateLevel (GameTime gameTime)
		{
			if (LastLevelUpTime == null || LastLevelUpTime.Equals (TimeSpan.Zero))
				LastLevelUpTime = gameTime.TotalGameTime;
			
			var currentTime = gameTime.TotalGameTime;
			if (currentTime.Subtract (LastLevelUpTime) >= LevelUpInterval) {
				LevelUp (gameTime);				
				LastLevelUpTime = currentTime;
			}
		}

		private void LevelUp (GameTime gameTime)
		{
			var levels = HandScreen.Levels;
			for (int i = 0, levelsCount = levels.Count; i < levelsCount; i++)
				levels [i].IncrementLevel (gameTime);
			
			HudScreen.Level++;
		}

		public override void Draw (GameTime gameTime)
		{
				
			if (!GameControl.IsPlaying && !GameControl.IsEnding)
				return;
						
			base.Draw (gameTime);
			
			if (loadingOn) {
				Resolution.BeginDraw ();
				DrawLoading (gameTime);
				Resolution.EndDraw ();
			}
			
		}

		public override void Reset (GameTime gameTime)
		{
			if (String.IsNullOrEmpty (MyLogger.CurrentMessage)) {
				MyLogger.CurrentMessage = "Unloading Contents...";
				UnloadContent ();
			} else if (MyLogger.CurrentMessage.Equals ("Unloading Contents...")) {
				MyLogger.CurrentMessage = "Loading Contents...";
				LoadContent ();
			} else if (MyLogger.CurrentMessage.Equals ("Loading Contents...")) {
				MyLogger.CurrentMessage = "Resetting...";				
				base.Reset (gameTime);
			} 
		}

		private void DrawLoading (GameTime gameTime)
		{			
			var remainder = (int)(gameTime.TotalGameTime.TotalMilliseconds / 500) % 4;
			string dots = string.Empty;
			for (int i = 0; i < remainder; i++) {
				dots += ".";
			}
			Resolution.DrawStringAtOrigin (SmallFont, LOADING_STRING + dots, LoadingStringOrigin, Color.White);
			
		}

		private SpriteFont LoadSpriteFont (string assetName)
		{
			return Game.Content.Load<SpriteFont> (assetName);
		}

		private SoundEffect LoadSoundFX (string asset)
		{
			return Game.Content.Load<SoundEffect> (asset);
		}

		#endregion
	}
}

