using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;
namespace PowerLanguage.Strategy {
public class nfsMacdBB2 : SignalObject {


        private IPlotObject bbMacd, bbMacdFrame;
		private IPlotObject bbAvg, bbUpper, bbLower;
		private IPlotObject macdCross, macdLine, zeroLine;
		private IOrderMarket enterLong, enterShort, exitLong, exitShort;
        private Function.MACD macd;
        private VariableSeries<double> Values;
        private XAverage XFastAvg, XSlowAvg;
        private int m_fastLength = 12, m_slowLength = 26, m_bandLength = 10;
	
	
        [Input]
		public int FastLength { get; set;}
		[Input]
		public int SlowLength {get; set;}
		
		[Input]
		public int HowManyBuy{get;set;}
		[Input]
        public bool IsPositionBasis { get; set; }
		
		
		public nfsMacdBB2(object _ctx):base(_ctx)
		{
        //--------Toma los valores del input
        FastLength     = 12;
        SlowLength = 26;
	
		HowManyBuy = 10; //-----Tamaño de contratos
	    }
			
		protected override void Create() {
			// create variable objects, function objects, order objects etc.
			
			
			enterLong = OrderCreator.MarketNextBar(new 
                SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));

            enterShort = OrderCreator.MarketNextBar(new 
                SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));

            exitLong = OrderCreator.MarketNextBar(new 
                SOrderParameters(Contracts.UserSpecified, EOrderAction.Sell));

            exitShort = OrderCreator.MarketNextBar(new 
                SOrderParameters(Contracts.UserSpecified, EOrderAction.BuyToCover));
			
			
			macd = new Function.MACD(this);
			Values = new VariableSeries<double>( this );
			
			
	
		}
		protected override void StartCalc() {
			macd.Price = Bars.Close;
			macd.FastLength = FastLength;
			macd.SlowLength = SlowLength;
		}
		protected override void CalcBar(){
			
			double macdValue = macd[0];
			Values.Value = macdValue;
			double macAvgValue = Values.Average(m_bandLength);
			double sdValue = Values.StandardDeviationCustom( m_bandLength,  1 );
			
			Color dotColor = Color.Red;
			
		
			
			
			
				if (macdValue < macd[1]) {
					
					if (macdValue < macAvgValue - sdValue)
				dotColor = Color.Red;
				else
					enterShort.Send(HowManyBuy);
			}
			
					if (macdValue > macd[1]) {
				if (macdValue > macAvgValue + sdValue)
				
			dotColor = Color.Red;
				else
					exitShort.Send(HowManyBuy);
			} 
	
		}
		
	}
}