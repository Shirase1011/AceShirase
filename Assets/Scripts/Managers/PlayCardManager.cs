using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// プレイボード上のカードの効果発動処理クラス
/// </summary>
public class PlayCardManager : MonoBehaviour
{
	// オブジェクト・コンポーネント
	[HideInInspector] public BattleManager battleManager; // 戦闘画面マネージャクラス
	private FieldManager fieldManager;
	private CharacterManager characterManager;

	// カード効果実行Sequence
	private Sequence playSequence;



	// 初期化関数(BattleManager.csから呼出)
	public void Init (BattleManager _battleManager)
	{
		// 参照取得
		battleManager = _battleManager;
		fieldManager = battleManager.fieldManager;
		characterManager = battleManager.characterManager;
	}

	/// <summary>
	/// カードの全ての効果を発動する
	/// </summary>
	/// <param name="targetCard">対象カード</param>
	/// <param name="useCharaID">このカードの使用者のキャラクターID</param>
	/// <param name="boardIndex">プレイボード上のこのカードの順番(0-4)</param>
	/// <returns>効果発動成功フラグ(true:発動成功)</returns>
	public bool PlayCard (Card targetCard, int useCharaID)
	{
		// 相手キャラクターのID
		int targetCharaID = useCharaID ^ 1;

		// カードの各効果量
		int damagePoint = 0; //与ダメ
		int sealdPoint = 0; //シールド
		int drawPoint = 0; //ドロー数
		int discardPoint = 0; //捨てる枚数


		// カード内のそれぞれの効果を実行
		// ①：他の効果より優先して実行される効果。ここを参考に発動制限のカードを作る。
		//foreach (var effect in targetCard.effects)
		//{
		//	switch (effect.cardEffect)
		//	{
		//		case CardEffectDefine.CardEffect.ForceEqual: // 強度n限定
		//			if (effect.value != targetCard.forcePoint)
		//				return false; // カード強度が指定の数値と異なるなら全ての効果を無効
		//			break;
		//	}
		//}
		// ②：通常の効果
		#region 
		foreach (var effect in targetCard.effects)
		{
			switch (effect.cardEffect)
			{
				case CardEffectDefine.CardEffect.Damage: // ダメージ
					damagePoint += effect.value;
					break;
				
				case CardEffectDefine.CardEffect.Seald: //シールド
					sealdPoint += effect.value;
					break;

				case CardEffectDefine.CardEffect.Draw: //ドロー
					drawPoint += effect.value;
					break;

				case CardEffectDefine.CardEffect.Discard: //手札捨てる
					discardPoint += effect.value;
					break;
			}
		}
		#endregion

		// 各種計算数値を対象ごとに適用
		// ダメージ
		characterManager.ChangeStatus_NowHP (targetCharaID, -damagePoint);
		//シールド
		characterManager.ChangeStatus_NowSield (useCharaID, +sealdPoint);
		//ドロー
		if(fieldManager.playerDeckData.Count == 0)
			fieldManager.DeckShuffle();
		fieldManager.DrawCards(drawPoint);
		//手札捨てる
		//fieldManager.StartCoroutine(fieldManager.DiscardNum(discardPoint));

		return true;
	}

	#region その他Set・Get系
	/// <summary>
	/// カードの効果実行中ならtrueを返す
	/// </summary>
	public bool IsPlayingCards ()
	{
		if (playSequence != null)
			return playSequence.IsPlaying ();
		return false;
	}
	#endregion
}