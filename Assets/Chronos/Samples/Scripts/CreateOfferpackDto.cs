using System;

public record CreateOfferpackDto
{
    public DateTimeOffset StartAt { get; }
    public DateTimeOffset EndAt { get; }

    public CreateOfferpackDto(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        StartAt = startAt;
        EndAt = endAt;
    }

    public override string ToString()
    {
        return $"CreateOfferpack <start:{StartAt}> , <end:{EndAt}>";
    }
}
