﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MuMech
{
    public class MechJebModuleAttitudeAdjustment : DisplayModule
    {
        public EditableDouble TfX;
        public EditableDouble TfY;
        public EditableDouble TfZ;
        public EditableDouble TfMin;
        public EditableDouble TfMax;
        public EditableDouble kpFactor;
        public EditableDouble kiFactor;
        public EditableDouble kdFactor;

        [Persistent(pass = (int)Pass.Global)]
        public bool showInfos = false;

        public MechJebModuleAttitudeAdjustment(MechJebCore core) : base(core) { }

        public override void OnStart(PartModule.StartState state)
        {
            TfX = new EditableDouble(core.attitude.TfV.x);
            TfY = new EditableDouble(core.attitude.TfV.y);
            TfZ = new EditableDouble(core.attitude.TfV.z);
            TfMin = new EditableDouble(core.attitude.TfMin);
            TfMax = new EditableDouble(core.attitude.TfMax);
            kpFactor = new EditableDouble(core.attitude.kpFactor);
            kiFactor = new EditableDouble(core.attitude.kiFactor);
            kdFactor = new EditableDouble(core.attitude.kdFactor);
            base.OnStart(state);
        }

        protected override void WindowGUI(int windowID)
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("Reset"))
            {
                core.attitude.ResetConfig();
                OnStart(PartModule.StartState.None);
            }

            core.GetComputerModule<MechJebModuleCustomWindowEditor>().registry.Find(i => i.id == "Toggle:AttitudeController.useSAS").DrawItem();

            if (!core.attitude.useSAS)
            {
                core.attitude.Tf_autoTune = GUILayout.Toggle(core.attitude.Tf_autoTune, " Tf auto-tuning");

                if (!core.attitude.Tf_autoTune)
                {
                    GUILayout.Label("Larger ship do better with a larger Tf");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Tf (s)", GUILayout.ExpandWidth(true));
                    GUILayout.Label("P", GUILayout.ExpandWidth(false));
                    TfX.text = GUILayout.TextField(TfX.text, GUILayout.ExpandWidth(true), GUILayout.Width(40));
                    GUILayout.Label("R", GUILayout.ExpandWidth(false));
                    TfZ.text = GUILayout.TextField(TfZ.text, GUILayout.ExpandWidth(true), GUILayout.Width(40));
                    GUILayout.Label("Y", GUILayout.ExpandWidth(false));
                    TfY.text = GUILayout.TextField(TfY.text, GUILayout.ExpandWidth(true), GUILayout.Width(40));
                    GUILayout.EndHorizontal();

                    TfX = Math.Max(0.01, TfX);
                    TfY = Math.Max(0.01, TfY);
                    TfZ = Math.Max(0.01, TfZ);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Tf", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.TfV.xzy), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Tf range");
                    GuiUtils.SimpleTextBox("min", TfMin, "", 50);
                    TfMin = Math.Max(TfMin, 0.01);
                    GuiUtils.SimpleTextBox("max", TfMax, "", 50);
                    TfMax = Math.Max(TfMax, 0.01);
                    GUILayout.EndHorizontal();
                }

                GUILayout.Label("PID factors");
                GuiUtils.SimpleTextBox("Kd = ", kdFactor, " / Tf", 50);
                kdFactor = Math.Max(kdFactor, 0.01);
                GuiUtils.SimpleTextBox("Kp = pid.Kd / (", kpFactor, " * Math.Sqrt(2) * Tf)", 50);
                kpFactor = Math.Max(kpFactor, 0.01);
                GuiUtils.SimpleTextBox("Ki = pid.Kp / (", kiFactor, " * Math.Sqrt(2) * Tf)", 50);
                kiFactor = Math.Max(kiFactor, 0.01);

                core.attitude.RCS_auto = GUILayout.Toggle(core.attitude.RCS_auto, " RCS auto mode");

                showInfos = GUILayout.Toggle(showInfos, "Show Numbers");
                if (showInfos)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Kp", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.pid.Kp.xzy), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Ki", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.pid.Ki.xzy), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Kd", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.pid.Kd.xzy), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Error", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.error * Mathf.Rad2Deg), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("prop. action.", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.pid.propAct), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("deriv. action", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.pid.derivativeAct), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("integral action.", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.pid.intAccum), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("PID Action", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.pidAction), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Axis Control ", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(core.attitude.AxisState, "F0"), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    Vector3d torque = vesselState.torqueAvailable + vesselState.torqueFromEngine * vessel.ctrlState.mainThrottle;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("torque", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(torque), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("|torque|", GUILayout.ExpandWidth(true));
                    GUILayout.Label(torque.magnitude.ToString("F3"), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    Vector3d inertia = Vector3d.Scale(
                        vesselState.angularMomentum.Sign(),
                        Vector3d.Scale(
                            Vector3d.Scale(vesselState.angularMomentum, vesselState.angularMomentum),
                            Vector3d.Scale(torque, vesselState.MoI).Invert()
                            )
                        );
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("inertia", GUILayout.ExpandWidth(true));
                    GUILayout.Label(MuUtils.PrettyPrint(inertia), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("|inertia|", GUILayout.ExpandWidth(true));
                    GUILayout.Label(inertia.magnitude.ToString("F3"), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    Vector3d ratio = Vector3d.Scale(vesselState.MoI, torque.Invert());

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("|MOI| / |Torque|", GUILayout.ExpandWidth(true));
                    GUILayout.Label(ratio.magnitude.ToString("F3"), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("fixedDeltaTime", GUILayout.ExpandWidth(true));
                    GUILayout.Label(TimeWarp.fixedDeltaTime.ToString("F3"), GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();
                }
            }

            MechJebModuleAttitudeController.useCoMVelocity = GUILayout.Toggle(MechJebModuleAttitudeController.useCoMVelocity, "Use CoM velocity instead of stock");

            MechJebModuleDebugArrows arrows = core.GetComputerModule<MechJebModuleDebugArrows>();


            GuiUtils.SimpleTextBox("Arrows length", arrows.arrowsLength, "", 50);

            arrows.seeThrough = GUILayout.Toggle(arrows.seeThrough, "Visible through object");
            GUILayout.BeginHorizontal();

            arrows.comSphereActive = GUILayout.Toggle(arrows.comSphereActive, "Display the CoM. Radius of ", GUILayout.ExpandWidth(false));
            arrows.comSphereRadius.text = GUILayout.TextField(arrows.comSphereRadius.text, GUILayout.Width(40));
            GUILayout.EndHorizontal();
            arrows.displayAtCoM = GUILayout.Toggle(arrows.displayAtCoM, "Arrows origins at the CoM");
            arrows.podSrfVelocityArrowActive = GUILayout.Toggle(arrows.podSrfVelocityArrowActive, "Pod Surface Velocity (yellow)");
            arrows.comSrfVelocityArrowActive = GUILayout.Toggle(arrows.comSrfVelocityArrowActive, "CoM Surface Velocity (green)");
            arrows.podObtVelocityArrowActive = GUILayout.Toggle(arrows.podObtVelocityArrowActive, "Pod Orbital Velocity (red)");
            arrows.comObtVelocityArrowActive = GUILayout.Toggle(arrows.comObtVelocityArrowActive, "CoM Orbital Velocity (orange)");
            arrows.forwardArrowActive = GUILayout.Toggle(arrows.forwardArrowActive, "Command Pod Forward (Navy Blue)");
            //arrows.avgForwardArrowActive = GUILayout.Toggle(arrows.avgForwardArrowActive, "Forward Avg (blue)");

            arrows.requestedAttitudeArrowActive = GUILayout.Toggle(arrows.requestedAttitudeArrowActive, "Requested Attitude (Gray)");

            arrows.debugArrowActive = GUILayout.Toggle(arrows.debugArrowActive, "Debug (Magenta)");


            GUILayout.EndVertical();

            if (!core.attitude.Tf_autoTune)
            {
                if (core.attitude.TfV.x != TfX || core.attitude.TfV.y != TfY || core.attitude.TfV.z != TfZ)
            	{
            		core.attitude.TfV.x = TfX;
            		core.attitude.TfV.y = TfY;
            		core.attitude.TfV.z = TfZ;
            		core.attitude.setPIDParameters();
            	}
            }
            else
            {
            	if (core.attitude.TfMin != TfMin || core.attitude.TfMax != TfMax)
            	{
            		core.attitude.TfMin = TfMin;
            		core.attitude.TfMax = TfMax;
            		core.attitude.setPIDParameters();
            	}
            	if (core.attitude.kpFactor != kpFactor || core.attitude.kiFactor != kiFactor || core.attitude.kdFactor != kdFactor)
            	{
            		core.attitude.kpFactor = kpFactor;
            		core.attitude.kiFactor = kiFactor;
            		core.attitude.kdFactor = kdFactor;
            		core.attitude.setPIDParameters();
            	}
            }
            base.WindowGUI(windowID);
        }

        public override GUILayoutOption[] WindowOptions()
        {
            return new GUILayoutOption[] { GUILayout.Width(300), GUILayout.Height(150) };
        }

        public override string GetName()
        {
            return "Attitude Adjustment";
        }
    }
}
