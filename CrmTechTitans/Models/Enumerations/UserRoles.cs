namespace CrmTechTitans.Models.Enumerations
{
    /// <summary>
    /// Defines the roles available in the application
    /// </summary>
    public static class UserRoles
    {
        public const string Administrator = "Administrator";
        public const string Editor = "Editor";
        public const string ReadOnly = "ReadOnly";
        
        public static readonly string[] AllRoles = new[] 
        { 
            Administrator, 
            Editor, 
            ReadOnly 
        };
    }
} 