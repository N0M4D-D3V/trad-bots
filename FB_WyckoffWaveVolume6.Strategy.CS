//LEER TODAS LAS INSTRUCCIONES ANTES DE COMENZAR
//LEER TODAS LAS INSTRUCCIONES ANTES DE COMENZAR
//LEER TODAS LAS INSTRUCCIONES ANTES DE COMENZAR                         --------
//LEER TODAS LAS INSTRUCCIONES ANTES DE COMENZAR                         | LEER |
//LEER TODAS LAS INSTRUCCIONES ANTES DE COMENZAR                         --------
//LEER TODAS LAS INSTRUCCIONES ANTES DE COMENZAR
//LEER TODAS LAS INSTRUCCIONES ANTES DE COMENZAR
//----------------------------------------------------------------------------------------------------------------------
//                                           INSTRUCCIONES DE USO                                                      |
//----------------------------------------------------------------------------------------------------------------------
//-------------------------------------
// FUNCIONA SOLO CON EL S&P500 E-MINI |
//-------------------------------------
//
//1.COLOCAR EL MERCADO EN 5 MINUTOS
//2.COLOCAR HORARIO COMERCIAL EN HORARIO DE APERTURA (CREADO PREVIAMENTE EN QUOTEMANAGER - EN "TOOLS" SESSION TEMPLATES)
//SI NO ESTÁ CREADO:
//----------------------------------------------------
//OPEN   |  TIME  | CLOSE  |  TIME   | SESSION END   |
//----------------------------------------------------
//MONDAY    01:00  MONDAY    22:00   |  CHECKBOX= ON |
//TUESDAY   00:00  TUESDAY   22:00   |  CHECKBOX= ON |
//WEDNESDAY 00:00  WEDNESDAY 22:00   |  CHECKBOX= ON |
//THURSDAY  00:00  THURSDAY  22:00   |  CHECKBOX= ON |
//FRIDAY    00:00  FRIDAY    21:00   |  CHECKBOX= ON |
//----------------------------------------------------
//
//3.INTRODUCIR SEÑAL : FB_WyckoffWaveVolume5(ESTE) Y FB_WyckoffWaveVolume3, DANDOLOE CLICK DERECHO EN LA GRÁFICA
//4.PONER EL MERCASO EN "ASync" O "SA" EN EL LADO SUPERIOR IZQUIERDA DE LA GRAFICA
//5.ACTIVAR ROBOT EN SA (PASO 3).

//******************************************************************************************
//******************************************************************************************
//******************************************************************************************
//------------------------------
//                       NOTA: |
//---------------------------------------------------------------------
// CADA VEZ QUE SE AUMENTE DE TAMAÑO DE CONTRATO EL "HowManyBuy",     |
//SE TIENE QUE AUMENTAR PROPORCIONALMENTE 1:1 EL PROFIT TARGET, ESTE: |
//--------------------------------------------------------------------------------------------------
//CurSpecOrdersMode = IsPositionBasis ? ESpecOrdersMode.PerPosition : ESpecOrdersMode.PerContract; |
//GenerateProfitTarget(2100);                                                                      |
//--------------------------------------------------------------------------------------------------
// BUSCARLO CON CTRL+F |
//----------------------
//------------------------------------------------------------------------FIN---------------------------------------------
using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;
namespace PowerLanguage.Strategy {
[ CalcAtOpenNextBar ( true )]
[SameAsSymbol(true), UpdateOnEveryTick(false)]
public class FB_WyckoffWaveVolume6 : SignalObject {
	
	
	
//-----------------------------------------------------------------Comieza el código------------------------------------------------//	
	
	    private SwingHigh m_swinghigh1;
        private SwingLow m_swinglow1;
        private VariableSeries<double> m_swingprice;
	private Int64 m_counterBars;
	private Boolean m_bUp;
//********************************************************************************************-TRENDLINE2	
	    private SwingHigh m_swinghigh2;
        private SwingLow m_swinglow2;
        private VariableSeries<double> m_swingprice2;
	private Int64 m_counterBars2;
	private Boolean m_bUp2;
	
//----------------------------------------------------Aqui va la logica	
	private ISeries<double> m_volume;
	private VariableSeries<double> m_WyckoffVolume;
	private Double m_Volume;
	private Double m_VolumeTemp;
        private VariableSeries<DateTime> m_swingtime;
	private Boolean m_bswingLow;
        private VariableObject<int> m_tldir;
        private VariableObject<Boolean> m_saveswing;
        private VariableObject<Boolean> m_addtl;
        private VariableObject<Boolean> m_updatetl;
//*************************************************************************************************** LOGICA 2
	    private ISeries<Double> m_volume2;
	private VariableSeries<Double> m_WyckoffVolume2;
	private Double m_Volume2;
	private Double m_VolumeTemp2;
        private VariableSeries<DateTime> m_swingtime2;
	private Boolean m_bswingLow2;
        private VariableObject<int> m_tldir2;
        private VariableObject<Boolean> m_saveswing2;
        private VariableObject<Boolean> m_addt2;
        private VariableObject<Boolean> m_updatet2;

//----------------------------------------------------Aquí va la compra y venta	
	
	private IOrderMarket enterLong, enterShort, exitLong, exitShort;
	
//------------------------------------------------Aqui va la Linea de Tendencia
	
	private FB_WyckoffWaveMode m_wyckoffmode;
	    private IPlotObject power_line;
        private VariableObject<ITrendLineObject>m_tlref;
	private ISeries<double> price { get; set; }
	private bool alreadyCalculated;
//****************************************************************************************** LINEA2
	
	private FB_WyckoffWaveMode m_wyckoffmode2;
	    private IPlotObject power_line2;
        private VariableObject<ITrendLineObject> m_tlref2;
	private ISeries<double> price2 { get; set; }
	private bool alreadyCalculated2;
	
	//----------------------------------------------	Aqui va el Cálculo MacD
 
	private Function.MACD macd;
        private VariableSeries<double> Values;
        private XAverage XFastAvg, XSlowAvg;
        private int m_fastLength = 12, m_slowLength = 26, m_bandLength = 5;
	
	
	//-----------------------------------------------Aqui va el Cálculo EMA	
        private XAverage EMA;
        private VariableSeries<double> lowestClose, highestClose, emaValues;
	
	//------------------------------------------------Cálculo PIVOT	
	//-------Pivots ara entrar en largos.
	private PivotLowVS m_PivotLowVS;
        private ISeries<double> LPrice { get; set; }
	private IOrderPriced m_PivRevLE;
	
	
        private SwingHigh m_SwingHigh;
        private VariableObject<Boolean> m_Setup;
        private VariableObject<double> m_PivotHigh;
	
	 //---------------------------Pivots para 	
	private SwingLow m_SwingLow;
        private VariableObject<double> m_PivotLow;
	
	//------Pivot para cerrar cortos.
	private PivotHighVS m_PivotHighVS;
        private ISeries<double> HPrice { get; set; } 
	
    //------RSI
	
	private RSI m_RSI;
        private VariableSeries<double> m_myrsi;
	
	//------Stochastic
	
	private Stochastic m_Stochastic;
        private VariableSeries<double> m_oFastK;
        private VariableSeries<double> m_oFastD;
        private VariableSeries<double> m_oSlowK;
        private VariableSeries<double> m_oSlowD;
	
	
	//-----Bollinger Largo
	
        private VariableSeries<double> m_LowerBand;
	
