using System;
using System.Collections.Generic;

namespace CircuitJSharp
{
    public class DiodeModel : IComparable<DiodeModel>
    {
        static Dictionary<String, DiodeModel> modelMap;

        int flags;
        public String name, description;
        public double saturationCurrent, seriesResistance, emissionCoefficient, breakdownVoltage;

        // used for UI code, not guaranteed to be set
        double forwardVoltage, forwardCurrent;

        bool dumped;
        bool readOnly;
        public bool builtIn;
        public bool oldStyle;
        bool internaL;
        public const int FLAGS_SIMPLE = 1;

        // Electron thermal voltage at SPICE's default temperature of 27 C (300.15 K):
        public const double vt = 0.025865;

        // The diode's "scale voltage", the voltage increase which will raise current by a factor of e.
        public double vscale;

        // The multiplicative equivalent of dividing by vscale (for speed).
        public double vdcoef;

        // voltage drop @ 1A
        double fwdrop;

        protected DiodeModel(double sc, double sr, double ec, double bv, String d)
        {
            saturationCurrent = sc;
            seriesResistance = sr;
            emissionCoefficient = ec;
            breakdownVoltage = bv;
            description = d;
//	CirSim.console("creating diode model " + this);
//	CirSim.debugger();
            updateModel();
        }

        static DiodeModel getModelWithName(String name)
        {
            createModelMap();
            modelMap.TryGetValue(name, out var lm);
            if (lm != null)
                return lm;
            lm = new DiodeModel();
            lm.name = name;
            modelMap.Add(name, lm);
            return lm;
        }

        public static DiodeModel getModelWithNameOrCopy(String name, DiodeModel oldmodel)
        {
            createModelMap();
            modelMap.TryGetValue(name, out var lm);
            if (lm != null)
                return lm;
            if (oldmodel == null)
            {
                CirSim.console("model not found: " + name);
                return getDefaultModel();
            }

//	CirSim.console("copying to " + name);
            lm = new DiodeModel(oldmodel);
            lm.name = name;
            modelMap.Add(name, lm);
            return lm;
        }

        static void createModelMap()
        {
            if (modelMap != null)
                return;
            modelMap = new Dictionary<String, DiodeModel>();
            addDefaultModel("spice-default", new DiodeModel(1e-14, 0, 1, 0, null));
            addDefaultModel("default", new DiodeModel(1.7143528192808883e-7, 0, 2, 0, null));
            addDefaultModel("default-zener", new DiodeModel(1.7143528192808883e-7, 0, 2, 5.6, null));

            // old default LED with saturation current that is way too small (causes numerical errors)
            addDefaultModel("old-default-led", new DiodeModel(2.2349907006671927e-18, 0, 2, 0, null).setInternal());

            // default for newly created LEDs, https://www.diyaudio.com/forums/software-tools/25884-spice-models-led.html
            addDefaultModel("default-led", new DiodeModel(93.2e-12, .042, 3.73, 0, null));

            // https://www.allaboutcircuits.com/textbook/semiconductors/chpt-3/spice-models/
            addDefaultModel("1N5711", new DiodeModel(315e-9, 2.8, 2.03, 70, "Schottky"));
            addDefaultModel("1N5712", new DiodeModel(680e-12, 12, 1.003, 20, "Schottky"));
            addDefaultModel("1N34", new DiodeModel(200e-12, 84e-3, 2.19, 60, "germanium"));
            addDefaultModel("1N4004", new DiodeModel(18.8e-9, 28.6e-3, 2, 400, "general purpose"));
            //	addDefaultModel("1N3891", new DiodeModel(63e-9, 9.6e-3, 2, 0));  // doesn't match datasheet very well

            // http://users.skynet.be/hugocoolens/spice/diodes/1n4148.htm
            addDefaultModel("1N4148", new DiodeModel(4.352e-9, .6458, 1.906, 75, "switching"));
            addDefaultModel("x2n2646-emitter", new DiodeModel(2.13e-11, 0, 1.8, 0, null).setInternal());

            // for TL431
            // loadInternalModel("~tl431ed-d_ed 0 1e-14 5 1 0 0");

            // for LM317
            // loadInternalModel("~lm317-dz 0 1e-14 0 1 6.3 0");
        }

