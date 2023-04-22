using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// (DataManagerオブジェクトにアタッチ)
/// データマネージャー
/// ゲーム起動中常に同じインスタンス(オブジェクト)が１つ存在している
/// </summary>
public class Data : MonoBehaviour
{
	#region シングルトン維持用処理(変更不要)
	// シングルトン維持用
	public static Data instance;

	// Awake(Startより前に１度だけ実行)
	private void Awake ()
	{
		// シングルトン用処理
		if (instance != null)
		{
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);

		// ゲーム起動時処理
		InitialProcess ();
	}
	#endregion

	// 各種コンポーネント
	public PlayerDeckData playerDeckData; // デッキ管理クラス

	// シーン間保存データ
	public static SystemLanguage nowLanguage; // 現在の設定言語

	/// <summary>
	/// ゲーム開始時(インスタンス生成時)に一度だけ実行される処理
	/// </summary>
	private void InitialProcess ()
	{
		// 乱数シード値初期化
		Random.InitState (System.DateTime.Now.Millisecond);

		// 実行環境の言語設定を取得する
		nowLanguage = GetLanguageData ();
		// プレイヤーデッキデータ初期化処理
		playerDeckData.Init ();
		// プレイヤー所持カードデータ初期化(セーブ機能実装後は別のタイミングで呼び出し)
		playerDeckData.DataInitialize ();
	}


	/// <summary>
	/// 実行環境の言語設定を取得して返す
	/// </summary>
	/// <returns>言語データ</returns>
	private SystemLanguage GetLanguageData ()
	{
		// 実行環境の言語設定を取得
		var language = Application.systemLanguage;
		// 日本語以外の言語だった場合全て英語で対応する
		if (language != SystemLanguage.Japanese)
			language = SystemLanguage.English;

		return language;
	}
}