	//-----	Bollinger Bajo
	
	private VariableSeries<double> m_HigherBand;
	
	//-----	Triángulo	
	
	private VariableSeries<ITrendLineObject>lineAtoB, lineAtoC, lineBtoC;
	
	
	
	
	
//-----Valores ¨Por defecto----------------------

	
public FB_WyckoffWaveVolume6 (object _ctx):base(_ctx) 
{ 
	
//--------TRENDLINE	
HowManyBuy = 1;
linewidth = 1;
linecolor = Color.Yellow;
retracepnts = 6;
//----------MACD	
FastLength = 4;
SlowLength = 30;
//------------EMA	
EMALength      = 8;
LookbackPeriod = 10;
//----------TrendPower	
fastLength = 7;
slowLength = 14;	
strongLevel = 0.4;	
//----------- ProfitTarget
IsPositionBasis = true;	
//***************************TRENDLINE  2
linewidth2 = 1;
linecolor2 = Color.Red;
retracepnts2 = 4;
//---------Pivot	
RightStrength = 4;
LeftStrength = 4;
Strength = 4;
//-----RSI
OverSold = 30;
LengthRSI = 14;	
OverBought = 70;	
//----- Stochastic
 OverSold2 = 20;
OverBought2 = 80;	
LengthSTOCHASTIC= 14;	
//-------Bollinger Largo
LengthBollinger = 20;
NumDevsDn = 2;	
//------Bollinger BAJO
NumDevsUp = 2;
}





//----Crear Inputs-------------------------------	
[Input]
public int HowManyBuy{get;set;}
[Input]
public double retracepnts { get; set; }
[Input]
public Color linecolor { get; set; }
[Input]
public double linewidth { get; set; }
[Input]
public int FastLength { get; set;}
[Input]
public int SlowLength {get; set;}
[Input]
public int EMALength { get; set; }
[Input]
public int LookbackPeriod { get; set; }
[Input]
public bool IsPositionBasis { get; set; }
[Input]
public int fastLength { get; set; }
[Input]
public int slowLength { get; set; }
[Input]
public double strongLevel { get; set; }	
[Input]
public double Amount { get; set; }

//*********************************************************** TRENDLINE2
[Input]
public double retracepnts2 { get; set; }
[Input]
public Color linecolor2 { get; set; }
[Input]
public double linewidth2 { get; set; }

[Input]
public int Strength { get; set; }

//-------------PivotLargo
[Input]
public int LeftStrength { get; set; }
[Input]
public int RightStrength { get; set; }

//--------------RSI
[Input]
public int LengthRSI { get; set; }
[Input]
public double OverSold { get; set; }
[Input]
public double OverBought { get; set; }

//--------------Stochastic
[Input]
public int LengthSTOCHASTIC { get; set; }
[Input]
public double OverSold2 { get; set; }
[Input]
public double OverBought2 { get; set; }

//------------- Bollinger Largo
[Input]
public int LengthBollinger { get; set; }
[Input]
public double NumDevsDn { get; set; }

//------------- Bollinger Bajo
[Input]
public double NumDevsUp { get; set; }


	     private double AvgVal( int length ){
	 double aval = 0.0;
	 for (int i = 0; i < length; ++i)
	 aval += Bars.AvgPrice(i);
	 return aval / length;
	 }	
	
	
	private int TrendDir(){
        if ((Bars.Low[0] < Bars.Low[1])&&(Bars.High[0] < Bars.High[1])) return -1;
        if ((Bars.Low[0] > Bars.Low[1])&&(Bars.High[0] > Bars.High[1])) return 1;
        return 0;
    }	

