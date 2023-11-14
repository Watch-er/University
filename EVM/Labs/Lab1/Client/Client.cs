using System.IO.Pipes;
using System.Runtime.CompilerServices;

namespace Client
{
    public struct Data
    {
        public int num1;
        public int num2;
    }

    class PipeClient
    {

        static byte[] Serialize(Data received)
        {
            byte[] mod = new byte[Unsafe.SizeOf<Data>()];
            Unsafe.As<byte, Data>(ref mod[0]) = received;
            return mod;
        }

        public static Data Deserialize(NamedPipeClientStream Client)
        {
            byte[] bytes = new byte[Unsafe.SizeOf<Data>()];
            Client.Read(bytes, 0, bytes.Length);
            return Unsafe.As<byte, Data>(ref bytes[0]);
        }

        static void Main()
        {
            using NamedPipeClientStream Client = new(".", "channel", PipeDirection.InOut);
            Console.WriteLine("Подключение к серверу...");
            Client.Connect();
            Console.WriteLine("Клиент подключился к серверу");

            Data received = Deserialize(Client);

            Console.WriteLine("Число1: " + received.num1);
            Console.WriteLine("Число2: " + received.num2);

            byte[] mod = Serialize(received);
            Client.Write(mod, 0, mod.Length);
            Console.WriteLine("Клиент отправил ответ серверу о получении сообщения");

            Console.ReadKey();
        }
    }
}