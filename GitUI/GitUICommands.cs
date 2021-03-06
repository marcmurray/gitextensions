﻿using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using GitCommands;
using GitUI.Blame;
using GitUI.Plugin;
using GitUI.Tag;
using GitUIPluginInterfaces;
using GitUIPluginInterfaces.RepositoryHosts;
using PatchApply;

namespace GitUI
{
    public class GitUICommands : IGitUICommands
    {
        private static GitUICommands instance = null;

        public static GitUICommands Instance
        {
            get
            {
                if (instance == null)
                    instance = new GitUICommands();

                return instance;
            }
        }

        public event GitUIEventHandler PreBrowse;
        public event GitUIEventHandler PostBrowse;

        public event GitUIEventHandler PreDeleteBranch;
        public event GitUIEventHandler PostDeleteBranch;

        public event GitUIEventHandler PreCheckoutRevision;
        public event GitUIEventHandler PostCheckoutRevision;

        public event GitUIEventHandler PreCheckoutBranch;
        public event GitUIEventHandler PostCheckoutBranch;

        public event GitUIEventHandler PreFileHistory;
        public event GitUIEventHandler PostFileHistory;

        public event GitUIEventHandler PreBlame;
        public event GitUIEventHandler PostBlame;

        public event GitUIEventHandler PreCompareRevisions;
        public event GitUIEventHandler PostCompareRevisions;

        public event GitUIEventHandler PreAddFiles;
        public event GitUIEventHandler PostAddFiles;

        public event GitUIEventHandler PreCreateBranch;
        public event GitUIEventHandler PostCreateBranch;

        public event GitUIEventHandler PreClone;
        public event GitUIEventHandler PostClone;

        public event GitUIEventHandler PreCommit;
        public event GitUIEventHandler PostCommit;

        public event GitUIEventHandler PreInitialize;
        public event GitUIEventHandler PostInitialize;

        public event GitUIEventHandler PrePush;
        public event GitUIEventHandler PostPush;

        public event GitUIEventHandler PrePull;
        public event GitUIEventHandler PostPull;

        public event GitUIEventHandler PreViewPatch;
        public event GitUIEventHandler PostViewPatch;

        public event GitUIEventHandler PreApplyPatch;
        public event GitUIEventHandler PostApplyPatch;

        public event GitUIEventHandler PreFormatPatch;
        public event GitUIEventHandler PostFormatPatch;

        public event GitUIEventHandler PreStash;
        public event GitUIEventHandler PostStash;

        public event GitUIEventHandler PreResolveConflicts;
        public event GitUIEventHandler PostResolveConflicts;

        public event GitUIEventHandler PreCherryPick;
        public event GitUIEventHandler PostCherryPick;

        public event GitUIEventHandler PreMergeBranch;
        public event GitUIEventHandler PostMergeBranch;

        public event GitUIEventHandler PreCreateTag;
        public event GitUIEventHandler PostCreateTag;

        public event GitUIEventHandler PreDeleteTag;
        public event GitUIEventHandler PostDeleteTag;

        public event GitUIEventHandler PreEditGitIgnore;
        public event GitUIEventHandler PostEditGitIgnore;

        public event GitUIEventHandler PreEditGitAttributes;
        public event GitUIEventHandler PostEditGitAttributes;

        public event GitUIEventHandler PreSettings;
        public event GitUIEventHandler PostSettings;

        public event GitUIEventHandler PreArchive;
        public event GitUIEventHandler PostArchive;

        public event GitUIEventHandler PreMailMap;
        public event GitUIEventHandler PostMailMap;

        public event GitUIEventHandler PreVerifyDatabase;
        public event GitUIEventHandler PostVerifyDatabase;

        public event GitUIEventHandler PreRemotes;
        public event GitUIEventHandler PostRemotes;

        public event GitUIEventHandler PreRebase;
        public event GitUIEventHandler PostRebase;

        public event GitUIEventHandler PreSubmodulesEdit;
        public event GitUIEventHandler PostSubmodulesEdit;

        public event GitUIEventHandler PreUpdateSubmodules;
        public event GitUIEventHandler PostUpdateSubmodules;

        public event GitUIEventHandler PreUpdateSubmodulesRecursive;
        public event GitUIEventHandler PostUpdateSubmodulesRecursive;

        public string GitCommand(string arguments)
        {
            return GitCommandHelpers.RunCmd(Settings.GitCommand, arguments);
        }

        public string CommandLineCommand(string cmd, string arguments)
        {
            return GitCommandHelpers.RunCmd(cmd, arguments);
        }


        public bool StartCommandLineProcessDialog(string command, string arguments)
        {
            FormProcess process = new FormProcess(command, arguments);
            process.ShowDialog();
            return true;
        }

