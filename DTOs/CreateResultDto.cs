namespace RaceDayAPI.DTOs
{
    public class CreateResultDto
    {
        public int EventId { get; set; }

        public int ParticipantId { get; set; }

        public int FinishingPosition { get; set; }

        public TimeSpan FinishTime { get; set; }
    }
}