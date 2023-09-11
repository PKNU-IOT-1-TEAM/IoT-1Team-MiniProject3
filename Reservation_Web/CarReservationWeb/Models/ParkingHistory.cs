using System;
using System.Collections.Generic;

namespace CarReservationWeb.Models;

public partial class ParkingHistory
{
    public int IdX { get; set; }

    public string Id { get; set; } = null!;

    public DateTime? EntranceTime { get; set; }

    public DateTime? DeparureTime { get; set; }
}
