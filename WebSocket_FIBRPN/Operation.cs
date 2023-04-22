using System.Text;
using System.Text.Json.Serialization;

namespace WebSocket_FIBRPN
{
    public enum SeatStatus
    {
        Free,
        Reserved,
        Locked,
    }

    public enum Command
    {
        InitRoom,
        GetRoomSize,
        UpdateSeats,
        LockSeat,
        UnlockSeat,
        ReserveSeat,
        RoomSize,
        SeatStatus,
        LockResult,
        Error
    }

    public class Operation
    {
        [JsonPropertyName("type")]
        public Command Type { get; set; }

        [JsonPropertyName("row")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Row { get; set; }

        [JsonPropertyName("rows")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Rows { get; set; }

        [JsonPropertyName("column")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Column { get; set; }

        [JsonPropertyName("columns")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Columns { get; set; }

        [JsonPropertyName("lockId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LockId { get; set; }

        [JsonPropertyName("status")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SeatStatus? Status { get; set; }

        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }

        public static Operation Error(string message)
        {
            return new Operation
            {
                Type = Command.Error,
                Message = message
            };
        }

        public static Operation RoomSize(int rows, int columns)
        {
            return new Operation
            {
                Type = Command.RoomSize,
                Rows = rows,
                Columns = columns
            };
        }

        public static Operation SeatStatus(int row, int column, SeatStatus status)
        {
            return new Operation
            {
                Type = Command.SeatStatus,
                Row = row,
                Column = column,
                Status = status
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("{\n");
            sb.Append($"\t\"type\": \"{Type}\",\n");
            if (Row != null) sb.Append($"\t\"row\": {Row},\n");
            if (Rows != null) sb.Append($"\t\"rows\": {Rows},\n");
            if (Column != null) sb.Append($"\t\"column\": {Column},\n");
            if (Columns != null) sb.Append($"\t\"columns\": {Columns},\n");
            if (LockId != null) sb.Append($"\t\"lockId\": \"{LockId}\",\n");
            if (Status != null) sb.Append($"\t\"status\": \"{Status}\",\n");
            if (Message != null) sb.Append($"\t\"message\": \"{Message}\"\n,");
            sb.Append("}\n");

            return sb.ToString();
        }

    }
}
