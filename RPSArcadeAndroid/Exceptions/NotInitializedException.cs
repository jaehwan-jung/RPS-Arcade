#region File Description
//-----------------------------------------------------------------------------//
// NotInitializedException.cs												   //
//																			   //
// Exception																   //
// 																			   //
// Jae-Hwan Jung															   //
// Copyright (C) Jae-Hwan Jung. All rights reserved.						   //
//-----------------------------------------------------------------------------//
#endregion

#region Using Statements
using System;

#endregion
namespace RPSArcadeAndroid
{
	public sealed class NotInitializedException : System.Exception
	{
		public NotInitializedException ()
		{
		}

		public NotInitializedException (string message)
        : base (message)
		{
		}

		public NotInitializedException (string message, Exception inner)
        : base (message, inner)
		{
		}
	}
}

