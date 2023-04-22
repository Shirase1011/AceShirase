using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// カードのUI表示設定クラス
/// </summary>
public class CardUI : MonoBehaviour
{
	// このカードの処理クラス
	private Card card;

	// UIプレハブ
	[SerializeField] private GameObject cardIconPrefab = null;		// カードアイコンPrefab
	[SerializeField] private GameObject cardEffectTextPrefab = null;// カード効果テキストPrefab
	// オブジェクト内UI参照
	[SerializeField] private TextMeshProUGUI cardNameText = null;				// カード名Text
	[SerializeField] private GameObject JobText = null;	//職業名
	[SerializeField] private Transform JobTextParent = null;	//職業名テキストの親Transform
	[SerializeField] private Transform cardIconParent = null;		// カードアイコンリストの親Transform
	[SerializeField] private Transform cardEffectTextParent = null; // カード効果テキストリストの親Transform
	[SerializeField] private TextMeshProUGUI costText = null; // カードコストテキスト
	[SerializeField] private GameObject hilightImageObject = null;	// 選択時強調表示画像Object
	// 各種スプライト素材

	//作成したジョブリスト
	private Dictionary<JobDefine, TextMeshProUGUI> cardJobTextDic;
	// 作成した効果Textリスト
	private Dictionary<CardEffectDefine, TextMeshProUGUI> cardEffectTextDic;

	// 初期化関数(Card.csから呼出)
	public void Init (Card _card)
	{
		card = _card;
		cardEffectTextDic = new Dictionary<CardEffectDefine, TextMeshProUGUI> ();
		cardJobTextDic = new Dictionary<JobDefine, TextMeshProUGUI>();
	}

	/// <summary>
	/// カード名表示
	/// </summary>
	/// <param name="name_JP">カード名(日本語)</param>
	/// <param name="name_EN">カード名(英語)</param>
	public void SetCardNameText (string name_JP, string name_EN)
	{
		if (Data.nowLanguage == SystemLanguage.Japanese)
			cardNameText.text = name_JP;
		else if (Data.nowLanguage == SystemLanguage.English)
			cardNameText.text = name_EN;
	}

	/// <summary>
	/// カードアイコンUIを追加
	/// </summary>
	public void AddCardIconImage (Sprite sprite)
	{
		// オブジェクト作成
		var obj = Instantiate (cardIconPrefab, cardIconParent);
		// スプライトを設定
		obj.GetComponent<Image> ().sprite = sprite;
	}
	public void AddJobName (JobDefine jobs)
	{
		var obj = Instantiate (JobText, JobTextParent);
		cardJobTextDic.Add(jobs, obj.GetComponent<TextMeshProUGUI> ());
		cardJobTextDic[jobs].text = string.Format(JobDefine.Dic_JobName_JP[jobs.Jobname]);
	}
	public void AddCardEffectText (CardEffectDefine effectData)
	{
		// オブジェクト作成
		var obj = Instantiate (cardEffectTextPrefab, cardEffectTextParent);
		// TextUIとカード効果を紐づける
		cardEffectTextDic.Add (effectData, obj.GetComponent<TextMeshProUGUI> ());
		// Textの内容を更新
		ApplyCardEffectText (effectData);
	}
	/// <summary>
	/// カード効果Textの表示内容を更新
	/// </summary>
	public void ApplyCardEffectText (CardEffectDefine effectData)
	{
		// 対象のTextUIを取得
		var targetText = cardEffectTextDic[effectData];
		// 効果量を取得
		int effectValue = effectData.value;
		string effectValueMes = "";
 
		// 効果量を文字列化
		effectValueMes = effectValue.ToString ();
		if (Data.nowLanguage == SystemLanguage.Japanese)
			targetText.text = string.Format (CardEffectDefine.Dic_EffectName_JP[effectData.cardEffect], effectValueMes);
		else if (Data.nowLanguage == SystemLanguage.English)
			targetText.text = string.Format (CardEffectDefine.Dic_EffectName_EN[effectData.cardEffect], effectValueMes);
	}

	public void SetCostText (int cost)
	{
		costText.text = cost.ToString ();
	}

	/// <summary>
	/// カード強調表示画像を表示・非表示にする
	/// </summary>
	public void SetHilightImage (bool mode)
	{
		hilightImageObject.SetActive (mode);
	}

	/// <summary>
	/// アイコンと効果の表示をリセットする
	/// </summary>
	public void ClearIconsAndEffects ()
	{
		// アイコン初期化
		int length = cardIconParent.childCount;
		for (int i = 0; i < length; i++)
		{
			Destroy (cardIconParent.GetChild (i).gameObject);
		}
		// 効果初期化
		length = cardEffectTextParent.childCount;
		for (int i = 0; i < length; i++)
		{
			Destroy (cardEffectTextParent.GetChild (i).gameObject);
		}
	}
}