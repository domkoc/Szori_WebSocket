using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using static WebSocket_FIBRPN.CinemaModel;

namespace WebSocket_FIBRPN
{
    [Route("WebSocket_FIBRPN/cinema")]
    public class CinemaEndpoint
    {
        private static CinemaModel Cinema = CinemaModel.Instance;

        public async Task Open(WebSocket socket)
        {
            Console.WriteLine("WebSocket opened.");
        }

        public async Task Close(WebSocket socket)
        {
            Console.WriteLine("WebSocket closed.");
        }

        public async Task SendError(WebSocket socket, string message)
        {
            await StringJsonEncoder.SendAsync(socket, Operation.Error(message));
        }

        public async Task<Operation> InvalidOperationMessage(WebSocket socket, Operation operation)
        {
            return Operation.Error("Invalid operation.");
        }

        public async Task InitRoom(WebSocket socket, Operation operation)
        {
            if (operation.Rows is null || operation.Columns is null)
            {
                await SendError(socket, "No row or column number.");
            }
            else if (operation.Rows.Value < 1 || operation.Columns.Value < 1)
            {
                await SendError(socket, "Invalid row or column number.");
            }
            else
            {
                Cinema.InitRoom(operation.Rows.Value, operation.Columns.Value);
            }
        }

        public async Task<Operation> GetRoomSize()
        {
            var roomSize = Cinema.GetRoomSize();
            return Operation.RoomSize(roomSize.rows, roomSize.columns);
        }

        public async Task UpdateSeats(WebSocket socket)
        {
            foreach (CinemaRow row in Cinema.CinemaRows)
            {
                foreach (CinemaSeat seat in row.seats)
                {
                    await StringJsonEncoder.SendAsync(socket, Operation.SeatStatus(row.RowNumber, seat.ColumnNumber, seat.GetSeatStatus()));
                }
            }
        }

        public async Task<Operation?> LockSeat(WebSocket socket, Operation operation)
        {
            var roomSize = Cinema.GetRoomSize();
            if (operation.Row is null || operation.Column is null)
            {
                await SendError(socket, "No row or column number.");
                return null;
            }
            else if (operation.Row.Value < 1 || operation.Row.Value > roomSize.rows || operation.Column.Value < 1 || operation.Column.Value > roomSize.columns)
            {
                await SendError(socket, "Invalid row or column number.");
                return null;
            }
            else
            {
                if (Cinema.GetSeatStatus(operation.Row.Value, operation.Column.Value) != SeatStatus.Free)
                {
                    await SendError(socket, "Seat is not free.");
                    return null;
                }
                else
                {
                    CinemaModel.LockedSeat? lockedSeat = Cinema.LockSeat(operation.Row.Value, operation.Column.Value);
                    if (lockedSeat == null)
                    {
                        await SendError(socket, "Unable to lock seat.");
                        return null;
                    }
                    else
                    {
                        await StringJsonEncoder.SendAsync(socket, new Operation { Type = Command.LockResult, LockId = lockedSeat.Value.lockid });
                        return Operation.SeatStatus(operation.Row.Value, operation.Column.Value, lockedSeat.Value.seatStatus);
                    }
                }
            }
        }
        
        public async Task<Operation?> UnlockSeat(WebSocket socket, Operation operation)
        {
            if (operation.LockId is null)
            {
                await SendError(socket, "No lock id.");
                return null;
            }
            else
            {
                var unlockedSeat = Cinema.UnlockSeat(operation.LockId);
                if (unlockedSeat == null)
                {
                    await SendError(socket, "Unable to free lock.");
                    return null;
                }
                else
                {
                    return Operation.SeatStatus(unlockedSeat.Value.row, unlockedSeat.Value.column, unlockedSeat.Value.seatStatus);
                }
            }
        }

        public async Task<Operation?> ReserveSeat(WebSocket socket, Operation operation)
        {
            if (operation.LockId is null)
            {
                // await SendError(socket, "No lock id.");
                return null;
            }
            else
            {
                var reservedSeat = Cinema.ReserveSeat(operation.LockId);
                if (reservedSeat == null)
                {
                    await SendError(socket, "Unable to reserve seat.");
                    return null;
                }
                else
                {
                    return Operation.SeatStatus(reservedSeat.Value.row, reservedSeat.Value.column, reservedSeat.Value.seatStatus);
                }
            }
        }
    }
}