        public bool StartGitCommandProcessDialog(string arguments)
        {
            FormProcess process = new FormProcess(arguments);
            process.ShowDialog();
            return true;
        }

        public bool StartBrowseDialog(string filter)
        {
            if (!InvokeEvent(PreBrowse))
                return false;

            FormBrowse form = new FormBrowse(filter);
            form.ShowDialog();

            InvokeEvent(PostBrowse);

            return true;
        }

        public bool StartBrowseDialog()
        {
            return StartBrowseDialog("");
        }

        public bool StartDeleteBranchDialog(string branch)
        {
            if (!InvokeEvent(PreDeleteBranch))
                return false;

            FormDeleteBranch form = new FormDeleteBranch(branch);
            form.ShowDialog();

            InvokeEvent(PostDeleteBranch);

            return true;
        }

        public bool StartCheckoutRevisionDialog()
        {
            if (!InvokeEvent(PreCheckoutRevision))
                return false;

            FormCheckout form = new FormCheckout();
            form.ShowDialog();

            InvokeEvent(PostCheckoutRevision);

            return true;
        }

        public bool StartCheckoutBranchDialog()
        {
            if (!InvokeEvent(PreCheckoutBranch))
                return false;

            FormCheckoutBranch form = new FormCheckoutBranch();
            form.ShowDialog();

            InvokeEvent(PostCheckoutBranch);

            return true;
        }

        public bool StartFileHistoryDialog(string fileName, GitRevision revision)
        {
            if (!InvokeEvent(PreFileHistory))
                return false;

            FormFileHistory form = new FormFileHistory(fileName, revision);
            form.ShowDialog();

            InvokeEvent(PostFileHistory);

            return false;
        }

        public bool StartFileHistoryDialog(string fileName)
        {
            return StartFileHistoryDialog(fileName, null);
        }

        public bool StartCompareRevisionsDialog()
        {
            if (!InvokeEvent(PreCompareRevisions))
                return false;

            FormDiff form = new FormDiff();
            form.ShowDialog();

            InvokeEvent(PostCompareRevisions);

            return false;
        }

        public bool StartAddFilesDialog()
        {
            if (!InvokeEvent(PreAddFiles))
                return false;

            FormAddFiles form = new FormAddFiles();
            form.ShowDialog();

            InvokeEvent(PostAddFiles);

            return false;
        }

        public bool StartCreateBranchDialog()
        {
            if (!InvokeEvent(PreCreateBranch))
                return false;

            FormBranch form = new FormBranch();
            form.ShowDialog();

            InvokeEvent(PostCreateBranch);

            return true;
        }

        public bool StartCloneDialog()
        {
            if (!InvokeEvent(PreClone))
                return false;

            FormClone form = new FormClone();
            form.ShowDialog();

            InvokeEvent(PostClone);

            return true;
        }


        public bool StartCommitDialog()
        {
            if (!InvokeEvent(PreCommit))
                return true;

            FormCommit form = new FormCommit();
            form.ShowDialog();

            InvokeEvent(PostCommit);

            if (!form.NeedRefresh)
                return false;

            return true;
        }


        public bool StartInitializeDialog()
        {
            if (!InvokeEvent(PreInitialize))
                return true;

            if (!GitCommands.Settings.ValidWorkingDir())
                new FormInit(GitCommands.Settings.WorkingDir).ShowDialog();
            else
                new FormInit().ShowDialog();

            InvokeEvent(PostInitialize);

            return true;
        }

        public bool StartInitializeDialog(string dir)
        {
            if (!InvokeEvent(PreInitialize))
                return true;

            new FormInit(dir).ShowDialog();

            InvokeEvent(PostInitialize);

            return true;
        }

        public bool StartPushDialog()
        {
            return StartPushDialog(false);
        }

        public bool StartPushDialog(bool pushOnShow)
        {
            if (!InvokeEvent(PrePush))
                return true;

            FormPush form = new FormPush();
            form.PushOnShow = pushOnShow;
            form.ShowDialog();

            InvokeEvent(PostPush);

            return true;
        }

        public bool StartPullDialog()
        {
            if (!InvokeEvent(PrePull))
                return true;

            new FormPull().ShowDialog();

            InvokeEvent(PostPull);

            return true;
        }

        public bool StartViewPatchDialog()
        {
            if (!InvokeEvent(PreViewPatch))
                return true;

            ViewPatch applyPatch = new ViewPatch();
            applyPatch.ShowDialog();

            InvokeEvent(PostViewPatch);

            return true;
        }

