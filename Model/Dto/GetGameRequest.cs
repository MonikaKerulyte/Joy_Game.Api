namespace Game.Api.Model.Dto
{
    public class GetGameRequest
    {
        public required string Mode { get; set; }
        public int AmountOfJokes { get; set; }
    }
}
