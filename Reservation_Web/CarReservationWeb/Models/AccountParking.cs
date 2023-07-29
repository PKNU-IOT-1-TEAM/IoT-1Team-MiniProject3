using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarReservationWeb.Models;

public partial class AccountParking
{
	[Key]
	public int IdX { get; set; }

	public string? CarNumber { get; set; }

	public string Id { get; set; } = null!;

	[DataType(DataType.Password)]
	public string Password { get; set; } = null!;

	public sbyte Admin { get; set; }

	public string? NfcRegistered { get; set; }
}
