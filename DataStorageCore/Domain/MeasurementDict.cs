﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataStorageCore.Models;

public partial class MeasurementDict
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}