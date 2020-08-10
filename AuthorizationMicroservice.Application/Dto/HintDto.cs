namespace AuthorizationMicroservice.Application.Dto
{
    public class HintDto
    {
        public bool CanGetNewHint { get; set; } = false;
        public UserHintDto User { get; set; }
        public string Token { get; set; }
    }

}
