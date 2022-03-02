using Microsoft.AspNetCore.Mvc;

namespace web.auth
{
    /// <summary>
    /// Custom ApiVersion attribute, to support groupname formatted as "major-status"
    /// </summary>
    public class CustomApiVersion : ApiVersion
    {
        public CustomApiVersion(int major, int minor, string status)
            : base(major, minor, status)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override string ToString(string format)
        {
            return base.ToString(format);
        }

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "V-'unspecified'") return $"{MajorVersion}-{Status}";

            return base.ToString(format, formatProvider);
        }

        /// <summary>
        /// Parse version into supported CustomApiVersion
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public new static CustomApiVersion Parse(string version)
        {
            var apiVersion = ApiVersion.Parse(version);
            return new CustomApiVersion(apiVersion.MajorVersion.GetValueOrDefault(), apiVersion.MinorVersion.GetValueOrDefault(), apiVersion.Status);
        }
    }
}
