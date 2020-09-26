using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DnDBeyondBusiness
{
    // Enum to represent the different Resistance Types.  Normal is a catch for mistakes
    // in the file loading process.
    public enum ResistanceTypes { 
        Resistance,
        Immunity,
        Vulnerable,
        Normal
    }

    // Enum to represet the various damage types. Error is a catch for mistakes in the
    // file loading process and the Attack POST api action.
    public enum DamageTypes { 
        Bludgeoning,
        Piercing,
        Slashing,
        MagicalBludgeoning,
        MagicalPiercing,
        MagicalSlashing,
        Fire,
        Cold,
        Lightning,
        Thunder,
        Force,
        Acid,
        Poison,
        Radiant,
        Necrotic,
        Psychic,
        Error
    }

    // Class representing all the data a Character would need.
    public class Character
    {
        // Character's Name
        private string _name = "";
        // Character's total levels across all classes
        private int _level = 0;
        // List of all of a character's Classes
        private List<PlayerClass> _classes = new List<PlayerClass>();
        // List of all of a character's items
        private List<Item> _items = new List<Item>();
        // Dictionary for maintaining a Character's Resistances.
        // Key: DamageTypes enum value
        // Value: ResistanceTypes enum value
        private Dictionary<DamageTypes, ResistanceTypes> _resistances = new Dictionary<DamageTypes, ResistanceTypes>();
        // Dictionary for containing all of a Character's ability scores
        // Key: string, name of ability score
        // Value: int, value of the ability score
        private Dictionary<string, int> _abilityScores = new Dictionary<string, int>();
        // Dictionary for containing all of a Character's ability score modifiers so you won't need to calculate them every time they're needed.
        // Key: string, name of ability score
        // Value: int, value of the ability score mod
        private Dictionary<string, int> _abilityScoreMods = new Dictionary<string, int>();
        // Maximum HP value
        private int _maxHp = 0;
        // Temporary HP value
        private int _tempHp = 0;
        // Current HP value
        private int _currHp = 0;

        #region Properties

        public string Name { get { return _name; } set { _name = value; } }
        public int Level { get { return _level; } set { _level = value; } }
        public List<PlayerClass> Classes { get{ return _classes; } set{ _classes = value; } }
        public List<Item> Items { get { return _items; } set { _items = value; } }
        public Dictionary<DamageTypes, ResistanceTypes> Resistances { get{ return _resistances; } set{ _resistances = value; } }
        public Dictionary<string, int> AbilityScores { get{ return _abilityScores; } set{ _abilityScores = value; } }
        public Dictionary<string, int> AbilityScoreMods { get{ return _abilityScoreMods; } set{ _abilityScoreMods = value; } }
        public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
        public int TempHp { get { return _tempHp; } set { _tempHp = value; } }
        public int CurrHp { get { return _currHp; } set { _currHp = value; } }

        #endregion

        #region Functions

        public Character() { }

        // FUNCTION: LoadFromFile
        // RETURNS: Character
        // Loads up briv.json file from project folder, creates a Character and fills
        // up the object with data from the deserialized json file.
        static public Character LoadFromFile()
        {
            var briv = new Character();
            using(StreamReader r = File.OpenText("briv.json"))
            {
                string json = r.ReadToEnd();
                dynamic jsonArray = JsonConvert.DeserializeObject(json);
                briv.Name = jsonArray["name"];
                briv.Level = jsonArray["level"];
                foreach(var c in jsonArray["classes"])
                {
                    var characterClass = new PlayerClass();
                    characterClass.Name = c["name"];
                    characterClass.Levels = c["classLevel"];
                    characterClass.HitDiceValue = c["hitDiceValue"];
                    briv.Classes.Add(characterClass);
                }

                foreach(var i in jsonArray["items"])
                {
                    var newItem = new Item();
                    newItem.Name = i["name"];
                    newItem.Mods.AffectedObject = i["modifier"]["affectedObject"].Value;
                    newItem.Mods.AffectedValue = i["modifier"]["affectedValue"].Value;
                    newItem.Mods.Value = Convert.ToInt32(i["modifier"]["value"].Value);
                    briv.Items.Add(newItem);
                }

                // Loop through all ability scores, then loop through all of a character's items
                // to see if any of them affect ability scores. If they do, figure in their value
                // to Briv's final ability scores.
                // 
                // DEV_NOTE: Ideally you'd only grab the items from Items that actuall affect stats
                // then loop through that selection, or grab the items that affect the current looped
                // stat. However with a character file this small this shouldn't be an issue.
                string[] scores = { "strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma" };

                foreach(var s in scores)
                {
                    var itemMod = 0;
                    foreach(var i in briv.Items)
                    {
                        if (i.Mods.AffectedObject == "stats" && i.Mods.AffectedValue == s)
                            itemMod += i.Mods.Value;
                    }
                    briv.AbilityScores.Add(s, Convert.ToInt32(jsonArray["stats"][s].Value) + itemMod);
                }

                foreach (var d in jsonArray["defenses"])
                {
                    var type = DamageTypes.Bludgeoning;
                    var res = ResistanceTypes.Vulnerable;

                    switch (d["defense"].Value)
                    {
                        case "resistance":
                            res = ResistanceTypes.Resistance;
                            break;
                        case "immunity":
                            res = ResistanceTypes.Immunity;
                            break;
                        case "vulnerable":
                            res = ResistanceTypes.Vulnerable;
                            break;
                        default:
                            res = ResistanceTypes.Normal;
                            break;
                    }

                    switch (d["type"].Value)
                    {
                        case "bludgeoning":
                            type = DamageTypes.Bludgeoning;
                            break;
                        case "piercing":
                            type = DamageTypes.Piercing;
                            break;
                        case "slashing":
                            type = DamageTypes.Slashing;
                            break;
                        case "magicalbludgeoning":
                            type = DamageTypes.MagicalBludgeoning;
                            break;
                        case "magicalpiercing":
                            type = DamageTypes.MagicalPiercing;
                            break;
                        case "magicalslashing":
                            type = DamageTypes.MagicalSlashing;
                            break;
                        case "fire":
                            type = DamageTypes.Fire;
                            break;
                        case "cold":
                            type = DamageTypes.Cold;
                            break;
                        case "lightning":
                            type = DamageTypes.Lightning;
                            break;
                        case "thunder":
                            type = DamageTypes.Thunder;
                            break;
                        case "force":
                            type = DamageTypes.Force;
                            break;
                        case "acid":
                            type = DamageTypes.Acid;
                            break;
                        case "poison":
                            type = DamageTypes.Poison;
                            break;
                        case "radiant":
                            type = DamageTypes.Radiant;
                            break;
                        case "necrotic":
                            type = DamageTypes.Necrotic;
                            break;
                        case "psychic":
                            type = DamageTypes.Psychic;
                            break;
                        default:
                            type = DamageTypes.Error;
                            break;
                    }

                    // If there was an error or mispelling in the character file, skip adding data to the Resistances dictionary.
                    if(res != ResistanceTypes.Normal && type != DamageTypes.Error)
                    {
                        briv.Resistances.Add(type, res);
                    }
                }
            }

            briv.SetAbilityScoreMods();
            briv.CalculateHp();

            return briv;
        }

        // FUNCTION: GetAbilityScoreModifier
        // PARAMETERS: abilityScore:string, name of ability score
        // RETURNS: int, the caluclated ability score mod
        // Takes the name of an ability score, grabs the score from the Characters AbilityScores dictionary,
        // and calculates the ability score modifier.
        private int GetAbilityScoreModifier(string abilityScore)
        {
            if (!string.IsNullOrEmpty(abilityScore))
            {
                int abilityScoreValue = 0;
                if (AbilityScores.ContainsKey(abilityScore))
                {
                    abilityScoreValue = AbilityScores[abilityScore];
                }

                int totalAS = abilityScoreValue;
                double modifier = ((Convert.ToDouble(totalAS) - 10) / 2.0);
                return (int)Math.Floor(modifier);
            }

            return 0;
        }

        // FUNCTION: SetAbilityScoreMods
        // Tidy up function, fills the AbilityScoreMods dictionary for all ability scores.
        public void SetAbilityScoreMods()
        {
            AbilityScoreMods.Add("strength", GetAbilityScoreModifier("strength"));
            AbilityScoreMods.Add("dexterity", GetAbilityScoreModifier("dexterity"));
            AbilityScoreMods.Add("constitution", GetAbilityScoreModifier("constitution"));
            AbilityScoreMods.Add("intelligence", GetAbilityScoreModifier("intelligence"));
            AbilityScoreMods.Add("wisdom", GetAbilityScoreModifier("wisdom"));
            AbilityScoreMods.Add("charisma", GetAbilityScoreModifier("charisma"));
        }

        // FUNCTION: CalculateHp
        // Using the Character's constitution modifier, class levels, and class hit dice,
        // rolls for a Character's total Max Hp.
        public void CalculateHp() {
            int conMod = AbilityScoreMods["constitution"];
            int totalHp = 0;
            var rand = new Random();
            foreach(var c in Classes)
            {
                int cumulativeHp = 0;
                for (int i = 0; i < c.Levels; i++)
                {
                    cumulativeHp += (rand.Next(1, c.HitDiceValue+1) + conMod);
                }
                totalHp += cumulativeHp;
            }

            MaxHp = totalHp;
            CurrHp = MaxHp;
        }

        // FUNCTION: Heal
        // PARAMETERS: healValue:int, the amount you wish to heal
        // Heals the Character by the amount passed to the function.  If the amount healed
        // would put the Character over their Max Hp, sets CurrHp to MaxHp.
        public void Heal(int healValue)
        {
            if (CurrHp + healValue > MaxHp)
                CurrHp = MaxHp;
            else
                CurrHp += healValue;
        }

        // FUNCTION: AddTempHp
        // PARAMETERS: tempHPValue:int, the amount of temporary Hp you wish to add to the character
        // If the value passed in is currently higher than the Character's current temporary hp,
        // sets TempHp to the value passed
        public void AddTempHp(int tempHPValue)
        {
            if (tempHPValue > TempHp)
                TempHp = tempHPValue;
        }

        // FUNCTION: TakeDamage
        // PARAMETERS:
        //    -dt:DamageTypes, an enum representing the type of damage dealt
        //    -damageValue:int, the amount of damage dealt to the Character
        // Calculates damage dealt to the character, factors in if the Character has resistances
        // to the DamageTypes value passed in.  If a Character has Resistance to a damage type,
        // ensures that at minimum 1 damage is dealt, else round down.  If a Character has temporary
        // hp, takes away from TempHp first and any remaining damage gets taken from CurrHp. If damage
        // would put a Character under 0 hp, sets hp to 0.
        public void TakeDamage(DamageTypes dt, int damageValue)
        {
            double tempDamage = Convert.ToDouble(damageValue);
            if (Resistances.ContainsKey(dt))
            {
                double damageMod = 1.0;
                switch (Resistances[dt])
                {
                    case ResistanceTypes.Immunity:
                        damageMod = 0.0;
                        break;
                    case ResistanceTypes.Resistance:
                        damageMod = 0.5;
                        break;
                    case ResistanceTypes.Vulnerable:
                        damageMod = 2.0;
                        break;
                    default:
                        damageMod = 1.0;
                        break;
                }
                tempDamage = (Convert.ToDouble(damageValue) * damageMod);
                if (tempDamage > 0.0 && tempDamage < 1.0)
                    tempDamage = 1.0;
                else
                    tempDamage = Math.Floor(tempDamage);
            }

            if(tempDamage > 0.0)
            {
                if(TempHp > 0)
                {
                    if(TempHp >= (int)tempDamage)
                    {
                        TempHp -= (int)tempDamage;
                    } else
                    {
                        int postDamage = (int)tempDamage - TempHp;
                        TempHp = 0;
                        CurrHp -= postDamage;
                    }
                } else
                {
                    CurrHp -= (int)tempDamage;
                    if (CurrHp < 0)
                        CurrHp = 0;
                }
            }
        }

        #endregion

    }
}
