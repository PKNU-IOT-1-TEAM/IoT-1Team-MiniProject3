using System;
using System.Collections.Generic;

namespace CarReservationWeb.Models;

public partial class ParkingStatus
{
    public int IdX { get; set; }

    public sbyte ParkingIr { get; set; }

    public string? Nfc { get; set; }

    public string? ReservationStatus { get; set; }
}
