/*    
    Copyright (C) Paul Falstad and Iain Sharp
    
    This file is part of CircuitJS1.

    CircuitJS1 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 of the License, or
    (at your option) any later version.

    CircuitJS1 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with CircuitJS1.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;

namespace CircuitJSharp
{
    public enum ProbeMode
    {
        TP_VOL = 0,
        TP_RMS = 1,
        TP_MAX = 2,
        TP_MIN = 3,
        TP_P2P = 4,
        TP_BIN = 5,
        TP_FRQ = 6,
        TP_PER = 7,
        TP_PWI = 8,
        TP_DUT = 9, //mark to space ratio
    }
    
    public class ProbeElm : TwoNodeElm
    {
        private static int FLAG_SHOWVOLTAGE = 1;
        protected ProbeMode meter;
        int units;
        int scale;

        private int SCALE_AUTO = 0; //TODO Find whats that

        public ProbeElm(int xx, int yy) : base (xx, yy)
        {
            meter = ProbeMode.TP_VOL;

            // default for new elements
            flags = FLAG_SHOWVOLTAGE;
            scale = SCALE_AUTO;
        }

        double rmsV = 0, total, count;
        protected double binaryLevel = 0; //0 or 1 - double because we only pass doubles back to the web page
        int zerocount = 0;
        double maxV = 0, lastMaxV;
        double minV = 0, lastMinV;
        // double frequency = 0;
        double period = 0;
        double pulseWidth = 0;
        double dutyCycle = 0;
        protected double selectedValue = 0;

        bool increasingV = true, decreasingV = true;
        long periodStart, periodLength, pulseStart; //time between consecutive max values

        Point center;

        public bool mustShowVoltage()
        {
            return (flags & FLAG_SHOWVOLTAGE) != 0;
        }

        private long currentTimeMillis()
        {
            return (long)sim.t * 1000;
        }

        public override void stepFinished()
        {
            count++; //how many counts are in a cycle
            double v = getVoltageDiff();
            total += v * v; //sum of squares

            if (v < 2.5)
                binaryLevel = 0;
            else
                binaryLevel = 1;

            //V going up, track maximum value with 
            if (v > maxV && increasingV)
            {
                maxV = v;
                increasingV = true;
                decreasingV = false;
            }

            if (v < maxV && increasingV)
            {
                //change of direction V now going down - at start of waveform
                lastMaxV = maxV; //capture last maximum 
                //capture time between
                periodLength = currentTimeMillis() - periodStart;
                periodStart = currentTimeMillis();
                period = periodLength;
                pulseWidth = currentTimeMillis() - pulseStart;
                dutyCycle = pulseWidth / periodLength;
                minV = v; //track minimum value with V
                increasingV = false;
                decreasingV = true;

                //rms data
                total = total / count;
                rmsV = Math.Sqrt(total);
                if (Double.IsNaN(rmsV))
                    rmsV = 0;
                count = 0;
                total = 0;
            }

            if (v < minV && decreasingV)
            {
                //V going down, track minimum value with V
                minV = v;
                increasingV = false;
                decreasingV = true;
            }

            if (v > minV && decreasingV)
            {
                //change of direction V now going up
                lastMinV = minV; //capture last minimum
                pulseStart = currentTimeMillis();
                maxV = v;
                increasingV = true;
                decreasingV = false;

                //rms data
                total = total / count;
                rmsV = Math.Sqrt(total);
                if (Double.IsNaN(rmsV))
                    rmsV = 0;
                count = 0;
                total = 0;
            }

            //need to zero the rms value if it stays at 0 for a while
            if (v == 0)
            {
                zerocount++;
                if (zerocount > 5)
                {
                    total = 0;
                    rmsV = 0;
                    maxV = 0;
                    minV = 0;
                }
            }
            else
            {
                zerocount = 0;
            }
        }

        public override bool getConnection(int n1, int n2)
        {
            return false;
        }

        // public EditInfo getEditInfo(int n)
        // {
        //     if (n == 0)
        //     {
        //         EditInfo ei = new EditInfo("", 0, -1, -1);
        //         ei.checkbox = new Checkbox("Show Value", mustShowVoltage());
        //         return ei;
        //     }
        //
        //     if (n == 1)
        //     {
        //         EditInfo ei = new EditInfo("Value", selectedValue, -1, -1);
        //         ei.choice = new Choice();
        //         ei.choice.add("Voltage");
        //         ei.choice.add("RMS Voltage");
        //         ei.choice.add("Max Voltage");
        //         ei.choice.add("Min Voltage");
        //         ei.choice.add("P2P Voltage");
        //         ei.choice.add("Binary Value");
        //         //ei.choice.add("Frequency");
        //         //ei.choice.add("Period");
        //         //ei.choice.add("Pulse Width");
        //         //ei.choice.add("Duty Cycle");
        //         ei.choice.select(meter);
        //         return ei;
        //     }
        //
        //     if (n == 2)
        //     {
        //         EditInfo ei = new EditInfo("Scale", 0);
        //         ei.choice = new Choice();
        //         ei.choice.add("Auto");
        //         ei.choice.add("V");
        //         ei.choice.add("mV");
        //         ei.choice.add(Locale.muString + "V");
        //         ei.choice.select(scale);
        //         return ei;
        //     }
        //
        //     return null;
        // }

        // public void setEditValue(int n, EditInfo ei)
        // {
        //     if (n == 0)
        //     {
        //         if (ei.checkbox.getState())
        //             flags = FLAG_SHOWVOLTAGE;
        //         else
        //             flags &= ~FLAG_SHOWVOLTAGE;
        //     }
        //
        //     if (n == 1)
        //     {
        //         meter = ei.choice.getSelectedIndex();
        //     }
        //
        //     if (n == 2)
        //         scale = ei.choice.getSelectedIndex();
        // }
    }
}