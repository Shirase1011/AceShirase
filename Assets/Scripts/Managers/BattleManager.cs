using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 戦闘画面マネージャクラス
/// </summary>
public class BattleManager : MonoBehaviour
{
	// 管理下コンポーネント
	public FieldManager fieldManager; // フィールド管理クラス
	public CharacterManager characterManager; // キャラクターデータ管理クラス
	public PlayCardManager PlayCardManager; //カード効果発動管理クラス

	// (デバッグ用)出現敵データ
	[SerializeField] private EncountEnemyGroupsSO EncountEnemies = null;
	//[SerializeField] private EnemyStatusSO enemyStatusSO = null;

	// Start
	void Start()
    {
		// 管理下コンポーネント初期化
		fieldManager.Init (this);
		characterManager.Init (this);
		PlayCardManager.Init (this);

		// (デバッグ用)敵を画面に出現させる
		DOVirtual.DelayedCall (
			1.0f, // 1秒遅延
			() =>
			{
				// 敵出現
				characterManager.SpawnAllEnemies (EncountEnemies);
			}, false
		);
	}

    // Update
    void Update()
    {
		// (デバッグ用)
		if (Input.GetKeyDown (KeyCode.Space))
			characterManager.ChangeStatus_NowHP (Card.CharaID_Player, -5);
	}
}