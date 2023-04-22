using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤ側デッキ・所持カード管理クラス
/// </summary>
public class PlayerDeckData : MonoBehaviour
{
	// 各種設定データ(Inspectorからセット)
	public List<CardDataSO> allPlayerCardsList; // プレイヤー側カード全部のリスト
	public List<CardDataSO> playerInitialDeck; // プレイヤー側初期デッキカードリスト

	// プレイヤー側全カードデータと通し番号を紐づけたDictionary
	public static Dictionary<int, CardDataSO> CardDatasBySerialNum;

	// 現在のプレイヤーデッキデータ(通し番号で管理)
	public static List<int> deckCardList;

	// 初期化処理
	public void Init ()
	{
		// プレイヤー側全カードデータと通し番号を紐づける
		CardDatasBySerialNum = new Dictionary<int, CardDataSO> ();
		foreach (var item in allPlayerCardsList)
		{
			CardDatasBySerialNum.Add (item.serialNum, item);
		}
	}

	/// <summary>
	/// ゲーム初回起動時の初期デッキを設定する
	/// </summary>
	public void DataInitialize ()
	{
		// プレイヤーの現在デッキデータに初期デッキ設定を反映
		deckCardList = new List<int> ();
		foreach (var cardData in playerInitialDeck)
		{
			AddCardToDeck (cardData.serialNum);
		}
	}

	/// <summary>
	/// デッキにカードを１枚追加する
	/// </summary>
	/// <param name="cardSerialNum">カードの通し番号</param>
	public static void AddCardToDeck (int cardSerialNum)
	{
		deckCardList.Add (cardSerialNum);
		deckCardList.Sort ();
	}
}