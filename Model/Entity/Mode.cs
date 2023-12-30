namespace Game.Api.Model.Entity
{
    public class Mode
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
    }
}
