﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace G_Effects
{
	/// <summary>
	/// Reads and holds configuration parameters
	/// </summary>
	public class Configuration
	{
		
		//Configurable parameters:
        public double gResistance = 300; //Expresses the ability of the blood system to resist G effectsloat 
        public float downwardGMultiplier = 1.0f; //Multiplier of a G force that pulls a kerbal downwards
        public float upwardGMultiplier = 2.0f; //Multiplier of a G force that pulls a kerbal upwards
        public float forwardGMultiplier = 0.5f; //Multiplier of a G force that pulls a kerbal forward
        public float backwardGMultiplier = 0.75f; //Multiplier of a G force that pulls a kerbal backward
        public double deltaGTolerance = 3; //The G effects start if the G value is below 1 - tolerance or above 1 + tolerance
        public float gLocStartCoeff = 1.1f; //How much more our poor kerbal should suffer after complete loss of vision to have a G-LOC
        public bool IVAOnly = false; //If set to true then g-effects will be rendered in IVA mode only
        public String gLocScreenWarning = null; //Text of a warning displayed when a kerbal loses consience. Leave empty to disable.
        public Color redoutRGB = Color.red; //Red, green, blue components of a redout color (in case you are certain that green men must have green blood, for example)
		public int gLocFadeSpeed = 4; //Speed of fade-out visual effect when a kerbal is losing conscience
		//You can disable specific sound effects by specifying 0 volumes.
		//Volumes are specified as a fraction of KSP voice volume global setting (less than 1 means quiter, greater than 1 means louder)
		public float gruntsVolume = 1.0f; //Volume of grunts when a kerbal tries to push blood back in his head on positive and frontal over-G
		public float breathVolume = 1.0f; //Volume of heavy breath when a kerbal rests after over-G
		public float heartBeatVolume = 1.0f; //Volume of blood beating in kerbal's ears on negative over-G
		public float femaleVoicePitch = 1.4f; //How much female kerbals' voice pitch is higher than males' one
		public float breathSoundPitch = 1.6f; //Pitch of heavy breath's sounds
		
		//Kerbal personal modifiers are used as multipliers for the gResistance parameter and also affect the speed of G effects accumulation  
		public float femaleModifier = 1; //How stronger are females than males
		//Modifiers by specialization traits:
		public  Dictionary<string, float> traitModifiers = new Dictionary<string, float>() {
			{"Pilot", 1.3f}, {"Engineer", 1.0f}, {"Scientist", 1.0f}, {"Tourist", 0.6f}
		};
		
		//Calculated parameters:
		public double positiveThreshold = 4;
        public double negativeThreshold = -2;
        public double MAX_CUMULATIVE_G = 10000;
        public double GLOC_CUMULATIVE_G = 11000;
		
		public Configuration()
		{
		}
		
		public void loadConfiguration(string root) {
        	ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes(root);
        	if ((nodes == null) || (nodes.Length == 0)) {
        	    return;
        	}
        	Double.TryParse(nodes[0].GetValue("gResistance"), out gResistance);
        	float.TryParse(nodes[0].GetValue("downwardGMultiplier"), out downwardGMultiplier);
        	float.TryParse(nodes[0].GetValue("upwardGMultiplier"), out upwardGMultiplier);
        	float.TryParse(nodes[0].GetValue("fowardGMultiplier"), out forwardGMultiplier);
        	float.TryParse(nodes[0].GetValue("backwardGMultiplier"), out backwardGMultiplier);
        	Double.TryParse(nodes[0].GetValue("deltaGTolerance"), out deltaGTolerance);
        	float.TryParse(nodes[0].GetValue("gLocStartCoeff"), out gLocStartCoeff);
        	        	
        	float.TryParse(nodes[0].GetValue("femaleModifier"), out femaleModifier);
        	ConfigNode traitNode = nodes[0].GetNode("TRAIT_MODIFIERS");
        	if (traitNode != null) {
        		foreach (string valueName in traitNode.values.DistinctNames()) {
        			float value;
        			if (float.TryParse(traitNode.GetValue(valueName), out value)) {
        				if (traitModifiers.ContainsKey(valueName)) {
        					traitModifiers[valueName] = value;
        				} else {
        					traitModifiers.Add(valueName, value);
        				}
    				}
        		}
        	} else KSPLog.print("" + root + " node not found");
        	
        	bool.TryParse(nodes[0].GetValue("IVAOnly"), out IVAOnly);
        	gLocScreenWarning = nodes[0].GetValue("gLocScreenWarning");
        	string redoutColor = nodes[0].GetValue("redoutRGB");
        	string[] redoutComponents;
        	if ((redoutColor != null)  && ((redoutComponents = redoutColor.Split(',')).Length == 3) ) {
        		float r, g, b;
        		if (float.TryParse(redoutComponents[0].Trim(' '), out r) &&
        		    float.TryParse(redoutComponents[1].Trim(' '), out g) &&
        		    float.TryParse(redoutComponents[2].Trim(' '), out b) ) {
        			redoutRGB.r = r / 255;
        			redoutRGB.g = g / 255;
        			redoutRGB.b = b / 255;
        		}
        	}
        	int.TryParse(nodes[0].GetValue("gLocFadeSpeed"), out gLocFadeSpeed);
        	float.TryParse(nodes[0].GetValue("gruntsVolume"), out gruntsVolume);
        	float.TryParse(nodes[0].GetValue("breathVolume"), out breathVolume);
        	float.TryParse(nodes[0].GetValue("heartBeatVolume"), out heartBeatVolume);
        	float.TryParse(nodes[0].GetValue("femaleVoicePitch"), out femaleVoicePitch);
        	float.TryParse(nodes[0].GetValue("breathSoundPitch"), out breathSoundPitch);
        	
        	positiveThreshold = 1 + deltaGTolerance;
			negativeThreshold = 1 - deltaGTolerance;
			MAX_CUMULATIVE_G = gResistance * gResistance;
			GLOC_CUMULATIVE_G = gLocStartCoeff * MAX_CUMULATIVE_G;
			
        }
		
		
		
	}
}
