namespace Cryptool.Plugins.ChaCha
{
    public class Version
    {
        public static readonly Version IETF = new Version("IETF", 32, 96, 1);
        public static readonly Version DJB = new Version("DJB", 64, 64, 0);
        public string Name { get; }
        public uint CounterBits { get; }
        public uint IVBits { get; }
        public uint InitialCounter { get; }

        private Version(string name, uint counterBits, uint ivBits, uint initialCounter)
        {
            Name = name;
            CounterBits = counterBits;
            IVBits = ivBits;
            InitialCounter = initialCounter;
        }
    }
}