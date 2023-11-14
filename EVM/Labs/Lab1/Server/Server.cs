using System.IO.Pipes;
using System.Runtime.CompilerServices;

namespace Server
{
    public struct Data
    {
        public int num1;
        public int num2;
    }

    class PipeServer
    {

        static byte[] Serialize(Data msg)
        {
            byte[] bytes = new byte[Unsafe.SizeOf<Data>()];
            Unsafe.As<byte, Data>(ref bytes[0]) = msg;
            return bytes;
        }
                
        public static Data Deserialize(StreamWriter sw)
        {
            byte[] received = new byte[Unsafe.SizeOf<Data>()];
            sw.BaseStream.Read(received, 0, received.Length);
            return Unsafe.As<byte, Data>(ref received[0]);

        }

        static void Main()
        {
            using NamedPipeServerStream Server = new("channel", PipeDirection.InOut);
            Console.WriteLine("Ожидается подключения клиента...");
            Server.WaitForConnection();
            Console.WriteLine("Клиент подключен");

            StreamWriter sw = new(Server)
            {
                AutoFlush = true
            };

            Console.Write("Введите первое число: ");
            int num_1 = int.Parse(Console.ReadLine());
            Console.Write("Введите второе число: ");
            int num_2 = int.Parse(Console.ReadLine());


            Data msg = new()
            {
                num1 = num_1,
                num2 = num_2
            };

            byte[] bytes = Serialize(msg);
            sw.BaseStream.Write(bytes, 0, bytes.Length);

            Data received = Deserialize(sw);
            Console.WriteLine($"Полученные данные: первое число = {received.num1}, второе число = {received.num2}");

            Console.ReadKey();
        }
    }
}