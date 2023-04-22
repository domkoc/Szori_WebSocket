namespace WebSocket_FIBRPN
{
    public sealed class CinemaModel
    {

        private static CinemaModel? _instance = null;
        private static readonly object padlock = new();
        public List<CinemaRow> cinemaRows = new();

        CinemaModel(){}

        public static CinemaModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new CinemaModel();
                    }
                    return _instance;
                }
            }
        }

        public bool InitRoom(int rows, int columns)
        {
            if (rows < 1 || columns < 1) return false;
            lock (padlock)
            {
                this.cinemaRows = new List<CinemaRow>();
                for (int i = 1; i <= rows; i++)
                {
                    this.cinemaRows.Add(new CinemaRow(i, columns));
                }
                return true;
            }
        }

        public struct RoomSize
        {
            public int rows;
            public int columns;
        }
        public RoomSize GetRoomSize()
        {
            lock (padlock)
            {
                return new RoomSize { rows = this.cinemaRows.Count, columns = this.cinemaRows[0].seats.Count };
            }
        }

        public SeatStatus GetSeatStatus(int row, int column)
        {
            return this.cinemaRows[row - 1].seats[column - 1].GetSeatStatus();
        }

        public struct LockedSeat
        {
            public SeatStatus seatStatus;
            public string lockid;
        }
        public LockedSeat? LockSeat(int row, int column)
        {
            var lockId = this.cinemaRows[row - 1].seats[column - 1].LockSeat();
            return lockId == null ? null : new LockedSeat { seatStatus = SeatStatus.Locked, lockid = lockId};
        }

        public SeatStatus? UnlockSeat(string lockid)
        {
            foreach (CinemaRow row in this.cinemaRows)
            {
                foreach (CinemaSeat seat in row.seats)
                {
                    if (seat.Lockid == lockid)
                    {
                        seat.UnlockSeat();
                        return seat.GetSeatStatus();
                    }
                }
            }
            return null;
        }

        public SeatStatus? ReserveSeat(string lockid)
        {
            foreach (CinemaRow row in this.cinemaRows)
            {
                foreach (CinemaSeat seat in row.seats)
                {
                    if (seat.Lockid == lockid)
                    {
                        seat.ReserveSeat();
                        return seat.GetSeatStatus();
                    }
                }
            }
            return null;
        }
    }
    public class CinemaRow
    {
        private int RowNumber;
        public List<CinemaSeat> seats;
        private readonly object padlock = new();

        public CinemaRow(int rowNumber, int columns)
        {
            this.RowNumber = rowNumber;
            seats = new List<CinemaSeat>();
            for (int i = 1; i <= columns; i++)
            {
                this.seats.Add(new CinemaSeat(i));
            }
        }
    }

    public class CinemaSeat
    {
        public int ColumnNumber;
        private SeatStatus SeatStatus;
        public string? Lockid;
        private readonly object padlock = new();

        public CinemaSeat(int columnNumber)
        {
            this.ColumnNumber = columnNumber;
            this.SeatStatus = SeatStatus.Free;
            this.Lockid = null;
        }

        public string? LockSeat()
        {
            lock (padlock)
            {
                if (this.SeatStatus == SeatStatus.Free)
                {
                    this.SeatStatus = SeatStatus.Locked;
                    this.Lockid = Guid.NewGuid().ToString();
                    return this.Lockid;
                }
                return null;
            }
        }
        public SeatStatus GetSeatStatus()
        {
            lock (padlock)
            {
                return this.SeatStatus;
            }
        }

        public void UnlockSeat()
        {
            lock (padlock)
            {
                this.SeatStatus = SeatStatus.Free;
                this.Lockid = null;
            }
        }

        public void ReserveSeat()
        {
            lock (padlock)
            {
                this.SeatStatus = SeatStatus.Reserved;
                this.Lockid = null;
            }
        }
    }
}
