using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のステータス定義クラス(ScriptableObject)
/// </summary>
[CreateAssetMenu (fileName = "EncountEnemyGroupsSO", menuName = " ScriptableObjects/EncountEnemyGroupsSO")]
public class EncountEnemyGroupsSO : ScriptableObject
{
    [Header("出てくる敵")]
    public List<EnemyStatusSO> EncountEnemies;

	[Header ("撃破ボーナス：選択肢の個数")]
	public int bonusOptions;
	[Header ("撃破ボーナス：獲得できる個数")]
	public int bonusPoint;
	[Header ("撃破ボーナス：選択肢に出現するプレイヤーカード")]
	public List<CardDataSO> bonusCardList;

}
