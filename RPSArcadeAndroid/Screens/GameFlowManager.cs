#region File Description
//-----------------------------------------------------------------------------//
// GameFlowManager.cs														   //
//																			   //
// Class																	   //
// Represents the manager of the menu screens						           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;

#endregion
namespace RPSArcadeAndroid
{
	public enum Menu
	{
		Start,
		Resume,
		Restart,
		LeaderBoard,
		Sound,
		Exit,
		None
	}

	public class GameFlowManager: ScreenManager
	{
		#region Fields

		private bool loadingOn;
		private bool leaderboardOn;
		public const string SMALL_FONT_XNB = "Fonts/motorWerkFontSmall";
		public const string LOADING_STRING = "RETRIEVING GLOBAL SCORES";
		public const float LOADING_STRING_ORIGIN_X = 50f;
		public const float LOADING_STRING_ORIGIN_Y = 240f;

		#endregion

		#region Properties

		public bool IsSoundOn { get { return MenuScreen.IsSoundOn; } }

		public LoadingScreen LoadingScreen { get; set; }

		public MenuScreen MenuScreen { get; set; }

		public Vector2 LoadingStringOrigin { get; set; }

		public LeaderboardScreen LeaderboardScreen { get; set; }

		public SpriteFont SmallFont { get; set; }

		public Song Song1{ get; set; }

		public Song Song2{ get; set; }

		public Song Song3{ get; set; }

		public Song Song4{ get; set; }

		private List<Song> Songs { get; set; }

		public List<Song> BackgroundMusic { get; set; }

		public Song MenuSong { get; set; }

		#endregion

		#region Singleton

		private static volatile GameFlowManager instance;

		public static GameFlowManager GetInstance (Game game)
		{
			if (instance == null)
				instance = new GameFlowManager (game);
				
			return instance;
		}

		#endregion

		#region Initialization

		private GameFlowManager (Game game)
            : base (game)
		{
			LoadingStringOrigin = new Vector2 (LOADING_STRING_ORIGIN_X, LOADING_STRING_ORIGIN_Y);
			Songs = new List<Song> ();
		}

		public override void Initialize ()
		{
			TouchPanel.EnabledGestures = GestureType.Tap;			
			base.Initialize ();
		}

		protected override void LoadContent ()
		{				
			MenuScreen = new MenuScreen (Game, this);
			AddScreen (MenuScreen);
			LoadingScreen = new LoadingScreen (Game, this);
			AddScreen (LoadingScreen);	
			LeaderboardScreen = new LeaderboardScreen (Game, this);
			AddScreen (LeaderboardScreen);
			SmallFont = LoadSpriteFont (SMALL_FONT_XNB);
			Song1 = LoadSong ("Songs/marimbaLoop");
			Songs.Add (Song1);
			Song2 = LoadSong ("Songs/MTA");
			Songs.Add (Song2);
			Song3 = LoadSong ("Songs/Killing Time");
			Songs.Add (Song3);
			Song4 = LoadSong ("Songs/Electrodoodle");
			Songs.Add (Song4);
			MenuSong = LoadSong ("Songs/Aurea Carmina");
			base.LoadContent ();
		}

		#endregion

		#region Update and Draw

		public bool IsMenuSongPlaying { get; set; }

		public bool IsBackgroundMusicPlaying { get; set; }

		public override async void Update (GameTime gameTime)
		{	
			if (IsSoundOn) {
				if (GameControl.IsPaused)
					PauseMusic ();
				else if (GameControl.IsEnding)
					StopMusic ();
				else if ((GameControl.GamePlayState == GamePlayState.NotStarted || GameControl.IsTerminated) && !IsMenuSongPlaying)
					StartMenuSong ();
				else if (IsMenuSongPlaying)
					ResumeMusic ();
				else if (!GameControl.IsTerminated && !IsBackgroundMusciDone ())
					ResumeMusic ();
				else if (!GameControl.IsTerminated && IsBackgroundMusciDone ())
					PlayNextBackgroundMusic ();
			} else {
				PauseMusic ();
			}
			if (GameControl.IsPlaying || GameControl.IsEnding || loadingOn)
				return;
				
			
			if (InputManager.IsTapped ()) {
				if (IsGameExiting)
					HandleExit ();
				else if (leaderboardOn) {
					leaderboardOn = false;
					LeaderboardScreen.CloseLeaderboard ();
				} else {
					var menu = GetTappedMenu (InputManager.LastTappedPosition);				
					switch (menu) {
					case Menu.Start:
						await LoadingScreen.ShowLoadingScreen (gameTime);
						await GameControl.Start ();
						LoadingScreen.CloseLoadingScreen ();
						if (IsSoundOn)
							StartBackgroundMusic ();
						break;
					case Menu.Resume:
						GameControl.Resume ();
						if (IsSoundOn)
							ResumeMusic ();
						break;
					case Menu.Restart:
						await LoadingScreen.ShowLoadingScreen (gameTime);
						await GameControl.Restart ();
						LoadingScreen.CloseLoadingScreen ();
						if (IsSoundOn)
							StartBackgroundMusic ();
						break;
					case Menu.LeaderBoard:
						loadingOn = true;
						leaderboardOn = true;
						var userscore = await AzureDB.GetUserScoreAsync ();
						if (userscore != null)
							UserAccount.Score = userscore.Score;
						await LeaderboardScreen.ShowGlobalScores ();
						loadingOn = false;					
						break;
					case Menu.Sound:
						if (IsSoundOn)
							MenuScreen.SoundOff ();
						else
							MenuScreen.SoundOn ();
						break;
					case Menu.Exit:
						IsGameExiting = true;						
						return;
					case Menu.None:
						break;
					default:
						throw new Exception (string.Format ("Unrecognized Menu Type: {0}", menu));
					}
				}
			}
			
			base.Update (gameTime);
		}

