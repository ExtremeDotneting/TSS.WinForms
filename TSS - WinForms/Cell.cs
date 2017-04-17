using System;
using System.Collections.Generic;

namespace TSS.UniverseLogic
{
	public class Cell : UniverseObject
	{
		int age ;
        float energyLevel;
		//MoveDirection moveDirectionAspiration = MoveDirection.stand;
	    Genome genome;
        MoveDirection moveDisperation;

        public Cell()
        {
            Initialize(new Genome(), 1, UniverseConsts.DefCellEnergyLevel);
        }

        public Cell(Genome genome)
        {
            Initialize(genome, 1, UniverseConsts.DefCellEnergyLevel);
        }

        public Cell(Genome genome, float energyLevel) 
        {
            Initialize(genome, 1, energyLevel);
        }

        public Cell(Genome genome, float energyLevel, int descriptor)
        {
            Initialize(genome, descriptor, energyLevel);
        }

        void Initialize(Genome genome, int descriptor, float energyLevel) 
        {
            //this.disposed = false;
            this.genome = genome;
            age = 0;
            this.energyLevel = energyLevel;
            if (descriptor < 100)
            {
                int desc = StableRandom.rd.Next(100, int.MaxValue);
                this.descriptor = desc;
            }
            else
                this.descriptor = descriptor;
        }

        public Genome GetGenome()
        {
            return genome;
        }


        public MoveDirection GetMoveDisperation()
        {
            return moveDisperation;
        }

        public float GetEnergyLevel()
		{
            return energyLevel;
		}

		public void AddEnergy(float value)
		{
            energyLevel += value;
            if (energyLevel > UniverseConsts.MaxCellEnergyLevel)
                energyLevel = UniverseConsts.MaxCellEnergyLevel;

        }

		public bool IncAge()
		{
            return age++ > UniverseConsts.MaxCellAge;
		}

        public bool DecEnergy()
        {
            energyLevel -= UniverseConsts.EnergyEntropyPerSecond;
            return energyLevel  < 0;
        }

        public int GetAge()
        {
            return age;
        }

        public Cell Reproduct()
        {
            age = 0;
            energyLevel = (int)(energyLevel / 2);
            if (StableRandom.rd.Next(100) < UniverseConsts.MutationChancePercent && UniverseConsts.EnableMutation)
            {
                Cell res = new Cell(genome.CloneAndMutate(), 1);
                res.AddEnergy(/*UniverseConsts.DefCellEnergyLevel * 8*/energyLevel);
                return res;
            }
            else
                return new Cell(genome.Clone(), energyLevel, descriptor);
           
        }


        //genome
        //genome
        private bool IsAdult()
        {
            return GetAge() >= UniverseConsts.AdultCellAge;
        }

        private float AnalizeDescriptor(int x, int y)
        {
            int desc = universe.GetObjectDescriptor(x, y);
            if (desc == 0)//empty
            {
                return 0;
            }
            else if (desc == descriptor)//friend
            {
                return genome.GetFriendly();
            }
            else if (desc < 0)//food
            {
                if (desc == -3)
                    return genome.GetPoisonAddiction();
                return genome.GetHunger();
            }
            else//enemy
            {
                if (GetAge() >= UniverseConsts.AdultCellAge)
                    return genome.GetAggression();
                else
                    return UniverseConsts.ChildrenAggression;
            }



        }
        public MoveDirection CalcMoveDirectionAspiration()
        {
            float up, down, left, right;

            //up = AnalizeDescriptor(x - 1, y - 2) + 2*AnalizeDescriptor(x, y - 2) + AnalizeDescriptor(x + 1, y - 2) +
            //    2 * AnalizeDescriptor(x - 1, y - 1) + 3*AnalizeDescriptor(x, y - 1) + 2*AnalizeDescriptor(x + 1, y - 1);

            //down = AnalizeDescriptor(x - 1, y + 2) + 2*AnalizeDescriptor(x, y + 2) + AnalizeDescriptor(x + 1, y + 2) +
            //    2 * AnalizeDescriptor(x - 1, y + 1) + 3*AnalizeDescriptor(x, y + 1) + 2*AnalizeDescriptor(x + 1, y + 1);

            //left = AnalizeDescriptor(x - 2, y - 1) + 2*AnalizeDescriptor(x - 2, y) + AnalizeDescriptor(x - 2, y + 1) +
            //    2 * AnalizeDescriptor(x - 1, y - 1) + 3*AnalizeDescriptor(x - 1, y) + 2*AnalizeDescriptor(x - 1, y + 1);

            //right = AnalizeDescriptor(x + 2, y - 1) + 2*AnalizeDescriptor(x + 2, y) + AnalizeDescriptor(x + 2, y + 1) +
            //    2 * AnalizeDescriptor(x + 1, y - 1) + 3*AnalizeDescriptor(x + 1, y) + 2*AnalizeDescriptor(x + 1, y + 1);

            up = AnalizeDescriptor(x - 1, y - 2) + AnalizeDescriptor(x + 1, y - 2) + 3 * AnalizeDescriptor(x, y - 1)+
            2 * (AnalizeDescriptor(x, y - 2) + AnalizeDescriptor(x - 1, y - 1) + AnalizeDescriptor(x + 1, y - 1));

            down = AnalizeDescriptor(x - 1, y + 2) + AnalizeDescriptor(x + 1, y + 2) + 3 * AnalizeDescriptor(x, y + 1) +
                2*(AnalizeDescriptor(x - 1, y + 1) +  AnalizeDescriptor(x, y + 2)  +  AnalizeDescriptor(x + 1, y + 1));

            left = AnalizeDescriptor(x - 2, y - 1) + AnalizeDescriptor(x - 2, y + 1) + 3 * AnalizeDescriptor(x - 1, y) +
                2 * (AnalizeDescriptor(x - 1, y - 1) + AnalizeDescriptor(x - 2, y) + AnalizeDescriptor(x - 1, y + 1));

            right = AnalizeDescriptor(x + 2, y - 1) + 3 * AnalizeDescriptor(x + 1, y) + AnalizeDescriptor(x + 2, y + 1) +
                2 *( AnalizeDescriptor(x + 1, y - 1) +  AnalizeDescriptor(x + 2, y) +  AnalizeDescriptor(x + 1, y + 1));


            float biggest = up;
            if (down > biggest)
                biggest = down;
            if (left > biggest)
                biggest = left;
            if (right > biggest)
                biggest = right;

            MoveDirection res = MoveDirection.stand;
            if (biggest >= 0)
            {
                List<MoveDirection> md = new List<MoveDirection>(0);
                if (biggest == up)
                    md.Add(MoveDirection.up);
                if (biggest == down)
                    md.Add(MoveDirection.down);
                if (biggest == left)
                    md.Add(MoveDirection.left);
                if (biggest == right)
                    md.Add(MoveDirection.right);

                res= md[StableRandom.rd.Next(md.Count)];

            }
            moveDisperation = res;
            return res;


        }

        public bool CalcReproduction()
        {
            return ((GetEnergyLevel() >= (UniverseConsts.ReproductEnergyLevel - (genome.GetReproduction() * UniverseConsts.ReproductEnergyLevel / 10))) && IsAdult());
        }

        //genome
        //genome

    }
}
