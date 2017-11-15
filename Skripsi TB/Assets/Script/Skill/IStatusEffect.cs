public interface IStatusEffect{

	void addStatusEffect(Character target,string effectType);
	string getEffectType();
	void addStatusEffect (Character target, string effectType, int effectStartTurn);
}
