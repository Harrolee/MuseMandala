using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System;
//Copied on Aug/2/2019 from Maya Ackerman's team.
namespace extOSC.Examples
{
    public class CompressReadouts : MonoBehaviour
    {
        MGMT MGMT;

        public int TransmitPort = 5010;
        public int ReceivePort = 5000;

        #region Private Vars

        private OSCTransmitter _transmitter;

        private OSCReceiver _receiver;

        //networking

        private const string _oscAddress = "muse/elements/alpha_absolute"; 
        private const string _transmitAddress = "/muse/elements/alpha_absolute";
        private const string _transmitAddressErf = "/erf";
        //public string key = "Person0/elements/alpha_absolute"; // channel name?

        //memory
        public int memory = 10;
        public double weightRatio = 0.5;
        private double weightsum = 0;
        public int history_size = 300;

        private double[] weightVals;
        private double[][] AFvals;
        private double[][] Tvals;
        private double[] means;
        private double[] std_devs;
        private int numSamps = 0;
        private double[][] history;
        public double easiness = 0.5;
        public double fillratio = 0.5; //fill ratio is used to calculate a number between fill ratio and 1. fillratio + (1-fillratio)*cerf
        public double padding = 0.2; //padding is used to calculate the brightness.

        private int present = 0;
        private EffectsMaster effectsMaster;

        //Fixed Window Average (after implementation, consider using a sliding window avg)
        private float[] smoothedMerged;
        [SerializeField]
        int avgThreshold = 30;
        int mergedCount;
        float avgFromArray;
        float avg_total;

        #endregion

        #region Unity Methods


        protected virtual void Start()
        {
            //set MGMT
            MGMT = GameObject.FindWithTag("MGMT").GetComponent<MGMT>();

            // Creating a transmitter.
            _transmitter = gameObject.AddComponent<OSCTransmitter>();

            // Set remote host address.
            _transmitter.RemoteHost = "127.0.0.1";

            // Set remote port;
            _transmitter.RemotePort = TransmitPort;


            // Creating a receiver.
            _receiver = gameObject.AddComponent<OSCReceiver>();

            // Set local port.
            _receiver.LocalPort = ReceivePort;

            // Bind "MessageReceived" method to special address.
            _receiver.Bind(_oscAddress, MessageReceived);
            //connect to EffectMaster, which CompressReadouts
            //assumes to find on this gameobject.
            effectsMaster = GetComponent<EffectsMaster>();

            //Set Memory and Smoothing vars
            AFvals = new double[memory][];
            Tvals = new double[memory][];
            weightVals = new double[memory];
            means = new double[4];
            std_devs = new double[4];
            history = new double[history_size][];


            for (int i = 0; i < memory; i++)
            {
                AFvals[i] = new double[2];
                Tvals[i] = new double[2];
                for (int j = 0; j < 2; j++)
                {
                    AFvals[i][j] = -1;
                    Tvals[i][j] = -1;
                }

                weightVals[i] = (double)Mathf.Pow((float)weightRatio, (float)((memory - 1) - i));
                weightsum += weightVals[i];
            }
            for (int i = 0; i < history_size; i++)
            {
                history[i] = new double[4];
                for (int j = 0; j < 4; j++)
                {
                    history[i][j] = 0;
                }
            }
            //instantiate int array
            smoothedMerged = new float[50];
        }

        #endregion

        #region Protected Methods


        protected void MessageReceived(OSCMessage message)
        {
            double[] contents = new double[4];
            OSCValue[] preContents = message.GetValues(OSCValueType.Double);

            for (int ii = 0; ii < 4; ii++)
            {
                contents[ii] = preContents[ii].DoubleValue;
            }
          // Debug.LogFormat("contents 0 are: {0}", contents[0]);
            MuseTracker(contents);
        }

        double[] Smoothprocess(double[][] vals)
        {
            double[] smooth = new double[2];
            smooth[0] = 0;
            smooth[1] = 0;
            for (int i = 0; i < memory; i++)
            {
                smooth[0] += weightVals[i] * vals[i][0];
                smooth[1] += weightVals[i] * vals[i][1];
            }
            smooth[0] = smooth[0] / weightsum;
            smooth[1] = smooth[1] / weightsum;

            return smooth;
        }

        private static double Erf(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }

        private static double Phi(double x)
        {
            return 0.5 * (1 + Erf(x / Math.Sqrt(2)));
        }

        private void Store(double[] v)
        {
            string t = "";
            for (int i = 0; i < 4; i++) { t += v[i].ToString() + "\t"; }
            t += "\n";
            //Debug.Log("T: " + t);
            using (System.IO.StreamWriter sw = File.CreateText("log.txt"))
            {
                sw.WriteLine(t);
            }
        }

