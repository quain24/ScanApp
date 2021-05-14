using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// Provides way to seed database on application startup.
    /// </summary>
    public interface IInitialDataSeeder
    {
        /// <summary>
        /// Will insert data provided inside instance into the database, including migrations.
        /// <para>Will perform <strong>migration</strong> initialization when:
        /// <list type="bullet">
        ///     <item><description>Called database is missing.</description></item>
        ///     <item><description>Detects new migrations not applied to database.</description></item>
        ///     <item><description><paramref name="force"/> parameter is set to <see langword="True"/>.</description></item>
        /// </list>
        /// </para>
        /// <para>Will perform <strong>data</strong> initialization when:
        /// <list type="bullet">
        ///     <item><description>Called database is missing.</description></item>
        ///     <item><description><strong>Administrator</strong> account is missing.</description></item>
        ///     <item><description><paramref name="force"/> parameter is set to <see langword="True"/>.</description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="force">If <see langword="True"/> - will force initialization of the database even if conditions for it are not met.</param>
        /// <returns>Awaitable <see cref="Task"/> representing asynchronous initialization operation.</returns>
        Task Initialize(bool force);
    }
}