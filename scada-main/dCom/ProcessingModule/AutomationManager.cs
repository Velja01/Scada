using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for automated work.
    /// </summary>
    public class AutomationManager : IAutomationManager, IDisposable
	{
		private Thread automationWorker;
        private AutoResetEvent automationTrigger;
        private IStorage storage;
		private IProcessingManager processingManager;
		private int delayBetweenCommands;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="automationTrigger">The automation trigger.</param>
        /// <param name="configuration">The configuration.</param>
        public AutomationManager(IStorage storage, IProcessingManager processingManager, AutoResetEvent automationTrigger, IConfiguration configuration)
		{
			this.storage = storage;
			this.processingManager = processingManager;
            this.configuration = configuration;
            this.automationTrigger = automationTrigger;
        }

        /// <summary>
        /// Initializes and starts the threads.
        /// </summary>
		private void InitializeAndStartThreads()
		{
			InitializeAutomationWorkerThread();
			StartAutomationWorkerThread();
		}

        /// <summary>
        /// Initializes the automation worker thread.
        /// </summary>
		private void InitializeAutomationWorkerThread()
		{
			automationWorker = new Thread(AutomationWorker_DoWork);
			automationWorker.Name = "Aumation Thread";
		}

        /// <summary>
        /// Starts the automation worker thread.
        /// </summary>
		private void StartAutomationWorkerThread()
		{
			automationWorker.Start();
		}


		private void AutomationWorker_DoWork()
		{
			const int step = 10;
			EGUConverter egu = new EGUConverter();
			PointIdentifier kapija = new PointIdentifier(PointType.ANALOG_OUTPUT, 1000);
			PointIdentifier prepreka = new PointIdentifier(PointType.DIGITAL_INPUT, 2000);
			PointIdentifier otvaranje = new PointIdentifier(PointType.DIGITAL_OUTPUT, 3000);
            PointIdentifier zatvaranje = new PointIdentifier(PointType.DIGITAL_OUTPUT, 3001);
			List<PointIdentifier> lista = new List<PointIdentifier>() { kapija, prepreka, otvaranje, zatvaranje }; 
            while (!disposedValue)
			{
				List<IPoint> points =storage.GetPoints(lista);

                if (points[0].Alarm == AlarmType.HIGH_ALARM && points[3].RawValue == 1)
                {
                    processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, zatvaranje.Address, 0);
                    for (int i = 0; i < delayBetweenCommands; i += 1000)
                    {
                        automationTrigger.WaitOne();
                    }
                    continue;
                }
                if (points[0].Alarm == AlarmType.LOW_ALARM && points[2].RawValue == 1)
                {
                    processingManager.ExecuteWriteCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, otvaranje.Address, 0);
                    for (int i = 0; i < delayBetweenCommands; i += 1000)
                    {
                        automationTrigger.WaitOne();
                    }
                    continue;
                }

                if (points[2].RawValue != points[3].RawValue)
				{
                    if (points[2].RawValue == 1)
                    {
                        int value = (int)egu.ConvertToEGU(points[0].ConfigItem.ScaleFactor, points[0].ConfigItem.Deviation, points[0].RawValue);
                        value -= step;
                        processingManager.ExecuteWriteCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, kapija.Address, value);
                    }
                    if (points[3].RawValue == 1)
                    {

                        int value = (int)egu.ConvertToEGU(points[0].ConfigItem.ScaleFactor, points[0].ConfigItem.Deviation, points[0].RawValue);
                        if (points[1].RawValue == 1)
                        {
                            processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, zatvaranje.Address, 0);
                            processingManager.ExecuteWriteCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, otvaranje.Address, 1);
                            while (value > points[0].ConfigItem.EGU_Min)
                            {
                                value -= step;
                                processingManager.ExecuteWriteCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, kapija.Address, value);
                                for (int i = 0; i < delayBetweenCommands; i += 1000)
                                {
                                    automationTrigger.WaitOne();
                                }
                            }
                        }
                        else
                        {
                            value += step;
                            processingManager.ExecuteWriteCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, kapija.Address, value);
                        }
						

                    }
                }
				
                for (int i = 0; i < delayBetweenCommands; i += 1000)
				{
					automationTrigger.WaitOne();
				}
			
            }
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indication if managed objects should be disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				disposedValue = true;
			}
		}


		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}

        /// <inheritdoc />
        public void Start(int delayBetweenCommands)
		{
			this.delayBetweenCommands = delayBetweenCommands*1000;
            InitializeAndStartThreads();
		}

        /// <inheritdoc />
        public void Stop()
		{
			Dispose();
		}
		#endregion
	}
}