        protected void MuseTracker(double[] v)
        {
            // skip if any sensor is over 1
            //for (int i = 0; i < 4; i++) { if (v[i] > 1 || v[i] < 0) { return; } }


            //Filling history with samples from the users. 
            //It is currently set to take 75 samples. 
            //(history_size is set to 300, there are four entries per each sample, 300/4 = 75)
            if (numSamps <= history_size)
            {
                if (numSamps == 1)
                {
                    Debug.Log("starting sampling!");
                }
                if (numSamps == history_size / 2)
                {
                    Debug.Log("halfway done w/ samps!");
                }
                if (numSamps < history_size)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        history[numSamps][i] = v[i];
                        means[i] += v[i];
                        numSamps++;
                        print("numSampls: " + numSamps);
                    }
                }
                else if (numSamps == history_size)
                {
                    Debug.Log("sampling finished!");
                    StartCoroutine(MGMT.BeginSequence());
                    //AvgArray runs in self-modulating increments
                    StartCoroutine(TimeWindow());
                    for (int i = 0; i < history_size; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            means[j] += history[i][j];
                        }
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        means[i] /= history_size;
                    }
                    for (int i = 0; i < history_size; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            std_devs[j] += Math.Pow(history[i][j] - means[j], 2);
                            //Debug.LogFormat("step before, {0} {1}", j, std_devs[j]);
                        }
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        std_devs[i] /= numSamps;
                        std_devs[i] = Math.Sqrt(std_devs[i]);
                        //Debug.LogFormat("std devs[{0}]:{1}", i, std_devs[i]);
                    }           
                    numSamps++;
                }
            }
            else
            {
                //Debug.Log("v = " + v[0].ToString() + " " + v[1].ToString() + " " + v[2].ToString() + " " + v[3].ToString());

                //shift each entry in memory to the left by one.
                for (int i = 0; i < memory - 1; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        AFvals[i][j] = AFvals[i + 1][j];
                        Tvals[i][j] = Tvals[i + 1][j];
                    }
                }

                //then normalize by mean and std deviation and put the newest value in the last position
                AFvals[memory - 1][0] = (v[1] - means[1]) / std_devs[1];
                AFvals[memory - 1][1] = (v[2] - means[2]) / std_devs[2];
                Tvals[memory - 1][0] = (v[0] - means[0]) / std_devs[0];
                Tvals[memory - 1][1] = (v[3] - means[3]) / std_devs[3];

                if (AFvals[0][0] != -1)
                {
                    double[] AFweighted = Smoothprocess(AFvals);
                    double[] Tweighted = Smoothprocess(Tvals);

                    double left = (v[1] - means[1]) / std_devs[1];
                    double right = (v[2] - means[2]) / std_devs[2];
                    //Debug.Log("std_devs[1]: " + std_devs[1] + "std_devs[2]: " +std_devs[2]);
                    double merge = (1 - Phi((left + right) * Math.Sqrt(easiness) / 2));
                    double augmented_merge = fillratio + (1 - fillratio) * merge;
                    //Debug.Log("CERF:" + merge.ToString());
                    //Debug.Log("AugMerge: " + augmented_merge.ToString());
                    if(augmented_merge.ToString() == "NaN")
                    {
                        Debug.Log("Either Right adjust the circlet OR Restart Muse Direct");
                    }
                    
                    mergedArray.Add(Convert.ToSingle(augmented_merge));
                    mergedCount++;                    
                }
            }
        }

        List<float> mergedArray = new List<float>();
        float windowLength = 5;
        float windowSum;
        float windowAverage;
        float signalsPerSec;
        int msgCount;
        IEnumerator TimeWindow()
        {
            Debug.Log("Time Window Started");
            yield return new WaitForSeconds(windowLength);
            msgCount = mergedArray.Count();
            List<float> slice = mergedArray;
            foreach (float val in slice){
                windowSum += val;
            }
            mergedArray.Clear();
            windowAverage = windowSum / msgCount;
            //signalsPerSec
            //given the num of signals per second, 
            //we can modulate the window length.
            //Goal: allow the user to recognize how their
            //focus affects the fog.

            signalsPerSec = msgCount / windowLength;
            
            if (msgCount > windowLength * signalsPerSec)
            {
                //Debug.LogFormat("reset windowLength to {0}", windowLength);
                windowLength = msgCount / signalsPerSec;
            }
            //Debug.LogFormat("windowSum: {0} \n windowAvg: {1} \n signalsPerSec: {2}", windowSum, windowAverage, signalsPerSec);
            //send val to the fan
            effectsMaster.GiveFeedback(Convert.ToSingle(windowAverage));
            //clear the variable. The others are set at each run.
            windowSum = 0;
            StartCoroutine(TimeWindow());
        }

        public void StopCoro() { print("stopped coroutine");  StopAllCoroutines(); }

        #endregion
    }
}