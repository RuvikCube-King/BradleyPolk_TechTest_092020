using System;
using System.Collections.Generic;
using System.Text;

namespace DnDBeyondBusiness
{
	// Class representing the modifications that an Item can have
	public class ItemModifier
	{
		// What part of the character the modifier effects, e.g. stats
		private string _affectedObject = "";
		// Thing item actually modifies within the affected object, e.g. constitution
		private string _affectedValue = "";
		// Value of actual modification
		private int _value = 0;

		#region Properties

		public string AffectedObject { get { return _affectedObject; } set { _affectedObject = value; } }
		public string AffectedValue { get { return _affectedValue; } set { _affectedValue = value; } }
		public int Value { get { return _value; } set { _value = value; } }

		#endregion

		public ItemModifier() { }
	}

	// Class representing an item a character can have
	public class Item {
		// an ItemModifier object representing what the item affects
		private ItemModifier _mods;
		// an Item's name
		private string _name = "";

		public ItemModifier Mods {get{return _mods;} set{_mods = value;}}
		public string Name { get { return _name; } set { _name = value; } }

		public Item() { Mods = new ItemModifier(); }
	}


}