        static void addDefaultModel(String name, DiodeModel dm)
        {
            modelMap.Add(name, dm);
            dm.readOnly = dm.builtIn = true;
            dm.name = name;
        }

        public DiodeModel setInternal()
        {
            internaL = true;
            return this;
        }

// create a new model using given parameters, keeping backward compatibility.  The method we use has problems, but we don't want to
// change circuit behavior.  We don't do this anymore because we discovered that changing the leakage current to get a given fwdrop
// does not work well; the leakage currents can be way too high or low.
        public static DiodeModel getModelWithParameters(double fwdrop, double zvoltage)
        {
            createModelMap();

            double emcoef = 2;

            // look for existing model with same parameters
            foreach (var pair in modelMap)
            {
                DiodeModel dModel = pair.Value;
                if (Math.Abs(dModel.fwdrop - fwdrop) < 1e-8 && dModel.seriesResistance == 0 && Math.Abs(dModel.breakdownVoltage - zvoltage) < 1e-8 && dModel.emissionCoefficient == emcoef)
                    return dModel;
            }

            // create a new one, converting to new parameter values
            double vscale = emcoef * vt;
            double vdcoef = 1 / vscale;
            double leakage = 1 / (Math.Exp(fwdrop * vdcoef) - 1);
            String name = "fwdrop=" + fwdrop;
            if (zvoltage != 0)
                name = name + " zvoltage=" + zvoltage;
            DiodeModel dm = getModelWithName(name);
        //	CirSim.console("got model with name " + name);
            dm.saturationCurrent = leakage;
            dm.emissionCoefficient = emcoef;
            dm.breakdownVoltage = zvoltage;
            dm.readOnly = dm.oldStyle = true;
        //	CirSim.console("at drop current is " + (leakage*(Math.exp(fwdrop*vdcoef)-1)));
        //	CirSim.console("sat " + leakage + " em " + emcoef);
            dm.updateModel();
            return dm;
        }

        public static DiodeModel getDefaultModel()
        {
            return getModelWithName("default");
        }

        static List<DiodeModel> getModelList(bool zener)
        {
            List<DiodeModel> vector = new List<DiodeModel>();
            foreach (var pair in modelMap)
            {
                DiodeModel dm = pair.Value;
                if (dm.internaL)
                    continue;
                if (zener && dm.breakdownVoltage == 0)
                    continue;
                if (!vector.Contains(dm))
                    vector.Add(dm);
            }

            vector.Sort();
            return vector;
        }

        public int CompareTo(DiodeModel dm)
        {
            return String.Compare(name, dm.name, StringComparison.Ordinal);
        }

        DiodeModel()
        {
            saturationCurrent = 1e-14;
            seriesResistance = 0;
            emissionCoefficient = 1;
            breakdownVoltage = 0;
            updateModel();
        }

        DiodeModel(DiodeModel copy)
        {
            flags = copy.flags;
            saturationCurrent = copy.saturationCurrent;
            seriesResistance = copy.seriesResistance;
            emissionCoefficient = copy.emissionCoefficient;
            breakdownVoltage = copy.breakdownVoltage;
            forwardCurrent = copy.forwardCurrent;
            updateModel();
        }

// set emission coefficient for simple mode if we have enough data  
        private void setEmissionCoefficient()
        {
            if (forwardCurrent > 0 && forwardVoltage > 0)
                emissionCoefficient = (forwardVoltage / Math.Log(forwardCurrent / saturationCurrent + 1)) / vt;

            seriesResistance = 0;
        }

        private void setForwardVoltage()
        {
            if (forwardCurrent == 0)
                forwardCurrent = 1;
            forwardVoltage = emissionCoefficient * vt * Math.Log(forwardCurrent / saturationCurrent + 1);
        }

        void updateModel()
        {
            vscale = emissionCoefficient * vt;
            vdcoef = 1 / vscale;
            fwdrop = Math.Log(1 / saturationCurrent + 1) * emissionCoefficient * vt;
        }

        private bool isSimple()
        {
            return (flags & FLAGS_SIMPLE) != 0;
        }

        private void setSimple(bool s)
        {
            flags = (s) ? FLAGS_SIMPLE : 0;
        }
    }
}