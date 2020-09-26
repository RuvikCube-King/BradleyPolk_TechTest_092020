using System;

namespace DnDBeyondBusiness
{
	// class representing the different character classes a player could have
    public class PlayerClass
    {
		// the class's name
		private string _name = "";
		// the number of levels a character has taken in this class
		private int _levels = 0;
		// the hit dice value for this class, used to calculate hp
		private int _hitDiceValue = 0;

		#region Properties

		public string Name { get { return _name; } set { _name = value; } }
		public int Levels { get { return _levels; } set { _levels = value; } }
		public int HitDiceValue { get { return _hitDiceValue; } set { _hitDiceValue = value; } }

		#endregion

		public PlayerClass() { }

	}
}
