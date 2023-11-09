using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Client
{
    public struct Structure
    {
        public int num1;
        public int num2;
        //public int priority;
    }

    class Client
    {
        static byte[] Serialize(Structure recData)
        {
            int size = Unsafe.SizeOf<Structure>();
            byte[] mod = new byte[size];
            Unsafe.As<byte, Structure>(ref mod[0]) = recData;
            return mod;
        }

        public static Structure Deserialize(NamedPipeClientStream Cl)
        {
            int size = Unsafe.SizeOf<Structure>();
            byte[] bytes = new byte[size];
            Cl.Read(bytes, 0, bytes.Length);
            return Unsafe.As<byte, Structure>(ref bytes[0]);
        }

        static void Main()
        {
            using NamedPipeClientStream Cl = new(".", "channel", PipeDirection.InOut);
            Console.WriteLine("Подключение к серверу...");
            Cl.Connect();
            Console.WriteLine("Клиент подключился к серверу");
            while (true)
            {
                Structure recData = Deserialize(Cl);
                Console.WriteLine($"Полученные данные: num1 = {recData.num1}, num2 = {recData.num2}");//, приоритет = {receivedData.priority}");
                byte[] mod = Serialize(recData);
                Cl.Write(mod, 0, mod.Length);
            }
        }
    }
}