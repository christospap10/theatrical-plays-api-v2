﻿namespace Theatrical.Dto.EventDtos;

public class EventDto
{
    public DateTime DateEvent { get; set; }
    public string PriceRange { get; set; }
    public int ProductionId { get; set; }
    public int VenueId { get; set; }
}