using System;
using System.ComponentModel;


namespace CalcModule_Hydro
{    
	[DisplayName("Гидравлический расчёт кольцевого трубопровода")]
	
	public sealed class CalcModule_Hydro
    {
        private string ReportText;
		protected void DoCalculations()
		{
			try
			{
                
				//FeatureCollection fcInput = base.InputFeatureCollection;
								
				//FeatureCollection fcOutput = TECalcModule_RingPipeLine.RPInitialFlowDistribution.RunCalc(fcInput, true);

				//SetOutputFeatureCollection(fcOutput);
				
				//SetComplete_Success();

				ReportText = "Вычислительный модуль завершил работу";
                Console.WriteLine(ReportText);
			}
			catch (Exception ex)
			{
				try
				{
                    Console.WriteLine(ex.Message);
					//SetComplete_Error();
				}
				catch { }
			}
		}
	}
}


