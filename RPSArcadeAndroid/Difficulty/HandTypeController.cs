#region File Description
//-----------------------------------------------------------------------------//
// HandTypeController.cs											           //
//																			   //
// Class																	   //
// Represents an difficulty level that involves hand types			           //
// Manipulates the speed of hands.											   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion
namespace RPSArcadeAndroid
{
	public class HandTypeController: DifficultyController
	{
		#region Constants

		public const float HANDTYPE_CHANGE_INTERVAL_LIMIT = 0.5f;
		public const float HEIGHT_FOR_WHICH_CHANGE_OCCURS = 300f;
		public const float HEIGHT_FOR_WHICH_CHANGE_OCCURS_LIMIT = 550f;
		public const float DELTA_HEIGHT_FOR_WHICH_CHANGE_OCCURS = 50f;
		public const float DEFAULT_HANDTYPE_CHANGE_INTERVAL_IN_SEC = 3f;
		public const float DEFAULT_DELTA_HANDTYPE_CHANGE_IN_SEC = 0.5f;

		#endregion

		#region Fields

		private HandType HandType;
		private TimeSpan deltaChangeInterval;
		private TimeSpan changeInterval;
		private TimeSpan changeIntervalLimit;
		private float heightThreshold;
		private IDictionary<Hand, TimeSpan> lastChangedTimes;
		private Random random = new Random ();

		#endregion

		#region Initialization

		public HandTypeController () : base ()
		{
			changeInterval = TimeSpan.FromSeconds (DEFAULT_HANDTYPE_CHANGE_INTERVAL_IN_SEC);
			changeIntervalLimit = TimeSpan.FromSeconds (HANDTYPE_CHANGE_INTERVAL_LIMIT);
			deltaChangeInterval = TimeSpan.FromSeconds (DEFAULT_DELTA_HANDTYPE_CHANGE_IN_SEC);
			heightThreshold = HEIGHT_FOR_WHICH_CHANGE_OCCURS;
			lastChangedTimes = new Dictionary<Hand, TimeSpan> ();
		}

		#endregion

		#region implemented Members of DifficultyController

		public override void IncrementLevel (GameTime gameTime)
		{
			base.IncrementLevel (gameTime);
			if (IsOddNumber (Level))
				UpdateHandTypeChangeInterval (gameTime);
			else
				UpdateHeightForWhichChangeOccurs ();			
		}

		private bool IsOddNumber (int number)
		{
			return (number % 2) == 1;
		}

		private void UpdateHandTypeChangeInterval (GameTime gameTime)
		{
			changeInterval -= deltaChangeInterval;
			changeInterval = changeInterval > changeIntervalLimit ? changeInterval : changeIntervalLimit;
		}

		private void UpdateHeightForWhichChangeOccurs ()
		{
			heightThreshold += DELTA_HEIGHT_FOR_WHICH_CHANGE_OCCURS;
			heightThreshold = heightThreshold < HEIGHT_FOR_WHICH_CHANGE_OCCURS_LIMIT ? heightThreshold : HEIGHT_FOR_WHICH_CHANGE_OCCURS_LIMIT;
		}

		public override void ApplyLevel (HandScreen screen, Hand hand, GameTime gameTime)
		{			
			if (hand.CurrentAnimation.IsAnimating == false && hand.Origin.Y <= heightThreshold)
				UpdateHandType (hand, gameTime);
		}

		private void UpdateHandType (Hand hand, GameTime gameTime)
		{			
			if (!hand.IsScheduledToDraw)
				lastChangedTimes.Remove (hand);
			else {
				AddToDictionaryIfNotAlready (hand, gameTime);
				RandomizeHandType (hand, gameTime);
			}
		}

		private void AddToDictionaryIfNotAlready (Hand hand, GameTime gameTime)
		{
			var isKnownHand = lastChangedTimes.ContainsKey (hand);
			if (!isKnownHand)
				lastChangedTimes.Add (hand, gameTime.TotalGameTime);
		}

		private void RandomizeHandType (Hand hand, GameTime gameTime)
		{
			var hasEnoughTimeElapsed = gameTime.TotalGameTime.Subtract (lastChangedTimes [hand]) >= TimeSpan.FromMilliseconds (changeInterval.TotalMilliseconds / 2 + changeInterval.TotalMilliseconds * random.NextDouble ());
			if (hasEnoughTimeElapsed) {
				HandType = GetRandomHandType ();
				hand.Type = HandType;
				lastChangedTimes [hand] = gameTime.TotalGameTime;
			}
		}

		private HandType GetRandomHandType ()
		{
			var randomInt = GetRandomInt (0, 2);
			return (HandType)randomInt;
		}

		#endregion

		#region Helper Methods

		private int GetRandomInt (int startInclusive, int endInclusive)
		{
			return random.Next (startInclusive, endInclusive + 1);
		}

		#endregion
	}
}

