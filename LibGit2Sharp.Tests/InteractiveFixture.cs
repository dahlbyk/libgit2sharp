using LibGit2Sharp.Tests.TestHelpers;
using Xunit;
using Xunit.Extensions;

namespace LibGit2Sharp.Tests
{
    public class InteractiveFixture : BaseFixture
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InteractiveStateHasExpectedValuesForNewRepo(bool isBare)
        {
            SelfCleaningDirectory scd = BuildSelfCleaningDirectory();
            using (var repo = Repository.Init(scd.DirectoryPath, isBare))
            {
                var state = repo.InteractiveState;
                Assert.Equal("master", state.HeadName);
                Assert.Equal(RepositoryState.None, state.PendingOperation);
            }
        }

        [Fact]
        public void InteractiveStateHasExpectedValuesForABareRepo()
        {
            using (var repo = new Repository(BareTestRepoPath))
            {
                var state = repo.InteractiveState;
                Assert.Equal("master", state.HeadName);
                Assert.Equal(RepositoryState.None, state.PendingOperation);
            }
        }

        [Fact]
        public void InteractiveStateHasExpectedValuesForStandardRepo()
        {
            var path = BuildTemporaryCloneOfTestRepo(StandardTestRepoPath);
            using (var repo = new Repository(path.RepositoryPath))
            {
                var state = repo.InteractiveState;
                Assert.Equal("master", state.HeadName);
                Assert.Equal(RepositoryState.None, state.PendingOperation);

                repo.Checkout("track-local");
                Assert.Equal("track-local", state.HeadName);
            }
        }

        [Fact]
        public void InteractiveStateHasExpectedValuesForDetachedHead()
        {
            var path = BuildTemporaryCloneOfTestRepo(StandardTestRepoPath);
            using (var repo = new Repository(path.RepositoryPath))
            {
                repo.Checkout(repo.Head.Tip.Sha);

                var state = repo.InteractiveState;
                Assert.Equal("(32eab9c...)", state.HeadName);
                Assert.Equal(RepositoryState.None, state.PendingOperation);
            }
        }

        [Theory]
        [InlineData("MERGE_HEAD", RepositoryState.Merge)]
        [InlineData("REVERT_HEAD", RepositoryState.Revert)]
        [InlineData("CHERRY_PICK_HEAD", RepositoryState.CherryPick)]
        [InlineData("BISECT_LOG", RepositoryState.Bisect)]
        [InlineData("rebase-apply/rebasing", RepositoryState.Rebase)]
        [InlineData("rebase-apply/applying", RepositoryState.ApplyMailbox)]
        [InlineData("rebase-apply/whatever", RepositoryState.ApplyMailboxOrRebase)]
        [InlineData("rebase-merge/interactive", RepositoryState.RebaseInteractive)]
        [InlineData("rebase-merge/whatever", RepositoryState.RebaseMerge)]
        public void InteractiveStateHasExpectedPendingOperationValues(string stateFile, RepositoryState expectedState)
        {
            var path = BuildTemporaryCloneOfTestRepo(StandardTestRepoPath);
            path.TouchGit(stateFile);

            using (var repo = new Repository(path.RepositoryPath))
            {
                var state = repo.InteractiveState;
                Assert.Equal(expectedState, state.PendingOperation);
            }
        }

        [Fact]
        public void InteractiveStateHasExpectedHeadNameDuringRebase()
        {
            var path = BuildTemporaryCloneOfTestRepo(StandardTestRepoPath);
            path.TouchGit("rebase-merge/head-name", "refs/heads/master");

            using (var repo = new Repository(path.RepositoryPath))
            {
                repo.Checkout(repo.Head.Tip.Sha);

                var state = repo.InteractiveState;
                Assert.Equal("master", state.HeadName);
            }
        }
    }
}
