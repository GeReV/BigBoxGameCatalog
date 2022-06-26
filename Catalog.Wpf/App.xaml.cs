using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Catalog.Scrapers;
using Catalog.Wpf.GlContexts;
using Catalog.Wpf.GlContexts.Wgl;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const string FALLBACK_DIRECTORY_NAME = "BBGC";

        private const string BACKUP_DIRECTORY = "backups";
        private const string BACKUP_FILENAME_PATTERN = "*.sqlite";
        private const string BACKUP_FILENAME_DATE_PATTERN = "yyyyMMddhhmmss";
        private readonly TimeSpan BACKUP_AGE_THRESHOLD = TimeSpan.FromDays(7);
        
        private readonly GlContext glContext = new WglContext();

        public App()
        {
            var homeDirectory = EnsureHomeDirectory();

            Current.Properties.Add(nameof(ApplicationHelpers.HomeDirectory), homeDirectory);
            Current.Properties.Add(nameof(ApplicationHelpers.ScraperWebClient), new WebClient());

            InitializeDatabase();
            
            glContext.MakeCurrent();
        }

        private static string EnsureHomeDirectory()
        {
            var settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var homeDirectory = Path.Combine(settingsDirectory,
                Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title ??
                FALLBACK_DIRECTORY_NAME);

            if (!Directory.Exists(homeDirectory))
            {
                Directory.CreateDirectory(homeDirectory);
            }

            return homeDirectory;
        }

        private static DateTime? ParseDateTimeFromFilename(FileSystemInfo info)
        {
            var basename = info.Name.Replace(info.Extension, string.Empty);

            if (DateTime.TryParseExact(basename, BACKUP_FILENAME_DATE_PATTERN, null,
                DateTimeStyles.None, out var result))
            {
                return result;
            }

            return null;
        }

        private void BackupDatabase(DbContext context)
        {
            context.Database.OpenConnection();

            if (context.Database.IsSqlite())
            {
                var connection = context.Database.GetDbConnection();
                var sourceFileInfo = new FileInfo(connection.DataSource ?? throw new InvalidOperationException());

                Debug.Assert(sourceFileInfo.Directory != null, "sourceFileInfo.Directory != null");

                var backupDirectory = sourceFileInfo.Directory.CreateSubdirectory(BACKUP_DIRECTORY);

                var latestBackup = backupDirectory
                    .EnumerateFiles(BACKUP_FILENAME_PATTERN)
                    .OrderByDescending(info => info.CreationTime)
                    .FirstOrDefault();

                if (latestBackup == null ||
                    sourceFileInfo.LastWriteTime > latestBackup.CreationTime &&
                    DateTime.Now.Subtract(latestBackup.CreationTime) > BACKUP_AGE_THRESHOLD)
                {
                    MakeDatabaseBackup(sourceFileInfo, backupDirectory);
                }
            }

            context.Database.CloseConnection();
        }

        private static void MakeDatabaseBackup(FileInfo sourceFileInfo, FileSystemInfo backupDirectory)
        {
            var backupFilename = $"{DateTime.Now.ToString(BACKUP_FILENAME_DATE_PATTERN)}.sqlite";

            sourceFileInfo.CopyTo(Path.Join(backupDirectory.FullName, backupFilename));
        }

        private void InitializeDatabase()
        {
            var context = Current.Database();

            BackupDatabase(context);

            context.Database.Migrate();
            context.Database.EnsureCreated();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.Database().Dispose();
        }
    }
}
