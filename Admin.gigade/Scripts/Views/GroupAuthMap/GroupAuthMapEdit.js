editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/GroupAuthMap/AddAuthMap',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: CONTENTID,
            id: 'content_id',
            name: 'content_id',
            submitValue: true,
            hidden: true
        },
         {
             xtype: 'combobox',
             name: 'group_id',
             id: 'group_id',
             editable: false,
             fieldLabel: GROUPID,
             store: fGroupStore,
             queryMode: 'local',
             submitValue: true,
             displayField: 'groupName',
             valueField: 'rowid',
             typeAhead: true,
             forceSelection: false,
             value: 1
         },
        {
            xtype: 'textfield',
            name: 'table_name',
            id: 'table_name',
            submitValue: true,
            allowBlank: false,
            fieldLabel: TABLENAME
        }, {
            xtype: 'textfield',
            name: 'table_alias_name',
            id: 'table_alias_name',
            allowBlank: false,
            submitValue: true,
            fieldLabel: TABLEALIASNAME
        },
        {
            xtype: 'textfield',
            name: 'column_name',
            id: 'column_name',
            allowBlank: false,
            submitValue: true,
            fieldLabel: COLUMNNAME
        },
        {
            xtype: 'textfield',
            name: 'value',
            id: 'value',
            allowBlank: false,
            submitValue: true,
            fieldLabel: VALUE,
        }
        //{
        //    xtype: 'fieldcontainer',
        //    fieldLabel: '狀態',
        //    id: 'status',
        //    defaultType: 'radiofield',
        //    submitValue: true,
        //    defaults: {
        //        flex: 1
        //    },
        //    layout: 'hbox',
        //    items: [{
        //        boxLabel: '啟用',
        //        name: 'check_iden',
        //        inputValue: '1',
        //        id: 'radio1',
        //        checked: true
        //    }, {
        //        boxLabel: '禁用',
        //        name: 'check_iden',
        //        inputValue: '0',
        //        id: 'radio2'
        //    }
        //    ]
        //}
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            content_id: Ext.htmlEncode(Ext.getCmp('content_id').getValue()),
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            table_name: Ext.htmlEncode(Ext.getCmp('table_name').getValue()),
                            table_alias_name: Ext.htmlEncode(Ext.getCmp('table_alias_name').getValue()),
                            column_name: Ext.htmlEncode(Ext.getCmp('column_name').getValue()),
                            value: Ext.htmlEncode(Ext.getCmp('value').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS + result.msg);
                                fGroupStore.load();
                                GroupMapStore.load();
                                editWin.close();

                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE + result.msg);
                                fGroupStore.load();
                                GroupMapStore.load();
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, FAILURE + result.msg);
                            fGroupStore.load();
                            GroupMapStore.load();
                            editWin.close();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: ISCLOSE,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row);
            }
        }
    });
    editWin.show();
}
