using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicD
{
    public interface IBluetooth
    {
        void Connect(string device);
        void Disconnect();
        Task<uint> Send(string data);
        bool Recieve(string data);
    }
}
