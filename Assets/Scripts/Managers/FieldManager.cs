using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// フィールド管理クラス
/// </summary>
public class FieldManager : MonoBehaviour
{
	// オブジェクト・コンポーネント参照
	private BattleManager battleManager; // 戦闘画面マネージャ
	public RectTransform canvasRectTransform; // CanvasのRectTransform
	public Camera mainCamera; // メインカメラ
	[SerializeField] private DammyHandUI dammyHandUI = null; // ダミー手札制御クラス
	[SerializeField] private TextMeshProUGUI deckNumText = null; // 残りデッキ枚数表示Text
	[SerializeField] private TextMeshProUGUI DisCardNumText = null; //捨て札のカード枚数表示text
	//[SerializeField] private Button TurnEndButton = null;  // ターンエンドボタン

	// カード関連参照
	[SerializeField] private GameObject cardPrefab = null; // カードプレハブ
	[SerializeField] private Transform cardsParent = null; // 生成するカードオブジェクトの親Transform
	[SerializeField] private Transform deckIconTrs = null; // デッキオブジェクトTransform

	// コスト関連
	[SerializeField] private Transform costParent = null; //コストボタン用
	[SerializeField] private TextMeshProUGUI CostText = null;
	// プレイヤー現在デッキ(山札)データ
	public List<CardDataSO> playerDeckData;
	// プレイヤー現在デッキデータのバックアップ(戦闘中に山札を追加する時必要)
	public List<CardDataSO> playerDeckData_BackUp;
	//プレイヤー現在手札データ
	private List<CardDataSO> HandsCard;
	//プレイヤー現在捨て札データ
	public List<CardDataSO> DiscardDeckData;
	public int PlayerCost; //所持コスト

	// 各種変数・参照
	private Card draggingCard;			// ドラッグ操作中カード
	private List<Card> cardInstances;	// 生成したプレイヤー操作カードリスト
	private bool reserveHandAlign;		// 手札整列フラグ
	private bool isDrawing;				// true:手札補充中である
	private bool isDiscarding = false; //true:手札捨ててる最中
	private bool cardDroppedInPlayZone = false; //PlayZoneにカードがあるかをとりあえず偽にする

	// 表示テスト用：カードデータ
	[SerializeField] private CardDataSO testCardData;

	// 初期化処理
	public void Init (BattleManager _battleManager)
	{
		// 参照取得
		battleManager = _battleManager;

		// 変数初期化
		cardInstances = new List<Card> ();


		// デバッグ用ドロー処理(遅延実行)
		DOVirtual.DelayedCall (
			1.0f, // 1.0秒遅延
			() =>
			{
				OnBattleStarting ();
				OnTurnStarting ();
			}
		);
	}

	// Update
	void Update()
	{
		// ドラッグ操作中の処理
		if (draggingCard != null)
		{
			// 更新処理
			UpdateDragging ();
		}
	}

	// OnGUI(Updateのように繰り返し実行・UI制御用)
	void OnGUI ()
	{
		// 手札整列フラグが立っているなら整列
		if (reserveHandAlign)
		{
			AlignHandCards ();
			reserveHandAlign = false;
		}
	}

	#region ゲーム進行処理
	/// <summary>
	/// 敵との戦闘開始時処理
	/// </summary>
	public void OnBattleStarting ()
	{
		HandsCard = new List<CardDataSO> ();
		DiscardDeckData = new List<CardDataSO> (); //捨て札データ初期化
		// デッキデータ取得
		playerDeckData = new List<CardDataSO> ();
		playerDeckData_BackUp = new List<CardDataSO> ();
		foreach (var cardData in PlayerDeckData.deckCardList)
		{
			playerDeckData.Add (PlayerDeckData.CardDatasBySerialNum[cardData]);
			playerDeckData_BackUp.Add (PlayerDeckData.CardDatasBySerialNum[cardData]);
		}
		// デッキ残り枚数表示
		PrintPlayerDeckNum ();
		PrintPlayerDiscardNum (); //捨て札枚数表示
	}
	/// <summary>
	/// ターン開始時処理
	/// </summary>
	public void OnTurnStarting ()
	{
		// カードのドロー枚数決定
		int nextHandCardsNum = 5; // 手札枚数
		// ドロー処理
		DrawCardsUntilNum (nextHandCardsNum);
		reserveHandAlign = true;
		PlayerCost = 3;
		CostText.text = PlayerCost.ToString();
		// ターンエンドボタンを有効化
		//TurnEndButton.interactable = true;
	}


