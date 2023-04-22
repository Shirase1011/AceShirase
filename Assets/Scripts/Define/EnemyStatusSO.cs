using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のステータス定義クラス(ScriptableObject)
/// </summary>
[CreateAssetMenu (fileName = "EnemyStatusSO", menuName = " ScriptableObjects/EnemyStatusSO", order = 2)]
public class EnemyStatusSO : ScriptableObject
{
	[Header ("通し番号(敵ごとに固有の値)")]
	public int serialNum;
	[Header ("名前(日本語)")]
	public string enemyName_JP;
	[Header ("名前(英語)")]
	public string enemyName_EN;

	[Header ("敵画像")]
	public Sprite charaSprite;

	[Header ("最大HP(初期HP)")]
	public int maxHP;

	[Header ("各ターンに使用するカード")]
	public List<CardDataSO> useEnemyCardDatas;

}