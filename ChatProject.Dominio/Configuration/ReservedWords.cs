namespace ChatProject.Domain.Configuration
{
    public static class ReservedWords
    {
        public const string ListUsers = "LU";
        public const string Nickname = "NAME";
        public const string Userprivate = "PR";
        public const string Userpublic = "P";
        public const string CreateRoom = "CTR";
        public const string ChangeRoom = "CR";
        public const string ListRoom = "LR";
        public const string Help = "HELP";
        public const string Exit = "EXIT";

        public static bool IsReservedWord(string word)
        {
            if(word.Equals(ListUsers) || word.Equals(Userprivate) || word.Equals(Userpublic) || 
                word.Equals(CreateRoom) || word.Equals(ChangeRoom) || word.Equals(Help) || 
                word.Equals(Exit) || word.Equals(Nickname) || word.Equals(ListRoom))
            {
                return true;
            }
            return false;
        }
    }
}
