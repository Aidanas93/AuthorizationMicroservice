namespace AuthorizationMicroservice.Application.Dto
{
    public class AdminUserInfoDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PictureUrl { get; set; }
        public int MaxLvlReached { get; set; }
        public int HintLvl { get; set; }
        public string Role { get; set; }
    }
}
