using System;
using System.Collections.Generic;
using System.Threading;

namespace Ramsha
{
    public interface IRamshaAppPipeline<TApp>
    {
        IRamshaAppPipeline<TApp> Replace(string targetName, Action<TApp> newConfigure, RamshaPipelineEntryOptions? options = null);
        IRamshaAppPipeline<TApp> Replace(string targetName, string newName, Action<TApp> newConfigure, RamshaPipelineEntryOptions? options = null);
        IRamshaAppPipeline<TApp> Use(string name, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null);
        IRamshaAppPipeline<TApp> UseBefore(string targetName, string name, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null);
        IRamshaAppPipeline<TApp> UseAfter(string targetName, string name, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null);

        IRamshaAppPipeline<TApp> Use(Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null);
        IRamshaAppPipeline<TApp> UseBefore(string targetName, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null);
        IRamshaAppPipeline<TApp> UseAfter(string targetName, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null);

        IRamshaAppPipeline<TApp> MoveBefore(string entryName, string targetName);
        IRamshaAppPipeline<TApp> MoveAfter(string entryName, string targetName);
        bool Remove(string name);
        void Apply(TApp app);
        IReadOnlyList<PipelineEntry<TApp>> GetEntries();
    }

    public class RamshaAppPipeline<TApp> : IRamshaAppPipeline<TApp>
    {
        private bool _applied = false;
        private readonly List<PipelineEntry<TApp>> _entries = new();
        private static int _anonymousIndex;

        public IReadOnlyList<PipelineEntry<TApp>> GetEntries() => _entries.AsReadOnly();

        private static string GetAnonymousName() => $"__anonymous_{Interlocked.Increment(ref _anonymousIndex)}";

        public IRamshaAppPipeline<TApp> Replace(string targetName, Action<TApp> newConfigure, RamshaPipelineEntryOptions? options = null)
        {
            return Replace(targetName, targetName, newConfigure, options);
        }

        public IRamshaAppPipeline<TApp> Replace(string targetName, string newName, Action<TApp> newConfigure, RamshaPipelineEntryOptions? options = null)
        {
            if (string.IsNullOrEmpty(targetName))
                throw new ArgumentNullException(nameof(targetName));

            var index = _entries.FindIndex(e => e.Name == targetName);
            if (index < 0)
                throw new InvalidOperationException($"No pipeline entry found with name '{targetName}'.");

            var entry = _entries[index];
            if (!entry.Options.CanReplace)
            {
                throw new InvalidOperationException($"The pipeline entry '{targetName}' cannot be replaced");
            }

            _entries[index] = new PipelineEntry<TApp>(newConfigure, newName, options);
            return this;
        }


        #region Use Overloads

        public IRamshaAppPipeline<TApp> Use(string name, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            return AddEntry(configure, name, options, condition);
        }

        public IRamshaAppPipeline<TApp> Use(Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null)
        {
            return AddEntry(configure, GetAnonymousName(), options, condition);
        }

        #endregion

        #region UseBefore Overloads

        public IRamshaAppPipeline<TApp> UseBefore(string targetName, string name, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null)
        {
            if (string.IsNullOrEmpty(targetName)) throw new ArgumentNullException(nameof(targetName));
            return InsertBefore(targetName, configure, name, options, condition);
        }

        public IRamshaAppPipeline<TApp> UseBefore(string targetName, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null)
        {
            return InsertBefore(targetName, configure, GetAnonymousName(), options, condition);
        }

        #endregion

        #region UseAfter Overloads

        public IRamshaAppPipeline<TApp> UseAfter(string targetName, string name, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null)
        {
            if (string.IsNullOrEmpty(targetName)) throw new ArgumentNullException(nameof(targetName));
            return InsertAfter(targetName, configure, name, options, condition);
        }

        public IRamshaAppPipeline<TApp> UseAfter(string targetName, Action<TApp> configure, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null)
        {
            return InsertAfter(targetName, configure, GetAnonymousName(), options, condition);
        }

        #endregion

        #region Move & Remove

