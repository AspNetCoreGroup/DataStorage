﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataStorageCore.Models;

public partial class Measurement
{
    public int Id { get; set; }

    public int DeviceId { get; set; }

    public int MeasurumentDictId { get; set; }

    public virtual ICollection<Archive> Archives { get; set; } = new List<Archive>();

    public virtual Device Device { get; set; }

    public virtual MeasurementDict MeasurumentDict { get; set; }
}