
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カードのジョブの種類を定義
#region 
[System.Serializable]
public class JobDefine
{	
	[Header ("ジョブ名")]
	public JobName Jobname;

	public enum JobName
	{
		Bravery, //勇者
		Knight, //ナイト
		Mage, //初級魔法使い
		Warrior, //ウォーリアー
		DarkMage, //闇魔術師
		Dancer, //踊り子
		Marchant, //商人
		Necromancer, //ネクロマンサー
		HolyMage, //神秘魔導士
		Sorcerer, //ソーサラー
		Paladin, //パラディン
		Vampire, //ヴァンパイア
		SwordMaster, //剣聖
		Demon, //悪魔
		Angel, //天使
		FallenAngel, //堕天使
		Witch, //魔女
		Samurai, //侍
		Berserker, //バーサーカー
		GrimReaper, //死神

		Enemy, //敵
	}
	// 和名(JP)
	readonly public static Dictionary<JobName, string> Dic_JobName_JP = new Dictionary<JobName, string> ()
	{
		{JobName.Bravery,
			"勇者" },
		{JobName.Knight,
			"ナイト" },
		{JobName.Mage,
			"魔法使い" },
        {JobName.Warrior,
            "ウォーリアー"},
		{JobName.DarkMage,
			"闇魔導士"},
		{JobName.Dancer,
			"踊り子"},
		{JobName.Marchant,
			"商人"},
		{JobName.Necromancer,
			"ネクロマンサー"},
		{JobName.HolyMage,
			"神秘魔導士"},
		{JobName.Sorcerer,
			"ソーサラー"},
		{JobName.Paladin,
			"パラディン"},
		{JobName.Vampire,
			"ヴァンパイア"},
		{JobName.SwordMaster,
			"剣聖"},
		{JobName.Demon,
			"悪魔"},
		{JobName.Angel,
			"天使"},
		{JobName.FallenAngel,
			"堕天使"},
		{JobName.Witch,
			"魔女"},
		{JobName.Samurai,
			"侍"},
		{JobName.Berserker,
			"バーサーカー"},
		{JobName.GrimReaper,
			"死神"},
		{JobName.Enemy,
			"敵"},
	};
}

[System.Serializable]
public class CardType
{
	[Header ("カードタイプ")]
	public CardTypes Cardtype;
	public enum CardTypes
	{
		Magic, //魔法カード
		Attack, //アタックカード
		Sield, //シールドカード
		Skill, //スキルカード
		Curse, //呪いカード
	}
	

}
#endregion

/// <summary>
/// カードの効果の種類などを定義
/// </summary>
[System.Serializable]
public class CardEffectDefine
{
	// パラメータ
	[Header ("効果の種類")]
	public CardEffect cardEffect;
	[Header ("効果値")]
	public int value;

	#region 効果の種類定義部
	// カード効果定義
	public enum CardEffect
	{
		Damage,     // ダメージ
		Seald,      //シールド
		Draw,       // ドロー
        Discard,    //手札捨てる		
	}

	// 効果名(JP)
	readonly public static Dictionary<CardEffect, string> Dic_EffectName_JP = new Dictionary<CardEffect, string> ()
	{
		{CardEffect.Damage,
			"対象に{0}のダメージを与える。" },
		{CardEffect.Seald,
			"{0}のシールドを獲得する。" },
		{CardEffect.Draw,
			"カードを{0}枚引く。" },
        {CardEffect.Discard,
            "カードを{0}枚捨てる。"},
	};
	// 効果名(EN)
	readonly public static Dictionary<CardEffect, string> Dic_EffectName_EN = new Dictionary<CardEffect, string> ()
	{
		{CardEffect.Damage,
			"{0} Damage" },
		{CardEffect.Seald,
			"{0} Sield" },
		{CardEffect.Draw,
			"Draw {0} card" },
        {CardEffect.Discard,
            "Discard {0} card"},
	};

	// 効果説明(JP)
	readonly public static Dictionary<CardEffect, string> Dic_EffectExplain_JP = new Dictionary<CardEffect, string> ()
	{
		{CardEffect.Damage,
			"対象に {0} のダメージを与える。" },
		{CardEffect.Seald,
			" {0} のシールドを獲得する。" },
		{CardEffect.Draw,
			"カードを {0} 枚引く。" },
        {CardEffect.Discard,
            "カードを {0} 枚捨てる。"},
	};
	// 効果説明(EN)
	readonly public static Dictionary<CardEffect, string> Dic_EffectExplain_EN = new Dictionary<CardEffect, string> ()
	{
		{CardEffect.Damage,
			"{0} Damage" },
		{CardEffect.Seald,
			"{0} Sield" },
		{CardEffect.Draw,
			"Draw {0} card" },
        {CardEffect.Discard,
            "Discard {0} card"},
	};
	#endregion
}