using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// プレイヤーおよび敵キャラクターの管理クラス
/// </summary>
public class CharacterManager : MonoBehaviour
{
	// オブジェクト・コンポーネント
	[HideInInspector] public BattleManager battleManager; // 戦闘画面マネージャクラス

	// 敵画像オブジェクト関連
	[SerializeField] private Transform enemyPictureParent = null;	// 生成した敵オブジェクトの親Transform
	[SerializeField] private GameObject enemyPicturePrefab = null;	// 敵キャラクター画像Prefab
	[SerializeField] private GameObject enemyStatusBarPrefab = null;
	[SerializeField] private Transform enemyStatusBarsParent = null;
	private EnemyPicture enemyPicture; // 出現中の敵オブジェクト処理クラス

	// ステータスUI処理クラス
	[SerializeField] private StatusUI playerStatusUI = null;// プレイヤーステータス
	[SerializeField] private StatusUI enemyStatusUI = null; // 敵ステータス

	// エネミーデータ
	[HideInInspector] public EnemyStatusSO enemyData; // 敵定義データ(戦闘中ここは変更しない)
	public EncountEnemyGroupsSO enemyGroups; //エンカウントする敵グループ
    private List<EnemyStatusSO> encountEnemies; //グループの中身

	// キャラクターID別ステータスデータ
	// 現在HPデータ
	public int[] nowHP;
	// 最大HPデータ
	public int[] maxHP;

	public int [] nowSield; //現在のシールド量
	
	// 各種定数
	private const float ShakeAnimPower = 18.0f;// 振動演出強度
	public const float ShakeAnimTime = 0.4f;// 振動演出時間

	// 初期化関数(BattleManager.csから呼出)
	public void Init (BattleManager _gameManager)
	{
		// 参照取得
		battleManager = _gameManager;

		// 変数初期化
		nowHP = new int[Card.CharaNum];
		maxHP = new int[Card.CharaNum];
		nowSield = new int[Card.CharaNum];
		ResetHP_Player ();
        encountEnemies = enemyGroups.EncountEnemies;

		// UI初期化
		playerStatusUI.SetHPView (nowHP[Card.CharaID_Player], maxHP[Card.CharaID_Player]);
		enemyStatusUI.HideCanvasGroup (false); // 敵ステータスを非表示(アニメーションなし)
	}

	/// <summary>
	/// ターン終了時に実行される処理
	/// </summary>
	public void OnTurnEnd ()
	{

	}

	/// <summary>
	/// プレイヤーのHPを初期化する
	/// </summary>
	public void ResetHP_Player ()
	{
		// HP初期化
		maxHP[Card.CharaID_Player] = 30; // （暫定的な初期HP）
		nowHP[Card.CharaID_Player] = maxHP[Card.CharaID_Player];
		playerStatusUI.SetHPView (nowHP[Card.CharaID_Player], maxHP[Card.CharaID_Player]);
	}

	/// <summary>
	/// 指定キャラクターの現在HPを変更する
	/// </summary>
	/// <param name="value">変化量(+で回復)(-でダメージ)</param>