    protected override void Create() {
		lineAtoB = new VariableSeries<ITrendLineObject>(this);
		lineAtoC = new VariableSeries<ITrendLineObject>(this);	
		lineBtoC = new VariableSeries<ITrendLineObject>(this);	
	
//--------------------------------------Bollinger Bajo
	
		m_HigherBand = new VariableSeries<Double>(this);
	
//--------------------------------------Bollinger Largo
	
		m_LowerBand = new VariableSeries<Double>(this);
	
//--------------------------------------Stochastic	
	
		m_Stochastic = new Stochastic(this);
        m_oFastK = new VariableSeries<Double>(this);
        m_oFastD = new VariableSeries<Double>(this);
        m_oSlowK = new VariableSeries<Double>(this);
        m_oSlowD = new VariableSeries<Double>(this);

//--------------------------------------RSI	
	    m_RSI = new RSI(this);
        m_myrsi = new VariableSeries<Double>(this);	
	
//--------------------------------------Pivot Corto 1	
	
	    m_SwingLow = new SwingLow(this);
        m_Setup = new VariableObject<bool>(this);
        m_PivotLow = new VariableObject<double>(this);	
	
//-------------------------------------	Pivot Corto 2	
		m_PivotHighVS = new PivotHighVS(this);	
	
//------------------------------------	
	    m_SwingHigh = new SwingHigh(this);
        m_Setup = new VariableObject<bool>(this);
        m_PivotHigh = new VariableObject<double>(this);	
	
//------------------------Pivot para largos	
		m_PivotLowVS = new PivotLowVS(this);	
	
//-----------Ordenes de entrada	
	
		enterLong = OrderCreator.MarketNextBar(new 
        SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
        enterShort = OrderCreator.MarketNextBar(new 
        SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
        exitLong = OrderCreator.MarketNextBar(new 
        SOrderParameters(Contracts.UserSpecified, EOrderAction.Sell));
        exitShort = OrderCreator.MarketNextBar(new 
        SOrderParameters(Contracts.UserSpecified, EOrderAction.BuyToCover));
	       
	
//-----------Funcion MacD	
	
        macd = new Function.MACD(this);
        Values = new VariableSeries<double>( this );
	
//------------------------	Funcion EMA	
	
	
		lowestClose  = new VariableSeries<double>(this);
        highestClose = new VariableSeries<double>(this);
        emaValues    = new VariableSeries<double>(this);
		EMA = new XAverage(this);	
	
	
	
	    m_Volume = new Double();
		m_VolumeTemp = new Double();
        m_swinghigh1 = new SwingHigh(this);
        m_swinglow1 = new SwingLow(this);
        m_swingprice = new VariableSeries<Double>(this);
        m_swingtime = new VariableSeries<DateTime>(this);
        m_tldir = new VariableObject<int>(this);
        m_saveswing = new VariableObject<Boolean>(this);
        m_addtl = new VariableObject<Boolean>(this);
        m_updatetl = new VariableObject<Boolean>(this);
        m_tlref = new VariableObject<ITrendLineObject>(this);
		m_bswingLow = new Boolean();
		m_WyckoffVolume = new VariableSeries<Double>(this);
		m_counterBars = new Int64();
		m_wyckoffmode = new Function.FB_WyckoffWaveMode(this);
		m_bUp = new Boolean();
//******************************************************************************** TRENDLINE 2	
	 	m_Volume2 = new Double();
		m_VolumeTemp2 = new Double();
        m_swinghigh2 = new SwingHigh(this);
        m_swinglow2 = new SwingLow(this);
        m_swingprice2 = new VariableSeries<Double>(this);
        m_swingtime2 = new VariableSeries<DateTime>(this);
        m_tldir2 = new VariableObject<int>(this);
        m_saveswing2 = new VariableObject<Boolean>(this);
        m_addt2 = new VariableObject<Boolean>(this);
        m_updatet2 = new VariableObject<Boolean>(this);
        m_tlref2 = new VariableObject<ITrendLineObject>(this);
		m_bswingLow2 = new Boolean();
		m_WyckoffVolume2 = new VariableSeries<Double>(this);
		m_counterBars2 = new Int64();
		m_wyckoffmode2 = new Function.FB_WyckoffWaveMode(this);
		m_bUp2 = new Boolean();	
	} // cierre de create() 
	
    double old_power;
    protected override void StartCalc() {
	    old_power = 0;
	
	
		price = Bars.Close;
		m_volume = Bars.TrueVolume();
        m_swinghigh1.instance = 1;
        m_swinghigh1.price = price;
        m_swinghigh1.strength = 1;
        m_swinghigh1.length = 2;
        m_swinglow1.instance = 1;
        m_swinglow1.price = price;
        m_swinglow1.strength = 1;
        m_swinglow1.length = 2;
        m_swingprice.DefaultValue = price[0];
        m_swingtime.DefaultValue = Bars.Time[0];
		m_wyckoffmode.Points = retracepnts;
	
//***************************************************************TRENDLINE 2
		price2 = Bars.Close;
		m_volume2 = Bars.TrueVolume();
        m_swinghigh2.instance = 1;
        m_swinghigh2.price = price2;
        m_swinghigh2.strength = 1;
        m_swinghigh2.length = 2;
        m_swinglow2.instance = 1;
        m_swinglow2.price = price2;
        m_swinglow2.strength = 1;
        m_swinglow2.length = 2;
        m_swingprice2.DefaultValue = price2[0];
        m_swingtime2.DefaultValue = Bars.Time[0];
		m_wyckoffmode2.Points = retracepnts2;	
//**************************************************************MACD	
		macd.Price = Bars.Close;
	    macd.FastLength = FastLength;
	    macd.SlowLength = SlowLength;
	
//**************************************************************EMA	
		EMA.Length = EMALength;
        EMA.Price = Bars.Close;
	
//*************************************************************RECALCULAR VOLUMEN	
		VolumeProfile.EChanged += VolumeProfileOnEChanged;
	
//*******************************Pivot para salir de los cortos 1 	
	
	    HPrice = Bars.High;
        m_PivotHighVS.instance = 1;
        m_PivotHighVS.price = HPrice;
        m_PivotHighVS.leftstrength = LeftStrength;
        m_PivotHighVS.rightstrength = RightStrength;
        m_PivotHighVS.length = RightStrength + 1;	
	
//*******************************Pivot para salir de los cortos 2
	
		m_SwingLow.instance = 1;
        m_SwingLow.price = Bars.Low;
        m_SwingLow.strength = Strength;
        m_SwingLow.length = Strength + 1;	
	
//*******************************************RSI	

        m_RSI.price = price2;
        m_RSI.length = LengthRSI;	
	
//****************************************** STOCHASTIC
	
		m_Stochastic.priceh = Bars.High;
        m_Stochastic.pricel = Bars.Low;
        m_Stochastic.pricec = Bars.Close;
        m_Stochastic.stochlength = LengthSTOCHASTIC;
        m_Stochastic.length1 = 3;
        m_Stochastic.length2 = 3;
        m_Stochastic.smoothingtype = 1;
        m_Stochastic.ofastk = m_oFastK;
        m_Stochastic.ofastd = m_oFastD;
        m_Stochastic.oslowk = m_oSlowK;
        m_Stochastic.oslowd = m_oSlowD;	
	
	} // cierre startCalc()
	


 	protected override void CalcBar(){
	
	        LPrice = Bars.Low;
            m_PivotLowVS.instance = 1;
            m_PivotLowVS.price = LPrice;
            m_PivotLowVS.leftstrength = LeftStrength;
            m_PivotLowVS.rightstrength = RightStrength;
            m_PivotLowVS.length = RightStrength + 1;
	
	
//---------------------------------Funcion MacD	
             double macdValue = macd[0];
             Values.Value = macdValue;
             double macAvgValue = Values.Average(m_bandLength);
             double sdValue = Values.StandardDeviationCustom( m_bandLength,  1 );	
	
	
//----------------------------------Funcion Ema	
	
	        lowestClose.Value  = Bars.Close.Lowest(LookbackPeriod, 1);
            highestClose.Value = Bars.Close.Highest(LookbackPeriod, 1);
            emaValues.Value    = EMA[0];
	
//-------------------------------------------------- LO Que permite que funcione entrar con la TrendLine 	
  
            if ( Environment.ApplicationCode != EApplicationCode.Charting)
            	return;
	
//----------------------------------------------------	LO Que permite que obtenga valores del volumen	
	
			double curVolume = Bars.VolumeValue; 	

//---------------------------------------------Fuerza del impulso	
	
			double afast = AvgVal(fastLength);
            double aslow = AvgVal(slowLength);
            double power = Math.Abs(100*(afast - aslow)/Bars.Close[0]);	
	
//---------------------------------------------Manejo de entrada por el MACD
	
	
			Color dotColor = Color.Red;
	
//-------------------------------------------- Entrada RSI Largo
	
	 		m_myrsi.Value = m_RSI[0];
	
//--------------------------LÓGICA
	
	
			var m_newswingprice2 = m_swinghigh2[0];	
			var m_newswingprice = m_swinghigh1[0];
			m_counterBars++;
			
			if ((m_newswingprice != (-1)))
			{
				if ((PublicFunctions.DoubleLessEquals(m_tldir.Value, 0) 
					&&
					PublicFunctions.DoubleGreaterEquals(m_newswingprice, (m_swingprice.Value + retracepnts))))
				{
					m_counterBars=0;
					m_saveswing.Value = true;
					m_addtl.Value = true;
					m_tldir.Value = 1;
					m_bswingLow = false;
					
					if((macdValue > macd[8])
						&&(macdValue < macAvgValue - sdValue))
					{
						enterLong.Send(HowManyBuy);
					}	
                }
                else
				{
                    if (((m_tldir.Value == 1)
                        && PublicFunctions.DoubleGreaterEquals(m_newswingprice, m_swingprice.Value)))
						{
                        m_saveswing.Value = true;
                        m_updatetl.Value = true;	
                    	}
                }
            }
            else
			{
                m_newswingprice = m_swinglow1[0];
                if ((m_newswingprice != (-1)))
				{
					m_bswingLow=true;
					if ((PublicFunctions.DoubleGreaterEquals(m_tldir.Value, 0) 
						&& PublicFunctions.DoubleLessEquals(m_newswingprice, (m_swingprice.Value - retracepnts))))
					{
						m_counterBars=0;
                        m_saveswing.Value = true;
                        m_addtl.Value = true;
                        m_tldir.Value = (-1);
	
                    }
                	else
					{
                        if (((m_tldir.Value == (-1)) 
							&& PublicFunctions.DoubleLessEquals(m_newswingprice, m_swingprice.Value)))
						{
                        	m_saveswing.Value = true;
                       	 	m_updatetl.Value = true;
                        }
                    }
                }
			}
	
//--------------------------------------------------------	LOGICA	TRENDLINE	        
            if (m_saveswing.Value)
			{
                m_swingprice.Value = m_newswingprice;
                m_swingtime.Value = Bars.Time[1];
                m_saveswing.Value = false;
	
	    	}
	
	
            if (m_addtl.Value)
			{	
                m_tlref.Value = DrwTrendLine.Create(new ChartPoint(m_swingtime.Value, m_swingprice.Value),
                                                    new ChartPoint(m_swingtime[1], m_swingprice[1]));
                m_tlref.Value.ExtLeft = false;
                m_tlref.Value.ExtRight = false;
                m_tlref.Value.Size = ((int) (linewidth));
                m_tlref.Value.Color = linecolor;
				m_addtl.Value = false;	
	
            }
			else
			{
                if (m_updatetl.Value)
				{	
                    m_tlref.Value.End = new ChartPoint(m_swingtime.Value, m_swingprice.Value);
                    m_updatetl.Value = false;
	
                }
            }  
	
//----------------------------------------------- LOGICA Del Volumen

	        m_WyckoffVolume.Value=m_counterBars;//m_Volume;
	    	//Plot1.Set(0,m_swinghigh1.Value );/
	    	if(m_wyckoffmode.Value!= m_wyckoffmode[2])
	    	{
				m_bUp=true;
	
			}	
			if(m_wyckoffmode.Value<0)
			{
				m_bUp= false;	
			}	

//******************************************************************************************	
//******************************************************************************************
//******************************************************************************************
//******************************************************************************************
//************************AQUI VA LA TRENDLINE MAS LENTA ***********************************
//******************************************************************************************
//******************************************************************************************
//******************************************************************************************
//******************************************************************************************


			m_WyckoffVolume2.Value=m_counterBars2;//m_Volume;
			//Plot1.Set(0,m_swinghigh1.Value );/
			if(m_wyckoffmode2.Value!= m_wyckoffmode2[2])
			{
				m_bUp2=true;
			}	
			if(m_wyckoffmode2.Value<0)
			{
				m_bUp2= false;	
			}
	
//*************************************	


			m_counterBars2++;
			if ((m_newswingprice2 != (-1))) 
			{
				if ((PublicFunctions.DoubleLessEquals(m_tldir2.Value,0) 
					&& PublicFunctions.DoubleGreaterEquals(m_newswingprice2, (m_swingprice2.Value - retracepnts2))))
				{
					m_counterBars2=0;
					m_saveswing2.Value = true;
					m_addt2.Value = true;
					m_tldir2.Value = 1;
					m_bswingLow2 = false;
	
	    			if((curVolume>=20000)
						&&(m_wyckoffmode.Value!= m_wyckoffmode[1])
	    				&&(macdValue < macAvgValue - sdValue))
					{
						enterLong.Send(HowManyBuy);
					}	
	
                }
                else
                {
                    if (((m_tldir2.Value == 1)
                        && PublicFunctions.DoubleGreaterEquals(m_newswingprice2, m_swingprice2.Value)))
					{
                        m_saveswing2.Value = true;
                        m_updatet2.Value = true;	
	
                    }
                }
            }
            else
            {
                m_newswingprice2 = m_swinglow2[0];
                if ((m_newswingprice2 != (-1)))
				{
				m_bswingLow2=true;
	
					if ((PublicFunctions.DoubleGreaterEquals(m_tldir2.Value, 0) 
					&& PublicFunctions.DoubleLessEquals(m_newswingprice2, (m_swingprice2.Value - retracepnts2))))
					{
						m_counterBars2=0;
                        m_saveswing2.Value = true;
                        m_addt2.Value = true;
                        m_tldir2.Value = (-1);
	
                    }
                	else
	    			{
                        if (((m_tldir2.Value == (-1)) 
							&& PublicFunctions.DoubleLessEquals(m_newswingprice2, m_swingprice2.Value)))
						{
                        m_saveswing2.Value = true;
                        m_updatet2.Value = true;
                        }
                    }
                }
            } 	
	
	
	
	
//**************************************************************LOGICA	TRENDLINE 2	        
            if (m_saveswing2.Value)
			{
                m_swingprice2.Value = m_newswingprice2;
                m_swingtime2.Value = Bars.Time[1];
                m_saveswing2.Value = false;
	
	    	}
			if (m_addt2.Value)
			{	
                m_tlref2.Value = DrwTrendLine.Create(new ChartPoint(m_swingtime2.Value, m_swingprice2.Value),
                                                    new ChartPoint(m_swingtime2[1], m_swingprice2[1]));
                m_tlref2.Value.ExtLeft = false;
                m_tlref2.Value.ExtRight = false;
                m_tlref2.Value.Size = ((int) (linewidth2));
                m_tlref2.Value.Color = linecolor2;
				m_addt2.Value = false;	
			}
            else
			{
            	if ((m_updatet2.Value))
				{
                	m_tlref2.Value.End = new ChartPoint(m_swingtime2.Value, m_swingprice2.Value);
                	m_updatet2.Value = false;
	
					if((curVolume>=20000)
						&&(m_wyckoffmode2.Value!= m_wyckoffmode2[1])
						&&(macdValue < macd[5])&&(macdValue < macAvgValue + sdValue))
					{
						enterLong.Send(HowManyBuy);
					}
					if((m_wyckoffmode2.Value!= m_wyckoffmode2[1])
						&&(macdValue < macd[5])
						&&(macdValue > macAvgValue + sdValue))
					{
                		enterLong.Send(HowManyBuy);
					}
                }
            }  

//**************************************************************************************	
//**************************************************************************************	
//-**********************************ACCESOS RÁPIDOS************************************	
//**************************************************************************************	
//**************************************************************************************
	
//Color dotColor = Color.Red;	
//bool downBar = (Bars.Close [ 0 ] <Bars.Open [ 0 ]);
//bool lowerOpen = (Bars.Open [ 1 ] <Bars.Close [ 0 ]);	
	
	
//if((curVolume>=29000)&&(macdValue < macAvgValue)&&(macdValue < macd[5])&&(m_PivotHighVS[0] > 0)
//&&(Bars.Close[1] < emaValues[5])&&(Bars.Close[1] <= emaValues[5])&& (m_bUp))&&(m_PivotLowVS[0] > 0)	
	
//int _DirectionClosedTrades = -1;
//int _idx = 1;

//if (Positions.Count > _idx && Positions[_idx].ClosedTrades.Count > 0)
//{
//IMarketPosition _lastPos = Positions[_idx];

//if (_lastPos.ClosedTrades[0].IsLong){
//_DirectionClosedTrades = 1;
//enterShort.Send(HowManyBuy);
///}
//else{
//_DirectionClosedTrades = 2;
	//enterLong.Send(HowManyBuy);
//}
//}	
	
	
	
//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
	
	
			int _DirectionClosedTrades = -1;
			int _idx = 1;	
	

	
			if((m_wyckoffmode2.Value!= m_wyckoffmode2[2])
				&& (m_bUp)&&(Bars.Close[0] < emaValues[1])
				&&(Bars.Close[1] <= emaValues[1])
				&&(curVolume>=15000)
				|| (m_wyckoffmode.Value<0)
				&&(m_PivotHighVS[0] > 0)
				&&(macdValue > macAvgValue + sdValue)
				&&(CurrentPosition.MaxRunUp<=1)
				|| (m_wyckoffmode.Value<0)
				&&(m_PivotHighVS[0] > 0)
				&&(Bars.Close[0] <= emaValues[1])
				&&(curVolume<=8000))
			{
				exitShort.Send(HowManyBuy);	
			}	
	

			if((Bars.Close[0] >= emaValues[1])
				&&(m_wyckoffmode.Value<0)
				&&(macdValue < macAvgValue - sdValue)
				&&(m_PivotLowVS[0] > 0))
			{
				enterShort.Send(HowManyBuy);	
			}

//*************************************************************************************

			if((m_tldir.Value == (-1))
				&&(Bars.Close[0] >= emaValues[1])
				&&(macdValue < macAvgValue - sdValue)&&(macdValue > macd[5])
				&&(m_PivotHighVS[0] > 0))
			{
				enterShort.Send(HowManyBuy);	
			}

//**********************************************************************************

			if((Bars.CurrentBar > 1)
				&&(macdValue > macd[5])
				&&(macdValue > macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[5])
				&&(m_wyckoffmode2.Value<0)
				&&(m_PivotLowVS[0] > 0))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterShort.Send(HowManyBuy);	
				}
			}

//********************************************************************************
//Aquí

			if ((macdValue > macd[5])
				&&(macdValue > macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[5])
				&&(m_tldir2.Value == (-1)))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterShort.Send(HowManyBuy);	
				}
			}

//*********************************************************************************

