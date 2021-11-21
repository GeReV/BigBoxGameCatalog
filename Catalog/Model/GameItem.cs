using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Catalog.Model
{
    public class GameItem : IModel, ICloneable<GameItem>
    {
        public int GameItemId { get; set; }

        [Required] public ItemType ItemType { get; set; } = ItemTypes.BigBox;

        public GameCopy? Game { get; set; }

        public bool Missing { get; set; }

        public Condition? Condition { get; set; }

        public string? ConditionDetails { get; set; }

        public string? Notes { get; set; }

        public ICollection<Image> Scans { get; set; }

        public ICollection<File> Files { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        [NotMapped] public IEnumerable<object> Children => Scans.Concat<object>(Files);

        public int Id => GameItemId;
        public bool IsNew => GameItemId == 0;

        public GameItem Clone() =>
            new()
            {
                Condition = Condition,
                ConditionDetails = ConditionDetails,
                Files = Files
                    .Select(file => new File(file.Path) { Sha256Checksum = file.Sha256Checksum })
                    .ToList(),
                ItemType = ItemType,
                Missing = Missing,
                Notes = Notes,
                Scans = Scans
                    .Select(scan => new Image(scan.Path))
                    .ToList()
            };

        public void CopyFrom(GameItem other)
        {
            Condition = other.Condition;
            ConditionDetails = other.ConditionDetails;
            ItemType = other.ItemType;
            Missing = other.Missing;
            Notes = other.Notes;

            UpdateFiles(other.Files);
            UpdateScans(other.Scans);
        }

        private void UpdateScans(ICollection<Image> otherScans)
        {
            var currentScanIds = Scans
                .Select(f => f.ImageId)
                .ToImmutableHashSet();

            var nextScanIds = otherScans
                .Select(f => f.ImageId)
                .ToImmutableHashSet();

            var dropScans =
                Scans.Where(img => !nextScanIds.Contains(img.ImageId)).ToList();

            foreach (var dropScanItem in dropScans)
            {
                Scans.Remove(dropScanItem);
            }

            var addScans = otherScans
                .Where(img => !currentScanIds.Contains(img.ImageId));

            foreach (var addScanItem in addScans)
            {
                Scans.Add(addScanItem);
            }
        }

        private void UpdateFiles(ICollection<File> otherFiles)
        {
            var currentFileIds = Files
                .Select(f => f.FileId)
                .ToImmutableHashSet();

            var nextFileIds = otherFiles
                .Select(f => f.FileId)
                .ToImmutableHashSet();

            var dropFiles =
                Files.Where(f => !nextFileIds.Contains(f.FileId)).ToList();

            foreach (var dropFileItem in dropFiles)
            {
                Files.Remove(dropFileItem);
            }

            var addFiles = otherFiles
                .Where(f => !currentFileIds.Contains(f.FileId));

            foreach (var addFileItem in addFiles)
            {
                Files.Add(addFileItem);
            }
        }
    }
}
