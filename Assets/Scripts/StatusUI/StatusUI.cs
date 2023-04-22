using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// キャラクターステータスUI表示処理クラス
/// </summary>
public class StatusUI : MonoBehaviour
{
	// HPゲージImage
	[SerializeField] private Image hpGageImage = null;
	// HP表示Text
	[SerializeField] private TextMeshProUGUI hpText = null;

	// Sield表示text
	[SerializeField] private TextMeshProUGUI SieldText = null;

	// 敵キャラクター用パラメータ
	[Space (10)]
	[Header ("敵キャラクター用")]
	// CanvasGroup
	[SerializeField] private CanvasGroup enemyCanvasGroup = null;
	// キャラクター名Text
	/// <summary>
	/// HPを表示する
	/// </summary>
	/// <param name="nowHP">現在HP</param>
	/// <param name="maxHP">最大HP</param>
	public void SetSieldView (int nowSield)
	{
		if (nowSield > 0)
			SieldText.text = nowSield.ToString();
	}
	public void SetHPView (int nowHP, int maxHP)
	{
		// HP表示の最小値を設定
		if (nowHP < 0)
			nowHP = 0;
		if (maxHP < 0)
			maxHP = 0;

		// ゲージ表示
		float ratio = 0.0f; // 最大HPに対する現在HPの割合
		if (maxHP > 0) // 0除算にならないように
			ratio = (float)nowHP / maxHP;
		hpGageImage.fillAmount = ratio;

		// Text表示
		hpText.text = nowHP + " / " + maxHP;
	}

	#region 敵ステータス表示専用処理
	private Tween fadeTween; // フェード演出Tween
	private const float FadeTime = 0.8f; // フェード演出時間
	
	/// <summary>
	/// 全UIを表示
	/// </summary>
	public void ShowCanvasGroup ()
	{
		if (fadeTween != null)
			fadeTween.Kill ();
		// 全UI表示アニメーション
		fadeTween = enemyCanvasGroup.DOFade (1.0f, FadeTime);
	}

	/// <summary>
	/// 全UIを非表示
	/// </summary>
	/// <param name="isAnimation">フェード演出実行フラグ</param>
	public void HideCanvasGroup (bool isAnimation)
	{
		if (fadeTween != null)
			fadeTween.Kill ();
		// 全UI非表示アニメーション(isAnimationがtrueの時のみ)
		if (isAnimation)
			fadeTween = enemyCanvasGroup.DOFade (0.0f, FadeTime);
		else
			enemyCanvasGroup.alpha = 0.0f;
	}
	#endregion
}