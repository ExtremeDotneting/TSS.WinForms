using System;

namespace TSS.UniverseLogic
{
	public class Food : UniverseObject
	{

        float energyLevel;
		FoodType foodType;

        public Food()
        {
            //this.disposed = false;
            this.foodType = FoodType.defaultFood;
            descriptor = -1;
            energyLevel = UniverseConsts.FoodEnergyLevel;
        }

        public Food(FoodType foodType)
        {
            //this.disposed = false;
            switch (foodType)
            {
                case FoodType.deadCell:
                    descriptor = -2;
                    energyLevel = UniverseConsts.DeadCellsEnergyLevel;
                    break;

                case FoodType.defaultFood:
                    descriptor = -1;
                    energyLevel = UniverseConsts.FoodEnergyLevel;
                    break;

                case FoodType.poison:
                    descriptor = -3;
                    energyLevel = UniverseConsts.PoisonEnergyLevel;
                    break;

                default:
                    descriptor = -1;
                    energyLevel = 1;
                    break;
            }
            
            this.foodType = foodType;

        }

        public float GetEnergyLevel()
		{
            return energyLevel;
		}

		public FoodType GetFoodType()
		{
            return foodType;
		}
	}
}
