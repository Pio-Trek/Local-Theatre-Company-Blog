namespace LocalTheatre.Models
{
    public static class RoleName
    {
        // Use: [Authorize(Roles = RoleName.AdminRole)]

        public const string Admin = "Admin";
        public const string Writer = "Writer";
        public const string User = "User";

        public static string FromId(string roleId)
        {
            if (roleId == "1")
                roleId = Admin;
            else if (roleId == "2")
                roleId = Writer;
            else
                roleId = User;

            return roleId;
        }
    }
}