        public bool StartApplyPatchDialog()
        {
            return StartApplyPatchDialog(null);
        }

        public bool StartApplyPatchDialog(string patchFile)
        {
            if (!InvokeEvent(PreApplyPatch))
                return true;

            FormApplyPatch form = new FormApplyPatch();
            form.SetPatchFile(patchFile);
            form.ShowDialog();

            InvokeEvent(PostApplyPatch);

            return true;
        }

        public bool StartFormatPatchDialog()
        {
            if (!InvokeEvent(PreFormatPatch))
                return true;

            FormFormatPatch form = new FormFormatPatch();
            form.ShowDialog();

            InvokeEvent(PostFormatPatch);

            return false;
        }

        public bool StartStashDialog()
        {
            if (!InvokeEvent(PreStash))
                return true;

            FormStash form = new FormStash();
            form.ShowDialog();

            InvokeEvent(PostStash);

            return true;
        }

        public bool StartResolveConflictsDialog()
        {
            if (!InvokeEvent(PreResolveConflicts))
                return true;

            FormResolveConflicts form = new FormResolveConflicts();
            form.ShowDialog();

            InvokeEvent(PostResolveConflicts);

            return true;
        }

        public bool StartCherryPickDialog()
        {
            if (!InvokeEvent(PreCherryPick))
                return true;

            FormCherryPick form = new FormCherryPick();
            form.ShowDialog();

            InvokeEvent(PostCherryPick);

            return true;
        }

        public bool StartMergeBranchDialog(string branch)
        {
            if (!InvokeEvent(PreMergeBranch))
                return true;

            FormMergeBranch form = new FormMergeBranch(branch);
            form.ShowDialog();

            InvokeEvent(PostMergeBranch);

            return true;
        }

        public bool StartCreateTagDialog()
        {
            if (!InvokeEvent(PreCreateTag))
                return true;

            FormTag form = new FormTag();
            form.ShowDialog();

            InvokeEvent(PostCreateTag);

            return true;
        }

        public bool StartDeleteTagDialog()
        {
            if (!InvokeEvent(PreDeleteTag))
                return true;

            FormDeleteTag form = new FormDeleteTag();
            form.ShowDialog();

            InvokeEvent(PostDeleteTag);

            return true;
        }

        public bool StartEditGitIgnoreDialog()
        {
            if (!InvokeEvent(PreEditGitIgnore))
                return true;

            FormGitIgnore form = new FormGitIgnore();
            form.ShowDialog();

            InvokeEvent(PostEditGitIgnore);

            return false;
        }

        public bool StartEditGitAttributesDialog()
        {
            if (!InvokeEvent(PreEditGitAttributes))
                return true;

            FormGitAttributes form = new FormGitAttributes();
            form.ShowDialog();

            InvokeEvent(PostEditGitAttributes);

            return false;
        }

        public bool StartSettingsDialog()
        {
            if (!InvokeEvent(PreSettings))
                return true;

            FormSettings form = new FormSettings();
            form.ShowDialog();

            InvokeEvent(PostSettings);

            return false;
        }

        public bool StartArchiveDialog()
        {
            if (!InvokeEvent(PreArchive))
                return true;

            FormArchive form = new FormArchive();
            form.ShowDialog();

            InvokeEvent(PostArchive);

            return false;
        }

        public bool StartMailMapDialog()
        {
            if (!InvokeEvent(PreMailMap))
                return true;

            FormMailMap form = new FormMailMap();
            form.ShowDialog();

            InvokeEvent(PostMailMap);

            return true;
        }

        public bool StartVerifyDatabaseDialog()
        {
            if (!InvokeEvent(PreVerifyDatabase))
                return true;

            FormVerify form = new FormVerify();
            form.ShowDialog();

            InvokeEvent(PostVerifyDatabase);

            return true;
        }

        public bool StartRemotesDialog()
        {
            if (!InvokeEvent(PreRemotes))
                return true;

            FormRemotes form = new FormRemotes();
            form.ShowDialog();

            InvokeEvent(PostRemotes);

            return true;
        }

        public bool StartRebaseDialog(string branch)
        {
            if (!InvokeEvent(PreRebase))
                return true;

            FormRebase form = new FormRebase(branch);
            form.ShowDialog();

            InvokeEvent(PostRebase);

            return true;
        }

        public bool StartSubmodulesDialog()
        {
            if (!InvokeEvent(PreSubmodulesEdit))
                return true;

            FormSubmodules form = new FormSubmodules();
            form.ShowDialog();

            InvokeEvent(PostSubmodulesEdit);

            return true;
        }

