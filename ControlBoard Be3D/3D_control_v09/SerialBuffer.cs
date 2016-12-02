using System;
using System.IO.Ports;

namespace _3D_control_v09
{
    public class SerialBuffer
    {
        private readonly System.Text.Decoder _decoder = System.Text.Encoding.UTF8.GetDecoder();
        private byte[] _buffer;
        private int _startIndex = 0;
        private int _endIndex = 0;
        private char[] _charBuffer;

        public SerialBuffer(int initialSize)
        {
            _buffer = new byte[initialSize];
            _charBuffer = new char[256];
        }

        public void LoadSerial(SerialPort port)
        {
            lock (port)
            {
                int bytesToRead = port.BytesToRead;
                if (_buffer.Length < _endIndex + bytesToRead) // do we have enough buffer to hold this read?
                {
                    // if not, look and see if we have enough free space at the front
                    if (_buffer.Length - DataSize >= bytesToRead)
                    {
                        ShiftBuffer();
                    }
                    else
                    {
                        // not enough room, we'll have to make a bigger buffer
                        ExpandBuffer(DataSize + bytesToRead);
                    }
                }
                //Debug.Print("serial buffer load " + bytesToRead + " bytes read, " + DataSize + " buffer bytes before read");
                port.Read(_buffer, _endIndex, bytesToRead);

                //port.DiscardInBuffer();   // velký problem s příjmem dat když je aktivováno

                _endIndex += bytesToRead;
            }
        }

        private void ShiftBuffer()
        {
            // move the data to the left, reclaiming space from the data already read out
            Array.Copy(_buffer, _startIndex, _buffer, 0, DataSize);
            _endIndex = DataSize;
            _startIndex = 0;
        }

        private void ExpandBuffer(int newSize)
        {
            byte[] newBuffer = new byte[newSize];
            Array.Copy(_buffer, _startIndex, newBuffer, 0, DataSize);
            _buffer = newBuffer;
            _endIndex = DataSize;
            _startIndex = 0;
        }

        public byte[] Buffer
        {
            get { return _buffer; }
        }

        public int DataSize
        {
            get { return _endIndex - _startIndex; }
        }

        public string ReadLine()
        {
            lock (_buffer)
            {
                int lineEndPos = Array.IndexOf(_buffer, '\n', _startIndex, DataSize);
                    // HACK: not looking for \r, just assuming that they'll come together        
                if (lineEndPos > 0)
                {
                    int lineLength = lineEndPos - _startIndex;
                    if (_charBuffer.Length < lineLength) // do we have enough space in our char buffer?
                    {
                        _charBuffer = new char[lineLength];
                    }
                    int bytesUsed, charsUsed;
                    bool completed;
                    _decoder.Convert(_buffer, _startIndex, lineLength, _charBuffer, 0, lineLength, true, out bytesUsed,
                                    out charsUsed, out completed);
                    string line = new string(_charBuffer, 0, lineLength);
                    _startIndex = lineEndPos + 1;
                    //Debug.Print("found string length " + lineLength + "; new buffer = " + startIndex + " to " + endIndex);
                    return line;
                }
                else
                {
                    return null;
                }
            }
        }

        public string ReadCommand(char endChar)
        {
            lock (_buffer)
            {
                int lineEndPos = Array.IndexOf(_buffer, endChar, _startIndex, DataSize);
                // HACK: not looking for \r, just assuming that they'll come together        
                if (lineEndPos > 0)
                {
                    int lineLength = lineEndPos - _startIndex;
                    if (_charBuffer.Length < lineLength) // do we have enough space in our char buffer?
                    {
                        _charBuffer = new char[lineLength];
                    }
                    int bytesUsed, charsUsed;
                    bool completed;
                    _decoder.Convert(_buffer, _startIndex, lineLength, _charBuffer, 0, lineLength, true, out bytesUsed,
                                    out charsUsed, out completed);
                    string line = new string(_charBuffer, 0, lineLength);
                    _startIndex = lineEndPos + 1;
                    //Debug.Print("found string length " + lineLength + "; new buffer = " + startIndex + " to " + endIndex);
                    return line;
                }
                else
                {
                    return null;
                }
            }
        }

        public void ClearBuffer()
        {
            lock (_buffer)
            {
                for (int i = 0; i < _buffer.Length; i++)
                {
                    _buffer[i] = 0;
                }
            }
        }
    }
}