	//ターンエンド処理
	//public void TurnendButton ()
	//{
	//	// カードドラッグ中なら処理しない
	//	if (draggingCard != null)
	//		return;
 //
	//	// 実行ボタンを一時的に無効化
	//	TurnEndButton.interactable = false;
	//	battleManager.PlayCardManager.PlayCard(EnemyStatusSO.useEnemyCardDatas,1);
	//}
	#endregion
	#region ドロー処理
	/// <summary>
	/// デッキからカードを１枚引き手札に加える
	/// </summary>
	/// <param name="handID">対象手札番号</param>

	public void DrawCards(int drawCount) //枚数指定でカードを引く
	{
		// 現在の手札枚数を取得
		int nowHandNum = 0;
		foreach (var card in cardInstances)
		{
			if (card.nowZone == CardZone.ZoneType.Hand)
				nowHandNum++;
		}
		const float DrawIntervalTime = 0.1f; // ドロー間の時間間隔
		var drawSequence = DOTween.Sequence ();
		isDrawing = true;
		for (int i = 0; i < drawCount; i++)
		{
			// １枚引く処理
			drawSequence.AppendCallback (() =>
			{
				DrawCard (nowHandNum);
			});
			// 時間間隔を設定
			drawSequence.AppendInterval (DrawIntervalTime);
		}
		drawSequence.OnComplete (() => isDrawing = false);
		CheckHandCardsNum();
		AlignHandCards ();
	}
	public void DrawCard (int handID)
	{	
		//デッキ枚数が0枚か確認する
		if(playerDeckData.Count == 0)
			DeckShuffle();
		// オブジェクト作成
		var obj = Instantiate (cardPrefab, cardsParent);
		// カード処理クラスを取得・リストに格納
		Card objCard = obj.GetComponent<Card> ();
		cardInstances.Add (objCard);

		// デッキ内から引かれるカードをランダムに決定
		CardDataSO targetCard = playerDeckData[Random.Range (0, playerDeckData.Count)];
		// 現在デッキリストから該当カードを削除
		playerDeckData.Remove (targetCard);
		HandsCard.Add(targetCard);

		// カード初期設定
		int idInList = cardInstances.FindInstanceID (objCard);
		objCard.Init (this, deckIconTrs.position);
		objCard.PutToZone (CardZone.ZoneType.Hand, dammyHandUI.GetHandPos (handID));
		objCard.SetInitialCardData (targetCard, Card.CharaID_Player);

		// デッキ残り枚数表示
		PrintPlayerDeckNum ();
		PrintPlayerDiscardNum (); //捨て札枚数表示
		CheckHandCardsNum();
		AlignHandCards ();
	}
	
	/// <summary>
	/// 手札が指定枚数になるまでカードを引く
	/// </summary>
	/// <param name="num">指定枚数</param>
	private void DrawCardsUntilNum (int num)
	{
		// 現在の手札枚数を取得
		int nowHandNum = 0;
		foreach (var card in cardInstances)
		{
			if (card.nowZone == CardZone.ZoneType.Hand)
				nowHandNum++;
		}
		// 新たに引くべき枚数を取得
		int drawNum = num - nowHandNum;
		if (drawNum <= 0)
			return;

		// 手札UIに枚数を指定
		dammyHandUI.SetHandNum (nowHandNum + drawNum);

		// 連続でカードを引く(Sequence)
		const float DrawIntervalTime = 0.1f; // ドロー間の時間間隔
		var drawSequence = DOTween.Sequence ();
		isDrawing = true;
		for (int i = 0; i < drawNum; i++)
		{
			// １枚引く処理
			drawSequence.AppendCallback (() =>
			{
				DrawCard (nowHandNum);
				nowHandNum++;
			});
			// 時間間隔を設定
			drawSequence.AppendInterval (DrawIntervalTime);
		}
		drawSequence.OnComplete (() => isDrawing = false);
	}
	//private IEnumerator WaitForCardDropInPlayZone()
	//{
   //// カードがPlayZoneにドロップされるまで待機
   //	while (!cardDroppedInPlayZone)
   //	{
   //    	yield return null; // 次のフレームまで待機
   //	}
	//}

	////カード捨てる系
	//public IEnumerator WaitForDiscardEvent()
	//{
   //// カードがPlayZoneにドロップされるまで待機
   //	yield return StartCoroutine(WaitForCardDropInPlayZone());