        public IRamshaAppPipeline<TApp> MoveBefore(string entryName, string targetName)
        {
            var index = _entries.FindIndex(e => e.Name == entryName);
            var targetIndex = _entries.FindIndex(e => e.Name == targetName);
            if (index < 0 || targetIndex < 0 || index == targetIndex) return this;

            var entry = _entries[index];
            if (!entry.Options.CanMove)
            {
                throw new InvalidOperationException($"The pipeline entry '{entryName}' cannot be moved");
            }
            _entries.RemoveAt(index);
            targetIndex = _entries.FindIndex(e => e.Name == targetName);
            _entries.Insert(targetIndex, entry);
            return this;
        }

        public IRamshaAppPipeline<TApp> MoveAfter(string entryName, string targetName)
        {
            var index = _entries.FindIndex(e => e.Name == entryName);
            var targetIndex = _entries.FindIndex(e => e.Name == targetName);
            if (index < 0 || targetIndex < 0 || index == targetIndex) return this;

            var entry = _entries[index];
            if (!entry.Options.CanMove)
            {
                throw new InvalidOperationException($"The pipeline entry '{entryName}' cannot be moved");
            }


            _entries.RemoveAt(index);
            targetIndex = _entries.FindIndex(e => e.Name == targetName);
            _entries.Insert(targetIndex + 1, entry);
            return this;
        }

        public bool Remove(string name)
        {
            var index = _entries.FindIndex(e => e.Name == name);
            if (index >= 0)
            {
                var entry = _entries[index];


                if (!entry.Options.CanRemove)
                {
                    throw new InvalidOperationException($"The pipeline entry '{name}' cannot be removed");
                }
                ;
                _entries.RemoveAt(index);
                return true;
            }
            return false;
        }

        #endregion

        #region Apply

        public void Apply(TApp app)
        {
            if (_applied) return;

            foreach (var entry in _entries)
            {
                try
                {
                    if (entry.Condition())
                    {
                        entry.Action(app);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing pipeline entry '{entry.Name}': {ex.Message}");
                }
            }

            _applied = true;
        }

        #endregion

        #region Internal Helpers

        private IRamshaAppPipeline<TApp> AddEntry(Action<TApp> configure, string name, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null)
        {
            var entry = new PipelineEntry<TApp>(configure, name, options, condition);
            _entries.Add(entry);
            return this;
        }

        private IRamshaAppPipeline<TApp> InsertBefore(string targetName, Action<TApp> configure, string name, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null)
        {
            var index = _entries.FindIndex(e => e.Name == targetName);
            var entry = new PipelineEntry<TApp>(configure, name, options, condition);
            if (index >= 0) _entries.Insert(index, entry);
            else _entries.Insert(0, entry);
            return this;
        }

        private IRamshaAppPipeline<TApp> InsertAfter(string targetName, Action<TApp> configure, string name, RamshaPipelineEntryOptions? options = null, Func<bool>? condition = null)
        {
            var index = _entries.FindIndex(e => e.Name == targetName);
            var entry = new PipelineEntry<TApp>(configure, name, options, condition);
            if (index >= 0) _entries.Insert(index + 1, entry);
            else _entries.Add(entry);
            return this;
        }



        #endregion
    }

    public class PipelineEntry<TApp>
    {
        private static int _globalIndex;

        public Func<bool> Condition { get; }
        public int InsertionIndex { get; }
        public Action<TApp> Action { get; }
        public string Name { get; }

        public RamshaPipelineEntryOptions Options { get; }

        public PipelineEntry(
            Action<TApp> action,
            string name,
            RamshaPipelineEntryOptions? options = null,
            Func<bool>? condition = null)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Name = string.IsNullOrEmpty(name) ? $"__unnamed_{Interlocked.Increment(ref _globalIndex)}" : name;
            InsertionIndex = Interlocked.Increment(ref _globalIndex);
            Options = options ??= new RamshaPipelineEntryOptions();
            Condition = condition ?? (() => true);
        }

        public override string ToString() => $"PipelineEntry(Name={Name}, Index={InsertionIndex})";
    }

    public class RamshaPipelineEntryOptions
    {
        public bool CanMove { get; set; } = true;
        public bool CanReplace { get; set; } = true;
        public bool CanRemove { get; set; } = true;
    }
}
