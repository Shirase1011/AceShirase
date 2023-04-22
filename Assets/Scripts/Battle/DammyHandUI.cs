using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ダミー手札制御クラス
/// </summary>
public class DammyHandUI : MonoBehaviour
{
	// ダミー手札整列用HorizontalLayoutGroup
	[SerializeField] private HorizontalLayoutGroup layoutGroup = null;
	// ダミー手札プレハブ
	[SerializeField] private GameObject dammyHandPrefab = null;

	// 生成したダミー手札のリスト
	private List<Transform> dammyHandList;

	/// <summary>
	/// 指定の枚数になるようダミー手札を作成または削除する
	/// </summary>
	/// <param name="value">設定枚数</param>
	public void SetHandNum (int value)
	{
		if (dammyHandList == null)
		{// 初回実行時
			// リスト新規生成
			dammyHandList = new List<Transform> ();
			AddHandObj (value);
		}
		else
		{
			// 現在から変化する枚数を計算
			int differenceNum = value - dammyHandList.Count;
			// ダミー手札作成・削除
			if (differenceNum > 0) // 手札が増えるならダミー手札作成
				AddHandObj (differenceNum);
			else if (differenceNum < 0) // 手札が減るならダミー手札削除
				RemoveHandObj (differenceNum);
		}
	}

	/// <summary>
	/// ダミー手札を指定枚数追加する
	/// </summary>
	private void AddHandObj (int value)
	{
		// 追加枚数分オブジェクト作成
		for (int i = 0; i < value; i++)
		{
			// オブジェクト作成
			var obj = Instantiate (dammyHandPrefab, transform);
			// リストに追加
			dammyHandList.Add (obj.transform);
		}
	}
	/// <summary>
	/// ダミー手札を指定枚数削除する
	/// </summary>
	private void RemoveHandObj (int value)
	{
		// 削除枚数を正数で取得
		value = Mathf.Abs (value);
		// 削除枚数分オブジェクト削除
		for (int i = 0; i < value; i++)
		{
			if (dammyHandList.Count <= 0)
				break;

			// オブジェクト削除
			Destroy (dammyHandList[0].gameObject);
			// リストから削除
			dammyHandList.RemoveAt (0);
		}
	}

	/// <summary>
	/// 該当番号のダミー手札の座標を返す
	/// </summary>
	public Vector2 GetHandPos (int index)
	{
		if (index < 0 || index >= dammyHandList.Count)
			return Vector2.zero;
		// ダミー手札の座標を返す
		return dammyHandList[index].position;
	}

	/// <summary>
	/// レイアウトの自動整列機能を即座に適用する
	/// </summary>
	public void ApplyLayout ()
	{
		layoutGroup.CalculateLayoutInputHorizontal ();
		layoutGroup.SetLayoutHorizontal ();
	}
}