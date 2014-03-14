#region File Description
//-----------------------------------------------------------------------------//
// DownFallSpeedLevel.cs											    	   //
//																			   //
// Class																	   //
// Represents a difficulty level that involves speed and acceleration only     //
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
	public class SpeedController: DifficultyController
	{
		#region Constants

		public const double DEFAULT_INTERVAL_LIMIT = 0.3;
		public const int DEFAULT_INTERVAL = 2;
		public const int DEFAULT_SPEED = 100;
		public const int DEFAULT_SPEED_LIMIT = 1600;
		public const double DELTA_INTERVAL = 0.1;
		public const int DELTA_SPEED = 2;

		#endregion

		#region Fields

		private int deltaSpeed;
		private TimeSpan deltaInterval;
		private TimeSpan releaseInterval;
		private TimeSpan releaseIntervalLimit;
		private int speed;

		#endregion

		#region Initialization

		public SpeedController () : base ()
		{
			deltaSpeed = DELTA_SPEED;
			deltaInterval = TimeSpan.FromSeconds (DELTA_INTERVAL);
			releaseInterval = TimeSpan.FromSeconds (DEFAULT_INTERVAL);
			releaseIntervalLimit = TimeSpan.FromSeconds (DEFAULT_INTERVAL_LIMIT);
			speed = DEFAULT_SPEED;		
		}

		#endregion

		#region GameLevel Implementation

		public override void IncrementLevel (GameTime gameTime)
		{
			base.IncrementLevel (gameTime);
			UpdateSpeed ();
			UpdateInterval ();
		}

		private void UpdateSpeed ()
		{
			speed += deltaSpeed;
			speed = speed > DEFAULT_SPEED_LIMIT ? DEFAULT_SPEED_LIMIT : speed;
		}

		private void UpdateInterval ()
		{
			releaseInterval -= deltaInterval;
			releaseInterval = releaseInterval < releaseIntervalLimit ? releaseIntervalLimit : releaseInterval;
		}

		public override void ApplyLevel (HandScreen screen, Hand hand, GameTime gameTime)
		{			
			hand.Movement.HorizontalDirection = Direction.NONE;
			hand.Movement.VerticalDirection = Direction.DOWN;
			
			if (hand.CurrentAnimation.IsAnimating == true)
				hand.Movement.VerticalSpeed = 0;
			else
				hand.Movement.VerticalSpeed = speed;
			
			screen.NewHandReleaseInterval = releaseInterval;
			
		}

		#endregion
	}
}
