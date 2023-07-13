using System;

namespace WirelessSensorNetworkSimpleSecond
{
    public class DataPacket
    {
        public int Id { get; set; }
        public int SenderId { get; private set; }
        public int ReceiverId { get; private set; }
        public int Size;
        // Случайная генерация чисел для моделирования помех
        Random random = new Random();
        public string Data { get; private set; }
        public DataPacket(int senderId, int receiverId, string data)
        {
            this.SenderId = senderId;
            this.ReceiverId = receiverId;
            this.Data = data;
        }


        public bool TransmitData(DataPacket packet)
        {
            double errorProbability = 0.1;  // Установите это значение в зависимости от уровня помех, который вы хотите моделировать

            if (random.NextDouble() < errorProbability)
            {
                // Если происходит ошибка передачи данных, возвращаем false
                return false;
            }
            else
            {
                // Если передача данных успешна, возвращаем true
                return true;
            }
        }
    }

}