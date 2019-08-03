using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

namespace extOSC.Examples
{

    public class museRXTX2 : MonoBehaviour
    {
        #region Private Vars

        private OSCTransmitter _transmitter;

        private OSCReceiver _receiver;

        private const string _oscAddress = "/muse/*"; //"Person0/*";    // Also, you can use mask in address: /example/*/
        private const string _transmitAddress = "/muse/elements/alpha_absolute";
        private const string _transmitAddressErf = "/erf";
        public string key = "Person0/elements/alpha_absolute"; // channel name?
        public int memory = 10;
        public double weightRatio = 0.5;
        private double weightsum = 0;
        public int history_size = 300;
        public int recording_time = 900; //recording time in seconds!

        private double[] weightVals;
        private double[][] AFvals;
        private double[][] Tvals;
        private double[] means;
        private double[] std_devs;
        private int numSamps = 0;
        private double[][] history;

        private double[][] recording;
        private int present = 0;


        #endregion

        #region Unity Methods


        protected virtual void Start()
        {

            //Debug.Log("in Start()!");
            // Creating a transmitter.
            _transmitter = gameObject.AddComponent<OSCTransmitter>();

            // Set remote host address.
            _transmitter.RemoteHost = "127.0.0.1";

            // Set remote port;
            _transmitter.RemotePort = 7001;


            // Creating a receiver.
            _receiver = gameObject.AddComponent<OSCReceiver>();

            // Set local port.
            _receiver.LocalPort = 7000;

            // Bind "MessageReceived" method to special address.
            _receiver.Bind(_oscAddress, MessageReceived);

            AFvals = new double[memory][];
            Tvals = new double[memory][];
            weightVals = new double[memory];
            means = new double[4];
            std_devs = new double[4];
            history = new double[history_size][];
            recording = new double[recording_time * 10][]; //number of samples is recording time * 10 samples / sec, 4 electrodes
            for (int i = 0; i < recording.Length; i++)
            {
                recording[i] = new double[4]; //4 electrodes
            }

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
        }

        #endregion

        #region Protected Methods
        //these seem always to be zero.
        protected void MessageReceived(OSCMessage message)
        {
            //if(message.ToDouble(out double value))

            double[] contents = new double[4];
            int count = 0;
            //Debug.Log(message);
            foreach (extOSC.OSCValue d in message.Values)
            {
                if(d.DoubleValue!=0)
                print("val going in: " + d);
                contents[count] = d.FloatValue;
                count += 1;
            }

            MuseTracker(contents);
        }

        double[] smoothprocess(double[][] vals)
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



        private void store(double[] v)
        {
            string t = "";
            for (int i = 0; i < 4; i++) { t += v[i].ToString() + "\t"; }
            t += "\n";
            Debug.Log("T: " + t);
            using (System.IO.StreamWriter sw = File.CreateText("log.txt"))
            {
                sw.WriteLine(t);
            }
        }

        protected void MuseTracker(double[] v)
        {
            Debug.Log("IN MUSETRACKER");
            //            double avg = 0;

            // skip if any sensor is over 1
            // for (int i = 0; i < 4; i++) { if (v[i] > 1 || v[i] < 0) { return; } }

            if (numSamps <= history_size)
            {
                if (numSamps < history_size)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        history[numSamps][i] = v[i];
                        means[i] += v[i];
                        numSamps++;
                    }
                }
                else if (numSamps == history_size)
                {

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
                        }
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        std_devs[i] /= numSamps;
                        std_devs[i] = Math.Sqrt(std_devs[i]);
                    }
                    numSamps++;
                }
            }
            else
            {
                Debug.Log("v = " + v[0].ToString() + " " + v[1].ToString() + " " + v[2].ToString() + " " + v[3].ToString());

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
                Debug.LogFormat("std_dev 1 at line 246 is {}", std_devs[1]);
                if (AFvals[0][0] != -1)
                {
                    double[] AFweighted = smoothprocess(AFvals);
                    double[] Tweighted = smoothprocess(Tvals);

                    var message = new OSCMessage(_transmitAddress);
                    message.AddValue(OSCValue.Double(Tweighted[0]));
                    message.AddValue(OSCValue.Double(AFweighted[0]));
                    message.AddValue(OSCValue.Double(AFweighted[1]));
                    message.AddValue(OSCValue.Double(Tweighted[1]));
                    _transmitter.Send(message);

                    /* var message2 = new OSCMessage(_transmitAddressErf);

                     double left = (v[1] - means[1]) / std_devs[1];
                     double right = (v[2] - means[2]) / std_devs[2];
                     message2.AddValue(OSCValue.Double(Erf((left+right)/2)));
                     _transmitter.Send(message2);*/


                    Debug.Log("avg:" + AFweighted[0].ToString() + " " + AFweighted[1].ToString());
                    double merge = (AFweighted[0] + AFweighted[1]) / 2;

                    present++;
                }

            }



        }

        #endregion
    }
}