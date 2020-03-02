﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Analogy.LogViewer.XMLLogParser.Managers;

namespace Analogy.LogViewer.XMLLogParser.IAnalogy
{
    public class XMLDataProvider : IAnalogyOfflineDataProvider
    {
        public string OptionalTitle { get; } = "Analogy XML Text Parser";

        public Guid ID { get; } = new Guid("1DA46386-5604-449E-87FB-7D1036A85978");

        public bool CanSaveToLogFile { get; } = false;
        public string FileOpenDialogFilters { get; } = "XML log files|*.xml";
        public string FileSaveDialogFilters { get; } = string.Empty;
        public IEnumerable<string> SupportFormats { get; } = new[] { "*.xml" };

        public string InitialFolderFullPath => Directory.Exists(UserSettings?.Directory)
            ? UserSettings.Directory
            : Environment.CurrentDirectory;
        public XMLParser XMLParser { get; set; }

        private ILogParserSettings UserSettings { get; set; }
        public XMLDataProvider(ILogParserSettings userSettings)
        {
            UserSettings = userSettings;
        }
        public Task InitializeDataProviderAsync(IAnalogyLogger logger)
        {
            LogManager.Instance.SetLogger(logger);
            XMLParser = new XMLParser(UserSettingsManager.UserSettings.LogParserSettings);
            return Task.CompletedTask;
        }

        public void MessageOpened(AnalogyLogMessage message)
        {
            //nop
        }
        public async Task<IEnumerable<AnalogyLogMessage>> Process(string fileName, CancellationToken token, ILogMessageCreatedHandler messagesHandler)
        {
            if (CanOpenFile(fileName))
                return await XMLParser.Process(fileName, token, messagesHandler);
            return new List<AnalogyLogMessage>(0);

        }

        public IEnumerable<FileInfo> GetSupportedFiles(DirectoryInfo dirInfo, bool recursiveLoad)
            => GetSupportedFilesInternal(dirInfo, recursiveLoad);

        public Task SaveAsync(List<AnalogyLogMessage> messages, string fileName)
        {
            throw new NotSupportedException("Saving is not supported for Plain Text");
        }

        public bool CanOpenFile(string fileName) => UserSettingsManager.UserSettings.LogParserSettings.CanOpenFile(fileName);

        public bool CanOpenAllFiles(IEnumerable<string> fileNames) => fileNames.All(CanOpenFile);

        private List<FileInfo> GetSupportedFilesInternal(DirectoryInfo dirInfo, bool recursive)
        {

            List<FileInfo> files = dirInfo.GetFiles("*.*")
                .Where(f => UserSettings.CanOpenFile(f.FullName)).ToList();
            if (!recursive)
                return files;
            try
            {
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    files.AddRange(GetSupportedFilesInternal(dir, true));
                }
            }
            catch (Exception)
            {
                return files;
            }

            return files;
        }
    }
}