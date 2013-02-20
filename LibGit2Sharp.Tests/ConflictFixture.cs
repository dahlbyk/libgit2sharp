﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp.Tests.TestHelpers;
using Xunit;
using Xunit.Extensions;

namespace LibGit2Sharp.Tests
{
    public class ConflictFixture : BaseFixture
    {
        public static IEnumerable<object[]> ConflictData
        {
            get
            {
                return new[]
                {
                    new object[] { false, "DIFFERS-IN-CASE.TXT", "0a636a157ae305b03f3f4eb8c37fbfca7ab7564a", "e8f3bf056a12d2efa65f0a46f444e5eadb827913", "b607afb8beef6d80d8007c76f8600f3287d4b84c" },
                    new object[] { false, "Differs-In-Case.txt", "a1531c7e02a3aa4938858d6840a8b4859b188525", "920826a357cfa0867f8598e05038136c9d1a1f5e", "9dc12deeb6c9ed1746b788e3442df3e38e5deeae" },
                    new object[] { false, "ancestor-and-ours.txt", "5dee68477001f447f50fa7ee7e6a818370b5c2fb", "dad0664ae617d36e464ec08ed969ff496432b075", null },
                    new object[] { false, "ancestor-and-theirs.txt", "3aafd4d0bac33cc3c78c4c070f3966fb6e6f641a", null, "7b26cd5ac0ee68483ae4d5e1e00b064547ea8c9b" },
                    new object[] { false, "ancestor-only.txt", "9736f4cd77759672322f3222ed3ddead1412d969", null, null },
                    new object[] { false, "conflicts-one.txt", "1f85ca51b8e0aac893a621b61a9c2661d6aa6d81", "b7a41c703dc1f33185c76944177f3844ede2ee46", "516bd85f78061e09ccc714561d7b504672cb52da" },
                    new object[] { false, "conflicts-two.txt", "84af62840be1b1c47b778a8a249f3ff45155038c", "ef70c7154145b09c7d08806e55fd0bfb7172576d", "220bd62631c8cf7a83ef39c6b94595f00517211e" },
                    new object[] { false, "ours-and-theirs.txt", null, "9aaa9ae562a5f7362425a3fedc4d33ff74fe39e6", "0ca3f55d4ac2fa4703c149123b0b31d733112f86" },
                    new object[] { false, "ours-only.txt", null, "9736f4cd77759672322f3222ed3ddead1412d969", null },
                    new object[] { false, "theirs-only.txt", null, null, "9736f4cd77759672322f3222ed3ddead1412d969" },

                    new object[] { true, "DIFFERS-IN-CASE.TXT", "0a636a157ae305b03f3f4eb8c37fbfca7ab7564a", "e8f3bf056a12d2efa65f0a46f444e5eadb827913", "b607afb8beef6d80d8007c76f8600f3287d4b84c" },
                    new object[] { true, "Differs-In-Case.txt", "a1531c7e02a3aa4938858d6840a8b4859b188525", "920826a357cfa0867f8598e05038136c9d1a1f5e", "9dc12deeb6c9ed1746b788e3442df3e38e5deeae" },
                    new object[] { true, "ancestor-and-ours.txt", "5dee68477001f447f50fa7ee7e6a818370b5c2fb", "dad0664ae617d36e464ec08ed969ff496432b075", null },
                    new object[] { true, "ancestor-and-theirs.txt", "3aafd4d0bac33cc3c78c4c070f3966fb6e6f641a", null, "7b26cd5ac0ee68483ae4d5e1e00b064547ea8c9b" },
                    new object[] { true, "ancestor-only.txt", "9736f4cd77759672322f3222ed3ddead1412d969", null, null },
                    new object[] { true, "conflicts-one.txt", "1f85ca51b8e0aac893a621b61a9c2661d6aa6d81", "b7a41c703dc1f33185c76944177f3844ede2ee46", "516bd85f78061e09ccc714561d7b504672cb52da" },
                    new object[] { true, "conflicts-two.txt", "84af62840be1b1c47b778a8a249f3ff45155038c", "ef70c7154145b09c7d08806e55fd0bfb7172576d", "220bd62631c8cf7a83ef39c6b94595f00517211e" },
                    new object[] { true, "ours-and-theirs.txt", null, "9aaa9ae562a5f7362425a3fedc4d33ff74fe39e6", "0ca3f55d4ac2fa4703c149123b0b31d733112f86" },
                    new object[] { true, "ours-only.txt", null, "9736f4cd77759672322f3222ed3ddead1412d969", null },
                    new object[] { true, "theirs-only.txt", null, null, "9736f4cd77759672322f3222ed3ddead1412d969" },
                };
            }
        }

