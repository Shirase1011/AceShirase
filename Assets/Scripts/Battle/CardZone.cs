using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カードゾーン(領域)設定クラス
/// </summary>
public class CardZone : MonoBehaviour
{
	// ゾーン種類定義
	public enum ZoneType
	{
		// 手札
		Hand,
		// トラッシュ(ゴミ箱)
		Trash,
        Playzone,
        // カードを離す場所
	}

	// このゾーンの種類
	public ZoneType zoneType;

	// Start
	void Start ()
	{
	}
}