using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {

		public class PIDController
        {
            public readonly double dt; // i.e. 1.0 / ticks per second 
            public double min { get; set; }
            public double max { get; set; }
            public double Kp { get; set; }
            public double Ki
            {
                get { return m_Ki; }
                set { m_Ki = value; }
            }
            private double m_Ki;
            public double Ti
            {
                get { return m_Ki != 0.0 ? Kp / m_Ki : 0.0; }
                set
                {
                    if (value != 0.0)
                    {
                        Ki = Kp / value;
                    }
                    else Ki = 0.0;
                }
            }
            public double Kd
            {
                get { return m_Kd; }
                set { m_Kd = value; m_Kddt = m_Kd / dt; }
            }
            private double m_Kd, m_Kddt;
            public double Td
            {
                get { return Kd / Kp; }
                set { Kd = Kp * value; }
            }
            private double integral = 0.0;
            private double lastError = 0.0;
            public PIDController(double dt)
            {
                this.dt = dt;
                min = -1.0;
                max = 1.0;
            }
            public void Reset()
            {
                integral = 0.0;
                lastError = 0.0;
            }
            public double Compute(double error)
            {
                var newIntegral = integral + error * dt;
                var derivative = error - lastError;
                lastError = error;
                var CV = ((Kp * error) +
                (m_Ki * newIntegral) +
                (m_Kddt * derivative));
                if (CV > max)
                {
                    if (newIntegral <= integral) integral = newIntegral;
                    return max;
                }
                else if (CV < min)
                {
                    if (newIntegral >= integral) integral = newIntegral;
                    return min;
                }
                integral = newIntegral;
                return CV;
            }
        }
    }
}