			if (macdValue > macd[1]) 
			{
				if ((macdValue > macAvgValue - sdValue))
					dotColor = Color.Red;
				else if((Bars.Close[0] >= emaValues[1])
					&&(m_wyckoffmode2.Value!= m_wyckoffmode2[1])
					&&(m_PivotHighVS[0] > 0))
				{
					enterShort.Send(HowManyBuy);	
				}
			}

//******************************************************************************

			if (macdValue < macd[5]) 
			{
				if (macdValue < macAvgValue + sdValue)
					dotColor = Color.Red;
				else if ((this.CrossesOver(m_myrsi,OverBought))
					&&(m_PivotLowVS[0] > 0)
					&&(macdValue > macAvgValue - sdValue))
				{
					enterShort.Send(HowManyBuy);	
				}
			}

//*******************************************************************************

			if((m_PivotLowVS[0] > 0)
				&&(macdValue < macAvgValue + sdValue))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterShort.Send(HowManyBuy);	
				}
			}

//*******************************************************************************

			if((m_tldir.Value == (-1)))
			{
				if (this.CrossesOver(m_myrsi,OverBought)
					&&(m_PivotLowVS[0] > 0))
				{
					enterShort.Send(HowManyBuy);	
				}
			}

//*****************************************************************************

			m_Stochastic.Call();
			if (Bars.CurrentBar > 2
				&&(macdValue < macd[5])
				&&(m_wyckoffmode.Value!= m_wyckoffmode[1])
				&&(Bars.Close[0] >= emaValues[5])
				&& m_oSlowK.CrossesOver(m_oSlowD, ExecInfo.MaxBarsBack)
				&& PublicFunctions.DoubleLess(m_oSlowK.Value, OverSold))
			{
				enterShort.Send(HowManyBuy);	
			}

