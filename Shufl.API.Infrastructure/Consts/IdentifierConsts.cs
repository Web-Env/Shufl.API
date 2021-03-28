namespace Shufl.API.Infrastructure.Consts
{
    public static class IdentifierConsts
    {
        public static int UserIdentifierLength { get; private set; } = 64;

        public static int GroupIdentifierLength { get; private set; } = 24;
        public static int GroupInviteIdentifierExpiryOffsetDays { get; private set; } = 7;
    }
}
