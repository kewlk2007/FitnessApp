namespace Fitnessapp.Api.Shared
{
    public record Error(string Code, string Message)
    {
        public static readonly Error None = new(string.Empty, string.Empty);

        public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

        public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", "The specified condition was not met.");

        public static readonly Error NameAlreadyInUse = new("Error.NameAlreadyInUse", "Supplied name is already in use.");
    }
}