	public void ChangeStatus_NowSield (int charaID, int value)
	{
		nowSield[charaID] += value;
		playerStatusUI.SetSieldView(nowSield[charaID]);
	}
	public void ChangeStatus_NowHP (int charaID, int value)
	{
		// 既に現在HP0なら処理しない
		if (nowHP[charaID] <= 0)
			return;

		// シールド込みの現在HP変更
		if(nowSield[charaID] > 0) //現在のシールドが>0の時
		{
			nowSield[charaID] += value; //シールドの値を変化させ 
			if(nowSield[charaID] < 0) //シールドの値が<0になった場合
			{
				nowHP[charaID] += nowSield[charaID]; //その値をHPに反映する
				nowSield[charaID] = 0;
			}
			playerStatusUI.SetSieldView(nowSield[charaID]);
			
		}
		else
			nowHP[charaID] += value;

		// 最大HPを越えないようにする処理
		if (nowHP[charaID] > maxHP[charaID])
			nowHP[charaID] = maxHP[charaID];

		// UI反映
		if (charaID == Card.CharaID_Player)
			playerStatusUI.SetHPView (nowHP[charaID], maxHP[charaID]);
		else
			enemyStatusUI.SetHPView (nowHP[charaID], maxHP[charaID]);
		// (敵専用)ダメージ演出
		if (charaID == Card.CharaID_Enemy)
		{
			// 撃破演出
			if (IsEnemyDefeated ())
				enemyPicture.DefeatAnimation ();
			// 被ダメージ演出
			else if (value < 0)
				enemyPicture.DamageAnimation ();
		}
	}

    
	/// <summary>
	/// 指定キャラクターの最大HPを変更する
	/// </summary>
	/// <param name="value">変化量</param>
	public void ChangeStatus_MaxHP (int charaID, int value)
	{
		// 既に最大HP0なら処理しない
		if (maxHP[charaID] <= 0)
			return;

		// 最大HP変更
		maxHP[charaID] += value;
		// 現在HPの上限・下限を反映
		nowHP[charaID] = Mathf.Clamp (nowHP[charaID], 0, maxHP[charaID]);

		// UI反映
		if (charaID == Card.CharaID_Player)
			playerStatusUI.SetHPView (nowHP[charaID], maxHP[charaID]);
		else
			enemyStatusUI.SetHPView (nowHP[charaID], maxHP[charaID]);
		// (敵専用)ダメージ演出
		if (charaID == Card.CharaID_Enemy)
		{
			// 撃破演出
			if (IsEnemyDefeated ())
				enemyPicture.DefeatAnimation ();
			// 被ダメージ演出
			else if (value < 0)
				enemyPicture.DamageAnimation ();
		}
	}

	#region 敵への処理
	/// <summary>
	/// 敵を出現・表示する
	/// </summary>
	/// <param name="spawnEnemyData">出現敵データ</param>
	public void SpawnEnemy (EnemyStatusSO spawnEnemyData)
	{
		// 敵データ取得
		enemyData = spawnEnemyData;

		// 敵ステータス初期化
		nowHP[Card.CharaID_Enemy] = enemyData.maxHP;
		maxHP[Card.CharaID_Enemy] = enemyData.maxHP;

		// 敵画像オブジェクト作成
		var obj = Instantiate (enemyPicturePrefab, enemyPictureParent);
		// 敵画像処理クラス取得
		enemyPicture = obj.GetComponent<EnemyPicture> ();
		// 敵画像処理クラス初期化
		enemyPicture.Init (this, enemyData.charaSprite);
    	// 敵ステータスバー生成
    	var statusBarObj = Instantiate(enemyStatusBarPrefab, enemyStatusBarsParent);
    	// 敵ステータスUI処理クラス取得

		// 敵ステータスUI表示
		enemyStatusUI.ShowCanvasGroup ();
		enemyStatusUI.SetHPView (nowHP[Card.CharaID_Enemy], maxHP[Card.CharaID_Enemy]);
	}

    public void SpawnAllEnemies(EncountEnemyGroupsSO encountEnemyGroups)
    {
		List<EnemyStatusSO> encountEnemies = encountEnemyGroups.EncountEnemies;
        foreach (EnemyStatusSO spawnEnemyData in encountEnemies)
        {
            SpawnEnemy(spawnEnemyData);
        }
    }

	/// <summary>
	/// 敵を消去する
	/// </summary>
	public void DeleteEnemy ()
	{
        // オブジェクト削除
		Destroy (enemyPicture.gameObject);
	}
	#endregion

	#region 各種Get系
	/// <summary>
	/// プレイヤーのHPが0以下であるかを返す
	/// </summary>
	/// <returns>プレイヤー敗北フラグ</returns>
	public bool IsPlayerDefeated ()
	{
		if (nowHP[Card.CharaID_Player] <= 0)
			return true;
		else
			return false;
	}

	/// <summary>
	/// 敵のHPが0以下であるかを返す
	/// </summary>
	/// <returns>敵撃破フラグ</returns>
	public bool IsEnemyDefeated ()
	{
		if (nowHP[Card.CharaID_Enemy] <= 0)
			return true;
		else
			return false;
	}
	#endregion
}
