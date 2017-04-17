using System;

namespace TSS.UniverseLogic
{
	public interface IOutputManager
	{
		void StartSimulation();

		void PauseSimulation();

		//void stopSimulation();

		void SetTicksPerSecond(float value);

        float GetTicksPerSecond();

    }
}
