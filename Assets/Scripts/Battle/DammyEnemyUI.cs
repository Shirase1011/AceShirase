using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ダミーエネミー制御クラス
/// </summary>
public class DammyEnemyUI : MonoBehaviour
{
	// ダミーエネミー整列用HorizontalLayoutGroup
	[SerializeField] private HorizontalLayoutGroup layoutGroup = null;
	// ダミーエネミープレハブ
	[SerializeField] private GameObject dammyEnemyPrefab = null;

	// 生成したダミーエネミーのリスト
	private List<Transform> dammyEnemyList;

	/// <summary>
	/// 指定の枚数になるようダミーエネミーを作成または削除する
	/// </summary>
	/// <param name="value">設定枚数</param>
	public void SetEnemyNum (int value)
	{
		if (dammyEnemyList == null)
		{// 初回実行時
			// リスト新規生成
			dammyEnemyList = new List<Transform> ();
			AddEnemyObj (value);
		}
		else
		{
			// 現在から変化する枚数を計算
			int differenceNum = value - dammyEnemyList.Count;
			// ダミーエネミー作成・削除
			if (differenceNum > 0) // エネミーが増えるならダミーエネミー作成
				AddEnemyObj (differenceNum);
			else if (differenceNum < 0) // エネミーが減るならダミーエネミー削除
				RemoveEnemyObj (differenceNum);
		}
	}

	/// <summary>
	/// ダミーエネミーを指定枚数追加する
	/// </summary>
	private void AddEnemyObj (int value)
	{
		// 追加枚数分オブジェクト作成
		for (int i = 0; i < value; i++)
		{
			// オブジェクト作成
			var obj = Instantiate (dammyEnemyPrefab, transform);
			// リストに追加
			dammyEnemyList.Add (obj.transform);
		}
	}
	/// <summary>
	/// ダミーエネミーを指定枚数削除する
	/// </summary>
	private void RemoveEnemyObj (int value)
	{
		// 削除枚数を正数で取得
		value = Mathf.Abs (value);
		// 削除枚数分オブジェクト削除
		for (int i = 0; i < value; i++)
		{
			if (dammyEnemyList.Count <= 0)
				break;

			// オブジェクト削除
			Destroy (dammyEnemyList[0].gameObject);
			// リストから削除
			dammyEnemyList.RemoveAt (0);
		}
	}

	/// <summary>
	/// 該当番号のダミーエネミーの座標を返す
	/// </summary>
	public Vector2 GetEnemyPos (int index)
	{
		if (index < 0 || index >= dammyEnemyList.Count)
			return Vector2.zero;
		// ダミーエネミーの座標を返す
		return dammyEnemyList[index].position;
	}

	/// <summary>
	/// レイアウトの自動整列機能を即座に適用する
	/// </summary>
	public void ApplyLayout ()
	{
		layoutGroup.CalculateLayoutInputHorizontal ();
		layoutGroup.SetLayoutHorizontal ();
	}
    public Vector3 GetDammyEnemyPosition(int index)
    {
        if (index < dammyEnemyList.Count)
        {
            return dammyEnemyList[index].position;
        }
        else
        {
            Debug.LogError("Invalid index: " + index);
            return Vector3.zero;
        }
    }
}
