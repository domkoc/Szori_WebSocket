using System.Text.Json.Serialization;

namespace WebSocket_FIBRPN
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SeatStatus
    {
        [JsonPropertyName("free")]
        Free,
        [JsonPropertyName("reserved")]
        Reserved,
        [JsonPropertyName("locked")]
        Locked,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Command
    {
        [JsonPropertyName("initRoom")]
        InitRoom,
        [JsonPropertyName("getRoomSize")]
        GetRoomSize,
        [JsonPropertyName("updateSeats")]
        UpdateSeats,
        [JsonPropertyName("lockSeat")]
        LockSeat,
        [JsonPropertyName("unlockSeat")]
        UnlockSeat,
        [JsonPropertyName("reserveSeat")]
        ReserveSeat,
        [JsonPropertyName("roomSize")]
        RoomSize,
        [JsonPropertyName("seatStatus")]
        SeatStatus,
        [JsonPropertyName("lockResult")]
        LockResult,
        [JsonPropertyName("error")]
        Error
    }

    public class Operation
    {
        [JsonPropertyName("type")]
        public Command Type { get; set; }

        [JsonPropertyName("rows")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? RowNumber { get; set; }

        [JsonPropertyName("columns")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ColumnNumber { get; set; }

        [JsonPropertyName("lockId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LockId { get; set; }

        [JsonPropertyName("status")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SeatStatus? Status { get; set; }

        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }
    }
}
