namespace Game.Api.Model.Dto
{
    public class GameDto
    {
        public ModeDto? Mode { get; set; }
        public IEnumerable<JokeDto> Jokes { get; set; } = Enumerable.Empty<JokeDto>();
    }
}
