using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Completed;

namespace ItemSpace
{
	public abstract class EquipItem : Item
	{
		protected double attackMult, speedMult, dodgeBonus, blockBonus;
		protected int attackBonus, hpBonus;
		protected int weight;

		protected readonly double[] multTiers = new double[] {1.1, 1.25, 1.5, 2};
		protected readonly int[] attackBonusTiers = new int[] {3, 6, 12, 25};
		protected readonly int[] hpBonusTiers = new int[] {2, 5, 10, 15};

		protected void setup(int weight, int prefixAttr, int prefixTier, int infixAttr, int infixTier, int suffixAttr, int suffixTier) {
			attackMult = speedMult = dodgeBonus = blockBonus = 1;
			attackBonus = hpBonus = 0;

			this.weight = weight;

			BuffAttr (prefixAttr, prefixTier);
			BuffAttr (infixAttr, infixTier);
			BuffAttr (suffixAttr, suffixTier);

			CreateName (0, weight, AttrIndex (prefixAttr, prefixTier), 
				AttrIndex (infixAttr, infixTier), AttrIndex (suffixAttr, suffixTier));
		}

		protected virtual void BuffWeight (int weight) {
			if (weight == 0)
				speedMult *= 1.05;
			else
				speedMult *= 0.95;
		}

		public void CreateName (int type, int weight, int prefix, int infix, int suffix) {
			name = "";
			foreach (string s in new string[] {Weights[weight], Prefixes[prefix], Infixes[infix], Types[type], Suffixes[suffix]}) {
				name += s;
				if (!s.Equals (""))
					name += " ";
			}
			name = name.Trim ();
		}

		protected abstract string[] Types {
			get;
		}

		protected abstract string[] Weights {
			get;
		}

		protected abstract string[] Prefixes {
			get;
		}

		protected abstract string[] Infixes {
			get;
		}

		protected abstract string[] Suffixes {
			get;
		}

		protected void BuffAttr(int attr, int tier) {
			if (attr == 0) {
				attackMult *= multTiers [tier];
			} else if (attr == 1) {
				speedMult *= multTiers [tier];
			} else if (attr == 2) {
				attackBonus += attackBonusTiers [tier];
			} else if (attr == 3) {
				hpBonus += hpBonusTiers [tier];
			} else if (attr == 4) {
				dodgeBonus *= multTiers [tier];
			} else if (attr == 5) {
				dodgeBonus *= multTiers [tier];
			}
		}

		protected static void RandomAttr(out int attr, out int tier) {
			int attrSeed = UnityEngine.Random.Range (0, 12);
			if (attrSeed % 2 == 0) {
				attr = attrSeed / 2;
				int tierSeed = UnityEngine.Random.Range (0, 12);
				if (tierSeed < 6) {
					tier = 0;
				} else if (tierSeed < 9) {
					tier = 1;
				} else if (tierSeed < 11) {
					tier = 2;
				} else {
					tier = 3;
				}
			} else {
				attr = 6;
				tier = 0;
			}
		}

		protected static int AttrIndex(int attr, int tier) {
			return attr * 4 + tier;
		}

		public double AttackMult {
			get {
				return attackMult;
			}
		}

		public double SpeedMult {
			get {
				return speedMult;
			}
		}

		public int AttackBonus {
			get {
				return attackBonus;
			}
		}

		public int HpBonus {
			get {
				return hpBonus;
			}
		}

		public double DodgeBonus {
			get {
				return dodgeBonus;
			}
		}

		public double BlockBonus {
			get {
				return blockBonus;
			}
		}

		virtual public XElement serialize(){
			XElement node = new XElement("weapon",
				new XElement("name", this.name),
				new XElement("weight", this.weight),
				new XElement("attackMult", this.attackMult),
				new XElement("speedMult", this.speedMult),
				new XElement("attackBonus", this.attackBonus),
				new XElement("hpBonus", this.hpBonus),
				new XElement("dodgeBonus", this.dodgeBonus),
				new XElement("blockBonus", this.blockBonus));

			return node;
		}

		virtual public bool deserialize(XElement s){
			List<XElement> info = s.Descendants().ToList();
			name = info [0].Value;
			weight = Convert.ToInt16 (info [1].Value);
			attackMult = Convert.ToDouble (info [2].Value);
			speedMult = Convert.ToDouble (info [3].Value);
			attackBonus = Convert.ToInt16 (info [4].Value);
			hpBonus = Convert.ToInt16 (info [5].Value);
			dodgeBonus = Convert.ToDouble (info [6].Value);
			blockBonus = Convert.ToDouble (info [7].Value);

			return true;
		}
	}

	public enum EquipAttribute
	{
		AttackMult,
		SpeedMult,
		AttackBonus,
		HpBonus,
		DodgeBonus,
		BlockBonus,
		None
	}
}