   //// カードがPlayZoneにドロップされた後の処理
   //	draggingCard.DestroyCard(0.5f);
	//}

	//public IEnumerator DiscardNum(int numCardsToDiscard)
	//{
    //	isDiscarding = true;
	//	int discardedCards = 0;
//
    //// 指定された枚数のカードが捨てられるまで繰り返す
    //	while (discardedCards < numCardsToDiscard)
    //	{
    //    	// カードがPlayZoneにドロップされるまで待機
    //    	yield return StartCoroutine(WaitForCardDropInPlayZone());
//
    //    	// カードがPlayZoneにドロップされた後の処理
    //    	draggingCard.DestroyCard(0.5f);
//
    //    	// 捨てられたカードの枚数をカウント
    //    	discardedCards++;
//
    //    	// カードがPlayZoneにドロップされたフラグをリセット
    //    	cardDroppedInPlayZone = false;
    //	}
	//	isDiscarding = false;
	//}
	#endregion
	#region 手札整理
	/// <summary>
	/// 手札のカードを整列させる
	/// </summary>
	private void AlignHandCards ()
	{
		// 手札整列処理
		int index = 0; // 手札内番号
		// ダミー手札を整列
		dammyHandUI.ApplyLayout ();
		// 各カードをダミー手札に合わせて移動
		foreach (var card in cardInstances)
		{
			if (card.nowZone == CardZone.ZoneType.Hand)
			{
				card.PutToZone (CardZone.ZoneType.Hand, dammyHandUI.GetHandPos (index));
				index++;
			}
		}
	}

	/// <summary>
	/// 現在の手札の枚数を手札UI処理クラスに反映させて整列する
	/// </summary>
	private void CheckHandCardsNum ()
	{
		// 現在の手札枚数を取得
		int nowHandNum = 0;
		foreach (var item in cardInstances)
		{
			if (item.nowZone == CardZone.ZoneType.Hand)
				nowHandNum++;
		}
		// ダミー手札に枚数を指定
		dammyHandUI.SetHandNum (nowHandNum);
		// 手札枚数に合わせて手札を整列
		// (手札枚数を変更した同フレームではダミー手札オブジェクトが動いていないため一瞬だけ遅延実行)
		reserveHandAlign = true;
	}
	#endregion
	#region UI
	////カードを捨て札に行く処理
	////プレイヤーの捨て札のカード枚数の表示を更新する
	private void PrintPlayerDiscardNum()
	{	//捨て札が0枚の時は0を表示しない
		if(DiscardDeckData.Count > 0)
			DisCardNumText.text = DiscardDeckData.Count.ToString ();
	}
	/// <summary>
	/// プレイヤー側デッキ残り枚数の表示を更新する
	/// </summary>
	private void PrintPlayerDeckNum ()
	{
		// デッキ残り枚数
		int deckCardNum = playerDeckData.Count;
		// 枚数をText表示
		deckNumText.text = playerDeckData.Count.ToString ();
	}

	//デッキシャッフル
	public void DeckShuffle()
	{
		playerDeckData.AddRange(DiscardDeckData);
		DiscardDeckData.Clear();
	}
	#endregion

	#region カードドラッグ処理
	/// <summary>
	/// カードのドラッグ操作を開始する
	/// </summary>
	/// <param name="dragCard">操作対象カード</param>
	public void StartDragging (Card dragCard)
	{
		// 手札補充演出中なら終了
		if (isDrawing)
			return;

		// 操作対象カードを記憶
		draggingCard = dragCard;
		// 他のカードオブジェクトより兄弟間で一番後ろにする(最前面表示にする)
		draggingCard.transform.SetAsLastSibling ();
		draggingCard.OriginalPosition = draggingCard.transform.position;
	}
	/// <summary>
	/// ドラッグ操作更新処理
	/// </summary>
	private void UpdateDragging ()
	{
		// タップ位置を取得
		Vector2 tapPos = Input.mousePosition;
		// タップ座標を変換する(スクリーン座標→Canvasのローカル座標)
		RectTransformUtility.ScreenPointToLocalPointInRectangle (
			canvasRectTransform,// CanvasのRectTransform
			tapPos,             // 変換元座標データ
			mainCamera,         // メインカメラ
			out tapPos);        // 変換先座標データ

		// 座標を適用
		draggingCard.rectTransform.anchoredPosition = tapPos;
		if (draggingCard.nowZone == CardZone.ZoneType.Playzone)
    	{
        	cardDroppedInPlayZone = true;
    	}
	}

