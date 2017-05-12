using UnityEngine;
using System.Collections;
using Pb;
using System.IO;
using Google.Protobuf;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Position p = new Position();
        p.X = 5;
        p.Y = 6;

        using (var output = File.Create("s.dat"))
        {
            p.WriteTo(output);
        }

        /*byte[] bt = { 0, 0 };

        MemoryStream ms = new MemoryStream(bt);
        Position p;
        using (var input = File.OpenRead("s.dat"))
        {
            p = Position.Parser.ParseFrom(input);
            
        }

        Debug.Log(p.X + "  " + p.Y);*/
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
