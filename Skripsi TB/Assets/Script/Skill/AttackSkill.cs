using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkill : Skill {

	public override IEnumerator unleash (Character targets, Team teamBelong)
	{
		return base.unleash (targets, teamBelong);
	}
}
