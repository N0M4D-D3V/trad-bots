using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class StochasticWithRSI : IndicatorObject {
		//RSI variables
		private Function.RSI rsiFunction;
        private VariableSeries<Double> rsi;
        private IPlotObject RSITrendLine;
        private IPlotObject RSIOverboughtLine;
        private IPlotObject RSIOversoldLine;

		//Stochastic variables
		private Stochastic m_stochastic1;
        private VariableSeries<Double> m_ofastk;
        private VariableSeries<Double> m_ofastd;
        private VariableObject<Double> m_oslowk;
        private VariableObject<Double> m_oslowd;
        private IPlotObject Plot1;
        private IPlotObject Plot2;
        private IPlotObject Plot3;
        private IPlotObject Plot4;
		private ISeries<double> priceh { get; set; }
        private ISeries<double> pricel { get; set; }
        private ISeries<double> pricec { get; set; }

		//RSI inputs
		[Input] public int length { get; set; }
        [Input] public double oversoldRSI { get; set; }
        [Input] public double overboughtRSI { get; set; }
        [Input] public Color overscolor { get; set; }
        [Input] public Color overbcolor { get; set; }

		//Stochastic Inputs
        [Input] public int stochlength { get; set; }
		[Input] public int smoothinglength1 { get; set; }
        [Input] public int smoothinglength2 { get; set; }
        [Input] public int smoothingtype { get; set; }
        [Input] public double oversold { get; set; }
        [Input] public double overbought { get; set; }

		public StochasticWithRSI(object _ctx):base(_ctx){
			DefaultRSIConfiguration();
			DefaultStochasticConfiguration();
		}
	
		protected override void Create() {
			CreateRSI();
			CreateStochastic();
		}

		protected override void StartCalc() {
			StartCalcRSI();
			StartCalcStochastic();
		}

		protected override void CalcBar(){
			CalcBarRSI();
			CalcBarStochastic();
		}

		//RSI functions
		private void DefaultRSIConfiguration(){
			overbcolor = Color.Red;
            overscolor = Color.Cyan;
            overboughtRSI = 70;
            oversoldRSI = 30;
            length = 14;
		}

		private void CreateRSI(){
			rsiFunction = new Function.RSI(this);
            rsi = new VariableSeries<Double>(this);
            RSITrendLine = AddPlot(new PlotAttributes("RSI", 0, Color.Silver, Color.Empty, 0, 0, true));
            RSIOverboughtLine = AddPlot(new PlotAttributes("OverBot", 0, Color.Green, Color.Empty, 0, 0, true));
            RSIOversoldLine = AddPlot(new PlotAttributes("OverSld", 0, Color.Green, Color.Empty, 0, 0, true));
		}

		private void StartCalcRSI(){
			rsiFunction.price = Bars.Close;
            rsiFunction.length = length;
		}

		private void CalcBarRSI(){
			rsi.Value = rsiFunction[0];
            RSITrendLine.Set(0, rsi.Value);
            RSIOverboughtLine.Set(0, overboughtRSI);
            RSIOversoldLine.Set(0, oversoldRSI);
            if (PublicFunctions.DoubleGreater(rsi.Value, overboughtRSI)){
                RSITrendLine.Colors[0] = overbcolor;
            }
            else{
                if (PublicFunctions.DoubleLess(rsi.Value, oversoldRSI)){
                    RSITrendLine.Colors[0] = overscolor;
                }
            }
            if (this.CrossesOver(rsi, oversoldRSI)){
                Alerts.Alert("Indicator exiting oversold zone");
            }
            else{
                if (this.CrossesUnder(rsi, overboughtRSI)){
                    Alerts.Alert("Indicator exiting overbought zone");
                }
            }
		}

		//STOCHASTIC FUNCTIONS
		private void DefaultStochasticConfiguration(){
			overbought = 70;
            oversold = 30;
            smoothingtype = 1;
            smoothinglength2 = 3;
            smoothinglength1 = 3;
            stochlength = 14;
		}

		private void CreateStochastic(){
			m_stochastic1 = new Stochastic(this);
            m_ofastk = new VariableSeries<Double>(this);
            m_ofastd = new VariableSeries<Double>(this);
            m_oslowk = new VariableObject<Double>(this);
            m_oslowd = new VariableObject<Double>(this);
            Plot1 = AddPlot(new PlotAttributes("FastK", 0, Color.Yellow, Color.Empty, 0, 0, true));
            Plot2 = AddPlot(new PlotAttributes("FastD", 0, Color.Blue, Color.Empty, 0, 0, true));
            Plot3 = AddPlot(new PlotAttributes("OverBot", 0, Color.Green, Color.Empty, 0, 0, true));
            Plot4 = AddPlot(new PlotAttributes("OverSld", 0, Color.Green, Color.Empty, 0, 0, true));
		}

		private void StartCalcStochastic(){
			priceh = Bars.High;
            pricel = Bars.Low;
            pricec = Bars.Close;
            m_stochastic1.priceh = priceh;
            m_stochastic1.pricel = pricel;
            m_stochastic1.pricec = pricec;
            m_stochastic1.stochlength = stochlength;
            m_stochastic1.length1 = smoothinglength1;
            m_stochastic1.length2 = smoothinglength2;
            m_stochastic1.smoothingtype = smoothingtype;
            m_stochastic1.ofastk = m_ofastk;
            m_stochastic1.ofastd = m_ofastd;
            m_stochastic1.oslowk = m_oslowk;
            m_stochastic1.oslowd = m_oslowd;
		}

		private void CalcBarStochastic(){
			m_stochastic1.Call();
            Plot1.Set(0, m_ofastk.Value);
            Plot2.Set(0, m_ofastd.Value);
            Plot3.Set(0, overbought);
            Plot4.Set(0, oversold);
            if (this.CrossesOver(m_ofastd, oversold)){
                Alerts.Alert("FastD exiting oversold zone");
            }
            else{
                if (this.CrossesUnder(m_ofastd, overbought)){
                    Alerts.Alert("FastD exiting overbought zone");
                }
            }
		}
	}
}