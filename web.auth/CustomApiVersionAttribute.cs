using Microsoft.AspNetCore.Mvc.Versioning;

namespace web.auth
{
    /// <summary>
    /// Custom ApiVersionAttribute to support the use of a custom ApiVersion instance
    /// </summary>
    public class CustomApiVersionAttribute : ApiVersionsBaseAttribute, IApiVersionProvider
    {
        public ApiVersionProviderOptions Options => ApiVersionProviderOptions.None; // Mapped?

        public CustomApiVersionAttribute(CustomApiVersion version)
            : base(version)
        {
        }

        public CustomApiVersionAttribute(string version)
            : base(CustomApiVersion.Parse(version))
        {
        }
    }
}
