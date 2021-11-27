
using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	[SameAsSymbol(true)]
	public class BarCount3 : IndicatorObject 
	{
		#region Private Variables
        private int fontSize = 20;
        private int barsNeededForSetup = 9; // Default setting for BarsNeededForSetup
        private int barsNeededForCountdown = 13; // Default setting for BarsNeededForCountdown
        private int lookBackBarsForSetup = 4; // Default setting for LookBackBarsForSetup
        private int lookBackBarsForCountdown = 2; // Default setting for LookBackBarsForCountdown
		#endregion
		
		#region Parameters
        [Input]
        public int BarsNeededForSetup
        {
            get { return barsNeededForSetup; }
            set { barsNeededForSetup = Math.Max(2, value); }
        }
        [Input]
        public int BarsNeededForCountdown
        {
            get { return barsNeededForCountdown; }
            set { barsNeededForCountdown = Math.Max(2, value); }
        }
        [Input]
        public int LookBackBarsForSetup
        {
            get { return lookBackBarsForSetup; }
            set { lookBackBarsForSetup = Math.Max(2, value); }
        }
        [Input]
        public int LookBackBarsForCountdown
        {
            get { return lookBackBarsForCountdown; }
            set { lookBackBarsForCountdown = Math.Max(2, value); }
        }
        [Input]
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }
        #endregion
		
		public BarCount3(object _ctx)
			: base(_ctx){}
		
		private Function.BarCount _barCount;
			
		protected override void Create() 
		{
			// create variable objects, function objects, plot objects etc.
			_barCount = new Function.BarCount(this);
		}
		protected override void StartCalc() 
		{
			// assign inputs 
			_barCount.FontSize = fontSize;
			_barCount.BarsNeededForSetup = barsNeededForSetup;
			_barCount.BarsNeededForCountdown = barsNeededForCountdown;
			_barCount.LookBackBarsForSetup = lookBackBarsForSetup;
			_barCount.LookBackBarsForCountdown = lookBackBarsForCountdown;
		}
		protected override void CalcBar()
		{
            var i = _barCount[0];
		}
	}
}