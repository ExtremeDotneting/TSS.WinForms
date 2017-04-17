using System;
using TSS.UniverseLogic;

namespace TSS.WinForms
{
	public class RealTimeOutputManager : IOutputManager
	{
		float ticksPerSecond;

		public void StartSimulationThread()
		{
			throw new NotImplementedException();
		}

		public void stopSimulationThread()
		{
			throw new NotImplementedException();
		}

		public void startRenderThread()
		{
			throw new NotImplementedException();
		}

		public void stopRenderThread()
		{
			throw new NotImplementedException();
		}

		public void StartSimulation()
		{
			throw new NotImplementedException();
		}

		public void PauseSimulation()
		{
			throw new NotImplementedException();
		}

		public void stopSimulation()
		{
			throw new NotImplementedException();
		}

        public void SetTicksPerSecond(float value)
        {
            ticksPerSecond = value;
        }

        public float GetTicksPerSecond()
        {
            return ticksPerSecond;
        }
    }
}
