using System;
using System.Linq;
using UnityEngine;

public class NewEmptyCSharpScript
{
    public class MACAddress
    {
        private readonly byte[] _addressBytes = new byte[6];
        private byte[] bytes;

        public MACAddress(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public static MACAddress Parse(string address)
        {
            var parts = address.Split(':');
            return new MACAddress(parts.Select(p => Convert.ToByte(p, 16)).ToArray());
        }

        public bool IsBroadcast() => _addressBytes.All(b => b == 0xFF);
    }
}
