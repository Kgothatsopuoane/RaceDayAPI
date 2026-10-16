namespace RaceDayAPI.Models
{
   
    
        public class Result
        {
            public int ResultId { get; set; }

            public int EventId { get; set; }

            public int ParticipantId { get; set; }

            public int FinishingPosition { get; set; }

            public TimeSpan FinishTime { get; set; }

            public Event? Event { get; set; }

            public User? Participant { get; set; }
        }
    }