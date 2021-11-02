using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public class FB_WyckoffWaveMode : FunctionSeries<System.Int32>
		{
			private double m_dPriceOld;
			private Boolean m_bmodeUp;
			
			double m_dpoints;
			Int32 m_iCounter;
			
			public double Points{
                get { return m_dpoints; }
                set { m_dpoints = value; }
            }
								
			public FB_WyckoffWaveMode(CStudyControl _master) : base(_master) 
			{
				m_dPriceOld= new Double();	
				m_bmodeUp = new Boolean();
				m_iCounter = new Int32();
			}
			
			public FB_WyckoffWaveMode(CStudyControl _master, int _ds) : base(_master, _ds) { }

			protected override void Create()
			{
				// create variable objects and function objects
			}

			protected override void StartCalc()
			{
				// assign inputs
				m_dPriceOld= Bars.Close[0];
				m_iCounter=0;
			}

			protected override System.Int32 CalcBar()
			{
				// function logic
				if(m_bmodeUp == true )
				{
					if( (m_dPriceOld - Bars.Close[1]) >= m_dpoints)
					{
						this.bucle();
						m_bmodeUp=false;	
					}
					else if (m_dPriceOld < Bars.Close[1])
					{
						m_dPriceOld = Bars.Close[1];
					}
					
				}
				else
				{
					if( (Bars.Close[1] - m_dPriceOld ) >= m_dpoints)
					{
						m_iCounter=0;
						this.bucle();
						m_bmodeUp=true;	
					}
					else if (m_dPriceOld > Bars.Close[1])
					{
						m_dPriceOld = Bars.Close[1];
					}
				}
				
							
				if(m_bmodeUp)
				{
					return (1*(m_iCounter));	
				}
				else
				{
					return (-1*m_iCounter);
				}
			}
			
			private void bucle(){
				m_iCounter=0;
				for(int i=1; i<100; i++)
					{
					m_iCounter++;
					if(Bars.Close[i] == m_dPriceOld)
						break;
					}
			}
				
		}
	}
}