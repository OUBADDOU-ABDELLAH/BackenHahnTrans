using HahnTransportAutomate.DTOs;

namespace HahnTransportAutomate.Models
{
    public class CargoTransporter
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public Node? Position { get; set; }
        public bool InTransit { get; set; }
        public int Capacity { get; set; }
        public int Load { get; set; }
        public List<Order> LoadedList { get; set; }

        public CargoTransporterDto ToDto()
        {
            return new CargoTransporterDto
            {
                Id = Id,
                PositionNodeId = Position?.Id ?? -1,
                InTransit = InTransit,
                Capacity = Capacity,
                Load = Load,
                LoadedOrders = LoadedList.Select(order => order.ToDto()).ToList()
            };
        }
    }

}
