//C# Unity Useful Utils

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JLBUtils 
{
    //Calculates the percentage
    public static float percentage(float index, float min, float max)
    {
        return (index - min) / (max - min);
    }
	
    public static float percentageMapped(float index, float min, float max, float mapStart, float mapEnd)
    {
        return (mapStart + ((mapEnd - mapStart) * ((index - min) / (max - min))));
    }
    
    public static bool AreSerialPortsReal(string[] userPassedSerialPortIDs)
    {
        try
        {
            int portExists = 0;
            string[] existingSerialPorts = System.IO.Ports.SerialPort.GetPortNames();
            for (int i = 0; i < existingSerialPorts.Length; i++)
            {
                // Debug.Log("existingSerialPorts:" + existingSerialPorts[i]);
                for (int j = 0; j < userPassedSerialPortIDs.Length; j++)
                {
					
                    // Debug.Log("userinputport:" + userPassedSerialPortIDs[j]);
					
#if UNITY_STANDALONE_OSX
						string existport = existingSerialPorts[i].Substring(9);
						string userinputport = userPassedSerialPortIDs[j].Substring(8);
#endif
#if UNITY_STANDALONE_WIN
                    string existport = existingSerialPorts[i];
                    string userinputport = userPassedSerialPortIDs[j];
#endif
						
                    if (existport == userinputport)
                    {
							
                        portExists++;
                        break;
                    }
                }
            }
					
            if (portExists == userPassedSerialPortIDs.Length)
            {
                Debug.Log("Got All Ports");
                return true;
            }
            else
            {
                Debug.LogError("Catched Error: One or More Serial Ports Do Not Exist");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            return false;
        }

    }
}