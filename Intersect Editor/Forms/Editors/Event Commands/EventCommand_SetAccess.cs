﻿using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_SetAccess : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_SetAccess(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbAccess.SelectedIndex = _myCommand.Ints[0];
        }

        private void InitLocalization()
        {
            grpSetAccess.Text = Strings.Get("eventsetaccess", "title");
            lblAccess.Text = Strings.Get("eventsetaccess", "label");
            cmbAccess.Items.Clear();
            cmbAccess.Items.Add(Strings.Get("eventsetaccess", "access0"));
            cmbAccess.Items.Add(Strings.Get("eventsetaccess", "access1"));
            cmbAccess.Items.Add(Strings.Get("eventsetaccess", "access2"));
            btnSave.Text = Strings.Get("eventsetaccess", "okay");
            btnCancel.Text = Strings.Get("eventsetaccess", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = cmbAccess.SelectedIndex;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}