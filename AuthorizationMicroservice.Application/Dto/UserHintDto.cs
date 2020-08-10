using System;

namespace AuthorizationMicroservice.Application.Dto
{
    public class UserHintDto
    {
        public DateTime LastHintTime { get; set; }
        public int HintLvl { get; set; }
    }

}
