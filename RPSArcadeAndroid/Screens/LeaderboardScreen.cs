#region File Description
//-----------------------------------------------------------------------------//
// LeaderboardScreen.cs														   //
//																			   //
// Class																	   //
// Represents the global leaderboard								           //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion
namespace RPSArcadeAndroid
{
	public sealed class LeaderboardScreen: GameScreen
	{
		#region Constants

		public const string LEADERBOARD_XNB = "Textures/leaderboard";
		public const string FONT_XNB = "Fonts/motorWerkFont";
		public const string SMALL_FONT_XNB = "Fonts/motorWerkFontSmall";
		public const string LOADING_STRING = "RETRIEVING GLOBAL SCORES";
		public const float LEADERBOARD_CENTER_X = 240f;
		public const float LEADERBOARD_CENTER_Y = 400f;
		public const float LEADERBOARD_WIDTH = 420f;
		public const float LEADERBOARD_HEIGHT = 500f;
		public const float LEADERBOARD_TITLE_CENTER_Y = 200f;
		public const float LEADERBOARD_NAME_ORIGIN_X = 55f;
		public const float LEADERBOARD_SCORE_ORIGIN_X = 300f;
		public const float LEADERBOARD_SCORE_ORIGIN_Y = 240f;
		public const float GAP_BETWEEN_SCORES = 35f;
		public const float LOADING_STRING_ORIGIN_X = 50f;
		public const float LOADING_STRING_ORIGIN_Y = 240f;

		#endregion

		#region Properties

		public Texture2D LeaderboardTexture { get; set; }

		public Vector2 LeaderboardCenter { get; set; }

		public Vector2 LeaderboardSize  { get; set; }

		public SpriteFont Font { get; set; }

		public SpriteFont SmallFont { get; set; }

		public  List<UserScore> Scores { get; set; }

		#endregion

		#region Initialization

		public LeaderboardScreen (Game game, ScreenManager screenManager)
			: base (game, screenManager)
		{
			LoadTextures ();			
		}

		private void LoadTextures ()
		{
			LeaderboardTexture = LoadTexture2D (LEADERBOARD_XNB);			
			LeaderboardCenter = new Vector2 (LEADERBOARD_CENTER_X, LEADERBOARD_CENTER_Y);
			LeaderboardSize = new Vector2 (LEADERBOARD_WIDTH, LEADERBOARD_HEIGHT);
			Font = LoadSpriteFont (FONT_XNB);
			SmallFont = LoadSpriteFont (SMALL_FONT_XNB);
			
		}

		#endregion

		#region Update and Draw

		public override void Update (GameTime gameTime)
		{
			if (GameControl.IsPlaying)
				Scores = null;
			
			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			if (!GameControl.IsPlaying) {
				if (Scores != null && Scores.Count != 0) {
					DrawLeaderboardBackground ();
					DrawScores ();
				} 
				base.Draw (gameTime);
			}
		}

		private void DrawLeaderboardBackground ()
		{
			Resolution.DrawAtCenter (LeaderboardTexture, LeaderboardCenter, LeaderboardSize);
		}

		private void 	DrawScores ()
		{									
			Resolution.DrawStringAtCenter (Font, "GLOBAL RANK", new Vector2 (LEADERBOARD_CENTER_X, LEADERBOARD_TITLE_CENTER_Y), Color.Black);			
			float center_y = LEADERBOARD_SCORE_ORIGIN_Y;
			int i = 0;
			bool isUserNameInTop = false;
			foreach (var score in Scores) {		
				if (score.UserName == null)
					continue;
				i++;
				var rankAndName = string.Format ("{0,-2} {1, -12}", i, score.UserName, score.Score);
				var scoreString = string.Format ("{0, 10}", score.Score);
				Color color;
				if (score.UserName.Equals (UserAccount.Id)) {
					isUserNameInTop = true;
					color = Color.Red;
				} else
					color = Color.Black;					
				Resolution.DrawStringAtOrigin (SmallFont, rankAndName, new Vector2 (LEADERBOARD_NAME_ORIGIN_X, center_y), color);
				Resolution.DrawStringAtOrigin (SmallFont, scoreString, new Vector2 (LEADERBOARD_SCORE_ORIGIN_X, center_y), color);
				center_y += GAP_BETWEEN_SCORES;
			}			
			
			if (!isUserNameInTop) {
				var line = string.Format ("{0,-2} {1, -12}", "??", UserAccount.Id);
				Resolution.DrawStringAtOrigin (SmallFont, line, new Vector2 (LEADERBOARD_NAME_ORIGIN_X, center_y), Color.Red);
				line = string.Format ("{0, 10}", UserAccount.Score.ToString ());
				Resolution.DrawStringAtOrigin (SmallFont, line, new Vector2 (LEADERBOARD_SCORE_ORIGIN_X, center_y), Color.Red);
			}
		}

		public async Task ShowGlobalScores ()
		{
			Scores = await UserAccount.GetGlobalLeaderBoard (10);
		}

		public void CloseLeaderboard ()
		{
			Scores = null;
		}

		#endregion
	}
}

