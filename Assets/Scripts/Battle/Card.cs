using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

/// <summary>
/// カード処理クラス
/// </summary>
public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	// オブジェクト・コンポーネント参照
	[SerializeField] private CardUI cardUI = null; // カードUI表示設定クラス
	private FieldManager fieldManager; // フィールド管理クラス
	public RectTransform rectTransform; // オブジェクトのRectTransform

	// カードデータ
	[HideInInspector] public CardDataSO baseCardData;	// 基となるScriptableObject側カードデータ
	[HideInInspector] public List<Sprite> iconSprites;	// カードアイコンリスト
	[HideInInspector] public List<CardEffectDefine> effects; // カード効果リスト
	[HideInInspector] public int costs; // コスト
	[HideInInspector] public List<JobDefine> Jobs; //職業リスト
	[HideInInspector] public int controllerCharaID; // カードの使用キャラクターID(後述の定数を使用)

	// 各種変数
	private Vector2 basePos; // 基本座標(ドラッグ終了後に戻ってくる座標)
	private Tween moveTween; // 移動Tween
	[HideInInspector] public CardZone.ZoneType nowZone; // このカードが置かれているカードゾーン
	public const int CharaNum = 2;		// 戦闘内のキャラクター人数
	public const int CharaID_Player = 0;// キャラクターID:主人公(プレイヤーキャラ)
	public const int CharaID_Enemy = 1;	// キャラクターID:敵
	public const int CharaID_None = -1;	// キャラクターID:(無し)

	//カードかざした関係
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float hoverTransitionDuration = 0.2f;
    private Vector3 originalScale;
    private bool isHovering;
	public CardDataSO cardDataSO{ get; private set; }


	/// <summary>
	/// 初期化処理
	/// </summary>
	/// <param name="_fieldManager">FieldManager参照</param>
	/// <param name="initPos">初期座標</param>
	public void Init (FieldManager _fieldManager, Vector2 initPos)
	{
		// 参照取得
		fieldManager = _fieldManager;
		// 配下コンポーネント初期化
		cardUI.Init (this);

		// 変数初期化
		rectTransform.position = initPos;
		basePos = initPos;
		nowZone = CardZone.ZoneType.Hand;
		iconSprites = new List<Sprite> ();
		effects = new List<CardEffectDefine> ();
		Jobs = new List<JobDefine> ();
	}
	

	/// <summary>
	/// (初期化時に呼出)
	/// カード定義データから各種パラメータを取得してセットする
	/// </summary>
	/// <param name="cardControllerCharaID">使用者キャラクターID(定数を使用)</param>

	public void SetInitialCardData (CardDataSO cardData, int cardControllerCharaID)
	{
		baseCardData = cardData;

		this.cardDataSO = cardData;
		// カード名
		cardUI.SetCardNameText (cardData.cardName_JP, cardData.cardName_EN);
		// カードアイコン
		cardUI.AddCardIconImage (cardData.CardIllust);
		//カード効果テキスト
		//cardUI.AddCardText (cardData.cardEffect);
		//職業名
		SetJobName(cardData.JobName[0]);
		//コスト
		cardUI.SetCostText (cardData.cost);
		// カード効果リスト
		foreach (var item in cardData.effectList)
			AddCardEffect (item);
		// カード使用者データ
		controllerCharaID = cardControllerCharaID;

	}
	#region パラメータ変更・追加処理
 
	/// <summary>
	/// カード効果を新規追加する
	/// </summary>
	/// <param name="newEffect">効果の種類・数値データ</param>
	private void AddCardEffect (CardEffectDefine newEffect)
	{
		// 効果データを新規作成する
		var effectData = new CardEffectDefine ();
		effectData.cardEffect = newEffect.cardEffect;
		effectData.value = newEffect.value;
		// 効果リストに追加
		effects.Add (effectData);
		//UI表示
		cardUI.AddCardEffectText (effectData);
	}

	public void SetJobName (JobDefine newJob)
	{
		var jobs = new JobDefine ();
		jobs.Jobname = newJob.Jobname;
		Jobs.Add (jobs);
		cardUI.AddJobName(jobs);
	}
	public void SetCostPoint (int value)
	{
		// パラメータをセット
		int costs = value;
	}
	#endregion


	// Update(毎フレーム1回ずつ実行)
	void Update()
    {
    }

	#region オブジェクト移動・表示演出
	/// <summary>
	/// 基本座標までカードを移動させる
	/// </summary>
	public void BackToBasePos ()
	{
		const float MoveTime = 0.4f; // カード移動アニメーション時間

		// 既に実行中の移動アニメーションがあれば停止
		if (moveTween != null)
			moveTween.Kill ();

		// 指定地点まで移動するアニメーション(DOTween)
		moveTween = rectTransform
			.DOMove (basePos, MoveTime) // 移動Tween
			.SetEase (Ease.OutQuart);   // 変化の仕方を指定
	}

	//元の位置を記録
    public Vector3 OriginalPosition { get; set; }
	/// <summary>
	/// カードを指定のゾーンに設置する
	/// </summary>
	/// <param name="zoneType">対象カードゾーン</param>
	/// <param name="targetPos">対象座標</param>
	public void PutToZone (CardZone.ZoneType zoneType, Vector2 targetPos)
	{
		// 座標を取得して移動
		basePos = targetPos;
		BackToBasePos ();
		// カードゾーンの種類を保存
		nowZone = zoneType;
	}
	#endregion



	#region タップイベント処理
	/// <summary>
	/// タップ開始時に実行
	/// IPointerDownHandlerが必要
	/// </summary>
	/// <param name="eventData">タップ情報</param>
	public void OnPointerDown (PointerEventData eventData)
	{
		// ドラッグ開始処理
		fieldManager.StartDragging (this);
	}
	/// <summary>
	/// タップ終了時に実行
	/// IPointerUpHandlerが必要
	/// </summary>
	/// <param name="eventData">タップ情報</param>
	public void OnPointerUp (PointerEventData eventData)
	{
		// ドラッグ終了処理
		fieldManager.EndDragging ();
	}
	public void DestroyCard(float fadeDuration = 1f)
	{
    // フェードアウトアニメーションを実行し、その後カードを削除
    	StartCoroutine(FadeOutAndDestroy(fadeDuration));
	}
	private IEnumerator FadeOutAndDestroy(float duration)
	{
    // フェードアウト開始
    	float elapsedTime = 0f;
    	float initialAlpha = GetComponent<SpriteRenderer>().color.a;

    	while (elapsedTime < duration)
    	{
    	    elapsedTime += Time.deltaTime;
    	    float alpha = Mathf.Lerp(initialAlpha, 0, elapsedTime / duration);
    	    Color color = GetComponent<SpriteRenderer>().color;
    	    color.a = alpha;
    	    GetComponent<SpriteRenderer>().color = color;
    	    yield return null;
    	}

    // フェードアウト完了後、カードを削除
    Destroy(gameObject);
	}

	//かざしたときに拡大する
 	private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering)
        {
            isHovering = true;
            StartCoroutine(ScaleCard(hoverScale));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHovering)
        {
            isHovering = false;
            StartCoroutine(ScaleCard(1.0f));
        }
    }

//PlayZoneにいる場合そのままデカい状態を維持する
    private IEnumerator ScaleCard(float targetScaleFactor)
    {
        float timeElapsed = 0.0f;
        Vector3 targetScale = originalScale * targetScaleFactor;
        Vector3 initialScale = transform.localScale;

        while (timeElapsed < hoverTransitionDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / hoverTransitionDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
#endregion
}