//***************************************************************************

			m_Stochastic.Call();
			if (Bars.CurrentBar > 2
				&&(macdValue < macd[8])
				&&(m_wyckoffmode2.Value<0)
				&&(Bars.Close[0] >= emaValues[5])
				&&(Bars.Close[0] <= emaValues[8])
				&& m_oSlowK.CrossesOver(m_oSlowD, ExecInfo.MaxBarsBack)
				&& PublicFunctions.DoubleLess(m_oSlowK.Value, OverSold2))
			{
				enterShort.Send(HowManyBuy);	
			}
//**************************************************************************



//***************************************************************************

			m_HigherBand.Value = Bars.Close.BollingerBandCustom(LengthBollinger, NumDevsUp);
			if ((Bars.CurrentBar > 1)
				&&(Bars.Close[0] >= emaValues[1])
				&&(m_wyckoffmode2.Value<0)
				&&(macdValue < macd[5]) 
				&& (Bars.Close.CrossesUnder(m_HigherBand, ExecInfo.MaxBarsBack)))
			{
				enterShort.Send(HowManyBuy);
			}

//***************************************************************************

			if ((macdValue < macd[8]))
			{
				if (macdValue < macAvgValue + sdValue)
					dotColor = Color.Red;
				else if((m_wyckoffmode.Value<0)
						&& (Bars.Close.CrossesUnder(m_HigherBand, ExecInfo.MaxBarsBack)))
				{
					enterShort.Send(HowManyBuy);	
				}
			}

//*************************************************************************** 

			m_Stochastic.Call();
			if( m_oSlowK.CrossesOver(m_oSlowD, ExecInfo.MaxBarsBack)
				&& PublicFunctions.DoubleLess(m_oSlowK.Value, OverBought)
				&& (Bars.Close.CrossesUnder(m_HigherBand, ExecInfo.MaxBarsBack)))
			{
				enterShort.Send(HowManyBuy);	
			}

//***************************************************************************

 			m_Stochastic.Call();
			if ((Bars.CurrentBar > 1)
				&& (Bars.Close.CrossesUnder(m_HigherBand, ExecInfo.MaxBarsBack))
				&&(m_PivotHighVS[0] > 0)
				&& m_oSlowK.CrossesOver(m_oSlowD, ExecInfo.MaxBarsBack)&&(macdValue > macAvgValue + sdValue)
				&& PublicFunctions.DoubleLess(m_oSlowK.Value, OverBought2))
			{
				enterShort.Send(HowManyBuy);	
			}

//***************************************************************************

			if ((macdValue < macd[1]))
			{
				if (macdValue > macAvgValue + sdValue)
					dotColor = Color.Red;
				else if((Bars.Close[0] <= emaValues[8])
					&&(m_wyckoffmode.Value<0)&&(power>0.15)
					&&(m_tldir.Value == (1))	
					&&(Bars.CurrentBar > 2)
					&&(Bars.Close.CrossesOver(m_LowerBand, ExecInfo.MaxBarsBack)))
				{
					enterShort.Send(HowManyBuy);	
				}
			}

//********************************************************************************

			if((curVolume>=85000)
				&&(Bars.CurrentBar== Bars.Close[0]))
			{
				exitLong.Send(HowManyBuy);
			}
			if((curVolume>=100000)
				&&(curVolume<=110000))
			{
				exitShort.Send(HowManyBuy);
			}

//**************************************************************************************

			if((curVolume>=150000)
				&&(Bars.CurrentBar== Bars.Close[0]))
			{
				exitLong.Send(HowManyBuy);
				exitShort.Send(HowManyBuy);
			}

//**************************************************************************************

			if((power>=0.3)
				&&(power<=0.7))
			{
				exitLong.Send(HowManyBuy);
			}

//**************************************************************************************

			CurSpecOrdersMode = IsPositionBasis ? ESpecOrdersMode.PerPosition : ESpecOrdersMode.PerContract;
			GenerateProfitTarget(50);
			GenerateStopLoss(200);