        public bool StartUpdateSubmodulesDialog()
        {
            if (!InvokeEvent(PreUpdateSubmodules))
                return true;

            FormProcess process = new FormProcess(GitCommandHelpers.SubmoduleUpdateCmd(""));
            process.ShowDialog();

            InvokeEvent(PostUpdateSubmodules);

            return true;
        }

        public bool StartUpdateSubmodulesRecursiveDialog()
        {
            if (!InvokeEvent(PreUpdateSubmodulesRecursive))
                return true;

            FormProcess process = new FormProcess(GitCommandHelpers.SubmoduleUpdateCmd(""));
            process.ShowDialog();
            UpdateSubmodulesRecursive();

            InvokeEvent(PostUpdateSubmodulesRecursive);

            return true;
        }

        public bool StartPluginSettingsDialog()
        {
            new FormPluginSettings().ShowDialog();
            return true;
        }


        private static void UpdateSubmodulesRecursive()
        {
            string oldworkingdir = Settings.WorkingDir;

            foreach (GitSubmodule submodule in (new GitCommandsInstance()).GetSubmodules())
            {
                if (!string.IsNullOrEmpty(submodule.LocalPath))
                {
                    Settings.WorkingDir = oldworkingdir + submodule.LocalPath;

                    if (Settings.WorkingDir != oldworkingdir && File.Exists(GitCommands.Settings.WorkingDir + ".gitmodules"))
                    {
                        FormProcess process = new FormProcess(GitCommandHelpers.SubmoduleUpdateCmd(""));
                        process.ShowDialog();

                        UpdateSubmodulesRecursive();
                    }

                    Settings.WorkingDir = oldworkingdir;
                }
            }

            Settings.WorkingDir = oldworkingdir;
        }

        private bool InvokeEvent(GitUIEventHandler gitUIEventHandler)
        {
            return InvokeEvent(this, gitUIEventHandler);
        }

        internal static bool InvokeEvent(object sender, GitUIEventHandler gitUIEventHandler)
        {
            try
            {
                GitUIEventArgs e = new GitUIEventArgs(GitUICommands.Instance);
                if (gitUIEventHandler != null)
                    gitUIEventHandler(sender, e);

                return !e.Cancel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
            return true;
        }

        public bool StartBlameDialog(string fileName)
        {
            return StartBlameDialog(fileName, null);
        }

        private bool StartBlameDialog(string fileName, GitRevision revision)
        {
            if (!InvokeEvent(PreBlame))
                return false;

            new FormBlame(fileName, revision).ShowDialog();

            InvokeEvent(PostBlame);

            return false;
        }

        private static void WrapRepoHostingCall(string name, IRepositoryHostPlugin gitHoster, Action<IRepositoryHostPlugin> call)
        {
            if (!gitHoster.ConfigurationOk)
            {
                var eventArgs = new GitUIEventArgs(GitUICommands.Instance);
                gitHoster.Execute(eventArgs);
            }

            if (gitHoster.ConfigurationOk)
            {
                try
                {
                    call(gitHoster);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("ERROR: {0} failed. Message: {1}\r\n\r\n{2}", name, ex.Message, ex.StackTrace), "Error! :(");
                }
            }
        }

        public void StartCloneForkFromHoster(IRepositoryHostPlugin gitHoster)
        {
            WrapRepoHostingCall("View pull requests", gitHoster, (gh) => (new RepoHosting.ForkAndCloneForm(gitHoster)).ShowDialog());
        }

        internal void StartPullRequestsDialog(IRepositoryHostPlugin gitHoster)
        {
            WrapRepoHostingCall("View pull requests", gitHoster, (gh) => (new RepoHosting.ViewPullRequestsForm(gitHoster)).ShowDialog());
        }

        public void StartCreatePullRequest()
        {
            var relevantHosts = (from gh in RepoHosting.RepoHosts.GitHosters where gh.CurrentWorkingDirRepoIsRelevantToMe select gh).ToList();
            if (relevantHosts.Count == 0)
                MessageBox.Show("Could not find any repo hosts for current working directory");
            else if (relevantHosts.Count == 1)
                StartCreatePullRequest(relevantHosts.First());
            else
                MessageBox.Show("StartCreatePullRequest:Selection not implemented!");
        }

        public void StartCreatePullRequest(IRepositoryHostPlugin gitHoster)
        {
            StartCreatePullRequest(gitHoster, null, null);
        }

        public void StartCreatePullRequest(IRepositoryHostPlugin gitHoster, string chooseRemote, string chooseBranch)
        {
            WrapRepoHostingCall("Create pull request", gitHoster, (gh) => (new RepoHosting.CreatePullRequestForm(gitHoster, chooseRemote, chooseBranch)).ShowDialog());
        }
    }
}
