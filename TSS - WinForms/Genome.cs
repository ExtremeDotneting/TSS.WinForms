using System;
using System.Collections.Generic;

namespace TSS.UniverseLogic
{
	public class Genome
	{
        int hunger;
        int aggression;
        int Reproduction;
        int friendly;
        int poisonAddiction;

        public Genome(int hunger, int aggression, int Reproduction, int friendly, int poisonAddiction)
        {

            this.hunger = hunger;
            this.aggression = aggression;
            this.Reproduction = Reproduction;
            this.friendly = friendly;
            this.poisonAddiction = poisonAddiction;
        }

        public Genome()
        {
            hunger = SpecialConsts.GetHunger();
            aggression = SpecialConsts.GetAggression();
            Reproduction = SpecialConsts.GetReproduction();
            friendly = SpecialConsts.GetFriendly();
            poisonAddiction = SpecialConsts.GetPoisonAddiction();
        }



        public int GetHunger()
        {
            return hunger;
        }

        public int GetAggression()
        {
            return aggression;
        }

        public int GetReproduction()
        {
            return Reproduction;
        }

        public int GetFriendly()
        {
            return friendly;
        }

        public int GetPoisonAddiction()
        {
            return poisonAddiction;
        }

        public Genome Clone()
        {
            return new Genome(hunger, aggression, Reproduction, friendly, poisonAddiction);
        }

        public Genome CloneAndMutate()
        {
            int modificator;
            int hunger = this.hunger, aggression = this.aggression, Reproduction = this.Reproduction, friendly = this.friendly, poisonAddiction = this.poisonAddiction;

            for (int i = 0; i < UniverseConsts.MutationAtOne; i++)
            {
                if (StableRandom.rd.Next(2) == 0)
                    modificator = -1;
                else
                    modificator = 1;
                switch (StableRandom.rd.Next(1, 6))
                {
                    case 1:
                        hunger += modificator;
                        break;

                    case 2:
                        aggression += modificator;
                        break;

                    case 3:
                        Reproduction += modificator;
                        break;

                    case 4:
                        friendly +=modificator;
                        break;

                    case 5:
                        poisonAddiction += modificator;
                        break;

                }
                
            }
            return new Genome(hunger, aggression, Reproduction, friendly, poisonAddiction);
           

        }



    }
}
