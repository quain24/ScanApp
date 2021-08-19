using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedExtensions;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Infrastructure.Persistence
{
    /// <summary>
    /// Provides a way to convert <see cref="Version"/> object to and from a SQL database compatible type.<br/>
    /// <see cref="Version"/> object is used as a user-friendly representation of entity rowversion, therefore<br/>
    /// this converter translates <see cref="T:byte[]"/> to <see cref="Version"/> and vice-versa.
    /// </summary>
    public class VersionConverter : ValueConverter<Version, byte[]>
    {
        /// <summary>
        /// Creates new instance of <see cref="VersionConverter"/>.
        /// </summary>
        public VersionConverter() : base(
            c => c.IsEmpty ? null : Convert.FromBase64String(c.Value),
            x => x.IsNullOrEmpty() ? Version.Empty : Version.Create(Convert.ToBase64String(x)))
        {
        }
    }
}