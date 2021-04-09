using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCGLobby
{
	[System.Serializable]
	[CreateAssetMenu]
	public class Cond_GameType : ConditionData
	{
		public string gameType;
		public int gameId;
	}

	[System.Serializable]
	[CreateAssetMenu]
	public class Cond_HallType : ConditionData
	{
		public int hallId_contain;
	}

	[System.Serializable]
	[CreateAssetMenu]
	public class Cond_WinType : ConditionData
	{
		public int winRate_greaterThan;
		public int winValue_range;
	}

	[System.Serializable]
	[CreateAssetMenu]
	public class Cond_Item : ConditionData
	{
		public string itemGameType;
		public int itemCardType_contain;
		public int itemHallType;
		public int itemStar;
	}

	[System.Serializable]
	[CreateAssetMenu]
	public class Cond_Chip : ConditionData
	{
		public int chipGameType;
	}

	[System.Serializable]
	[CreateAssetMenu]
	public class Cond_gem : ConditionData
	{
		public int gemValue;
	}

	[System.Serializable]
	[CreateAssetMenu]
	public class Cond_else : ConditionData
	{
		public object elseCondition1;
		public object elseCondition2;
	}
}