        [Theory]
        [InlineData(true, "ancestor-and-ours.txt", true, false, FileStatus.Removed, 2)]
        [InlineData(false, "ancestor-and-ours.txt", true, true, FileStatus.Removed |FileStatus.Untracked, 2)]
        [InlineData(true, "ancestor-and-theirs.txt", true, false, FileStatus.Nonexistent, 2)]
        [InlineData(false, "ancestor-and-theirs.txt", true, true, FileStatus.Untracked, 2)]
        [InlineData(true, "conflicts-one.txt", true, false, FileStatus.Removed, 3)]
        [InlineData(false, "conflicts-one.txt", true, true, FileStatus.Removed | FileStatus.Untracked, 3)]
        [InlineData(true, "conflicts-two.txt", true, false, FileStatus.Removed, 3)]
        [InlineData(false, "conflicts-two.txt", true, true, FileStatus.Removed | FileStatus.Untracked, 3)]
        [InlineData(true, "ours-and-theirs.txt", true, false, FileStatus.Removed, 2)]
        [InlineData(false, "ours-and-theirs.txt", true, true, FileStatus.Removed | FileStatus.Untracked, 2)]
        [InlineData(true, "ours-only.txt", true, false, FileStatus.Removed, 1)]
        [InlineData(false, "ours-only.txt", true, true, FileStatus.Removed | FileStatus.Untracked, 1)]
        [InlineData(true, "theirs-only.txt", true, false, FileStatus.Nonexistent, 1)]
        [InlineData(false, "theirs-only.txt", true, true, FileStatus.Untracked, 1)]
        /* Conflicts clearing through Index.Remove() only works when a version of the entry exists in the workdir.
         * This is because libgit2's git_iterator_for_index() seem to only care about stage level 0.
         * Corrolary: other cases only work out of sheer luck (however, the behaviour is stable, so I guess we
         *   can rely on it for the moment.
         * [InlineData(true, "ancestor-only.txt", false, false, FileStatus.Nonexistent, 0)]
         * [InlineData(false, "ancestor-only.txt", false, false, FileStatus.Nonexistent, 0)]
         */
        public void CanClearConflictsByRemovingFromTheIndex(
            bool removeFromWorkdir, string filename, bool existsBeforeRemove, bool existsAfterRemove, FileStatus lastStatus, int removedIndexEntries)
        {
            var path = CloneMergedTestRepo();
            using (var repo = new Repository(path))
            {
                int count = repo.Index.Count;

                string fullpath = Path.Combine(repo.Info.WorkingDirectory, filename);

                Assert.Equal(existsBeforeRemove, File.Exists(fullpath));
                Assert.NotNull(repo.Conflicts[filename]);

                repo.Index.Remove(filename, removeFromWorkdir);

                Assert.Null(repo.Conflicts[filename]);
                Assert.Equal(count - removedIndexEntries, repo.Index.Count);
                Assert.Equal(existsAfterRemove, File.Exists(fullpath));
                Assert.Equal(lastStatus, repo.Index.RetrieveStatus(filename));
            }
        }

        [SkippableTheory, PropertyData("ConflictData")]
        public void CanRetrieveSingleConflictByPath(bool ignorecase, string filepath, string ancestorId, string ourId, string theirId)
        {
            SetIgnoreCaseOrSkip(MergedTestRepoWorkingDirPath, ignorecase);

            using (var repo = new Repository(MergedTestRepoWorkingDirPath))
            {
                Conflict conflict = repo.Conflicts[filepath];
                Assert.NotNull(conflict);

                ObjectId expectedAncestor = ancestorId != null ? new ObjectId(ancestorId) : null;
                ObjectId expectedOurs = ourId != null ? new ObjectId(ourId) : null;
                ObjectId expectedTheirs = theirId != null ? new ObjectId(theirId) : null;

                Assert.Null(repo.Index[filepath]);
                Assert.Equal(expectedAncestor, conflict.Ancestor != null ? conflict.Ancestor.Id : null);
                Assert.Equal(expectedOurs, conflict.Ours != null ? conflict.Ours.Id : null);
                Assert.Equal(expectedTheirs, conflict.Theirs != null ? conflict.Theirs.Id : null);
            }
        }

        private string GetPath(Conflict conflict)
        {
            if (conflict.Ancestor != null)
            {
                return conflict.Ancestor.Path;
            }
            if (conflict.Ours != null)
            {
                return conflict.Ours.Path;
            }
            if (conflict.Theirs != null)
            {
                return conflict.Theirs.Path;
            }

            return null;
        }

        private string GetId(IndexEntry e)
        {
            if (e == null)
            {
                return null;
            }

            return e.Id.ToString();
        }

        [SkippableTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void CanRetrieveAllConflicts(bool ignorecase)
        {
            SetIgnoreCaseOrSkip(MergedTestRepoWorkingDirPath, ignorecase);

            using (var repo = new Repository(MergedTestRepoWorkingDirPath))
            {
                var expected = ConflictData.Where(d => (bool)d[0] == ignorecase).Select(d => Preview(d[1], d[2], d[3], d[4])).ToArray();
                var actual = repo.Conflicts.Select(c => Preview(GetPath(c), GetId(c.Ancestor), GetId(c.Ours), GetId(c.Theirs))).ToArray();
                Assert.Equal(expected, actual);
            }
        }

        private static object Preview(object p, object a, object o, object t)
        {
            return new { p, a, o, t };
        }
    }
}
