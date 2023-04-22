using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーまたは敵が使用するカードの（合成前の）各データ(ScriptableObject)
/// </summary>
[System.Serializable]
[CreateAssetMenu (fileName = "CardData", menuName = " ScriptableObjects/CardData", order = 1)]
public class CardDataSO : ScriptableObject
{
	[Header ("通し番号(カードごとに固有の値)")]
	public int serialNum;

	[Header ("カード名(日本語)")]
	public string cardName_JP;
	[Header ("カード名(英語)")]
	public string cardName_EN;

	[Header ("画像")]
	public Sprite CardIllust;
	[Header ("職業名")]
	public List<JobDefine> JobName;

	[Header ("効果リスト")]
	public List<CardEffectDefine> effectList;

	//[Header ("効果")]
	//[Multiline(5)] public string cardEffect;

	[Header ("コスト")]
	public int cost;

	[Header ("カードタイプ")]
	public List<CardType> CardType;

}