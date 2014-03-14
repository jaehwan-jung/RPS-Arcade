#region File Description
//-----------------------------------------------------------------------------//
// GameControl.cs															   //
//																			   //
// Statc Class																   //
// Represents the control for all stages of the game (start, stop, etc)		   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace RPSArcadeAndroid
{
	public static class GameControl
	{
		#region Fields

		private static bool isInitialized;

		#endregion

		#region Properties

		public static GamePlayState GamePlayState { get; set; }

		public static bool IsPlaying { get { return GamePlayState == GamePlayState.Playing; } }

		public static bool IsRestarting { get { return GamePlayState == GamePlayState.Restarting; } }

		public static bool IsTerminated { get { return GamePlayState == GamePlayState.Ended; } }

		public static bool IsPaused { get { return GamePlayState == GamePlayState.Paused; } }

		public static bool IsEnding { get { return GamePlayState == GamePlayState.Ending; } }

		#endregion

		#region Initialization

		static GameControl ()
		{
			GamePlayState = GamePlayState.NotStarted;
		}

		#endregion

		#region Methods

		public static async Task Start ()
		{
			Initialize ();			
						
			await Restart ();
		}

		private static void Initialize ()
		{
			isInitialized = true;
		}

		public static async Task Restart ()
		{			
			Stop ();
			GamePlayState = GamePlayState.Restarting;
			await Task.Factory.StartNew (() => {
				while (GamePlayState == GamePlayState.Restarting)
					Thread.Sleep (100);
			}
			);
			GamePlayState = GamePlayState.Playing;					
		}

		public static void InformResetCompletion ()
		{
			GamePlayState = GamePlayState.NotStarted;
		}

		public static void Resume ()
		{
			ThrowExceptionIfNotReady ();
			
			if (GamePlayState == GamePlayState.Paused)
				GamePlayState = GamePlayState.Playing;
		}

		public static void Pause ()
		{
			ThrowExceptionIfNotReady ();
			
			if (GamePlayState == GamePlayState.Playing)
				GamePlayState = GamePlayState.Paused;
		}

		public static void Stop ()
		{
			GamePlayState = GamePlayState.Ended;
		}

		private static void ThrowExceptionIfNotReady ()
		{
			if (!isInitialized)
				throw new NotInitializedException ("Initialize GameControl before using it.");
			
			if (GamePlayState == GamePlayState.Restarting)
				throw new Exception ("Game is restarting. Inform GameControl once resetting is done.");
		}

		#endregion
	}
}

