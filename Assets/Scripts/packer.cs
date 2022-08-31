using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class packer
{
    struct Message
    {
        public int ID;
        public byte[] Data;
    }
    public void Pack(byte[] Data) 
    {
        byte[] data = new byte[8];
        byte[] counts = new byte[16];
        byte[] ndata = new byte[data.Length + counts.Length + Data.Length];
        ndata.CopyTo(ndata, 0);
        counts.CopyTo(ndata, data.Length);
        counts.CopyTo(Data , counts.Length);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
