namespace Ramsha.Localization.Json
{
    public class JsonFileChangeMonitor : IDisposable
    {
        private readonly FileSystemWatcher _watcher;
        private readonly Func<Task> _onChange;

        private readonly object _debounceLock = new();
        private DateTime _lastChange = DateTime.MinValue;

        public JsonFileChangeMonitor(string folderPath, Func<Task> onChange)
        {
            _onChange = onChange;

            _watcher = new FileSystemWatcher(folderPath, "*.json")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            _watcher.Changed += OnFileChanged;
            _watcher.Created += OnFileChanged;
            _watcher.Deleted += OnFileChanged;
            _watcher.Renamed += OnFileChanged;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            lock (_debounceLock)
            {
                var now = DateTime.Now;
                if ((now - _lastChange).TotalMilliseconds < 200) return;
                _lastChange = now;
            }

            Task.Run(_onChange);
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }

}