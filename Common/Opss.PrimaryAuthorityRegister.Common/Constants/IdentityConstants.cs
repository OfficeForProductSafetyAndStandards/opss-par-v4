namespace Opss.PrimaryAuthorityRegister.Common.Constants;

public static class IdentityConstants
{
    public static class Roles
    {
        public const string AuthorityManager = "Primary Authority Officer - Manager";
        public const string AuthorityMember = "Primary Authority Officer - Member";
        public const string EnforcingManager = "Enforcing Officer - Manager";
        public const string EnforcingMember = "Enforcing Officer - Member";
        public const string OrganisationManager = "Organisation - Manager";
        public const string OrganisationMember = "Organisation - Member";
        public const string Helpdesk = "Helpdesk Officer";
        public const string SeniorHelpdesk = "Senior Helpdesk Officer";
        public const string SecretaryOfState = "Secretary of State";
        public const string LAU = "Local authority unit (LAU) Officer";

        public readonly static string[] AllRoles = {
            AuthorityManager,
            AuthorityMember,
            EnforcingManager,
            EnforcingMember,
            OrganisationManager,
            OrganisationMember,
            Helpdesk,
            SeniorHelpdesk,
            SecretaryOfState,
            LAU
        };
    }
}