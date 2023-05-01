using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderService.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusEnum Status { get; set; }

        public DateTime Created { get; set; }

        public List<Good> Lines { get; set; }
    }

    public enum StatusEnum { New, Paid , WaitForPay , WaitForDelivery , Delivered , Completed }
    public class Good
    { 
        [Key]
        public Guid Id { get; set; }
        public uint Qty { get; set; }
        public Guid OrderId { get; set; }

    }
}