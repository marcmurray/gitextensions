﻿using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using GitPlugin.Commands;
using Microsoft.VisualStudio.CommandBars;
using Thread = System.Threading.Thread;

namespace GitPlugin
{
    /// <summary>
    ///   The object for implementing an Add-in.
    /// </summary>
    /// <seealso class = 'IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        private Plugin _gitPlugin;
        private DTE2 _applicationObject;

        #region IDTCommandTarget Members

        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, 
            ref vsCommandStatus status, ref object commandText)
        {
            if (neededText != vsCommandStatusTextWanted.vsCommandStatusTextWantedNone ||
                !_gitPlugin.CanHandleCommand(commandName)) 
                return;

            if (_gitPlugin.IsCommandEnabled(commandName))
                status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
            else
                status = vsCommandStatus.vsCommandStatusSupported;
        }

        public void Exec(string commandName, vsCommandExecOption executeOption, 
            ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;
            if (executeOption != vsCommandExecOption.vsCommandExecOptionDoDefault)
                return;

            handled = _gitPlugin.OnCommand(commandName);
        }

        #endregion

        #region IDTExtensibility2 Members

        /// <summary>
        /// Implements the OnConnection method of the IDTExtensibility2 interface.
        /// Receives notification that the Add-in is being loaded.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="connectMode">The connect mode.</param>
        /// <param name="addInInst">The add in inst.</param>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2"/>
        public void OnConnection(object application, ext_ConnectMode connectMode, 
            object addInInst, ref Array custom)
        {
            if (_gitPlugin != null)
                return;

            try
            {
                var cultureInfo = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = cultureInfo;

                _applicationObject = (DTE2) application;

                _gitPlugin = 
                    new Plugin((DTE2) application, (AddIn) addInInst, "GitExtensions", "GitPlugin.Connect");

                _gitPlugin.OutputPane.OutputString("Git Extensions plugin connected" + Environment.NewLine);

                //GitPlugin.DeleteCommandBar("GitExtensions");
                try
                {
                    _gitPlugin.RegisterCommand("GitExtensionsFileHistory", new ToolbarCommand<FileHistory>());
                    _gitPlugin.RegisterCommand("GitExtensionsCommit", new ToolbarCommand<Commit>());
                    _gitPlugin.RegisterCommand("GitExtensionsBrowse", new ToolbarCommand<Browse>());
                    _gitPlugin.RegisterCommand("GitExtensionsClone", new ToolbarCommand<Clone>());
                    _gitPlugin.RegisterCommand("GitExtensionsCreateBranch", new ToolbarCommand<CreateBranch>());
                    _gitPlugin.RegisterCommand("GitExtensionsSwitchBranch", new ToolbarCommand<SwitchBranch>());
                    _gitPlugin.RegisterCommand("GitExtensionsDiff", new ToolbarCommand<ViewDiff>());
                    _gitPlugin.RegisterCommand("GitExtensionsInitRepository", new ToolbarCommand<Init>());
                    _gitPlugin.RegisterCommand("GitExtensionsFormatPatch", new ToolbarCommand<FormatPatch>());
                    _gitPlugin.RegisterCommand("GitExtensionsPull", new ToolbarCommand<Pull>());
                    _gitPlugin.RegisterCommand("GitExtensionsPush", new ToolbarCommand<Push>());
                    _gitPlugin.RegisterCommand("GitExtensionsRebase", new ToolbarCommand<Rebase>());
                    _gitPlugin.RegisterCommand("GitExtensionsRevert", new ToolbarCommand<Revert>());
                    _gitPlugin.RegisterCommand("GitExtensionsMerge", new ToolbarCommand<Merge>());
                    _gitPlugin.RegisterCommand("GitExtensionsCherryPick", new ToolbarCommand<Cherry>());
                    _gitPlugin.RegisterCommand("GitExtensionsStash", new ToolbarCommand<Stash>());
                    _gitPlugin.RegisterCommand("GitExtensionsSettings", new ToolbarCommand<Settings>());
                    _gitPlugin.RegisterCommand("GitExtensionsSolveMergeConflicts",
                                              new ToolbarCommand<SolveMergeConflicts>());
                    _gitPlugin.RegisterCommand("GitExtensionsApplyPatch", new ToolbarCommand<ApplyPatch>());
                    _gitPlugin.RegisterCommand("GitExtensionsAbout", new ToolbarCommand<About>());
                    _gitPlugin.RegisterCommand("GitExtensionsBash", new ToolbarCommand<Bash>());
                    _gitPlugin.RegisterCommand("GitExtensionsGitIgnore", new ToolbarCommand<GitIgnore>());
                    _gitPlugin.RegisterCommand("GitExtensionsRemotes", new ToolbarCommand<Remotes>());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


                //Place the command on the tools menu.
                //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
                var menuBarCommandBar = ((CommandBars) _applicationObject.CommandBars)["MenuBar"];


                CommandBarControl toolsControl;
                CommandBarPopup toolsPopup = null;
                try
                {
                    toolsControl = menuBarCommandBar.Controls["Git"];
                }
                catch
                {
                    toolsControl = null;
                }

                try
                {
                    if (toolsControl == null)
                    {
                        toolsControl = menuBarCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing,
                                                                      Type.Missing, 4, false);
                        toolsControl.Caption = "Git";
                    }

                    toolsPopup = (CommandBarPopup) toolsControl;
                    toolsPopup.Caption = "Git";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    if (toolsControl == null)
                    {
                        toolsControl = menuBarCommandBar.Controls[GetToolsMenuName()];
                        toolsPopup = (CommandBarPopup) toolsControl;
                    }
                }

                try
                {
                    // add the toolbar and menu commands
                    var commandBar = _gitPlugin.AddCommandBar("GitExtensions", MsoBarPosition.msoBarTop);

                    _gitPlugin.AddToolbarCommandWithText(
                        commandBar, "GitExtensionsCommit", "Commit", "Commit changes", 7,1);

                    _gitPlugin.AddToolbarCommand(commandBar, 
                        "GitExtensionsBrowse", "Browse", "Browse repository", 12, 2);
                    _gitPlugin.AddToolbarCommand(commandBar, "GitExtensionsPull", "Pull",
                                                "Pull changes from remote repository", 9, 3);
                    _gitPlugin.AddToolbarCommand(commandBar, "GitExtensionsPush", "Push",
                                                "Push changes to remote repository", 8, 4);
                    _gitPlugin.AddToolbarCommand(commandBar, 
                        "GitExtensionsStash", "Stash", "Stash changes", 3, 5);
                    _gitPlugin.AddToolbarCommand(commandBar,
                        "GitExtensionsSettings", "Settings", "Settings", 2, 6);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                try
                {
                    var n = 1;
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsApplyPatch", "Apply patch", "Apply patch", 0,
                                              n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsBrowse", "Browse", "Browse repository", 12, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsSwitchBranch", "Checkout branch",
                                              "Switch to branch", 10, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsCherryPick", "Cherry pick", "Cherry pick commit",
                                              11, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsCommit", "Commit", "Commit changes", 7, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsCreateBranch", "Create branch",
                                              "Create new branch", 10, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsClone", "Clone repository", "Clone existing Git",
                                              14, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsGitIgnore", "Edit .gitignore",
                                              "Edit .gitignore file", 0, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsFormatPatch", "Format patch", "Format patch", 0,
                                              n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsBash", "Git bash", "Start git bash", 0, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsInitRepository", "Initialize new repository",
                                              "Initialize new Git repository", 13, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsRemotes", "Manage remotes",
                                              "Manage remote repositories", 0, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsMerge", "Merge", "merge", 0, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsPull", "Pull",
                                              "Pull changes from remote repository", 9, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsPush", "Push",
                                              "Push changes to remote repository", 8, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsRebase", "Rebase", "Rebase", 0, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsStash", "Stash", "Stash changes", 3, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsSettings", "Settings", "Settings", 2, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsSolveMergeConflicts", "Solve mergeconflicts",
                                              "Solve mergeconflicts", 0, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsDiff", "View changes",
                                              "View commit change history", 0, n++);
                    _gitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsAbout", "About Git Extensions",
                                              "About Git Extensions", 0, n++);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


                try
                {
                    _gitPlugin.AddMenuCommand("Item", "GitExtensionsFileHistory", "File history", "Show file history", 6,
                                             4);
                    _gitPlugin.AddMenuCommand("Item", "GitExtensionsRevert", "Undo file changes",
                                             "Undo changes made to this file", 4, 5);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                try
                {
                    _gitPlugin.AddMenuCommand("Easy MDI Document Window", "GitExtensionsFileHistory", "File history",
                                             "Show file history", 6, 4);
                    _gitPlugin.AddMenuCommand("Easy MDI Document Window", "GitExtensionsRevert", "Undo file changes",
                                             "Undo changes made to this file", 4, 5);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                try
                {
                    _gitPlugin.AddMenuCommand("Code Window", "GitExtensionsFileHistory", "File history",
                                             "Show file history", 6, 10);
                    _gitPlugin.AddMenuCommand("Code Window", "GitExtensionsRevert", "Undo file changes",
                                             "Undo changes made to this file", 4, 11);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            _gitPlugin.DeleteCommands();
        }

        public void OnAddInsUpdate(ref Array custom)
        {
        }

        public void OnStartupComplete(ref Array custom)
        {
        }

        public void OnBeginShutdown(ref Array custom)
        {
        }

        #endregion

        private string GetToolsMenuName()
        {
            string toolsMenuName;

            try
            {
                //If you would like to move the command to a different menu, change the word "Tools" to the 
                //  English version of the menu. This code will take the culture, append on the name of the menu
                //  then add the command to that menu. You can find a list of all the top-level menus in the file
                //  CommandBar.resx.
                string resourceName;
                var resourceManager = new ResourceManager("GitPlugin.CommandBar", Assembly.GetExecutingAssembly());
                var cultureInfo = new CultureInfo(_applicationObject.LocaleID);

                if (cultureInfo.TwoLetterISOLanguageName == "zh")
                {
                    var parentCultureInfo = cultureInfo.Parent;
                    resourceName = String.Concat(parentCultureInfo.Name, "Tools");
                }
                else
                {
                    resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");
                }
                toolsMenuName = resourceManager.GetString(resourceName);
            }
            catch
            {
                //We tried to find a localized version of the word Tools, but one was not found.
                //  Default to the en-US word, which may work for the current culture.
                toolsMenuName = "Tools";
            }

            return toolsMenuName;
        }
    }
}