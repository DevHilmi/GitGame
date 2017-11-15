using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaSpriteManager : MonoBehaviour {

	public Sprite[] warrior_sprites;
	public Sprite[] wizard_sprites;
	public Sprite[] healer_sprites;
	public Sprite[] effect_sprites;
	// Use this for initialization

	public Sprite getCharSprite(string profession){
		Sprite result = null;
		int index = 0;
		switch(profession){
		case Character.Warrior:{
				index = Random.Range(0,warrior_sprites.Length);
				result = warrior_sprites[index];
				break;
			}
		case Character.Healer:{
				index = Random.Range(0,healer_sprites.Length);
				result = healer_sprites[index];
				break;
			}
		case Character.Wizard:{
				index = Random.Range(0,wizard_sprites.Length);
				result = wizard_sprites[index];
				break;
			}
		}
		return result;
	}

	public Sprite getStatusEffectSprite(string effectType){
		Sprite result = null;
		switch(effectType){
		case Skill.BuffAtk:{
				result = effect_sprites[0];
				break;
			}
		case Skill.BuffDef:{
				result = effect_sprites[1];
				break;
			}
		case Skill.Blind:{
				result = effect_sprites[2];
				break;
			}
		case Skill.Poison:{
				result = effect_sprites[3];
				break;
			}
		}
		return result;
	}

}
