#region File Description
//-----------------------------------------------------------------------------//
// DifficultyController.cs												       //
//																			   //
// Class																	   //
// Represents the base class for controllers that affect difficulty of the game//
// Objects that affect the difficulty of the game are passed as parameters     //
// and modified to adjust the difficulty									   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;

#endregion
namespace RPSArcadeAndroid
{
	public abstract class DifficultyController
	{
		private Random random = new Random ();

		public int Level { get; set; }

		public virtual void IncrementLevel (GameTime gameTime)
		{
			Level++;
		}

		abstract public void ApplyLevel (HandScreen screen, Hand hand, GameTime gameTime);

		protected double GetRandomDouble (double startInclusive, double endInclusive)
		{
			var randomPercent = random.NextDouble ();
			return startInclusive + (endInclusive - startInclusive) * randomPercent;
		}
	}
}

