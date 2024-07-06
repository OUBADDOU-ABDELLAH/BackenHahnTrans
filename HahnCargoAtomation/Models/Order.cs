using HahnTransportAutomate.DTOs;
using System.Globalization;
using System.Xml.Linq;

namespace HahnTransportAutomate.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Node OriginNode { get; set; }
        public Node TargetNode { get; set; }
        public int Load { get; set; }
        public int Value { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public OrderDto ToDto()
        {
            return new OrderDto
            {
                Id = Id,
                OriginNodeId = OriginNode.Id,
                TargetNodeId = TargetNode.Id,
                Load = Load,
                Value = Value,
                DeliveryDateUtc = DeliveryDate.ToString(CultureInfo.InvariantCulture),
                ExpirationDateUtc = ExpirationDate.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
