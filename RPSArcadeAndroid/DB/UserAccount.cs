#region Using Statements
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

#endregion
namespace RPSArcadeAndroid
{
	public static class UserAccount
	{
		#region Properties

		public static string Id { get; set; }

		public static int Score { get; set; }

		#endregion

		#region Static Methods

		public static async Task PublishScore (int score)
		{
			UserScore userscore = await AzureDB.GetUserScoreAsync ();
			if (userscore == null)
				await AzureDB.InsertScoreAsync (new UserScore () { UserName = UserAccount.Id, Score = score });
			else if (userscore.Score < score) {
				var entry = new UserScore () { Id = userscore.Id, UserName = UserAccount.Id, Score = score };
				await AzureDB.UpdateScoreAsync (entry);
			} else
				UserAccount.Score = userscore.Score;
		}

		public static async Task<List<UserScore>> GetGlobalLeaderBoard (int count)
		{
			return await AzureDB.GetTopScoresAsync (count);
		}

		#endregion
	}
}