		void HandleExit ()
		{
			if (IsYesTappedForExitMenu ()) {
				Game.Exit ();
				System.Environment.Exit (0);
			} else {
				IsGameExiting = false;
				return;
			}
		}

		private bool IsYesTappedForExitMenu ()
		{
			return MenuScreen.ExitYesRectangle.Contains (InputManager.LastTappedPosition);
		}

		private Menu GetTappedMenu (Vector2 tappedPosition)
		{
			if (MenuScreen.StartRectangle.Contains (tappedPosition)) {
				if (GameControl.IsPaused)
					return Menu.Resume;
				else
					return Menu.Start;
			} else if (MenuScreen.RestartRectangle.Contains (tappedPosition))
				return Menu.Restart;
			else if (MenuScreen.LeaderBoardRectangle.Contains (tappedPosition))
				return Menu.LeaderBoard;
			else if (MenuScreen.SoundRectangle.Contains (tappedPosition))
				return Menu.Sound;
			else if (MenuScreen.ExitRectangle.Contains (tappedPosition))
				return Menu.Exit;
			else
				return Menu.None;
		}

		public override void Draw (GameTime gameTime)
		{	
			if (GameControl.IsPlaying)
				return;
				
			if (loadingOn) {
				Resolution.BeginDraw ();
				DrawLoading (gameTime);
				Resolution.EndDraw ();
			}
			
			base.Draw (gameTime);			
		}

		private void DrawLoading (GameTime gameTime)
		{			
			var remainder = (int)(gameTime.TotalGameTime.TotalMilliseconds / 500) % 4;
			string dots = string.Empty;
			for (int i = 0; i < remainder; i++) {
				dots += ".";
			}
			Resolution.DrawStringAtOrigin (SmallFont, LOADING_STRING + dots, LoadingStringOrigin, Color.DarkBlue);
			
		}

		private SpriteFont LoadSpriteFont (string assetName)
		{
			return Game.Content.Load<SpriteFont> (assetName);
		}

		private Song LoadSong (string assetName)
		{
			return Game.Content.Load<Song> (assetName);
		}

		private int currentBackgroundMusic;

		private void StartMenuSong ()
		{			
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Volume = 1f;
			MediaPlayer.Play (MenuSong);
			IsMenuSongPlaying = true;
		}

		private void StartBackgroundMusic ()
		{
			StopMusic ();
			
			BackgroundMusic = new List<Song> ();
			BackgroundMusic.AddRange (Songs);
			Shuffle (BackgroundMusic);
			currentBackgroundMusic = 0;
			MediaPlayer.IsRepeating = false;
			MediaPlayer.Play (BackgroundMusic [currentBackgroundMusic]);
			IsBackgroundMusicPlaying = true;
		}

		private void PauseBackgroundMusic ()
		{
			if (MediaPlayer.State == MediaState.Paused)
				MediaPlayer.Pause ();
		}

		private void StopMusic ()
		{
			if (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused) {
				BackgroundMusic = null;
				MediaPlayer.Stop ();
				IsBackgroundMusicPlaying = false;
				IsMenuSongPlaying = false;
			}
		}

		private void PauseMusic ()
		{
			MediaPlayer.Pause ();
		}

		private void ResumeMusic ()
		{
			if (MediaPlayer.State == MediaState.Paused)
				MediaPlayer.Resume ();
		}

		private bool IsBackgroundMusciDone ()
		{
			return (IsBackgroundMusicPlaying && MediaPlayer.State == MediaState.Stopped);
		}

		private void PlayNextBackgroundMusic ()
		{
			if (BackgroundMusic != null) {
				currentBackgroundMusic = (currentBackgroundMusic + 1) % BackgroundMusic.Count;
				MediaPlayer.Play (BackgroundMusic [currentBackgroundMusic]);
			}
			
		}

		public static void Shuffle<T> (IList<T> list)
		{
			int n = list.Count;
			Random rnd = new Random ();
			while (n > 1) {
				int k = (rnd.Next (0, n) % n);
				n--;
				T value = list [k];
				list [k] = list [n];
				list [n] = value;
			}
		}

		#endregion
	}
}