//*******************
// Para 2 contratos: GenerateProfitTarget(4200);
// Para 3 contratos: GenerateProfitTarget(6300);
// Para 4 contratos: GenerateProfitTarget(8400);
// Para 5 contratos: GenerateProfitTarget(10500);
//etc ...
//*******************
//También el StopLoss
//*******************
// Para 2 contratos: GenerateStopLoss(2000);
// Para 3 contratos: GenerateStopLoss(3000);
// Para 4 contratos: GenerateStopLoss(4000);
// Para 5 contratos: GenerateStopLoss(5000);
//etc ...
//*******************
//*******************
//*******************
			
			
//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
//*******************************LARGO**************************************************
//**************************************************************************************

			if (macdValue > macd[1]) 
			{
				if (macdValue > macAvgValue + sdValue)
					dotColor = Color.Red;
				else if((Bars.Close[0] >= emaValues[5])
					&&(Bars.Close[0] <= emaValues[8])
					&&(m_wyckoffmode2.Value!= m_wyckoffmode2[2])
					&&(m_PivotHighVS[0] > 0))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*******************************************************************************

			if((Bars.CurrentBar > 1)
				&&(macdValue > macd[5])
				&&(macdValue < macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[6])
				&&(m_wyckoffmode.Value!= m_wyckoffmode[2]))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*****************************************************************************

			if ((Bars.CurrentBar > 1)
				&&(macdValue > macd[1])
				&&(macdValue < macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[6])
				&&(m_wyckoffmode2.Value!= m_wyckoffmode2[2]))
			{
				if (this.CrossesUnder(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);
				}
			}

//*****************************************************************************

			if((Bars.CurrentBar > 1)
				&&(macdValue < macd[8])
				&&(macdValue < macAvgValue + sdValue)
				&&(m_wyckoffmode2.Value<0)
				&&(m_PivotHighVS[0] > 0))
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*****************************************************************************


			if((Bars.CurrentBar > 1)
				&&(macdValue > macd[1])
				&&(macdValue < macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[5])
				&&(m_wyckoffmode.Value!= m_wyckoffmode[2]))
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*****************************************************************************

			if ((macdValue > macd[1])
				&&(macdValue < macAvgValue - sdValue)
				&&(m_PivotHighVS[0] > 0))
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*****************************************************************************

			if ((macdValue < macd[8])
				&&(macdValue > macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[6])
				&&(Bars.Close[0] <= emaValues[8])
				&&(m_tldir2.Value == (-1)))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			}
//********************************************************************************

			if ((macdValue < macd[5])
				&&(macdValue > macAvgValue - sdValue)
				&&(Bars.Close[0] >= emaValues[1])
				&&(Bars.Close[0] <= emaValues[8])
				&&(m_wyckoffmode2.Value!= m_wyckoffmode2[2]))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			} 

//******************************************************************************

			if((macdValue > macd[5])
				&&(macdValue < macAvgValue - sdValue))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//******************************************************************************

			if((macdValue > macd[5])
				&&(macdValue < macAvgValue - sdValue))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//******************************************************************************

			if((m_tldir2.Value == (-1))
				&&(macdValue < macAvgValue - sdValue)
				&&(m_wyckoffmode2.Value!= m_wyckoffmode2[1]))
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//******************************************************************************
			if((m_tldir2.Value == (-1))
				&&(macdValue > macAvgValue - sdValue)
				&&(m_wyckoffmode2.Value!= m_wyckoffmode2[1]))
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}


//*******************************LARGO**************************************************
			if((curVolume>=500)&&(curVolume<=800)&&(m_wyckoffmode2.Value!= m_wyckoffmode2[1])
				|| (curVolume>=300)&&(curVolume<=800)&&(macdValue < macAvgValue)&&(Bars.Close[0] > emaValues[1])&&(Bars.Close[1] <= emaValues[5])
				|| (curVolume>=500)&&(curVolume<=800)&&(macdValue > macAvgValue)&&(Bars.Close[1] > emaValues[1])&&(Bars.Close[1] <= emaValues[5])
				|| (m_wyckoffmode2.Value<0)&&(Bars.Close[1] > emaValues[1])&&(Bars.Close[1] >= emaValues[5])&&(CurrentPosition.MaxRunUp>=1)
				|| (m_wyckoffmode2.Value<0)&&(Bars.Close[1] > emaValues[1])&&(Bars.Close[0] <= emaValues[8])&&(CurrentPosition.MaxRunUp<=1000)&& (m_bUp)
				|| (m_wyckoffmode.Value<0)&&(macdValue > macAvgValue + sdValue)&&(m_PivotLowVS[0] > 0)&&(Bars.Close[0] <= emaValues[1])&&(Bars.Close[1] < emaValues[1]))
			{
				exitLong.Send(HowManyBuy);
			}



///***********************************************************************************

			if(
				(Bars.Close[0] > emaValues[5])
				&&(Bars.Close[0] <= emaValues[8])
				&&(m_wyckoffmode.Value!= m_wyckoffmode[1])
				&&(m_PivotHighVS[0] > 0)
				&&(CurrentPosition.MaxRunUp<=1)
				&&(curVolume<=20000)
				|| (Bars.Close[1] > emaValues[1])
				&&(Bars.Close[0] <= emaValues[1])
				&&(m_wyckoffmode.Value!= m_wyckoffmode[2])
				&&(macdValue > macAvgValue)
				&&(power>0.11)
				&&(CurrentPosition.MaxRunUp<=1)
				&&(curVolume<=20000)
				)
			{
				enterLong.Send(HowManyBuy);
			}	

//************************************************************************************

			if((m_wyckoffmode.Value<0)
				&&(Bars.Close[1] > emaValues[5])
				&&(Bars.Close[0] <= emaValues[1])
				&&(curVolume>=20000)
				&&(macdValue > macAvgValue - sdValue)
				&&(macdValue > macd[8])
				||(curVolume>=30000)
				&&(macdValue < macAvgValue - sdValue)
				&&(m_PivotHighVS[0] > 0)
				&& (Bars.Close[0] > emaValues[5])
				)
			{
				enterLong.Send(HowManyBuy);
			}

//************************************************************************************
	
			if(
				(curVolume>=50000)
				&&(macdValue > macAvgValue + sdValue)
				&&(macdValue > macd[8])
				&&(Bars.Close[1] > emaValues[1])
				&&(Bars.Close[0] >= emaValues[8])
				&& (m_wyckoffmode.Value!= m_wyckoffmode[1])
			)
			{
				enterLong.Send(HowManyBuy);
			}	
//*************************************************************************************

			if(	
				(m_wyckoffmode.Value<0)
				&&(curVolume>=15000)
				&&(macdValue < macAvgValue - sdValue)
				&&(Bars.Close[0] >= emaValues[8])
				&&(m_PivotLowVS[0] > 0)	
			)
			{
				enterLong.Send(HowManyBuy);	
			}

//************************************************************************************

			if(
				(curVolume>=20000)
				&&(macdValue < macAvgValue + sdValue)
				&&(macdValue > macd[5])
				&&(Bars.Close[1] > emaValues[1])
				&&(Bars.Close[1] < emaValues[5])
				|| (curVolume>=30000)
				&&(m_PivotHighVS[0] > 0)&&(macdValue > macd[1])
				&& (Bars.Close[0] < emaValues[8])
				&&(m_wyckoffmode2.Value<0)
				|| (curVolume>=30000)
				&&(m_PivotLowVS[0] < 0)
				&&(macdValue > macAvgValue + sdValue)
				&& (Bars.Close[0] >= emaValues[5])	
				&&(macdValue < macd[5])
				&&(m_wyckoffmode.Value<0)	
			)
			{
				enterLong.Send(HowManyBuy);
			}


//**************************************************************************************

			if ((m_wyckoffmode2.Value<0)
				&&(m_tldir.Value == (-1))
				&&(curVolume>=20000)
				&&(m_PivotLowVS[0] > 0)
				&&(macdValue < macAvgValue - sdValue)	
			)
			{	
				enterLong.Send(HowManyBuy);
			}

