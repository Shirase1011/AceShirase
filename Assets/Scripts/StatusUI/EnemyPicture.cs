using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 敵キャラクターの画像オブジェクト管理クラス
/// </summary>
public class EnemyPicture : MonoBehaviour
{
	// キャラクターマネージャ
	private CharacterManager characterManager;

	// コンポーネント参照
	[SerializeField] private RectTransform rectTransform = null;
	[SerializeField] private Image image = null;

	// 敵画像初期座標
	private Vector2 basePosition;

	// 被ダメージ時ランダム移動Sequence
	private Sequence randomMoveSequence;

	// 初期化・敵出現処理(CharacterManager.csから呼出)
	public void Init (CharacterManager _characterManager, Sprite enemySprite)
	{
		// 参照取得
		characterManager = _characterManager;

		// 初期座標を保存
		basePosition = rectTransform.anchoredPosition;

		// 敵画像表示
		image.sprite = enemySprite;
		image.SetNativeSize (); // オブジェクトの大きさを画面の大きさに合わせる

		// 敵が画面上部から降りてくるアニメーション
		const float AnimTime = 0.5f; // 演出時間
        // 初期透明度を設定
        Color color = image.color;
        color.a = 0f;
        image.color = color;

        // 透明度変更アニメーション(Tween)
        image.DOFade(1f, AnimTime);
    }

	/// <summary>
	/// 被ダメージアニメーションを再生する
	/// </summary>
	public void DamageAnimation ()
	{
		// Sequence初期化
		if (randomMoveSequence != null)
			randomMoveSequence.Kill ();
		randomMoveSequence = DOTween.Sequence ();

		// 敵ランダム移動アニメーション
		const float JumpPosX_Width = 50.0f;	// 移動先のX方向範囲
		const float JumpPosY_Height = 50.0f;	// 移動先のY方向範囲
		const float AnimTime_Move = 0.05f;		// 移動時間
		// ランダム移動先を設定
		Vector2 pos = rectTransform.anchoredPosition;
		pos.x += Random.Range (-JumpPosX_Width / 2.0f, JumpPosX_Width / 2.0f);
		pos.y += Random.Range (-JumpPosY_Height / 2.0f, JumpPosY_Height / 2.0f);
		// ジャンプ移動アニメーション(Tween)
		randomMoveSequence.Append (rectTransform.DOAnchorPos (pos, AnimTime_Move));

		// 元の位置に戻るアニメーション
		const float AnimTime_Back = 1.2f;	// 移動時間
		// ジャンプ移動アニメーション(Tween)
		randomMoveSequence.Append (rectTransform.DOAnchorPos (basePosition, AnimTime_Back));
	}

	/// <summary>
	/// 撃破時の非表示化アニメーションを再生する
	/// </summary>
	public void DefeatAnimation ()
	{
		// 再生中のSequenceを停止
		if (randomMoveSequence != null)
			randomMoveSequence.Kill ();

		// 撃破時演出シーケンス初期化
		var defeatSequence = DOTween.Sequence ();

		// 敵が跳ねる(ジャンプする)アニメーション
		const float JumpPosX_Relative = 100.0f;    // ジャンプ移動先のX相対座標
		const float JumpPosY_Relative = 50.0f;	// ジャンプ移動先のY相対座標
		const float JumpPower = 30.0f;			// ジャンプの強度
		const float AnimTime = 0.7f;	// 演出時間
		// ジャンプ移動先を設定
		Vector2 pos = rectTransform.anchoredPosition;
		pos.x += JumpPosX_Relative;
		pos.y += JumpPosY_Relative;
		// ジャンプ移動アニメーション(Tween)
		defeatSequence.Append (rectTransform.DOJumpAnchorPos (pos, JumpPower, 1, AnimTime)
			.SetEase (Ease.Linear)); // 変化の仕方を指定

		// Scale縮小(Tween)
		defeatSequence.Join (rectTransform.DOScale (0.0f, AnimTime)
			.SetEase (Ease.Linear)); // 変化の仕方を指定

		// アニメーション完了時にオブジェクトを削除
		defeatSequence.OnComplete (() =>
		{
			characterManager.DeleteEnemy ();
		});
	}
}