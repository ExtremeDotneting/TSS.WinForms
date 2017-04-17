using System;
using TSS.UniverseLogic;

namespace TSS.WinForms
{
    public class VideoOutputManager : IOutputManager
	{
		float ticksPerSecond;

		protected void renderPicture()
		{
			throw new NotImplementedException();
		}

		public void SetFileName(string value)
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