//*************************************************************************************

			if(
				(m_tldir.Value == 1)
				&&(curVolume>=30000)
				&&(Bars.Close[0] >= emaValues[1])
				&&(macdValue < macAvgValue - sdValue)
				&&(macdValue > macd[8])
			)
			{
				enterLong.Send(HowManyBuy);	
			}

//************************************************************************************
			if(
				(m_tldir.Value == 1)
				&&(curVolume>=10000)
				&&(Bars.Close[0] <= emaValues[5])
				&&(macdValue < macAvgValue - sdValue)
				&&(macdValue > macd[5])
				&&(m_PivotHighVS[0] > 0))
			{
				enterLong.Send(HowManyBuy);	
			}
//**********************************************************************************

			if(
				(m_tldir.Value == 1)
				&&(curVolume>=1000)
				&&(curVolume<=19000)
				&&(Bars.Close[0] >= emaValues[5])
				&&(macdValue < macAvgValue - sdValue)
				&&(macdValue > macd[5])&&(m_PivotLowVS[0] > 0))
			{
				enterLong.Send(HowManyBuy);	
			}


//***********************************************************************************

			if(
				(m_tldir.Value == 1)
				&&(curVolume>=20000)
				&& (m_wyckoffmode.Value!= m_wyckoffmode[1])
				&&(m_PivotLowVS[0] > 0))
			{
				enterLong.Send(HowManyBuy);	
			}
//**********************************************************************************


			if(
				(m_tldir.Value == 1)
				&& (m_wyckoffmode2.Value!= m_wyckoffmode2[2])
				&&(m_PivotHighVS[0] > 0)
				&&(m_PivotLowVS[0] > 0))
			{
				enterLong.Send(HowManyBuy);	
			}

//*******************************************************************************

			if (macdValue > macd[1]) 
			{
				if (macdValue > macAvgValue + sdValue)
					dotColor = Color.Red;
				else if(
					(Bars.Close[0] >= emaValues[5])
					&&(Bars.Close[0] <= emaValues[8])
					&&(m_wyckoffmode2.Value!= m_wyckoffmode2[2])
					&&(m_PivotHighVS[0] > 0))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*******************************************************************************

			if(
				(Bars.CurrentBar > 1)
				&&(macdValue > macd[5])
				&&(macdValue < macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[6])
				&&(m_wyckoffmode.Value!= m_wyckoffmode[2]))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*****************************************************************************

 			if (
				(Bars.CurrentBar > 1)
				&&(macdValue > macd[1])
				&&(macdValue < macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[6])
				&&(m_wyckoffmode2.Value!= m_wyckoffmode2[2]))
			{
				if (this.CrossesUnder(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);
				}
			}

//*****************************************************************************

			if(
				(Bars.CurrentBar > 1)
				&&(macdValue < macd[8])
				&&(macdValue < macAvgValue + sdValue)
				&&(m_wyckoffmode2.Value<0)
				&&(m_PivotHighVS[0] > 0))
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*****************************************************************************


			if(
				(Bars.CurrentBar > 1)
				&&(macdValue > macd[1])
				&&(macdValue < macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[5])
				&&(m_wyckoffmode.Value!= m_wyckoffmode[2])
				)
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*****************************************************************************

			if (
				(macdValue > macd[1])
				&&(macdValue < macAvgValue - sdValue)
				&&(m_PivotHighVS[0] > 0))
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*****************************************************************************

			if (
				(macdValue < macd[8])
				&&(macdValue > macAvgValue + sdValue)
				&&(Bars.Close[0] >= emaValues[6])
				&&(Bars.Close[0] <= emaValues[8])
				&&(m_tldir2.Value == (-1)))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			}
			
//********************************************************************************

			if (
				(macdValue < macd[5])
				&&(macdValue > macAvgValue - sdValue)
				&&(Bars.Close[0] >= emaValues[1])
				&&(Bars.Close[0] <= emaValues[8])
				&&(m_wyckoffmode2.Value!= m_wyckoffmode2[2]))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			} 

//******************************************************************************

			if(
				(macdValue > macd[5])
				&&(macdValue < macAvgValue - sdValue)
				)
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//******************************************************************************

			if(
				(macdValue > macd[5])
				&&(macdValue < macAvgValue - sdValue))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//******************************************************************************

			if(
				(m_tldir2.Value == (-1))
				&&(macdValue < macAvgValue - sdValue)
				&&(m_wyckoffmode2.Value!= m_wyckoffmode2[1])
				)
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
				enterLong.Send(HowManyBuy);	
				}
			}

//******************************************************************************
			if(
				(m_tldir2.Value == (-1))
				&&(macdValue > macAvgValue - sdValue)
				&&(m_wyckoffmode2.Value!= m_wyckoffmode2[1])
				)
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*****************************************************************************

			if(
				(m_tldir.Value == (-1))
				&&(curVolume>=50000)
				)	
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}	
			}

//*****************************************************************************

			if(
				(m_tldir2.Value == (-1))
				&&(macdValue < macd[1])
				&&(m_wyckoffmode.Value!= m_wyckoffmode[1])
				)	
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}	
			}

//*****************************************************************************

			m_Stochastic.Call();
			if (Bars.CurrentBar > 1
				&&(m_wyckoffmode.Value!= m_wyckoffmode[1])
				&&(macdValue < macd[5])
				&& m_oSlowK.CrossesOver(m_oSlowD, ExecInfo.MaxBarsBack)
				&&PublicFunctions.DoubleLess(m_oSlowK.Value, OverSold2))
			{
				if (this.CrossesOver(m_myrsi,OverSold))
				{
					enterLong.Send(HowManyBuy);	
				}	
			}

//****************************************************************************

			m_Stochastic.Call();
			if (
				(Bars.CurrentBar > 2)
				&&(macdValue < macd[5])
				&&(m_wyckoffmode.Value<0)
				&&(Bars.Close[0] >= emaValues[6])
				&&(Bars.Close[0] <= emaValues[8])
				&& m_oSlowK.CrossesOver(m_oSlowD, ExecInfo.MaxBarsBack)
				&& PublicFunctions.DoubleLess(m_oSlowK.Value, OverSold))
			{
				enterLong.Send(HowManyBuy);	
			}


//***************************************************************************

			m_Stochastic.Call();
			if (
				(Bars.CurrentBar > 2)
				&&(m_wyckoffmode.Value<0)
				&&(macdValue > macd[8])
				&& m_oSlowK.CrossesOver(m_oSlowD, ExecInfo.MaxBarsBack)
				&& PublicFunctions.DoubleLess(m_oSlowK.Value, OverSold))
			{
				if (this.CrossesOver(m_myrsi,OverSold2))
				{
					enterLong.Send(HowManyBuy);	
				}	
			}

//**************************************************************************


			m_Stochastic.Call();
			if (
				(Bars.CurrentBar > 2)
				&&(m_wyckoffmode.Value!= m_wyckoffmode[1])
				&&(macdValue < macd[8])
				&& m_oSlowK.CrossesUnder(m_oSlowD, ExecInfo.MaxBarsBack)
				&& PublicFunctions.DoubleGreater(m_oSlowK.Value, OverBought2))
			{
				enterLong.Send(HowManyBuy);
			}

//*******************************************************************************
			m_Stochastic.Call();
			if(
				(m_PivotHighVS[0] > 0)
				&&(macdValue > macAvgValue - sdValue)
				&& m_oSlowK.CrossesOver(m_oSlowD, ExecInfo.MaxBarsBack)
				&& PublicFunctions.DoubleLess(m_oSlowK.Value, OverBought2))
			{
				if (this.CrossesOver(m_myrsi,OverBought))
				{
					enterLong.Send(HowManyBuy);	
				}
			}


