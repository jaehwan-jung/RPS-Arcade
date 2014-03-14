#region Using Statements
using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Android.Util;

#endregion
namespace RPSArcadeAndroid
{
	public static class AzureDB
	{
		#region Fields

		private static SHA256 mySHA256 = SHA256Managed.Create ();
		private const string ApplicationURL = Config.AzureDBUrl;
		private const string ApplicationKey = Config.AzureDBKey;
		private static MobileServiceClient MobileService;
		private static IMobileServiceTable<UserScore> scoreTable;
		private static IMobileServiceTable<UserCredential> credentialTable;

		#endregion

		#region Nested Class

		private class UserCredential
		{
			public string Id { get; set; }

			public string UserName { get; set; }

			public string Password { get; set; }
		}

		#endregion

		#region Initialization

		static AzureDB ()
		{
			CurrentPlatform.Init ();
			MobileService = new MobileServiceClient (ApplicationURL, ApplicationKey);
			GetScoreTable ();
			GetUserAuthenticationTable ();
		}

		public static void GetScoreTable ()
		{
			scoreTable = MobileService.GetTable<UserScore> ();
		}

		public static void GetUserAuthenticationTable ()
		{
			credentialTable = MobileService.GetTable<UserCredential> ();
		}

		#endregion

		#region DB Read

		public static async Task<List<UserScore>> GetTopScoresAsync (int count)
		{
			List<UserScore> scores = await scoreTable
				.OrderByDescending (score => score.Score)  
				.Take (count)
				.ToListAsync ();
			return scores;
		}

		public static async Task<UserScore> GetUserScoreAsync ()
		{
			List<UserScore> scores = await scoreTable
				.Where (score => score.UserName == UserAccount.Id)
				.ToListAsync ();
			if (scores == null || scores.Count == 0)
				return null;
			else
				return scores [0];
		}

		public static async Task<bool> Authenticate (string userName, string password)
		{
			string userSHA = BitConverter.ToString (mySHA256.ComputeHash (Encoding.UTF8.GetBytes (userName + password)));
			List<UserCredential> users = await credentialTable
				.Where (user => user.UserName == userName)
				.Where (user => user.Password == userSHA)
				.ToListAsync ();
			if (users == null || users.Count == 0)
				return false;
			else
				return true;
		}

		public static async Task<bool> DoesUserExist (string userName)
		{
			string userSHA = BitConverter.ToString (mySHA256.ComputeHash (Encoding.UTF8.GetBytes (userName)));
			List<UserCredential> users = await credentialTable
				.Where (user => user.UserName == userName)
				.ToListAsync ();
			if (users == null || users.Count == 0)
				return false;
			else
				return true;
		}

		#endregion

		#region DB Update

		public static async Task UpdateScoreAsync (UserScore score)
		{
			await scoreTable.UpdateAsync (score);
		}

		#endregion

		#region DB Insert

		public static async Task InsertScoreAsync (UserScore score)
		{
			await scoreTable.InsertAsync (score);
		}

		public static async Task InsertUserAsync (string name, string pw)
		{
			var pwSHA = BitConverter.ToString (mySHA256.ComputeHash (Encoding.UTF8.GetBytes (name + pw)));
			await credentialTable.InsertAsync (new UserCredential (){ UserName = name, Password = pwSHA });
		}

		#endregion
	}
}

