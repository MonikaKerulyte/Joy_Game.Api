namespace Game.Api.Model.Dto
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public byte[]? Picture { get; set; }
    }
}