//*************************************************************************

			if (Bars.CurrentBar > 1 
				&& Bars.Close.CrossesOver(m_LowerBand, ExecInfo.MaxBarsBack)
				&&(m_wyckoffmode.Value!= m_wyckoffmode[1])
				&&(macdValue > macd[1])
				&&(m_tldir2.Value == (1))
				&&(macdValue < macAvgValue - sdValue)
				&&(CurrentPosition.MaxRunUp<=1)
				)
			{
				enterLong.Send(HowManyBuy);
			}

//*************************************************************************

			if (Bars.CurrentBar > 1 
				&& Bars.Close.CrossesOver(m_LowerBand, ExecInfo.MaxBarsBack)
				&&(m_wyckoffmode.Value!= m_wyckoffmode[1])
				&&(macdValue < macd[1])
				&&(m_tldir2.Value == (1))
				&&(macdValue < macAvgValue + sdValue)
				&&(CurrentPosition.MaxRunUp>=1))
			{
				enterLong.Send(HowManyBuy);
			}



//*************************************************************************

			if ((macdValue < macd[1]))
			{
				if (macdValue > macAvgValue - sdValue)
					dotColor = Color.Red;
				else if(
					(Bars.Close[0] <= emaValues[8])
					&&(curVolume>=10000)
					&&(m_wyckoffmode2.Value!= m_wyckoffmode2[2])
					&&(m_tldir2.Value != (-1))	
					&& (Bars.Close.CrossesOver(m_LowerBand, ExecInfo.MaxBarsBack)))
				{
					enterLong.Send(HowManyBuy);	
				}
			}

//*************************************************************************

			if ((macdValue < macd[1]))
			{
				if (macdValue > macAvgValue - sdValue)
					dotColor = Color.Red;
				else if(
					(Bars.Close[0] <= emaValues[8])
					&&(curVolume>=10000)
					&&(m_wyckoffmode.Value!= m_wyckoffmode[2])
					&&(m_tldir2.Value != (-1))	
					&& (Bars.Close.CrossesOver(m_LowerBand, ExecInfo.MaxBarsBack)))
				{
					enterLong.Send(HowManyBuy);	
				}
			}


//***************************************************************************

			if  (
				(Bars.CurrentBar > 1)
				&& (Bars.Close.CrossesUnder(m_HigherBand, ExecInfo.MaxBarsBack))
				&&(m_wyckoffmode2.Value!= m_wyckoffmode2[2])
				&&(m_tldir2.Value == (1))
				&&(macdValue < macd[1])
				&&(macdValue > macAvgValue - sdValue)&&(power>0.1))
			{
				enterLong.Send(HowManyBuy);
			}

//********************************este***********************************************


//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
//**************************************************************************************
//***************************TRIÁNGULO**************************************************
//**************************************************************************************



	
	
            // Define the triangle's points
            ChartPoint pointA = new ChartPoint(
            Bars.Time[30], Bars.Close[30]);

            ChartPoint pointB = new ChartPoint(
            Bars.Time[Bars.High.HighestBar(20)], 
            Bars.High.Highest(20));

            ChartPoint pointC = new ChartPoint(
            Bars.Time[0], Bars.Close[0]);

            
            if (
				(m_PivotHighVS[0] > 0)
				&&(lineAtoB != null)
				&&(power>0.1))
            {
            	lineAtoB.Value = DrwTrendLine.Create(pointA, pointB);
				lineAtoB.Value.Size = ((int) (linewidth2));
            	lineAtoB.Value.Color = Color.Transparent;
            }
			
            if (
				(m_PivotHighVS[0] > 0)
				&&(lineAtoB != null)&&(power>0.1))
            {
            	lineBtoC.Value  = DrwTrendLine.Create(pointB, pointC);
				lineBtoC.Value.Size = ((int) (linewidth2));
            	lineBtoC.Value.Color = Color.Transparent;
            }
			
            if (
				(m_PivotHighVS[0] > 0)
				&&(lineAtoB != null)
				&&(power>0.1))
            {
            	lineAtoC.Value  = DrwTrendLine.Create(pointA, pointC);
				lineAtoC.Value.Size = ((int) (linewidth2));
            	lineAtoC.Value.Color = Color.Transparent;
	
				if(
					(m_wyckoffmode.Value!= m_wyckoffmode[2])
					&&(macdValue > macd[1])
					&&(macdValue < macAvgValue + sdValue)) 
				{
	    			enterLong.Send(HowManyBuy);	
            	}
            }
	
//**************************************************************************************            
	
			 if (
				(lineAtoB != null)
				&&(power>0.1)
				)
		    {
		        lineAtoC.Value  = DrwTrendLine.Create(pointA, pointC);
				lineAtoC.Value.Size = ((int) (linewidth2));
		        lineAtoC.Value.Color = Color.Transparent;
			
				if(
					(m_wyckoffmode.Value!= m_wyckoffmode[1])
					&&(macdValue > macd[5])
					&&(macdValue < macAvgValue - sdValue)
					&&(m_tldir2.Value == (-1))
				) 
				{
					enterLong.Send(HowManyBuy);	
		        }
		    }
	
//**************************************************************************************
	
			if (
				(lineAtoB != null)
				&&(power>0.1)
			)
	        {
		        lineAtoC.Value  = DrwTrendLine.Create(pointA, pointC);
				lineAtoC.Value.Size = ((int) (linewidth2));
		        lineAtoC.Value.Color = Color.Transparent;
			
				if(
					(m_wyckoffmode.Value<0)
					&&(macdValue < macd[8])
					&&(Bars.Close[0] >= emaValues[5])
					&&(macdValue > macAvgValue + sdValue)
					&&(m_tldir2.Value == (-1))
				) 
				{
			    	enterLong.Send(HowManyBuy);	
		        }
		    }
	
//**************************************************************************************
	
			if (
				(lineAtoB != null)
				&&(m_PivotHighVS[0] > 0)
			)
	        {
		        lineAtoC.Value  = DrwTrendLine.Create(pointA, pointC);
				lineAtoC.Value.Size = ((int) (linewidth2));
		        lineAtoC.Value.Color = Color.Transparent;
			
				if(
					(m_wyckoffmode2.Value!= m_wyckoffmode2[1])
					&&(macdValue > macd[8])
					&&(Bars.Close[0] <= emaValues[8])
					&&(macdValue > macAvgValue - sdValue)
					&&(CurrentPosition.MaxRunUp>=1)
					&&(m_tldir.Value != (-1))) 
				{
			    	enterLong.Send(HowManyBuy);	
		        }
		    }


//&&(power>0.118)&&(CurrentPosition.MaxRunUp<=2)&&(m_tldir2.Value == (-1))&&*************************
//*************************
//*************************
			StrategyInfo.SetPlotValue(1, lowestClose[0]);
			StrategyInfo.SetPlotValue(2, highestClose[0]);
			StrategyInfo.SetPlotValue(3, emaValues[0]);	
	}	
	
	private void VolumeProfileOnEChanged(bool full)
	{
		if (full) this.ExecControl.Recalculate();
	}
//exitLong.Send(HowManyBuy);
//exitShort.Send(HowManyBuy);
//enterLong.Send(HowManyBuy);
//enterShort.Send(HowManyBuy);
}
}