using System;
using System.Collections.Generic;

namespace CarReservationWeb.Models;

public partial class SensorDb
{
    public int IdX { get; set; }

    public float? Ad1RcvIrSensor { get; set; }

    public float? Ad1RcvTemperature { get; set; }

    public float? Ad1RcvHumidity { get; set; }

    public float? Ad1RcvDust { get; set; }

    public string? Ad1RcvParkingStatus { get; set; }

    public sbyte? Ad2RcvCguard { get; set; }

    public int? Ad3RcvWguardWave { get; set; }

    public string? Ad4RcvNfc { get; set; }

    public float? Ad4RcvWlCnnt { get; set; }
}