	/// <summary>
	/// カードのドラッグ操作を終了する
	/// </summary>
	public void EndDragging ()
	{
		// 重なっているオブジェクトの情報を全て取得する
		// (判定が必要なオブジェクトには全てBoxCollider2Dが付与されているのでそれを利用して判定)
		// このオブジェクトのスクリーン座標を取得する
		if(isDiscarding)
		{
			return;
		}
		if(draggingCard == null)
		{
			return;
		}
		Vector3 pos = RectTransformUtility.WorldToScreenPoint (mainCamera, draggingCard.transform.position);
		// メインカメラから上記で取得した座標に向けてRayを飛ばす
		Ray ray = mainCamera.ScreenPointToRay (pos);

		// ドラッグ先のオブジェクト取得処理
		CardZone targetZone = null; // ドラッグ先カードゾーン
		Card targetCard = null; // ドラッグ先カード
		// Rayが当たった全オブジェクトに対しての処理
		foreach (RaycastHit2D hit in Physics2D.RaycastAll (ray.origin, ray.direction, 10.0f))
		{
			// 当たったオブジェクトが存在しないなら終了
			if (!hit.collider)
				break;

			// 当たったオブジェクトがドラッグ中のカードと同一なら次へ
			var hitObj = hit.collider.gameObject;
			if (hitObj == draggingCard.gameObject)
				continue;

			// オブジェクトがカードエリアなら取得して次へ
			var hitArea = hitObj.GetComponent<CardZone> ();
			if (hitArea != null)
			{
				targetZone = hitArea;
				continue;
			}

			// オブジェクトがカードなら取得して次へ
			var hitCard = hitObj.GetComponent<Card> ();
			if (hitCard != null)
			{
				targetCard = hitCard;
				continue;
			}
		}

		// 重なった対象ごとによる処理
		if (targetCard != null &&
			(targetCard.nowZone >= CardZone.ZoneType.Playzone))
		{// プレイボードにあるカードと重なった場合
			// 合成処理(未実装)
		}
		else if (targetZone != null)
		{// カードと重ならずカードエリアと重なった場合

			int originalIndex = cardInstances.IndexOf(draggingCard); // カードの元のインデックスを取得

		    if (targetZone.zoneType == CardZone.ZoneType.Playzone)
        	{
            // PlayZoneにドロップした場合、カードを削除
			//削除のち効果発動
				int draggingCardCost =draggingCard.cardDataSO.cost;
				Debug.Log(draggingCardCost);
				Debug.Log(PlayerCost);
				//現在のプレイヤーコストがカードのコスト以上の時、カードを使用して消滅させる。
				if(draggingCardCost <= PlayerCost)
				{	
					draggingCard.PutToZone (targetZone.zoneType, targetZone.GetComponent<RectTransform> ().position);
					StartCoroutine(PlayCardAfterDestroy(draggingCard, 0.5f));
					PlayerCost = PlayerCost - draggingCardCost;
					CostText.text = PlayerCost.ToString();
	

				}
				//そうでない場合元の手札の場所に戻る。
				else
				{
					draggingCard.transform.position = draggingCard.OriginalPosition;	
				}		
        	}
			// 設置処理

			CheckHandCardsNum ();
			// 手札以外→手札への移動の場合、カードを元の位置に戻す
			if (draggingCard.nowZone != CardZone.ZoneType.Hand)
			{
				cardInstances.Remove (draggingCard);
				cardInstances.Insert (originalIndex, draggingCard);
			}
		}
		else
		{// いずれとも重ならなかった場合
			// 元の位置に戻す
			draggingCard.transform.position = draggingCard.OriginalPosition;
		}

		// 後処理
		draggingCard = null;
	}
	IEnumerator PlayCardAfterDestroy(Card draggingCard, float delay)
	{
    // カードを破壊
    draggingCard.DestroyCard(delay);

    // 指定した時間待機
    yield return new WaitForSeconds(delay);

    // カード破棄後に効果を発動
    battleManager.PlayCardManager.PlayCard(draggingCard, 0);
	// Cardオブジェクトに関連するCardDataSOを取得
	CardDataSO relatedCardData = draggingCard.baseCardData;
    // リスト操作を行う（例: 手札リストからカードを削除）
    HandsCard.Remove(relatedCardData);
	DiscardDeckData.Add(relatedCardData);
	PrintPlayerDiscardNum();

	}
	#endregion
}