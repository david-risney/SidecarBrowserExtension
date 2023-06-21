using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SidecarNativeComm
{
    // Defines the data protocol for reading and writing strings on our stream
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int b0 = ioStream.ReadByte();
            int b1 = ioStream.ReadByte();
            int b2 = ioStream.ReadByte();

            int verify = b0;
            if (verify != 0x99)
            {
                throw new Exception("Invalid verify byte: " + b0 + " " + b1 + " " + b2);
            }
            int len = b1 * 256;
            len += b2;
            if (len == 0)
            {
                return "";
            }
            else
            {
                byte[] inBuffer = new byte[len];
                ioStream.Read(inBuffer, 0, len);

                return streamEncoding.GetString(inBuffer);
            }
        }

        public int WriteString(string outString)
        {
            ioStream.WriteByte(0x99);
            if (outString.Length == 0)
            {
                ioStream.WriteByte(0);
                ioStream.WriteByte(0);
                ioStream.Flush();

                return 3;
            }
            else
            {
                byte[] outBuffer = streamEncoding.GetBytes(outString);
                int len = outBuffer.Length;
                if (len > UInt16.MaxValue)
                {
                    throw new Exception("Value too large");
                }
                ioStream.WriteByte((byte)(len / 256));
                ioStream.WriteByte((byte)(len & 0xFF));
                ioStream.Write(outBuffer, 0, len);
                ioStream.Flush();

                return outBuffer.Length + 3;
            }
        }
    }
}
