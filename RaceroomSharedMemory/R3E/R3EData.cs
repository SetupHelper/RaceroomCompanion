using RaceroomSharedMemory.R3E.Data;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace RaceroomSharedMemory.R3E
{
    public class R3EData : IDisposable
    {
        internal bool Mapped
        {
            get { return (_file != null); }
        }

        public Shared MyData { get; private set; }
        private MemoryMappedFile _file;
        private byte[] _buffer;

        private readonly TimeSpan _timeAlive = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _timeInterval = TimeSpan.FromMilliseconds(100);

        public void Dispose()
        {
            _file.Dispose();
        }

        internal bool Map()
        {
            try
            {
                _file = MemoryMappedFile.OpenExisting(Constant.SharedMemoryName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public bool Read()
        {
            try
            {
                var _view = _file.CreateViewStream();
                BinaryReader _stream = new BinaryReader(_view);
                _buffer = _stream.ReadBytes(Marshal.SizeOf(typeof(Shared)));
                GCHandle _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
                MyData = (Shared)Marshal.PtrToStructure(_handle.AddrOfPinnedObject(), typeof(Shared));
                _handle.Free();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}